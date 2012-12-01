using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace KinectPowerPointControl
{
    public class DataConnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DataConnection() 
        {
            Initialize();
        }
        private void Initialize()
        {
            server = "localhost";
            database = "hackathon";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
            
        }

        public String getPassword(String id)
        {
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "select pass from users where id = '"+id+"'";
            connection.Open();
            Reader = command.ExecuteReader();
            String output = "";
            while (Reader.Read())
            {
                string thisrow = "";
                for (int i = 0; i < Reader.FieldCount; i++)
                {
                    thisrow += Reader.GetValue(i).ToString();
                    output += thisrow;
                }


            }
            connection.Close();
            return output;
        }

        public void updateScore(String score,String id) 
        {
            connection.Open();
            String query = "UPDATE  game SET  score =  '"+score+"' WHERE  user_id =  '"+id+"'";
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            int numRowsUpdated = command.ExecuteNonQuery();
            connection.Close();
        }
        void getAll() 
        {
           
            MySqlCommand command = connection.CreateCommand();
            MySqlDataReader Reader;
            command.CommandText = "select * from users";
            connection.Open();
            Reader = command.ExecuteReader();
            while (Reader.Read())
            {
                string thisrow = "";
                for (int i = 0; i < Reader.FieldCount; i++) 
                {
                    thisrow += Reader.GetValue(i).ToString() + ",";
                    Console.WriteLine(thisrow);
                }
                    
                
            }
            connection.Close();
        }
    }
}
