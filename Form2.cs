using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace MapTestGen
{
    public partial class Form2 : Form
    {
        public ChromiumWebBrowser chromeBrowser;
        public void InitializeChromium()
        {
            Cef.EnableHighDPISupport();
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("http://localhost:8000/index.html");
            // Add it to the form and fill it to the form window.
            chromeBrowser.Dock = DockStyle.Fill;
            this.Controls.Add(chromeBrowser);
        }
        public Form2()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Height = 750;
            this.Width = 1000;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
