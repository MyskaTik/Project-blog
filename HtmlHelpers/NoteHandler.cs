using Backend_EF.ViewModels;
using Microsoft.Data.SqlClient;

namespace Backend_EF.HtmlHelpers
{
    public static class NoteHandler
    {
        public static string GetNoteTitle(User user, int i)
        {
            List<int> Ids = GetListIdUserWithNotes(user);
            string queryString = $"SELECT Title FROM Notes WHERE ID IN({Ids[i]})";
            SqlConnection connection = new SqlConnection(ApplicationContext.QUERYCONNECTION);
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.ExecuteNonQuery();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return reader.GetString(0);
            else
            {
                connection.Close();
                return string.Empty;
            }
        }
        public static List<string> GetNoteBody(User user, int i)
        {

            List<int> Ids = GetListIdUserWithNotes(user);
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
        private static List<int> GetListIdUserWithNotes(User user)
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
                    Ids.Add(reader.GetInt32(0));
                }
                reader.Close();
            }
            connection.Close();
            return Ids;//problem is here
            //Ids contains only 1 element, but msi has 2 notes
        }
    }
}
