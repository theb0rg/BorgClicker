using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BorgClicker
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private static POINT point;
        private static Thread thread;

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        static void Main(string[] args)
        {
            //Press alt - numpad 7 to repeat mouse click at desired location
            HotKeyManager.RegisterHotKey(Keys.NumPad7, KeyModifiers.Alt);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            thread = new Thread(new ThreadStart(DoMouseClick));
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            thread.Abort();
                            return;
                        default:
                            break;
                    }
                }
                Thread.Sleep(4);
            }
        }

        static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (thread.IsAlive)
            {
                thread.Abort();
                Console.Write("-->STOP\n");
                return;
            }
            Console.Write("RUN-->");
            point = GetCursorPosition();
            Console.Write("Point X = " + point.X + ";Point Y = " + point.Y);
            thread = new Thread(new ThreadStart(DoMouseClick));
            thread.Start();
        }

        public static POINT GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public static void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            while (true)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)point.X, (uint)point.Y, 0, 0);
                Thread.Sleep(4);
            }
        }
    }
}
