using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

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

            SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\musta\\OneDrive\\Desktop\\test typing\\prozha\\words.mdf\";Integrated Security=True;Connect Timeout=30");
            connection.Open();
             

            var modes = new[] { "easy", "normal", "hard" };
            List<string> results = new List<string>();

            foreach (var mode in modes)
            {
                string query =  "SELECT result FROM History WHERE difficulty = '"+mode+"'";
                SqlCommand command = new SqlCommand(query, connection); 
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string result = reader["result"].ToString();
                    results.Add(string.IsNullOrEmpty(result) ? "0" : result);
                }
                else
                {
                    results.Add("0");
                }

                reader.Close();
            }

            label5.Text = results[0];
            label6.Text = results[1];
            label7.Text = results[2];

            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
    }
}
