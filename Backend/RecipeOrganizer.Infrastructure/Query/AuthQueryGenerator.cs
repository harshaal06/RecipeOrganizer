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
        public string GetUserQuery(string email= "", string userName = "", string userId = "")
        {
            StringBuilder query = new StringBuilder();

            query.Append("SELECT COUNT(*) ");
            query.Append("FROM Users ");
            query.Append("WHERE IsActive = 1 ");
            if (!string.IsNullOrEmpty(email))
                query.AppendFormat("AND Email = '{0}' ", email);

            if (!string.IsNullOrEmpty(userName))
                query.AppendFormat("AND UserName = '{0}' ", userName);

            if (!string.IsNullOrEmpty(userId))
                query.AppendFormat("AND EntityId <> '{0}' ", userId);

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

            query.Append("SELECT * FROM (");
            query.Append("SELECT ");
            query.AppendFormat("(SELECT Id FROM Users WHERE UserName = '{0}' LIMIT 1) AS UserId, ", userName);
            query.AppendFormat("(SELECT Id FROM Roles WHERE Name = '{0}' LIMIT 1) AS RoleId ", roleName);
            query.Append(") t ");
            query.Append("WHERE t.UserId IS NOT NULL ");
            query.Append("AND t.RoleId IS NOT NULL");

            return query.ToString();
        }
        // public string GetUserIdAndRoleIdQuery(string userName, string roleName)
        // {
        //     StringBuilder query = new StringBuilder();

        //     query.Append("SELECT ");
        //     query.Append("u.Id AS UserId, ");
        //     query.Append("r.Id AS RoleId ");
        //     query.Append("FROM Users u ");
        //     query.Append("CROSS JOIN Roles r ");
        //     query.AppendFormat(
        //         "WHERE u.UserName = '{0}' " +
        //         "AND r.Name = '{1}' " +
        //         "AND u.IsActive = 1 " +
        //         "AND r.IsActive = 1",
        //         userName,
        //         roleName);

        //     return query.ToString();
        // }

        public string InsertUserQuery(User user)
        {
            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO Users ");
            query.Append("(");
            query.Append("EntityId,");
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

            query.AppendFormat("'{0}',", user.UserId);
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

        public string AssignRoleQuery(int userId, int roleId)
        {
            StringBuilder query = new StringBuilder();

            query.Append("INSERT INTO UserRoles ");
            query.Append("(UserId, RoleId) ");
            query.AppendFormat("VALUES ({0},{1})",
                userId,
                roleId);

            return query.ToString();
        }

        public string GetUserForLogin(string userNameOrEmail)
        {
            StringBuilder query = new();

            query.Append("SELECT ");
            query.Append("u.Id, u.EntityId, ");
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

            return query.ToString();
        }

        public string GetUserProfileQuery(UserProfileRequest request)
        {
            StringBuilder query = new();

            string users = string.Join(",", request.UserNames.Select(x => $"'{x}'"));

            query.Append("SELECT ");
            query.Append("u.Id, u.EntityId, ");
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
            query.Append("u.Id, u.EntityId, ");
            query.Append("u.FirstName, ");
            query.Append("u.LastName, ");
            query.Append("u.UserName, ");
            query.Append("u.Email");

            return query.ToString();
        }

        public string GetRolesQuery()
        {
            StringBuilder query = new();

            query.Append("SELECT Name ");
            query.Append("FROM Roles ");
            query.Append("WHERE IsActive = 1 ");
            query.Append("ORDER BY Name");

            return query.ToString();
        }

        public string InsertRolesQuery(List<RoleRequest> roles, string createdBy)
        {
            StringBuilder query = new();

            query.Append("INSERT INTO Roles ");
            query.Append("(EntityId, Name, Description, CreatedBy, CreatedAt, IsActive) ");
            query.Append("VALUES ");

            query.Append(string.Join(",",
                roles.Select(role => $"('{Guid.NewGuid()}','{role.Name}','{role.Description}','{createdBy}',NOW(),1)")
            ));

            return query.ToString();
        }

        public string RemoveRoleQuery(string roleName)
        {
            StringBuilder query = new();
            query.Append("UPDATE Roles ");
            query.Append("SET IsActive = 0 ");
            query.AppendFormat("WHERE Name = '{0}'", roleName);
            return query.ToString();
        }

        public string GetUserPasswordQuery(string userName)
        {
            StringBuilder query = new();

            query.Append("SELECT PasswordHash FROM Users ");

            query.AppendFormat("WHERE UserName = '{0}' AND IsActive = 1", userName);

            return query.ToString();
        }
        public string UpdatePasswordQuery(string userName, string passwordHash)
        {
            StringBuilder query = new();

            query.Append("UPDATE Users ");
            query.AppendFormat("SET PasswordHash = '{0}' ", passwordHash);

            query.AppendFormat("WHERE UserName = '{0}'", userName);

            return query.ToString();
        }

        public string UpdateProfileQuery(string userId, UpdateProfileRequest request)
        {
            StringBuilder query = new();

            query.Append("UPDATE Users SET ");

            query.AppendFormat( "FirstName = '{0}', ", request.FirstName);

            query.AppendFormat(
                "LastName = '{0}', ",
                request.LastName);

            query.AppendFormat(
                "Email = '{0}', ",
                request.Email);

            query.AppendFormat(
                "UserName = '{0}' ",
                request.UserName);

            query.AppendFormat(
                "WHERE Id = '{0}'",
                userId);

            return query.ToString();
        }
    }
}
