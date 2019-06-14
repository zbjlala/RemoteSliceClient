using DSkin.Forms;
using Remote_Client_fubao.Helper;
using Remote_Client_fubao.Helper.HttpHelper;
using Remote_Client_fubao.Model.ClientEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Remote_Client_fubao.kefu
{
    public partial class WorkOrder : DSkinForm
    {
        HttpProxy httpProxy = new HttpProxy();

        WorkOrderModel _order;    

        public WorkOrder(WorkOrderModel order)
        {
            InitializeComponent();
            this._order = order;
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
            InitOrder(order);
            
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
        /// <summary>
        /// 初始化显示表单
        /// </summary>
        /// <param name="order"></param>
        private void InitOrder(WorkOrderModel order)
        {
            if(order.Order_From != null)
            {
                this.dSkinTextBox2.Text = order.Order_From;
            }
            if(order.Product_From != null)
            {
                this.dSkinTextBox3.Text = order.Product_From;
            }
            if(order.Modle_From != null)
            {
                this.dSkinTextBox4.Text = order.Modle_From;
            }
            if(order.Customer_ID == null)
            {
                MessageBox.Show("远程客户ID获取失败，表单无法提交");
                this.Close();
                return;
            }
            if (order.Service_ID == null)
            {
                MessageBox.Show("获取ID失败，表单无法提交");
                this.Close();
                return;
            }
            if (order.Begin_Time == null)
            {
                MessageBox.Show("获取开始时间失败，表单无法提交");
                this.Close();
                return;
            }
            this.dSkinDateTimePicker1.Value = DateTimeHelper.TimeSpanToDateTime(Convert.ToInt64(order.Begin_Time));
            this.dSkinDateTimePicker2.Value = DateTime.Now;

        }


        private void dSkinButton1_Click(object sender, EventArgs e)
        {
            string submitOrder = httpProxy.GetRequestCommon(InterfaceUrl.SUBMIT_ORDER, GetOrderPatamer(),true,true);
            if(!String.IsNullOrEmpty(submitOrder))
            {
                MessageBox.Show("远程工单保存成功");
            }
            else
            {
                MessageBox.Show("远程工单保存失败");
            }
        }
        /// <summary>
        /// 获取表单参数
        /// </summary>
        /// <returns></returns>
        private IDictionary<string,string> GetOrderPatamer()
        {
            Dictionary<string, string> orderPatamer = new Dictionary<string, string>();
            //必填
            orderPatamer.Add("customer_id", _order.Customer_ID);
            orderPatamer.Add("servicer_id", _order.Service_ID);
            orderPatamer.Add("begin_time", DateTimeHelper.GetTimeSpan(this.dSkinDateTimePicker1.Value).ToString());
            orderPatamer.Add("end_time",DateTimeHelper.GetTimeSpan(DateTime.Now).ToString());
            //非必填
            if(!String.IsNullOrWhiteSpace (this.dSkinTextBox2.Text))
            {
                orderPatamer.Add("order_from", this.dSkinTextBox2.Text);
            }
            if(!String.IsNullOrWhiteSpace(this.dSkinTextBox3.Text))
            {
                orderPatamer.Add("product_from", this.dSkinTextBox3.Text);
            }
            if (!String.IsNullOrWhiteSpace(this.dSkinTextBox4.Text))
            {
                orderPatamer.Add("model_from", this.dSkinTextBox4.Text);
            }
            if (!String.IsNullOrWhiteSpace(this.dSkinTextBox5.Text))
            {
                orderPatamer.Add("order_title", this.dSkinTextBox5.Text);
            }
            if (!String.IsNullOrWhiteSpace(this.dSkinTextBox1.Text))
            {
                orderPatamer.Add("order_content", this.dSkinTextBox1.Text);
            }
            if(_order.Customer_IP != null)
            {
                orderPatamer.Add("customer_ip", _order.Customer_IP);
            }
            if(_order.Customer_Address != null)
            {
                orderPatamer.Add("customer_address", _order.Customer_Address);
            }
            return orderPatamer;
        }
    }
}
