using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeOrganizer.Domain.Entity;

namespace RecipeOrganizer.Infrastructure.Query
{
    public class AuthQueryGenerator
    {
        public string GetUserByEmail(string email)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT COUNT(*) ");
            query.Append("FROM Users ");
            query.AppendFormat("WHERE Email = '{0}'", email);

            return query.ToString();
        }

        public string GetUserByUserName(string userName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT COUNT(*) ");
            query.Append("FROM Users ");
            query.AppendFormat("WHERE UserName = '{0}'", userName);

            return query.ToString();
        }

        public string GetRoleIdByName(string roleName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT Id ");
            query.Append("FROM Roles ");
            query.AppendFormat("WHERE Name = '{0}'", roleName);

            return query.ToString();
        }

        public string InsertUser(User user)
        {
            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO Users ");
            query.Append("(");
            query.Append("Id,");
            query.Append("FirstName,");
            query.Append("LastName,");
            query.Append("UserName,");
            query.Append("Email,");
            query.Append("PasswordHash,");
            query.Append("IsEmailVerified,");
            query.Append("IsActive,");
            query.Append("CreatedAt");
            query.Append(")");

            query.Append(" VALUES(");

            query.AppendFormat("'{0}',", user.Id);
            query.AppendFormat("'{0}',", user.FirstName);
            query.AppendFormat("'{0}',", user.LastName);
            query.AppendFormat("'{0}',", user.UserName);
            query.AppendFormat("'{0}',", user.Email);
            query.AppendFormat("'{0}',", user.PasswordHash);

            query.Append("0,");
            query.Append("1,");
            query.AppendFormat("'{0}'", DateTime.Now.ToString("s"));

            query.Append(")");

            return query.ToString();
        }

        public string AssignRole(string userId, string roleId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO UserRoles ");
            query.Append("(UserId, RoleId) ");
            query.AppendFormat("VALUES ('{0}','{1}')",
                userId,
                roleId);

            return query.ToString();
        }
    }
}
