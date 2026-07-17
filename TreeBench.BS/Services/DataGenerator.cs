using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace TreeBench.BS.Services
{
    public class DataGenerator
    {
                                               //(Server Name)
        private const string ConnectionString = "Server=DESKTOP-CQCV21I\\SQLEXPRESS;Database=TreeBenchDB;Trusted_Connection=True;";

        public List<int> FetchDataFromSql()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT Value FROM TestNumbers WHERE DataType = 'Random' ORDER BY ID";

                try
                {
                    return connection.Query<int>(query).AsList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] SQL Bağlantı Hatası: {ex.Message}");
                    return new List<int>(); 
                }
            }
        }
    }
}