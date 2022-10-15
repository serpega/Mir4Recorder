using System;

using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Device = SharpDX.Direct3D11.Device;

namespace Win32.Shared.Interfaces
{
    public interface ICaptureMethod : IDisposable
    {
        bool IsCapturing { get; }


        public System.Drawing.Bitmap getBitmap();


        public IntPtr getHwnd();

        public void setHwnd(IntPtr hwnd);

        public string getName();

        public string getStatus();

        public void setStatus(string s);


        void StartCapture(IntPtr hWnd, Device device, Factory factory);

        Texture2D TryGetNextFrameAsTexture2D(Device device);

        void StopCapture();
    }
}