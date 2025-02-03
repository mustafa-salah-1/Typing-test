using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace prozha
{
    public partial class Form1 : Form  
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\musta\\OneDrive\\Desktop\\test typing\\prozha\\words.mdf\";Integrated Security=True;Connect Timeout=30");
        private static int correct = 0;  
        private static int second = 5;
        Random rnd = new Random();
        public static string difficult;
        public static string mode;
        private void timer2_Tick(object sender, EventArgs e)
        {
            second--; 
             if(correct == 100)
             {
                timer2.Enabled = false;
                textBox1.Enabled = false;

                MessageBox.Show(correct.ToString() + " words. Best typing for this difficulty.", "Result");

                connection.Open();
                SqlCommand check = new SqlCommand("select difficulty from History where difficulty = '" + mode + "'", connection);
                SqlDataAdapter da = new SqlDataAdapter(check);
                DataTable dt = new DataTable();
                da.Fill(dt);
                 
                if (dt.Rows.Count > 0 )
                {
                    SqlCommand update = new SqlCommand("update History set difficulty='" + mode + "',result='" + correct + "' where difficulty='" + mode + "'", connection);
                    update.ExecuteNonQuery();
                    connection.Close();
                }
                else
                {
                    SqlCommand insert = new SqlCommand("insert into History (difficulty,result) values ('" + mode + "','" + correct + "')", connection);
                    insert.ExecuteNonQuery();
                    connection.Close();
                }
                connection.Close();
             }
             else if(second == 0)
             {
                timer2.Enabled = false;
                textBox1.Enabled = false;
                if (correct > 20)
                {
                    MessageBox.Show(correct.ToString() + " words. you are Legend type.", "Result");
                }
                else if (correct > 15)
                {
                    MessageBox.Show(correct.ToString() + " words. Very Good type.", "Result");
                }
                else if (correct > 5)
                {
                    MessageBox.Show(correct.ToString() + " words. Good type.", "Result");
                }
                else
                {
                    MessageBox.Show(correct.ToString() + " words. Bad type.", "Result");
                }

                connection.Open();
                SqlCommand check = new SqlCommand("select difficulty from History where difficulty = '" + mode + "'", connection);
                SqlDataAdapter da = new SqlDataAdapter(check);
                DataTable dt = new DataTable();
                da.Fill(dt);

               

                if (dt.Rows.Count > 0 )
                {
                    string query = "SELECT result FROM History WHERE difficulty = '" + mode + "'";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int data = int.Parse(reader["result"].ToString());
                    reader.Close();

                    if (correct > data)
                    {
                        SqlCommand update = new SqlCommand("update History set difficulty='" + mode + "',result='" + correct + "' where difficulty='" + mode + "'", connection);
                        update.ExecuteNonQuery();
                        connection.Close();
                    }
                } else
                { 
                    SqlCommand insert = new SqlCommand("insert into History (difficulty,result) values ('" + mode + "','" + correct + "')", connection);
                    insert.ExecuteNonQuery();
                    connection.Close();
                }
                connection.Close();
             }
        }
        Boolean make = false;
        private void timer1_Tick(object sender, EventArgs e)
        { 
            if (textBox1.Text.Length > 0 && make == false)
            {
                timer2.Enabled = true;
                make = true;
            }
            circularProgressBar2.Value = correct;
            circularProgressBar2.Text = correct.ToString();
            circularProgressBar1.Value = second;
            circularProgressBar1.Text = second.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            textBox1.Focus();
            label1.Text = ""; 
        }
        SqlDataReader read;
        

        private void start()
        {
            connection.Open();
            SqlCommand letter = new SqlCommand("select word from " + difficult + " where id = '" + rnd.Next(1, 101) + "'", connection);
            read = letter.ExecuteReader();
            read.Read();
            label1.Text = read["word"].ToString();
            read.Close();
            connection.Close();
            make = false;
            newgame();
        }
        private void newgame()
        {
            try
            {
                second = 5; 
                correct = 0; 
                textBox1.Text = "";
                textBox1.Enabled = true;
                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex);
            }
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficult = "letter_four";
            mode = "easy";
            start();
        }
        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficult = "letter_five";
            mode = "normal";
            start();
        } 
        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficult = "letter_six";
            mode = "hard";
            start();
        } 

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            string sql = "select word from "+difficult+" where id = '" + rnd.Next(1, 101) + "'";
            connection.Open();
            SqlCommand letter = new SqlCommand(sql, connection);
            read = letter.ExecuteReader();

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Suppress the "ding" sound

                if (label1.Text == textBox1.Text)
                {
                    correct++;
                    second = 5;
                    if (read.Read())
                    {
                        label1.Text = read["word"].ToString();
                    }
                }
                textBox1.Clear();
            }
            connection.Close();
        }  
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Test Typing","Name App");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            History asa = new History();
            asa.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
