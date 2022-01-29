using Backend_EF.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Backend_EF.Helpers
{

    public class Entrance : DbContext
    {
        public ApplicationContext db { get; set; }
        public Entrance(ApplicationContext app)
        {
            db = app;
        }
        public const string QUERYCONNECTION = "Server=localhost\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Users;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";

        public string Login([Bind] User receivedUser)
        {
            //query do a sample of db and return true or false as result
            if (db.Usersdata.Any(user => user.Name == receivedUser.Name && user.Email == receivedUser.Email && user.Password == receivedUser.Password))
                return true.ToString();
            else
                return false.ToString();
        }//verified
        public bool Register([Bind] User receivedUser)
        {
            //adding into db new user

            if (db.Usersdata.Any(user => user.Name == receivedUser.Name && user.Email == receivedUser.Email && user.Password == receivedUser.Password))
            {
                db.Usersdata.Add(receivedUser);
                SetRole(QUERYCONNECTION);
                return true;
            }
            else
                return false;
        }//verified
        public bool IsExist([Bind] User user)
        {
            if (db.Usersdata.Any(receivedUser => receivedUser.Name == user.Name && receivedUser.Email == user.Email))
                return false;
            else
                return true;
        }//verified
        private void SetRole(string connectionString)
        {
            //setting role. defalut is "user"
            string queryUpd = "UPDATE Usersdata SET RoleName = 'admin' WHERE RoleID = 1 UPDATE Usersdata SET RoleName = 'moderator' WHERE RoleID = 2 UPDATE Usersdata SET RoleName = 'user' WHERE RoleID = 3";
            SqlConnection connection = new(connectionString);
            SqlCommand command = new(queryUpd, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }//verified
        private bool IsAdmin([Bind] User user, string connectionString)
        {
            string queryIsAdmin = $"SELECT Name, Email, Password, CASE  WHEN Name LIKE '{user.Name}' AND Email LIKE '{user.Email}' AND Password LIKE '{user.Password}' THEN  CASE  WHEN RoleName LIKE 'admin' THEN 'TRUE' ELSE 'FALSE' END ELSE 'FALSE' END AS result FROM Usersdata ORDER BY result DESC";
            SqlConnection connection = new(connectionString);
            SqlCommand command = new(queryIsAdmin, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetString(3) == "TRUE")
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (System.Exception)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return false;
            }
            finally
            {
                connection.Close();
            }
        }//verified
        private bool IsModerator([Bind] User user, string connectionString)
        {
            string queryIsAdmin = $"SELECT Name, Email, Password, CASE WHEN RoleName LIKE 'moderator' THEN CASE WHEN Name LIKE '{user.Name}' AND Email LIKE '{user.Email}' AND Password LIKE '{user.Password}' THEN 'TRUE' ELSE 'FALSE' END ELSE 'FALSE' END AS result FROM Usersdata ORDER BY result DESC";
            SqlConnection connection = new(connectionString);
            SqlCommand command = new(queryIsAdmin, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetString(3) == "TRUE")
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (System.Exception)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return false;
            }
            finally
            {
                connection.Close();
            }
        }//verified
    }
}
