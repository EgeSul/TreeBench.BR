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
            List<int> dataList = new List<int>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT Value FROM TestNumbers WHERE DataType = 'Random' ORDER BY ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open(); 

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            { 
                                int sqlValue = reader.GetInt32(0);
                                dataList.Add(sqlValue);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[!] SQL Bağlantı Hatası: {ex.Message}");
                    }
                }
            }
            return dataList;
        }
    }
}