using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignInSignUp.Helpers
{
    public static class SessionManager
    {
        private const string UserIdKey = "UserId";
        private const string UserNameKey = "UserName";

        public static int UserId
        {
            get { return GetSessionValue<int>(UserIdKey); }
            set { SetSessionValue(UserIdKey, value); }
        }

        public static string UserName
        {
            get { return GetSessionValue<string>(UserNameKey); }
            set { SetSessionValue(UserNameKey, value); }
        }

        private static T GetSessionValue<T>(string key)
        {
            if (HttpContext.Current.Session[key] != null)
            {
                return (T)HttpContext.Current.Session[key];
            }

            return default(T);
        }

        private static void SetSessionValue<T>(string key, T value)
        {
            HttpContext.Current.Session[key] = value;
        }
    }
}