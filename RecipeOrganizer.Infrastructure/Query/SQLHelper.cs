using MySqlConnector;
using System.Data;

namespace RecipeOrganizer.Infrastructure.Query
{
    public class SQLHelper
    {
        private MySqlConnection connection;

        /// <summary>
        /// Function use for initialised SqlConnection and open sql connetion.
        /// </summary>
        private MySqlConnection OpenSqlConnection(string connectionString)
        {
            if (connection != null && connection.ConnectionString.Equals(connectionString))
            {
                return connection;
            }
            else
            {
                CloseSqlConnection();
            }

            connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        /// <summary>
        ///  Function used for close sql connetion.
        /// </summary>
        public void CloseSqlConnection()
        {
            if (connection != null)
                connection.Close();
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>The query.</returns>
        /// <param name="query">Query.</param>
        public MySqlDataReader ExecuteQuery(string query, string connectionString)
        {

            connection = OpenSqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Connection = connection; //Pass the connection object to Command   
            return command.ExecuteReader();
        }

        public int ExecuteNonQuery(string query, string connectionString)
        {

            connection = OpenSqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Connection = connection; //Pass the connection object to Command   
            return command.ExecuteNonQuery();
        }

        public int ExecuteScalar(string query, string connectionString)
        {
            connection = OpenSqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Connection = connection; //Pass the connection object to Command   
            int a = Convert.ToInt32(command.ExecuteScalar());
            return a;
        }

        public static string GetStringValue(MySqlDataReader reader, string columnName)
        {
            try
            {
                if (reader[columnName] != null && reader[columnName] != System.DBNull.Value)
                    return Convert.ToString(reader[columnName]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("logging ex for " + columnName);
                Console.WriteLine(ex);
            }
            return "";
        }

        public static Object GetDateValue(MySqlDataReader reader, string column)
        {

            try
            {
                if (reader[column] != null && reader[column] != System.DBNull.Value)
                    return (DateTime)reader[column];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static int GetIntValue(MySqlDataReader reader, string columnName)
        {
            try
            {
                if (reader[columnName] != null && reader[columnName] != System.DBNull.Value)
                    return reader.GetInt32(reader.GetOrdinal(columnName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }


        public static bool GetBoolValue(MySqlDataReader reader, string columnName)
        {
            try
            {
                if (reader[columnName] != null && reader[columnName] != System.DBNull.Value)
                    return (bool)reader[columnName];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }


        //public static Object GetDateValue(SqlDataReader reader, string column)
        //{

        //    try
        //    {
        //        return Convert.ToDateTime(reader[column]);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return null;
        //}

        public static double GetDoubleValue(MySqlDataReader reader, string columnName)
        {
            try
            {
                if (reader[columnName] != null && reader[columnName] != System.DBNull.Value)
                    return ((double)reader[columnName]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }

        public static decimal GetDecimalValue(MySqlDataReader reader, string columnName)
        {
            try
            {
                if (reader[columnName] != null && reader[columnName] != System.DBNull.Value)
                    return ((decimal)reader[columnName]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return 0;
        }
    }
}

