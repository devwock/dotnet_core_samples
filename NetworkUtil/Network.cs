using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace NetworkUtil
{
    public class Network
    {
        public void SetIpAddress(string mac, string ipAddress, string subnetMask)
        {
            ManagementObject managementObject = GetManagementObject(mac);
            if (managementObject == null)
            {
                return;
            }

            ManagementBaseObject newIP = managementObject.GetMethodParameters("EnableStatic");
            newIP["IPAddress"] = new string[] { ipAddress };
            newIP["SubnetMask"] = new string[] { subnetMask };
            managementObject.InvokeMethod("EnableStatic", newIP, null);
        }

        private ManagementObject GetManagementObject(string mac)
        {
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                if (mac.Equals(managementObject["MACAddress"]))
                {
                    if ((bool)managementObject["IPEnabled"])
                    {
                        return managementObject;
                    }
                }
            }
            return null;
        }

        public void SetGateway(string mac, string gateway)
        {
            ManagementObject managementObject = GetManagementObject(mac);
            if (managementObject == null)
            {
                return;
            }

            ManagementBaseObject newGateway = managementObject.GetMethodParameters("SetGateways");
            newGateway["DefaultIPGateway"] = new string[] { gateway };
            newGateway["GatewayCostMetric"] = new int[] { 1 };
            managementObject.InvokeMethod("SetGateways", newGateway, null);
        }

        public void SetDns(string mac, string dns)
        {
            ManagementObject managementObject = GetManagementObject(mac);
            if (managementObject == null)
            {
                return;
            }

            ManagementBaseObject newDns = managementObject.GetMethodParameters("SetDNSServerSearchOrder");
            newDns["DNSServerSearchOrder"] = dns.Split(',');
            managementObject.InvokeMethod("SetDNSServerSearchOrder", newDns, null);
        }

        public string GetMacAddressByManagementObject()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true");
            IEnumerable<ManagementObject> adapters = searcher.Get().Cast<ManagementObject>();
            foreach (var adapter in adapters)
            {
                return adapter["MACAddress"].ToString();
            }
            return null;
        }

        public void SetNetwork(string mac, string ipAddress, string subnetMask, string gateway, string dns)
        {
            SetIpAddress(mac, ipAddress, subnetMask);
            SetGateway(mac, gateway);
            SetDns(mac, dns);
        }

        public string GetNicSpeed(NetworkInterface networkInterface)
        {
            long speed = networkInterface.Speed;
            string stringSpeed;

            if (speed > (1000 * 1000 * 1000))
            {
                speed = speed / (1000 * 1000 * 1000);
                stringSpeed = speed.ToString() + "Gbps";
                return stringSpeed;
            }

            if (speed > (1000 * 1000))
            {
                speed = speed / (1000 * 1000);
                stringSpeed = speed.ToString() + "Mbps";
                return stringSpeed;
            }

            if (speed > 1000)
            {
                speed = speed / 1000;
                stringSpeed = speed.ToString() + "Kbps";
                return stringSpeed;
            }

            stringSpeed = speed.ToString();
            return stringSpeed;
        }

        public Dictionary<string, string> GetActiveNetwork()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            Dictionary<string, string> nicInfo = new Dictionary<string, string>();
            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up || networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(GetGatewayAddress(networkInterface)))
                {
                    nicInfo.Add("Name", networkInterface.Name);
                    nicInfo.Add("Description", networkInterface.Description);
                    nicInfo.Add("Speed", GetNicSpeed(networkInterface));
                    nicInfo.Add("Mac", string.Join(":", StringUtil.SplitString(networkInterface.GetPhysicalAddress().ToString(), 2)));
                    nicInfo.Add("IPAddress", GetIpAddress(networkInterface));
                    nicInfo.Add("SubnetMask", GetSubnetMask(networkInterface));
                    nicInfo.Add("Gateway", GetGatewayAddress(networkInterface));
                    nicInfo.Add("Dns", GetDnsAddress(networkInterface));
                    return nicInfo;
                }
            }
            return null;
        }

        public string GetMacAddressBySpeed()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up || nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                string macAddress = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed && !string.IsNullOrEmpty(macAddress) && macAddress.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    return StringUtil.SplitInParts(macAddress, ":", 2);
                }
            }
            return null;
        }

        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public string GetMacAddressByArp(string ip)
        {
            IPAddress ipAddr = IPAddress.Parse(ip);
            byte[] ipBytes = ipAddr.GetAddressBytes();
            if (ipBytes.Length != 4)
            {
                throw new Exception("Must be an IPv4 Address");
            }
            int ipInt = BitConverter.ToInt32(ipBytes);

            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            if (SendARP(ipInt, 0, macAddr, ref macAddrLen) != 0)
            {
                throw new Exception("ARP command failed");
            }

            string[] str = new string[(int)macAddrLen];
            for (int i = 0; i < macAddrLen; i++)
            {
                str[i] = macAddr[i].ToString("x2").ToUpper();
            }

            return string.Join(":", str);
        }

        public string GetMacAddressByUnicast(string url)
        {
            var test = Dns.GetHostAddresses(url);
            TcpClient client = new TcpClient();
            client.Client.Connect(new IPEndPoint(Dns.GetHostAddresses(url)[0], 80));
            int count = 10;
            while (!client.Connected && count > 0)
            {
                count--;
                Thread.Sleep(500);
            }
            if (!client.Connected)
            {
                return null;
            }

            IPAddress ipAddress = ((IPEndPoint)client.Client.LocalEndPoint).Address.MapToIPv4();

            client.Client.Disconnect(false);
            client.Close();

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up || nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                UnicastIPAddressInformationCollection unicastAddresses = nic.GetIPProperties().UnicastAddresses;
                if ((unicastAddresses == null) || unicastAddresses.Count <= 0)
                {
                    continue;
                }

                foreach (var unicastAddress in unicastAddresses)
                {
                    if (unicastAddress.Address.Equals(ipAddress))
                    {
                        return StringUtil.ByteToString(nic.GetPhysicalAddress().GetAddressBytes());
                    }
                }
            }
            return null;
        }

        public string GetSubnetMask(NetworkInterface networkInterface)
        {
            foreach (UnicastIPAddressInformation ipAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ipAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddressInformation.IPv4Mask.ToString();
                }
            }
            return null;
        }

        public string GetDnsAddress(NetworkInterface networkInterface)
        {
            foreach (IPAddress dnsAddress in networkInterface.GetIPProperties().DnsAddresses)
            {
                if (dnsAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return dnsAddress.ToString();
                }
            }
            return null;
        }

        public string GetGatewayAddress(NetworkInterface networkInterface)
        {
            foreach (GatewayIPAddressInformation gatewayAddress in networkInterface.GetIPProperties().GatewayAddresses)
            {
                return gatewayAddress.Address.ToString();
            }
            return null;
        }

        public string GetIpAddress(NetworkInterface networkInterface)
        {
            foreach (UnicastIPAddressInformation ipAddressInformation in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ipAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddressInformation.Address.ToString();
                }
            }
            return null;
        }
    }
}