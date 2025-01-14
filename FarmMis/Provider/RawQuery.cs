using AAAErp.IProvider;
using AAAErp.Models;
using AAAErp.Utilities;
using AAAErp.ViewModel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Policy;

namespace AAAErp.Provider
{
    public class RawQuery : IRawQuery
    {

        private CoreDbContext _context;
        public RawQuery(CoreDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteCommand(DbConnectionVm dbConnection)
        {
           
            string connetionString = $"server={dbConnection.Server};user={dbConnection.UserName};pwd={Decryptor.Decrypt(dbConnection.Password)};database={dbConnection.Db};";
            MySqlConnection connection = new MySqlConnection(connetionString);
            var sqlCommand = new MySqlCommand(dbConnection.Query, connection);
            sqlCommand.Connection = connection;
            sqlCommand.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
            connection.Close();
        }

        public async Task<DataTable> ExecuteSqlQuery(DbConnectionVm dbConnection)
        {
            string connetionString = $"server={dbConnection.Server};user={dbConnection.UserName};pwd={Decryptor.Decrypt(dbConnection.Password)};database={dbConnection.Db};";
            MySqlConnection connection = new MySqlConnection(connetionString);
            var sqlCommand = new MySqlCommand(dbConnection.Query, connection);
            sqlCommand.Connection = connection;
            sqlCommand.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var reader = sqlCommand.ExecuteReader();
            var table = new DataTable();
            table.BeginLoadData();
            table.Load(reader);
            table.EndLoadData();

            sqlCommand.Dispose();
            connection.Close();

            return table;
        }

    }
}
