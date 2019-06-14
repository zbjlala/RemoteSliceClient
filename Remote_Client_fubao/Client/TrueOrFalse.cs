using DSkin.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Remote_Client_fubao.Client
{
    public partial class TrueOrFalse : DSkinWindowForm
    {
        private bool accept = true;

        public bool Accept { get { return accept; } }
        public TrueOrFalse(string username)
        {
            InitializeComponent();
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiX = graphics.DpiX;
                // float dpiY = graphics.DpiY;
                float zoom = dpiX / 96;
                MaximumSize = new Size((int)(Width * zoom), (int)(Height * zoom));
                Size = new Size((int)(Width * zoom), (int)(Height * zoom));
                if (zoom != (float)1)
                {
                    GetControls(zoom);
                }
            }
            this.TopMost = true;
            this.dSkinLabel2.Text = String.Format("工程师"+ username +"\r\n请求与您开始远程服务是否接受？");
        }
        /// <summary>
        /// 缩放所有窗体
        /// </summary>
        /// <param name="ctc"></param>
        /// <param name="zoom"></param>
        public void GetControls(float zoom)
        {
            foreach (Control control in this.Controls)
            {
                control.Location = new Point((int)(control.Location.X * zoom), (int)(control.Location.Y * zoom));
                control.Size = new Size((int)(control.Width * zoom), (int)(control.Height * zoom));
            }
        }

        private void btn_true_Click(object sender, EventArgs e)
        {
            this.accept = true;
            this.Close();
        }

        private void btn_false_Click(object sender, EventArgs e)
        {
            this.accept = false;
            this.Close();
        }
    }
}
