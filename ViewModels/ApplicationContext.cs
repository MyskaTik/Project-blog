using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Backend_EF.ViewModels
{
    public class ApplicationContext : DbContext
    {
        public const string QUERYCONNECTION = "Server=localhost\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Users;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";
        private const string ADMINEMAIL = "maximkirichenk0.06@gmail.com";
        private const string ADMINNAME = "Admin";
        private const string ALPHABET = "abdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string QueryConnectionPublic { get; set; } = "Server=localhost\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Users;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";
        public DbSet<User>? Usersdata { get; set; }
        public DbSet<ScoreModel>? Scoredata { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=localhost\\SQLEXPRESS;Data Source=maxim;Initial Catalog=Users;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False");
        }


        //handle user data
        public bool AddUser([Bind] User receivedUser)
        {
            if (IsExist(receivedUser))//if user isn`t exist
                return false;
            receivedUser.IdCode = GenerateIdCode(ALPHABET, 20);
            ScoreModel scoreUser = new()
            {
                Name = receivedUser.Name,
                Score = 0
            };
            Usersdata?.Add(receivedUser);
            Scoredata?.Add(scoreUser);
            SaveChanges();
            return true;
        }//verified
        public string LoginUser([Bind] User receivedUser)
        {
            if (IsExist(receivedUser))
                return GetRole(receivedUser);
            else
                return false.ToString();
        }//verified
        public void UpdateUserData(User changeableUser, User inputUser)
        {
            //updates user`s data
            string queryString = $"DELETE FROM Usersdata WHERE Name LIKE '{changeableUser.Name}' AND Email LIKE '{changeableUser.Email}' AND Password LIKE '{changeableUser.Password}' INSERT INTO Usersdata VALUES (3,'user','{inputUser.Name}','{inputUser.Email}','{inputUser.Password}', '{changeableUser.IdCode}')";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }//verified
        public bool IsExist(User receivedUser)
        {
            //returns true if user exist
            if (Usersdata.Any(user => user.Name == receivedUser.Name && user.Email == receivedUser.Email && user.Password == receivedUser.Password))
                return true;
            else
            {
                if (GetRole(receivedUser) == null)
                    SetRole(QUERYCONNECTION);
                return false;
            }

        }//verified
        public bool IsExistWithoutPassword(User receivedUser)
        {
            //this method using at 1 stage of recovering password and check is exist that user without using password
            if (Usersdata.Any(user => user.Name == receivedUser.Name && user.Email == receivedUser.Email))
                return true;
            else
                return false;

        }//verified
        public bool IsExistWithIdCode(User receivedUser)
        {
            //this method using at 2 stage of recovering password and check is exist user with that IdCode or not
            if (Usersdata.Any(user => user.Name == receivedUser.Name && user.Email == receivedUser.Email && user.IdCode == receivedUser.IdCode))
                return true;
            else
                return false;

        }//verified
        public string GetPassword(string IdCode)
        {
            //returns password by specified user`s IdCode
            string queryString = $"SELECT Password FROM Usersdata WHERE IdCode LIKE '{IdCode}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return reader.GetString(0);
            else
                return "password not found";
        }//verified
        public string GetIdCode(string name, string email, string password)
        {
            //returns IdCode of specified user using model MessageModel, because that model is used in view
            string queryString = $"SELECT IdCode FROM Usersdata WHERE Name LIKE '{name}' AND Email LIKE '{email}' AND Password LIKE '{password}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return reader.GetString(0);
            else
                return "IdCode not found";
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
        private string GetRole(User receivedUser)
        {
            //returns role of the selected user
            string result = "";
            string query = $"SELECT Name, RoleName FROM Usersdata WHERE Name LIKE '{receivedUser.Name}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(query, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result += reader.GetString(1);
            }
            connection.Close();
            return result;
        }//verified
        private string GenerateIdCode(string Alphabet, int Length)
        {
            //generating IdCode on registration`s stage
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder(Length - 1);
            int Position;

            for (int i = 0; i < Length; i++)
            {
                Position = rnd.Next(0, Alphabet.Length - 1);
                sb.Append(Alphabet[Position]);
            }
            return sb.ToString();
        }//verified


        //handle message data
        public string SendMessage([Bind] User user, MessageModel messageModel)
        {
            //sending message from user name
            string queryString = $"INSERT INTO Messagedata(Name, Email, Message, ToEmail) VALUES('{messageModel.Name}','{messageModel.Email}','{messageModel.Message}', '{ADMINEMAIL}')";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            try
            {
                if (!IsExistWithoutPassword(user))
                    return "Name or email is wrong";
                if (ManyMessagesToOne(messageModel, QUERYCONNECTION))
                    return "You have already sent message. Please wait while admin will read your message and answer you. Sincerely, administration";

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                return "Message was sent succesfully";
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return ex.Message.ToString();
            }
            finally
            {
                connection.Close();
            }

        }//verified
        public string SendMessageFromAdmin([Bind] User user, MessageModel messageModel)
        {
            //sending message from administration name
            string queryString = $"INSERT INTO Messagedata(Name, Email, Message, ToEmail) VALUES('{ADMINNAME}','{ADMINEMAIL}','{messageModel.Message}', '{messageModel.Email}')";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            try
            {
                if (!IsExistWithoutPassword(user))
                    return "Name or email is wrong";
                if (ManyMessagesToOne(messageModel, QUERYCONNECTION))
                    return "you sent more than 1 message to user, please delete your message when 3 days are went";

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                return "Message successfully sent";
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                return ex.Message.ToString();
            }
            finally
            {
                connection.Close();
            }

        }//verified
        public string GetMessageFromAdmin([Bind] User user, MessageModel messageModel)
        {
            //gets message only if it from administration. this method using for getting message to user
            string queryString = $"SELECT Name, Email, Message FROM Messagedata WHERE Name LIKE 'Admin' AND Email LIKE 'maximkirichenk0.06@gmail.com' AND ToEmail LIKE '{messageModel.Email}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            try
            {
                if (!IsExistWithoutPassword(user))
                    return "name or email is wrong";
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetString(2) is not null)
                        return reader.GetString(2);
                    else
                        return "you haven`t any message from administration yet";
                }
                else
                    return "there aren't any message";
            }
            catch (System.Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return ex.Message.ToString();
            }
            finally
            {
                connection.Close();
            }
        }//verified
        public string GetMessage([Bind] MessageModel messageModel)
        {
            //gets any message
            string queryString = $"SELECT Name, Email, Message FROM Messagedata WHERE Name LIKE 'Admin' AND Email LIKE 'maximkirichenk0.06@gmail.com'";
            string result = $"Name: {messageModel.Name}\nEmail: {messageModel.Email}\nMessage: ";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            try
            {
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader.GetString(2) is not null)
                        result = result + reader.GetString(2);
                    else
                        return "this user didn`t send any message";
                }
                else
                    return "there aren't any message ";
                return result;
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                return ex.Message.ToString();
            }
            finally
            {
                connection.Close();
            }
        }//verified
        public string GetUser([Bind] User user)
        {
            //gets all info about specific user
            List<string> listOfAuth = new List<string>()//list which contain headers of main columns of db
            {
                "Name",
                "Email",
                "Password",
                "RoleName"
            };
            string result = "";
            string queryString = $"SELECT Name, Email, Password, RoleName FROM Usersdata WHERE Name LIKE '{user.Name}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                for (int i = 0; i <= 3; i++)
                {
                    result += $"{listOfAuth[i]}: {reader.GetString(i)} \n";
                }
                return result;
            }
            else
                return "user isn`t exist";

        }//verified
        public string DeleteMessage([Bind] User user, string connectionString)
        {
            //delete message from somebody in the db if it necessary
            string querySelect = $"DELETE FROM Messagedata WHERE Name LIKE '{user.Name}' AND Email LIKE '{user.Email}' SELECT COUNT(Name) FROM Messagedata WHERE Name LIKE '{user.Name}' AND Email LIKE '{user.Email}'";
            SqlConnection connection = new(connectionString);
            SqlCommand command = new(querySelect, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (reader.GetInt32(0) == 0)
                    return true.ToString();
                else
                    return false.ToString();
            }
            else
                return false.ToString();
        }//verified
        public async void SendMail([Bind] MessageModel messageModel)
        {
            string password = GetPassword(messageModel.Name);
            MailAddress to = new("messageModel.Email");//куда приходит сообщение
            MailAddress from = new("faqblogms@gmail.com", "BlogAdministration");//откуда будут отправляться сообщения
            MailMessage userMessage = new(from, to);//создаем образ сообщения
            userMessage.Body = messageModel.Message;
            SmtpClient client = new("smtp.gmail.com", 44352)
            {

                EnableSsl = true,
                Credentials = new NetworkCredential("faqblogms@gmail.com", password)
            }; //создаем экземпляр клиента 
            await client.SendMailAsync(userMessage);//отправляем сообщение
        }//verified
        private bool ManyMessagesToOne([Bind] MessageModel messageModel, string connectionString)
        {
            //if there are a lot of messages to one user, method returns true
            string querySelect = $"SELECT COUNT(ToEmail) FROM Messagedata WHERE ToEmail LIKE '{messageModel.ToEmail}' AND Email LIKE '{messageModel.Email}'";
            SqlConnection connection = new(connectionString);
            SqlCommand command = new(querySelect, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                if (reader.GetInt32(0) == 1)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }//verified


        //handle score data
        public int GetScore(ScoreModel scoreModel, User user)
        {
            string queryString = $"SELECT Score FROM Scoredata WHERE Name LIKE '{scoreModel.Name}'";
            if (Usersdata.Any(dbUser => dbUser.Name == user.Name))
            {
                SqlConnection connection = new SqlConnection(QUERYCONNECTION);
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int score = reader.GetInt32(0);
                    connection.Close();
                    return score;
                }
                else
                {
                    connection.Close();
                    return -1;
                }
            }
            else
                return -1;
        }//verified
    }
}
