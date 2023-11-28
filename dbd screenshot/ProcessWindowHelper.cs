using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dbd_screenshot
{
    internal class ProcessWindowHelper
    {
        // Native Windows API function
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public IntPtr GetWindowHandleByProcessName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine($"No process found with name '{processName}'.");
                return IntPtr.Zero;
            }

            int processId = processes[0].Id;
            IntPtr windowHandle = GetWindowHandle(processId);

            if (windowHandle == IntPtr.Zero)
            {
                Console.WriteLine($"No window found with process name '{processName}'.");
                return IntPtr.Zero;
            }

            return windowHandle;
        }

        private IntPtr GetWindowHandle(int processId)
        {
            IntPtr windowHandle = IntPtr.Zero;
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.Id == processId)
                {
                    windowHandle = process.MainWindowHandle;
                    break;
                }
            }

            return windowHandle;
        }
    }
}
