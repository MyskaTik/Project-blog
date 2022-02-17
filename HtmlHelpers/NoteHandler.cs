using Backend_EF.ViewModels;
using Microsoft.Data.SqlClient;

namespace Backend_EF.HtmlHelpers
{
    public static class NoteHandler
    {
        public static string GetNoteTitleById(User user, int i)
        {
            //gets all titles of notes with specyfied ids
            List<int> Ids = GetListNotesByUserId(user);
            string result;
            string queryString = $"SELECT Title FROM Notes WHERE ID IN({Ids[i]})";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetString(0);
                reader.Close();
                connection.Close();
            }
            else
            {
                reader.Close();
                connection.Close();
                return "not found";
            }
            return result;
        }
        public static string GetNoteTitleByIdNote(Guid IdNote)
        {
            //gets all titles of notes with specyfied ids
            string result;
            string queryString = $"SELECT Title FROM Notes WHERE IdNote LIKE '{IdNote}'";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetString(0);
                reader.Close();
                connection.Close();
            }
            else
            {
                reader.Close();
                connection.Close();
                return "not found";
            }
            return result;
        }
        public static List<string> GetNoteBody(User user, int i)
        {
            //gets all notes with specyfied ids
            List<int> Ids = GetListNotesByUserId(user);
            string queryString = $"SELECT Body FROM Notes WHERE ID IN({Ids[i]})";
            List<string> notes = new List<string>();
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                notes.Add(reader.GetString(0));
                //in every iteration we create a new object SqlDataReader, therefore we have to close it before go to next iteration
                reader.Close();
            }
            else
                notes.Add("not found");
            connection.Close();
            return notes;
        }
        public static string GetNoteBodyByIdNote(Guid IdNote)
        {
            //gets all titles of notes with specyfied ids
            string result;
            string queryString = $"SELECT Body FROM Notes WHERE IdNote LIKE '{IdNote}'";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                result = reader.GetString(0);
                reader.Close();
                connection.Close();
            }
            else
            {
                reader.Close();
                connection.Close();
                return "not found";
            }
            return result;
        }
        public static int GetRowsCount(User user)
        {
            int rows;
            string queryString = $"SELECT COUNT(*) FROM Notes WHERE IdCode LIKE '{user.IdCode}'";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand cmd = new SqlCommand(queryString, connection);
            connection.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                rows = reader.GetInt32(0);
            else
                rows = -1;
            connection.Close();
            return rows;
        }
        public static Guid GetNoteId(string noteBody)
        {
            Guid idNote;
            string queryString = $"SELECT IdNote FROM Notes WHERE Body LIKE '{noteBody}'";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand cmd = new SqlCommand(queryString, connection);
            connection.Open();
            cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
                idNote = reader.GetGuid(0);
            else
                idNote = Guid.Empty;
            connection.Close();
            return idNote;
        }
        private static List<int> GetListNotesByUserId(User user)
        {
            List<int> Ids = new List<int>();    
            string queryString = $"SELECT ID FROM Notes WHERE IdCode LIKE '{user.IdCode}'";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //adds all ids of user`s notes
                    Ids.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            connection.Close();
            return Ids;
        }
        
    }
}
