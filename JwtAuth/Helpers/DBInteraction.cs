using System.Data.SqlClient;

namespace JwtAuth.Helpers
{
    public class DBInteraction
    {
        private string _connectionString;
        public SqlConnection SqlServerConnection { get; set; }

        public DBInteraction(string connectionString)
        {
            _connectionString = connectionString;
            CreateConnection(connectionString);
        }

        private void CreateConnection(string connectionString)
        {
            SqlServerConnection = new SqlConnection(_connectionString);
            SqlServerConnection.Open();
        }

        //Check whether yeild return needed or not
        public IEnumerable<object[]> DbFetchEntries(string query)
        {
            using (SqlCommand sqlCommand = new SqlCommand(query, SqlServerConnection))
            {
                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object[] arr = new object[reader.FieldCount];
                        reader.GetValues(arr);
                        yield return arr;
                    }
                }
            }
        }

        public bool UserExists(string tableName,string username) {
            string query = $"SELECT * FROM {tableName} WHERE UserName='{username}'";
            return this.DbFetchEntries(query).Any();           
        }
        public void WriteToUserInfoDb(string username,string passwordHash,string passwordSalt,string email,string phone)
        {
            string insertQuery = "INSERT INTO UserInfo (UserName,UserPasswordHash,UserPasswordSalt,UserEmailId,UserPhone)" +
                " VALUES (@UserName, @UserPasswordHash, @UserPasswordSalt,@UserEmailId,@UserPhone)";
            using (SqlCommand sqlCommand = new SqlCommand(insertQuery, SqlServerConnection))
            {
                sqlCommand.Parameters.AddWithValue("@UserName", username);
                sqlCommand.Parameters.AddWithValue("@UserPasswordHash", passwordHash);
                sqlCommand.Parameters.AddWithValue("@UserPasswordSalt", passwordSalt);
                sqlCommand.Parameters.AddWithValue("@UserEmailId", email);
                sqlCommand.Parameters.AddWithValue("@UserPhone", phone);

                sqlCommand.ExecuteNonQuery();
            }
        }





    }
}
