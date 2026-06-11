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
        public string GetUserByEmailQuery(string email)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT COUNT(*) ");
            query.Append("FROM Users ");
            query.AppendFormat("WHERE Email = '{0}'", email);

            return query.ToString();
        }

        public string GetUserByUserNameQuery(string userName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT COUNT(*) ");
            query.Append("FROM Users ");
            query.AppendFormat("WHERE UserName = '{0}'", userName);

            return query.ToString();
        }

        public string GetRoleIdByNameQuery(string roleName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT Id ");
            query.Append("FROM Roles ");
            query.AppendFormat("WHERE IsActive = 1 and Name = '{0}'", roleName);

            return query.ToString();
        }
        public string GetUserIdAndRoleIdQuery(string userName, string roleName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT ");
            query.Append("u.Id AS UserId, ");
            query.Append("r.Id AS RoleId ");
            query.Append("FROM Users u ");
            query.Append("CROSS JOIN Roles r ");
            query.AppendFormat(
                "WHERE u.UserName = '{0}' " +
                "AND r.Name = '{1}' " +
                "AND u.IsActive = 1 " +
                "AND r.IsActive = 1",
                userName,
                roleName);

            return query.ToString();
        }

        public string InsertUserQuery(User user)
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

        public string AssignRoleQuery(string userId, string roleId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO UserRoles ");
            query.Append("(UserId, RoleId) ");
            query.AppendFormat("VALUES ('{0}','{1}')",
                userId,
                roleId);

            return query.ToString();
        }

        public string GetUserForLogin(string userNameOrEmail)
        {
            StringBuilder query = new();

            query.Append("SELECT ");
            query.Append("u.Id, ");
            query.Append("u.FirstName, ");
            query.Append("u.LastName, ");
            query.Append("u.UserName, ");
            query.Append("u.Email, ");
            query.Append("u.PasswordHash, ");
            query.Append("r.Name AS Role ");

            query.Append("FROM Users u ");

            query.Append("INNER JOIN UserRoles ur ");
            query.Append("ON u.Id = ur.UserId ");

            query.Append("INNER JOIN Roles r ");
            query.Append("ON ur.RoleId = r.Id ");

            query.AppendFormat(
                "WHERE u.IsActive = 1 and ( u.Email = '{0}' OR u.UserName = '{0}')",
                userNameOrEmail);

            return query.ToString();
        }

        public string GetRolesByUserNameQuery(string userName)
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT r.Name AS RoleName ");
            query.Append("FROM Users u ");
            query.Append("INNER JOIN UserRoles ur ON u.Id = ur.UserId ");
            query.Append("INNER JOIN Roles r ON ur.RoleId = r.Id ");
            query.AppendFormat("WHERE u.UserName = '{0}' ", userName);
            query.Append("AND u.IsActive = 1 ");
            query.Append("AND r.IsActive = 1");

            return query.ToString();
        }

        public string GetUserProfileQuery(UserProfileRequest request)
        {
            StringBuilder query = new();

            string users = string.Join(",", request.UserNames.Select(x => $"'{x}'"));

            query.Append("SELECT ");
            query.Append("u.Id, ");
            query.Append("u.FirstName, ");
            query.Append("u.LastName, ");
            query.Append("u.UserName, ");
            query.Append("u.Email, ");
            query.Append("GROUP_CONCAT(r.Name) AS Roles ");

            query.Append("FROM Users u ");

            query.Append("LEFT JOIN UserRoles ur ");
            query.Append("ON u.Id = ur.UserId ");

            query.Append("LEFT JOIN Roles r ");
            query.Append("ON ur.RoleId = r.Id ");

            query.AppendFormat("WHERE u.UserName IN ({0}) and u.IsActive = 1 ", users);

            query.Append("GROUP BY ");
            query.Append("u.Id, ");
            query.Append("u.FirstName, ");
            query.Append("u.LastName, ");
            query.Append("u.UserName, ");
            query.Append("u.Email");

            return query.ToString();
        }

    }
}
