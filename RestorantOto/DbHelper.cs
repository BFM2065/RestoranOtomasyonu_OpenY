using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;
namespace RestorantOto
{
    class DbHelper
    {

        public MySqlDataAdapter adtr = new MySqlDataAdapter();
        public MySqlCommand cmd = new MySqlCommand();
        private string connectionString = "Server=localhost;Database=restorantys;Uid=root;Pwd='1234Fm6789';AllowUserVariables=True;UseCompression=True";
        public MySqlConnection baglantiopen()
        {
            MySqlConnection baglan = new MySqlConnection(connectionString);
            baglan.Open();
            return baglan;
        }
        public MySqlConnection baglanticlose()
        {
            MySqlConnection baglan = new MySqlConnection(connectionString);
            baglan.Close();
            return baglan;
        }


        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection baglan = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, baglan))
                {
                    command.Parameters.AddRange(parameters);
                    baglan.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        public int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection baglan = baglantiopen())
            {
                using (MySqlCommand command = new MySqlCommand(query, baglan))
                {
                    command.Parameters.AddRange(parameters);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public DataTable ExecuteQuery(string query, params MySqlParameter[] parameters)
        {
            using (MySqlConnection baglan = baglantiopen())
            {
                using (MySqlCommand command = new MySqlCommand(query, baglan))
                {
                    command.Parameters.AddRange(parameters);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        }
        public string GetAlanAdi(int masaNumarasi)
        {
            string alanAdi = "";
            string alanQuery = "SELECT alanlar.AlanAdi " +
                               "FROM masasayisi " +
                               "JOIN alanlar ON masasayisi.AlanID = alanlar.kategori AND masasayisi.alanNo=alanlar.SıraNo " +
                               "WHERE masasayisi.ID = @masaNumarasi";

            using (MySqlConnection baglan = baglantiopen())
            {
                using (MySqlCommand command = new MySqlCommand(alanQuery, baglan))
                {
                    command.Parameters.AddWithValue("@masaNumarasi", masaNumarasi);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        alanAdi = result.ToString();
                    }
                }
            }

            return alanAdi;
        }
        public MySqlDataReader ExecuteReader(string query, params MySqlParameter[] parameters)
        {
            MySqlConnection baglan = baglantiopen();
            MySqlCommand command = new MySqlCommand(query, baglan);
            command.Parameters.AddRange(parameters);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

    }
}
