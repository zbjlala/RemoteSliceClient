using Remote_Client_fubao.Client;
using Remote_Client_fubao.kefu;
using Remote_Client_fubao.Model.ClientEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Client_fubao client;
        [DSkin.DirectUI.JSFunction]
        private void button1_Click(object sender, EventArgs e)
        {
               client = new Client_fubao(new LoginInfo {
               LastID = "1",
               Email = "1@163.com",
               NickName = "1",
               OrgFullName = "1"
           }, "1");
            //client.OnNotice += OnNotice;
            this.Invoke(new Action(() =>
            {
               // InvokeJS("ng_robot.actions.invoke({responsedType: 'robot-tips', result: '已经连接，您的ID： " + client._userName + "'})");
            }));
            client.Start();
            this.textBox1.Text = "用户1已连接";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Customer_Service customer_Service = new Customer_Service("2", "{ \"user\": {\"name\": \"2\", \"userLongId\": \"2\"}}", "1");
            customer_Service.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.ClientDispose();
        }
    }
}
