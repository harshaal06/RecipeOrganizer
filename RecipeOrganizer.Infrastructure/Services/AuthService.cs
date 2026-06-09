using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using RecipeOrganizer.Domain.Entity;
using RecipeOrganizer.Domain.Services;
using RecipeOrganizer.Infrastructure.Query;

namespace RecipeOrganizer.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly SQLHelper _sqlHelper;
    private readonly AuthQueryGenerator _queryGenerator;

    public AuthService( IConfiguration configuration)
    {
        _configuration = configuration;
        _sqlHelper = new SQLHelper();
        _queryGenerator = new AuthQueryGenerator();
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        RegisterResponse response = new RegisterResponse();

        SQLHelper sqlHelper = new SQLHelper();

        AuthQueryGenerator queryGenerator = new AuthQueryGenerator();

        string connectionString = _configuration.GetConnectionString("RecipeOrganizerDB");
        try
        {
            string emailQuery = queryGenerator.GetUserByEmail(request.Email);

            int emailCount = sqlHelper.ExecuteScalar(emailQuery, connectionString);

            if (emailCount > 0)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Email already exists";
                return response;
            }

            string userNameQuery = queryGenerator.GetUserByUserName(request.UserName);

            int userCount = sqlHelper.ExecuteScalar(userNameQuery, connectionString);

            if (userCount > 0)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Username already exists";
                return response;
            }

            User user = new User();

            user.Id = Guid.NewGuid().ToString();

            user.FirstName = request.FirstName;

            user.LastName = request.LastName;

            user.UserName = request.UserName;

            user.Email = request.Email;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            string insertUserQuery = queryGenerator.InsertUser(user);

            sqlHelper.ExecuteNonQuery(insertUserQuery, connectionString);

            string roleQuery = queryGenerator.GetRoleIdByName("User");

            string roleId = string.Empty;

            using (MySqlDataReader reader = sqlHelper.ExecuteQuery(roleQuery, connectionString))
            {
                while (reader.Read())
                {
                    roleId = SQLHelper.GetStringValue(reader, "Id");
                }
            }

            string roleAssignQuery = queryGenerator.AssignRole(user.Id, roleId);

            sqlHelper.ExecuteNonQuery(roleAssignQuery, connectionString);

            response.UserId = user.Id;
            response.ResponseCode = 200;
            response.ResponseMessage = "Registration Successful";
        }
        catch (Exception ex)
        {
            response.ResponseCode = 500;
            response.ResponseMessage = ex.Message;
        }
        finally
        {
            sqlHelper.CloseSqlConnection();
        }

        return response;
    }
}
