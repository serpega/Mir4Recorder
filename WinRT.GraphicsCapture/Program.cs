using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using Win32.Shared;
using Win32.Shared.Interfaces;
using WinRT.GraphicsCapture.Properties;

namespace WinRT.GraphicsCapture
{
    internal static class Program
    {


        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
   



        [STAThread]
        public static void Main()
        {

            List<String> vs = new List<String>();

            if (Settings.Default.chkMir0) vs.Add("mir0");
            if (Settings.Default.chkMir1) vs.Add("mir1");
            if (Settings.Default.chkMir2) vs.Add("mir2");

            ConfigMain f = new ConfigMain(vs);

            f.Show();


            List<ICaptureMethod> gcaps = new List<ICaptureMethod>();
            gcaps.Add(new GraphicsCapture((IntPtr)0, "mir0"));
            gcaps.Add(new GraphicsCapture((IntPtr)0, "mir1"));
            gcaps.Add(new GraphicsCapture((IntPtr)0, "mir2"));

            /*if(m0 != IntPtr.Zero && Settings.Default.chkMir0)
            {
                gcaps.Add(new GraphicsCapture(m0,"mir0"));
            }
            if (m1 != IntPtr.Zero && Settings.Default.chkMir1)
            {
                gcaps.Add(new GraphicsCapture(m1, "mir1"));
            }
            if (m2 != IntPtr.Zero && Settings.Default.chkMir2)
            {
                gcaps.Add(new GraphicsCapture(m2, "mir2"));
            }*/


            //if (gcaps.Count > 0)
            //{

            using DxWindow window = new DxWindow("Capturing mirs", gcaps, Settings.Default.txtCode, Settings.Default.txtName, vs);
            f.dxWindow = window;
            window.Show();
           
            //}
        }
    }
}