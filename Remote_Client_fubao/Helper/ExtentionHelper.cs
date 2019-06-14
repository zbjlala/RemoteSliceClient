using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Remote_Client_fubao.Helper
{
    public static class ExtentionHelper
    {
        /// <summary>
        /// winform控件UI多线程处理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void InvokeAction(this Control control, Action action)
        {
            if (control.IsHandleCreated)
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public static List<string> ToList(this string[] arr)
        {
            List<string> list = new List<string>();
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    list.Add(arr[i]);
                }
            }
            return list;
        }
    }
}
