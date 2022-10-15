using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32.Shared;
using WinRT.GraphicsCapture.Properties;

namespace WinRT.GraphicsCapture
{
    public partial class ConfigMain : Form
    {

        List<String> vs;

        public DxWindow dxWindow = null;

        public ConfigMain(List<String> vs)
        {
            this.vs = vs;
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();

            lock (this.vs)
            {
                this.vs.Clear();
                if (Settings.Default.chkMir0) vs.Add("mir0");
                if (Settings.Default.chkMir1) vs.Add("mir1");
                if (Settings.Default.chkMir2) vs.Add("mir2");
            }

            if (Settings.Default.RunOnWindows)
            {
                if(!Startup.IsInStartup()){
                    Startup.RunOnStartup();
                }
            }
            else
            {
                if (Startup.IsInStartup())
                {
                    Startup.RemoveFromStartup();
                }
            }

            MessageBox.Show("Saved");

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.Visible)
            {

                if (this.WindowState == FormWindowState.Normal)
                {
                    this.Hide();
                }
            }
            else
            {
                this.Show();
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }

            }

        }

        private void ConfigMain_FormClosed(object sender, FormClosedEventArgs e)
        {


            if (dxWindow != null)
            {
                dxWindow.form.Close();
            }
        }

        private void ConfigMain_Load(object sender, EventArgs e)
        {
            timer1_Tick(null, null);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
                
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            var CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);


            
            foreach (var s in new string[] { "0", "1", "2" })
            {
                string curname = s;

                var imgdirectory = CurrentDirectory + "\\mir" + curname;

                if (System.IO.Directory.Exists(imgdirectory))
                    Directory.GetFiles(imgdirectory)
                 .Select(f => new FileInfo(f))
                 .Where(f => f.CreationTime < DateTime.Now.AddDays(-1))
                 .ToList()
                 .ForEach(f => {
                     f.Delete();
                     });
            }



        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
