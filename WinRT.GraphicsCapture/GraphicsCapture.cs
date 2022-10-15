using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Win32.Shared;
using Win32.Shared.Interfaces;

using WinRT.GraphicsCapture.Interop;

using Device = SharpDX.Direct3D11.Device;
using Microsoft.Graphics.Canvas;
using System.IO;

using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using SharpDX.WIC;
using SharpDX.Direct2D1;


namespace WinRT.GraphicsCapture
{
    internal class GraphicsCapture : ICaptureMethod
    {
        private static readonly Guid _graphicsCaptureItemIid = new Guid("79C3F95B-31F7-4EC2-A464-632EF5D30760");
        private Direct3D11CaptureFramePool _captureFramePool;
        private GraphicsCaptureItem _captureItem;
        private GraphicsCaptureSession _captureSession;

        public System.Drawing.Bitmap currentBitmap = null;
        private IntPtr captureHandle;

        private string status = "";
        public string name = "";

        public IntPtr getHwnd()
        {
            return captureHandle;
        }

        public void setHwnd(IntPtr hwnd)
        {
            captureHandle = hwnd;
        }


        public string getName()
        {
            return name;
        }



        public string getStatus()
        {
            return status;
        }

        public void setStatus(string s)
        {
            status = s;

        }



        public GraphicsCapture(IntPtr captureHandle, string name)
        {
            IsCapturing = false;
            this.captureHandle = captureHandle;
            this.name = name;
        }

        public System.Drawing.Bitmap getBitmap()
        {
            return currentBitmap;
        }

        public bool IsCapturing { get; private set; }

        public void Dispose()
        {
            StopCapture();
        }

