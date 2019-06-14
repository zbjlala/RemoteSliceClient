using Remote_Client_fubao.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Remote
{
    #region client

    public delegate void OnLoginedHandler(object sender, string msg);

    public delegate void OnSubedHandler(string msg);

    public delegate void ReConnectFaildHanlder();

    public delegate void OnServiceNotice(Message msg);


    #endregion
}
