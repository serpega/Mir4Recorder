using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using IronOcr;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;

using Win32.Shared.Interfaces;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace Win32.Shared
{
    public class DxWindow : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private readonly string _title;

        private DateTime lastdrt = DateTime.Now;
        private DateTime lastchangeshow = DateTime.Now;
        private int currgcap = 0;

        List<ICaptureMethod> gcaps;
        private string code;
        private string name;
        private List<string> vs;

        public RenderForm form;


        public DxWindow(string title, List<ICaptureMethod> gcaps, string code, string name, List<string> vs)
        {
            _title = title;
            this.gcaps = gcaps;
            this.code = code;
            this.name = name;
            this.vs = vs;

            
        }

        public void Dispose()
        {
            foreach(var method in gcaps) {

                method?.Dispose();

            }
          
        }


        public static void HttpUploadFile(string url, string file, string paramName, string contentType, System.Collections.Specialized.NameValueCollection nvc)
        {
            
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                
            }
            catch (Exception ex)
            {
                
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        public void Show()
        {
            form = new RenderForm(_title);

            // create a Device and SwapChain
            var swapChainDescription = new SwapChainDescription
            {
                BufferCount = 2,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out var device, out var swapChain);
            using var swapChain1 = swapChain.QueryInterface<SwapChain1>();

            // ignore all Windows events
            using var factory = swapChain1.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            using var vertexShaderByteCode = ShaderBytecode.CompileFromFile("./Shader.fx", "VS", "vs_5_0");
            using var vertexShader = new VertexShader(device, vertexShaderByteCode);

            using var pixelShaderByteCode = ShaderBytecode.CompileFromFile("./Shader.fx", "PS", "ps_5_0");
            using var pixelShader = new PixelShader(device, pixelShaderByteCode);

            using var layout = new InputLayout(device, vertexShaderByteCode, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0)
            });

            using var vertexes = Buffer.Create(device, BindFlags.VertexBuffer, new[]
            {
                new Vertex { Position = new RawVector3(-1.0f, 1.0f, 0.5f), TexCoord = new RawVector2(0.0f, 0.0f) },
                new Vertex { Position = new RawVector3(1.0f, 1.0f, 0.5f), TexCoord = new RawVector2(1.0f, 0.0f) },
                new Vertex { Position = new RawVector3(-1.0f, -1.0f, 0.5f), TexCoord = new RawVector2(0.0f, 1.0f) },
                new Vertex { Position = new RawVector3(1.0f, -1.0f, 0.5f), TexCoord = new RawVector2(1.0f, 1.0f) }
            });

            var samplerStateDescription = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagMipLinear
            };

            device.ImmediateContext.InputAssembler.InputLayout = layout;
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexes, Utilities.SizeOf<Vertex>(), 0));
            device.ImmediateContext.VertexShader.Set(vertexShader);
            device.ImmediateContext.PixelShader.SetSampler(0, new SamplerState(device, samplerStateDescription));
            device.ImmediateContext.PixelShader.Set(pixelShader);

            // create a first views
            var backBuffer = Resource.FromSwapChain<Texture2D>(swapChain1, 0);
            var renderView = new RenderTargetView(device, backBuffer);

            device.ImmediateContext.Rasterizer.SetViewport(0, 0, form.ClientSize.Width, form.ClientSize.Height);
            device.ImmediateContext.OutputMerger.SetTargets(renderView);

            // listen events (but processed in render loop)
            var isResized = false;
            form.UserResized += (_, __) => isResized = true;


            form.Shown += Form_Shown;

            RenderLoop.Run(form, () =>
            {


                if ((DateTime.Now - lastchangeshow).TotalSeconds >= 5)
                {
                    lastchangeshow = DateTime.Now;
                    currgcap++;
                    if(currgcap > gcaps.Count - 1)
                    {
                        currgcap = 0;
                    }
                }

                var send = false;
                if ((DateTime.Now - lastdrt).TotalSeconds >= 5)
                {
                    send = true;
                    lastdrt = DateTime.Now;
                }


                IntPtr m0 = FindWindow(null, "Mir4G[0]");
                IntPtr m1 = FindWindow(null, "Mir4G[1]");
                IntPtr m2 = FindWindow(null, "Mir4G[2]");


                foreach (var s in new string[] { "0", "1", "2" }) {
                    string curname = s;
                    foreach (var cap in gcaps)
                    {
                        if (cap.getName() == "mir" + curname)
                        {
                            if (this.vs.Contains("mir" + curname))
                            {
                                IntPtr mi = FindWindow(null, "Mir4G[" + curname + "]");
                                if (mi != cap.getHwnd())
                                {

                                    if (cap.IsCapturing)
                                    {
                                        cap.StopCapture();
                                        cap.setHwnd(mi);
                                    }
                                    else
                                    {
                                        cap.setHwnd(mi);
                                    }
                                }
                            }
                            else
                            {
                                if (cap.IsCapturing)
                                {
                                    cap.StopCapture();
                                    cap.setHwnd((IntPtr)0);
                                }
                            }
                        }
                    }
                }
          
                


                int ii = 0;
                foreach (var cap in gcaps)
                {
                    ii++;



                    ICaptureMethod _captureMethod = cap;


                    if (cap.getHwnd() == (IntPtr)0) continue;


                    // ReSharper disable AccessToDisposedClosure
                    if (!_captureMethod.IsCapturing)
                        _captureMethod.StartCapture(form.Handle, device, factory);

                    using var texture2d = _captureMethod.TryGetNextFrameAsTexture2D(device);

                    if (ii == currgcap && false)
                    {

                        if (isResized)
                        {
                            Utilities.Dispose(ref backBuffer);
                            Utilities.Dispose(ref renderView);

                            swapChain1.ResizeBuffers(swapChainDescription.BufferCount, form.ClientSize.Width, form.ClientSize.Height, Format.Unknown, SwapChainFlags.None);
                            backBuffer = Resource.FromSwapChain<Texture2D>(swapChain1, 0);
                            renderView = new RenderTargetView(device, backBuffer);

                            device.ImmediateContext.Rasterizer.SetViewport(0, 0, form.ClientSize.Width, form.ClientSize.Height);
                            device.ImmediateContext.OutputMerger.SetTargets(renderView);

                            isResized = false;
                        }

                        // clear view
                        device.ImmediateContext.ClearRenderTargetView(renderView, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
                        
                        if (texture2d != null)
                        {
                            using var shaderResourceView = new ShaderResourceView(device, texture2d);
                            device.ImmediateContext.PixelShader.SetShaderResource(0, shaderResourceView);
                        }
                        // draw it
                        device.ImmediateContext.Draw(4, 0);
                        swapChain1.Present(1, PresentFlags.None, new PresentParameters());
                    }

                    if (send)
                    {


                        Bitmap g = cap.getBitmap();

                        if (g != null)
                        {

                            var CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                            var imgdirectory = CurrentDirectory + "\\" + cap.getName();

                            if (!System.IO.Directory.Exists(imgdirectory)) System.IO.Directory.CreateDirectory(imgdirectory);

                            string n = string.Format("mir-{0:yyyy-MM-dd_HH-mm-ss}.jpg",
                                                                                            DateTime.Now);


                            var filename = imgdirectory + "\\" + n;

                            if (System.IO.File.Exists(filename)) System.IO.File.Delete(filename);
                            
                            var myImageCodecInfo = GetEncoderInfo("image/jpeg");
                            var myEncoder = Encoder.Quality;
                            var myEncoderParameters = new EncoderParameters(1);
                            var myEncoderParameter = new EncoderParameter(myEncoder, 20L);
                            myEncoderParameters.Param[0] = myEncoderParameter;

                            g.Save(filename, myImageCodecInfo, myEncoderParameters);


                           


                        }


                       




                    }


                }




                // ReSharper restore AccessToDisposedClosure
            });

            renderView.Dispose();
            backBuffer.Dispose();
            swapChain.Dispose();
            device.Dispose();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            ((RenderForm)sender).Hide();
        }
    }
}