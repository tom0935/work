using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Jasper.Modules
{
    public class CustomRoleProvider : RoleProvider
    {
        
        public override string[] GetRolesForUser(string userId)
        {
            Guid id;
            if(!Guid.TryParse(userId,out id))
            {
                return new String[]{string.Empty};
            }
            else
            {
                FormsIdentity uid = (FormsIdentity)HttpContext.Current.User.Identity;

                //取得FormsAuthenticationTicket物件
                FormsAuthenticationTicket ticket = uid.Ticket;

                //取得使用者的角色資料
                
                return new String[]{uid.Ticket.UserData};
                
            }
            
        }


        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}