using System;
using System.Net;

namespace NetworkUtil
{
    public class SubnetUtil
    {
        public static int SubnetMaskToBit(string subnetMask)
        {
            int totalBits = 0;
            foreach (string octet in subnetMask.Split('.'))
            {
                byte octetByte = byte.Parse(octet);
                while (octetByte != 0)
                {
                    totalBits += octetByte & 1; // logical AND on the LSB
                    octetByte >>= 1; // do a bitwise shift to the right to create a new LSB
                }
            }
            return totalBits;
        }

        public static string SubnetByteToString(byte subnet)
        {
            long mask = (0xffffffffL << (32 - subnet)) & 0xffffffffL;
            mask = IPAddress.HostToNetworkOrder((int)mask);
            return new IPAddress((uint)mask).ToString();
        }
    }
}