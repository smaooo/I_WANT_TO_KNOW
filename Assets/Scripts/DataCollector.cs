using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;

namespace DataCollector
{
    public class Collector
    {

        SqlConnectionStringBuilder builder;

        string id;
        public Collector()
        {
            this.builder = new SqlConnectionStringBuilder();

            this.builder.DataSource = "iwk.cs8ubrihhsr2.us-east-1.rds.amazonaws.com";
            this.builder.UserID = "admin";
            this.builder.Password = "Sma77361520!";
            this.builder.InitialCatalog = "PlayersInfo";
            

            try
            {
                
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO Players VALUES (NEWID())", connection);

                    command.ExecuteNonQuery();
                    
                    using (command = new SqlCommand("SELECT * FROM Players WHERE id=(SELECT max(id) FROM Players)", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                this.id = reader.GetValue(0).ToString();
                            }
                        }
                    }

                    connection.Close();
                }

            }

            catch(SqlException e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void AddInput(string input, string correctedInput, int questionNumber, int currentQuestion, Manager.ScoringToData scoring)
        {
            input = "'" + input + "'";
            correctedInput = "'" + correctedInput + "'";
            try
            {
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    string com = string.Format("INSERT INTO Inputs VALUES ({0},{1},{2},{3},{4})", input, correctedInput, questionNumber, this.id,currentQuestion);
                    SqlCommand command = new SqlCommand(com, connection);
                    command.ExecuteNonQuery();
                    int inputID = -1;
                    com = "SELECT * FROM Inputs WHERE id=(SELECT max(id) FROM Inputs)";
                    using (command = new SqlCommand(com, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inputID = (int)reader.GetValue(0);
                            }
                        }
                    }

                    com = string.Format("INSERT INTO Inputs_Scoring VALUES ({0},{1},{2},{3},{4},{5})",
                        scoring.cat1, scoring.cat2, scoring.cat3, scoring.cat4, scoring.catS, inputID);
                    command = new SqlCommand(com, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch(SqlException e)
            {
                Debug.Log(e.ToString());
            }
        }


       
    }    
}
