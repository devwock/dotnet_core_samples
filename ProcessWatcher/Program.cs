using System;
using System.Management;

namespace ProcessWatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += StartWatch_EventArrived;
            startWatch.Start();
            ManagementEventWatcher stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
            stopWatch.EventArrived += StopWatch_EventArrived;
            stopWatch.Start();
            Console.ReadLine();
        }

        private static void StopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            string processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();

            Console.WriteLine($"Process stopped. Name: {processName} | ID: {processID}");
        }

        private static void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = e.NewEvent.Properties["ProcessName"].Value.ToString();
            string processID = Convert.ToInt32(e.NewEvent.Properties["ProcessID"].Value).ToString();

            Console.WriteLine($"Process started. Name: {processName} | ID: {processID}");
        }
    }
}