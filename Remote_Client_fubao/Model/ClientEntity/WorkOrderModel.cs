using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Model.ClientEntity
{
    public class WorkOrderModel
    {
       
        //工单来源
        private string order_From;
        //产品来源
        private string product_From;
        //模块来源
        private string modle_From;
        //工单标题
        private string order_Title;
        //工单内容
        private string order_Content;
        //远程客户ID
        private string customer_ID;
        //客服ID
        private string service_ID;
        //远程客户IP
        private string customer_IP;
        //远程客户地区
        private string customer_Address;
        //远程服务开始时间
        private string begin_Time;
        //远程服务结束时间
        private string end_Time;

        public string Order_From { get => order_From; set => order_From = value; }
        public string Product_From { get => product_From; set => product_From = value; }
        public string Modle_From { get => modle_From; set => modle_From = value; }
        public string Order_Title { get => order_Title; set => order_Title = value; }
        public string Order_Content { get => order_Content; set => order_Content = value; }
        public string Customer_ID { get => customer_ID; set => customer_ID = value; }
        public string Service_ID { get => service_ID; set => service_ID = value; }
        public string Customer_IP { get => customer_IP; set => customer_IP = value; }
        public string Customer_Address { get => customer_Address; set => customer_Address = value; }
        public string Begin_Time { get => begin_Time; set => begin_Time = value; }
        public string End_Time { get => end_Time; set => end_Time = value; }
    }
}
