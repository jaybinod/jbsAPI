using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI
{
    public class UserSecurity
    {
        public static bool Login(string username, string password)
        {
            using (RestAPIDBEntities entities = new RestAPI.RestAPIDBEntities())
            {
                return entities.Users.Any(usr => usr.EmailAddress.Equals(username) && usr.Password == password);
            }
        }
    }
}