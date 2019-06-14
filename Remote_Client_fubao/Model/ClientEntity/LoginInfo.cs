using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remote_Client_fubao.Model.ClientEntity
{
    public class LoginInfo
    {
        /// <summary>
        /// 公司全名
        /// </summary>
        private string orgFullName;

        /// <summary>
        /// 部门名称
        /// </summary>
        private string nickName;

        /// <summary>
        /// 邮箱地址
        /// </summary>
        private string email;

        /// <summary>
        /// 唯一标识
        /// </summary>
        private string lastID;

        public string OrgFullName { get => orgFullName; set => orgFullName = value; }
        public string NickName { get => nickName; set => nickName = value; }
        public string Email { get => email; set => email = value; }
        public string LastID { get => lastID; set => lastID = value; }
    }
}
