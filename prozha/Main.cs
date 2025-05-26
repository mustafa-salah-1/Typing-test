using System; 
using System.Data; 
using System.Data.SQLite;
using System.Linq;
using System.Windows.Forms;

namespace prozha
{
    public partial class Main : Form  
    {
        public Main()
        {
            InitializeComponent(); 
            Database db = new Database();
            db.CreateTable("letter_four");
            db.CreateTable("letter_five");
            db.CreateTable("letter_six");
            db.CreateHistoryTable();
            db.AddWordsToTable("letter_four", db.wordsFour);
            db.AddWordsToTable("letter_five", db.wordsFive);
            db.AddWordsToTable("letter_six", db.wordsSix);
        }

        public static string connStr = "Data Source=typing_test.db;";
        SQLiteConnection connection = new SQLiteConnection(connStr);
         
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
                string query = "SELECT difficulty FROM History WHERE difficulty = @mode";
                SQLiteCommand check = new SQLiteCommand(query, connection);
                check.Parameters.AddWithValue("@mode", mode);

                DataTable dt = new DataTable();
                SQLiteDataAdapter da = new SQLiteDataAdapter(check);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    // Update existing row
                    string updateQuery = "UPDATE History SET result = @correct WHERE difficulty = @mode";
                    SQLiteCommand update = new SQLiteCommand(updateQuery, connection);
                    update.Parameters.AddWithValue("@correct", correct);
                    update.Parameters.AddWithValue("@mode", mode);
                    update.ExecuteNonQuery();
                }
                else
                {
                    // Insert new row
                    string insertQuery = "INSERT INTO History (difficulty, result) VALUES (@mode, @correct)";
                    SQLiteCommand insert = new SQLiteCommand(insertQuery, connection);
                    insert.Parameters.AddWithValue("@mode", mode);
                    insert.Parameters.AddWithValue("@correct", correct);
                    insert.ExecuteNonQuery();
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

                string checkQuery = "SELECT difficulty FROM History WHERE difficulty = @mode";
                using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@mode", mode);

                    DataTable dt = new DataTable();
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(checkCmd))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    { 
                        string resultQuery = "SELECT result FROM History WHERE difficulty = @mode";
                        using (SQLiteCommand resultCmd = new SQLiteCommand(resultQuery, connection))
                        {
                            resultCmd.Parameters.AddWithValue("@mode", mode);

                            using (SQLiteDataReader reader = resultCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int existingResult = int.Parse(reader["result"].ToString());
                                     
                                    if (correct > existingResult)
                                    {
                                        string updateQuery = "UPDATE History SET result = @correct WHERE difficulty = @mode";
                                        using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, connection))
                                        {
                                            updateCmd.Parameters.AddWithValue("@correct", correct);
                                            updateCmd.Parameters.AddWithValue("@mode", mode);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    { 
                        string insertQuery = "INSERT INTO History (difficulty, result) VALUES (@mode, @correct)";
                        using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@mode", mode);
                            insertCmd.Parameters.AddWithValue("@correct", correct);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
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
         
        private void start()
        { 
            string[] allowedTables = { "Easy", "Normal", "Hard" }; 
            if (!allowedTables.Contains(mode))
            {
                MessageBox.Show("Invalid difficulty level.");
                return;
            }

            int randomId = rnd.Next(1, 101);  

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=typing_test.db;"))
            {
                connection.Open();

                string query = $"SELECT word FROM {difficult} WHERE id = {randomId}";
                using (SQLiteCommand letter = new SQLiteCommand(query, connection))
                using (SQLiteDataReader read = letter.ExecuteReader())
                {
                    if (read.Read())
                    {
                        label1.Text = read["word"].ToString();
                    }
                    else
                    {
                        label1.Text = "(No word found)";
                    }
                }

                connection.Close();
            }

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
            mode = "Easy";
            start();
        }
        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficult = "letter_five";
            mode = "Normal";
            start();
        } 
        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            difficult = "letter_six";
            mode = "Hard";
            start();
        } 

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; 

                if (label1.Text == textBox1.Text)
                {
                    correct++;
                    second = 5; 
                    int randomId = rnd.Next(1, 101);
                    string sql = $"SELECT word FROM {difficult} WHERE id = @id";

                    using (SQLiteConnection connection = new SQLiteConnection("Data Source=typing_test.db;"))
                    {
                        connection.Open();
                        using (SQLiteCommand letter = new SQLiteCommand(sql, connection))
                        {
                            letter.Parameters.AddWithValue("@id", randomId);
                            using (SQLiteDataReader read = letter.ExecuteReader())
                            {
                                if (read.Read())
                                {
                                    label1.Text = read["word"].ToString();
                                }
                                else
                                {
                                    label1.Text = "(No word found)";
                                }
                            }
                        }
                    }
                }

                textBox1.Clear();
            }
        }  
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Developed by Mustafa Salah", "Typing test");
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
         
    }
}
