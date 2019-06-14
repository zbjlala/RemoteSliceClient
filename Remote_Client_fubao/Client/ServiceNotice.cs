
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
    public partial class ServiceNotice : Form
    {
        public ServiceNotice()
        {
            InitializeComponent();
        }

        public RichTextBox Notice
        {
            get { return this.richTextBox1; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
