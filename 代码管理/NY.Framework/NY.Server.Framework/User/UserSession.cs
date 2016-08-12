using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NY.Server.Framework.User
{
    public class UserSession
    {
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// 帐套号
        /// </summary>
        public string AccId { get; set; }

        /// <summary>
        /// 会计年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 会计期间
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// 会计期间开始日期
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// 会计期间结束日期
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 登录日期
        /// </summary>
        public DateTime LoginDate { get; set; }

        /// <summary>
        /// 登录用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 当期登录时间
        /// </summary>
        public DateTime CurLoginDate { get; set; }

    }
}
