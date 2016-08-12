using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NY.Server.Framework.User
{
    public class UserLogin
    {
        static Dictionary<string, UserSession> dicUserSession;

        /// <summary>
        ///  添加用户会话信息
        /// </summary>
        /// <param name="uSession"></param>
        /// <returns></returns>
        public static string AddUserSession(UserSession uSession)
        {
            if (uSession == null || string.IsNullOrEmpty(uSession.UserId))
            {
                uSession = new UserSession() { AccId = "001", UserId = "sys", UserName = "系统用户" };
            }

            if (dicUserSession == null)
            {
                dicUserSession = new Dictionary<string, UserSession>();
            }
            else
            {
                foreach (KeyValuePair<string, UserSession> item in dicUserSession)
                {
                    if (item.Value.UserId == uSession.UserId)
                    {
                        return item.Value.SessionId;
                    }
                }
            }
            uSession.SessionId = Guid.NewGuid().ToString();
            dicUserSession.Add(uSession.SessionId, uSession);

            return uSession.SessionId;
        }

        /// <summary>
        /// 获取当前用户会话ID
        /// </summary>
        /// <param name="sesionID">会话ID</param>
        /// <returns></returns>
        public static UserSession GetCurrSession(string sesionID)
        {
            if (dicUserSession == null || dicUserSession.Count == 0)
            {
                sesionID = AddUserSession(null);
            }
            else if (dicUserSession[sesionID] == null)
            {
                sesionID = dicUserSession.Keys.First();
            }
            return dicUserSession[sesionID];
        }
    }
}
