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
        public DbSet<User> Usersdata { get; set; }
        public DbSet<MessageModel> Messagedata { get; set; }
        public DbSet<NoteModel> Notes { get; set; }
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
            Usersdata?.Add(receivedUser);
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
        public void EditUserData(User changeableUser, User inputUser)
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
            string result = string.Empty;
            string queryString = $"SELECT Password FROM Usersdata WHERE IdCode LIKE '{IdCode}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetString(0);
                
            }
            else
                result = "password not found";
            reader.Close();
            return result;
        }//verified
        public string GetIdCode(User user)
        {
            //returns IdCode of specified user using model MessageModel, because that model is used in view
            string result = string.Empty;
            string queryString = $"SELECT IdCode FROM Usersdata WHERE Name LIKE '{user.Name}' AND Email LIKE '{user.Email}' AND Password LIKE '{user.Password}'";
            SqlConnection connection = new(QUERYCONNECTION);
            SqlCommand command = new(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                result = reader.GetString(0);
            else
                result = "IdCode not found";

            reader.Close();
            return result;
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
            reader.Close();
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
            if (!IsExistWithoutPassword(user))
                return "Name or email is wrong";
            if (ManyMessagesToOne(messageModel, QUERYCONNECTION))
                return "You have already sent message. Please wait while admin will read your message and answer you. Sincerely, administration";
            Messagedata.Add(messageModel);
            SaveChanges();
            return "Message was sent succesfully";
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

        }//verified, admin api
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
        }//verified, admin api
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
            string result = string.Empty;
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
                result = "user isn`t exist";

            reader.Close();
            return result;

        }//verified, admin api
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
        }//verified, admin api
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
                {
                    reader.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    return false;
                }
            }
            else
            {
                reader.Close();
                return false;
            }
        }//verified


        //handle notes
        public void CreateNote(NoteModel insertNoteModel, User user)
        {
            string queryString = $"INSERT INTO Notes (IdNote, IdCode, Title, Body)VALUES (NEWID(), '{user.IdCode}', '{insertNoteModel.Title}', '{insertNoteModel.Body}')";
            SqlConnection connection = new SqlConnection(QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }//verified
        public async Task CreateNoteAsync(NoteModel insertNoteModel, User user)
        { 
            string queryString = $"INSERT INTO Notes (IdNote, IdCode, Title, Body)VALUES (NEWID(), '{user.IdCode}', '{insertNoteModel.Title}', '{insertNoteModel.Body}')";
            SqlConnection connection = new SqlConnection(QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
            Task.CompletedTask.Wait();
        }//verified
        public void EditNote(NoteModel insertNoteModel, Guid deleteNoteId, User user)
        {
            string queryString = $"DELETE FROM Notes WHERE IdNote LIKE '{deleteNoteId}' INSERT INTO Notes(IdNote, IdCode, Title, Body) VALUES (NEWID(), '{user.IdCode}', '{insertNoteModel.Title}', '{insertNoteModel.Body}')";
            SqlConnection connection = new SqlConnection(QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public async Task EditNoteAsync(NoteModel insertNoteModel, NoteModel deleteNoteModel, User user)
        {
            string queryString = $"DELETE FROM Notes WHERE IdNote LIKE '{deleteNoteModel.IdNote}' INSERT INTO Notes(IdNote, IdCode, Title, Body) VALUES ('{new Guid()}', '{user.IdCode}', '{insertNoteModel.Title}', '{insertNoteModel.Body}')";
            SqlConnection connection = new SqlConnection(QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }
        public void DeleteNote(NoteModel deleteNoteModel)
        {
            string queryString = $"DELETE FROM Notes WHERE IdNote LIKE '{deleteNoteModel.IdNote}'";
            SqlConnection connection = new SqlConnection(QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }//verified
        public async Task DeleteNoteAsync(NoteModel deleteNoteModel)
        {
            string queryString = $"DELETE FROM Notes WHERE IdNote LIKE '{deleteNoteModel.IdNote}'";
            SqlConnection connection = new SqlConnection(QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
            Task.CompletedTask.Wait();
        }//verified
    }
}
