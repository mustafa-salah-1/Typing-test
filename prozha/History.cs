using System;
using System.Collections.Generic;  
using System.Windows.Forms;
using System.Data.SQLite;

namespace prozha
{
    public partial class History : Form
    {
        public History()
        {
            InitializeComponent();
        }
         
        private void History_Load(object sender, EventArgs e)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=typing_test.db"))
            {
                connection.Open();

                var modes = new[] { "Easy", "Normal", "Hard" };
                List<string> results = new List<string>();

                foreach (var mode in modes)
                {
                    string query = "SELECT result FROM History WHERE difficulty = @mode";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@mode", mode);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string result = reader["result"].ToString();
                                results.Add(string.IsNullOrEmpty(result) ? "0" : result);
                            }
                            else
                            {
                                results.Add("0");
                            }
                        }
                    }
                }

                label5.Text = results[0];
                label6.Text = results[1];
                label7.Text = results[2];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
    }
}