        public struct Rect
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }


        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);


        public IntPtr GetHandleWindow(string title)
        {
            return FindWindow(null, title);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);


        public void StartCapture(IntPtr hWnd, Device device, SharpDX.DXGI.Factory factory)
        {
            #region GraphicsCapturePicker version

            /*
            var capturePicker = new GraphicsCapturePicker();

            // ReSharper disable once PossibleInvalidCastException
            // ReSharper disable once SuspiciousTypeConversion.Global
            var initializer = (IInitializeWithWindow)(object)capturePicker;
            initializer.Initialize(hWnd);

            _captureItem = capturePicker.PickSingleItemAsync().AsTask().Result;
            */

            #endregion

            #region Window Handle version

            /*var capturePicker = new WindowPicker();
            var captureHandle = capturePicker.PickCaptureTarget(hWnd);
            if (captureHandle == IntPtr.Zero)
                return;*/

            //IntPtr captureHandle = GetHandleWindow(@"Mir4G[1]");

            IntPtr mirw = captureHandle;

            if (captureHandle == IntPtr.Zero)
                return;


            Rect Rect = new Rect();
            GetWindowRect(mirw, ref Rect);
          
            int width = Rect.Right - Rect.Left;
            int heigth = Rect.Bottom - Rect.Top;

            /// 1269x743
            //if (width != 1269 && heigth != 743)
            {
                //MoveWindow(mirw,0, 0, 1269, 743, true);

            }



            _captureItem = CreateItemForWindow(captureHandle);

            #endregion

            if (_captureItem == null)
                return;

            _captureItem.Closed += CaptureItemOnClosed;

            var hr = NativeMethods.CreateDirect3D11DeviceFromDXGIDevice(device.NativePointer, out var pUnknown);
            if (hr != 0)
            {
                StopCapture();
                return;
            }

            var winrtDevice = (IDirect3DDevice) Marshal.GetObjectForIUnknown(pUnknown);
            Marshal.Release(pUnknown);

            

            _captureFramePool = Direct3D11CaptureFramePool.Create(winrtDevice, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, _captureItem.Size);
            _captureSession = _captureFramePool.CreateCaptureSession(_captureItem);
            _captureSession.StartCapture();
            IsCapturing = true;
        }





        public Texture2D TryGetNextFrameAsTexture2D(Device device)
        {
            using var frame = _captureFramePool?.TryGetNextFrame();
            if (frame == null)
                return null;


            // ReSharper disable once SuspiciousTypeConversion.Global
            var surfaceDxgiInterfaceAccess = (IDirect3DDxgiInterfaceAccess) frame.Surface;
            var pResource = surfaceDxgiInterfaceAccess.GetInterface(new Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d"));

            using var surfaceTexture = new Texture2D(pResource); // shared resource

            var height = surfaceTexture.Description.Height;
            var width = surfaceTexture.Description.Width;

            var textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            using (var screenTexture = new Texture2D(device, textureDesc))
            {
                device.ImmediateContext.CopyResource(surfaceTexture, screenTexture);

                var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
                var boundsRect = new System.Drawing.Rectangle(0, 0, screenTexture.Description.Width, screenTexture.Description.Height);
                var bitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
              
                //Copy pixels from screen capture Texture to GDI bitmap
                var bitmapData = bitmap.LockBits(boundsRect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);
                var sourcePtr = mapSource.DataPointer;
                var destinationPtr = bitmapData.Scan0;
                for (int y = 0; y < height; y++)
                {
                    //Copy a single line
                    SharpDX.Utilities.CopyMemory(destinationPtr, sourcePtr, width * 4);
                    //Advance pointers
                    sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                    destinationPtr = IntPtr.Add(destinationPtr, bitmapData.Stride);
                }

                //Release source and dest locks
                bitmap.UnlockBits(bitmapData);
                device.ImmediateContext.UnmapSubresource(screenTexture, 0);

                //bitmap.Save("xxxxx.bmp");
                if(currentBitmap!=null) currentBitmap.Dispose();
                currentBitmap = bitmap;

                /*
                var Ocr = new IronTesseract(); // nothing to configure
                Ocr.Language = OcrLanguage.English;
                Ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
                using (var Input = new OcrInput())
                {
                    Input.AddImage( bitmap );
                    var Result = Ocr.Read(Input);
                    Console.WriteLine(Result.Text);
                }
                */


                

            }







            var texture2dDescription = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = surfaceTexture.Description.Height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                Width = surfaceTexture.Description.Width
            };
            var texture2d = new Texture2D(device, texture2dDescription);

            device.ImmediateContext.CopyResource(surfaceTexture, texture2d);

            /*var m_surface = new SharpDX.DXGI.Surface(pResource);

            using (SharpDX.Direct2D1.Factory factory = new SharpDX.Direct2D1.Factory())
            {
                var m_renderTarget = new RenderTarget(
                    factory,
                    m_surface,
                    new RenderTargetProperties()
                    {
                        DpiX = 0.0f, // default dpi
                        DpiY = 0.0f, // default dpi
                        MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
                        Type = RenderTargetType.Hardware,
                        Usage = RenderTargetUsage.None,
                        PixelFormat = new SharpDX.Direct2D1.PixelFormat(
                            Format.Unknown,
                            SharpDX.Direct2D1.AlphaMode.Premultiplied
                        )
                    }
                );
                var bitmap = new SharpDX.Direct2D1.Bitmap(m_renderTarget, m_surface, new SharpDX.Direct2D1.BitmapProperties(new SharpDX.Direct2D1.PixelFormat(
                                                      Format.R8G8B8A8_UNorm,
                                                      SharpDX.Direct2D1.AlphaMode.Premultiplied)));

            }*/





            return texture2d;
        }

        public void StopCapture() // ...or release resources
        {
            _captureSession?.Dispose();
            _captureFramePool?.Dispose();
            _captureSession = null;
            _captureFramePool = null;
            _captureItem = null;
            IsCapturing = false;
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        private static GraphicsCaptureItem CreateItemForWindow(IntPtr hWnd)
        {
            var factory = WindowsRuntimeMarshal.GetActivationFactory(typeof(GraphicsCaptureItem));
            var interop = (IGraphicsCaptureItemInterop) factory;
            var pointer = interop.CreateForWindow(hWnd, typeof(GraphicsCaptureItem).GetInterface("IGraphicsCaptureItem").GUID);
            var capture = Marshal.GetObjectForIUnknown(pointer) as GraphicsCaptureItem;
            Marshal.Release(pointer);

            return capture;
        }

        private void CaptureItemOnClosed(GraphicsCaptureItem sender, object args)
        {
            StopCapture();
        }
    }
}