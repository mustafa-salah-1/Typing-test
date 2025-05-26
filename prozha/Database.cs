using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prozha
{
    internal class Database
    {
        public void CreateTable(string nameTable)
        {
            if (string.IsNullOrWhiteSpace(nameTable))
                throw new ArgumentException("Table name cannot be null or empty.");

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=typing_test.db"))
            {
                conn.Open();

                string sql = $@"
                        CREATE TABLE IF NOT EXISTS {nameTable} (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            word TEXT NOT NULL
                        );";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateHistoryTable()
        {
            using (var conn = new SQLiteConnection("Data Source=typing_test.db"))
            {
                conn.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS History (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        difficulty TEXT NOT NULL UNIQUE,
                        result INTEGER NOT NULL
                    );";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
         
        public void AddWordsToTable(string tableName, List<string> words)
        {
            if (words == null || words.Count == 0)
                throw new ArgumentException("Words list cannot be empty.");

            string[] allowedTables = { "letter_four", "letter_five", "letter_six" };
            if (!allowedTables.Contains(tableName))
                throw new ArgumentException("Invalid table name.");

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=typing_test.db"))
            {
                conn.Open();
                 
                using (SQLiteCommand checkCmd = new SQLiteCommand($"SELECT COUNT(*) FROM {tableName}", conn))
                {
                    long rowCount = (long)checkCmd.ExecuteScalar();
                    if (rowCount > 0)
                    { 
                         return;
                    }
                }
                 
                using (SQLiteTransaction transaction = conn.BeginTransaction())
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO {tableName} (word) VALUES (@word)";
                    var wordParam = cmd.Parameters.Add("@word", System.Data.DbType.String);

                    foreach (var word in words.Take(100))
                    {
                        wordParam.Value = word;
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    Console.WriteLine($"Inserted {Math.Min(words.Count, 100)} words into table '{tableName}'.");
                }
            }
        }
         
        public List<string> wordsFour = new List<string>
{
    "ball", "tree", "game", "code", "book", "time", "love", "fire", "wind", "star",
    "rain", "snow", "bird", "fish", "frog", "lion", "wolf", "bear", "duck", "frog",
    "cake", "milk", "road", "ship", "salt", "coin", "leaf", "nest", "frog", "wave",
    "door", "hand", "foot", "head", "milk", "corn", "frog", "lamp", "fish", "bowl",
    "ring", "hair", "song", "tree", "moon", "rock", "frog", "milk", "cake", "book",
    "fish", "frog", "bird", "lion", "wolf", "bear", "duck", "frog", "frog", "door",
    "hand", "foot", "head", "milk", "corn", "frog", "lamp", "fish", "bowl", "ring",
    "hair", "song", "tree", "moon", "rock", "frog", "milk", "cake", "book", "fish",
    "frog", "bird", "lion", "wolf", "bear", "duck", "frog", "door", "hand", "foot",
    "head", "milk", "corn", "frog", "lamp", "fish", "bowl", "ring", "hair", "song"
};

        public List<string> wordsFive = new List<string>
{
    "apple", "brush", "chair", "dream", "earth", "flame", "glove", "heart", "input", "jelly",
    "knock", "lemon", "music", "night", "ocean", "plant", "queen", "river", "smile", "table",
    "under", "vivid", "whale", "xenon", "yield", "zebra", "angle", "bloom", "crane", "dance",
    "eagle", "field", "grain", "house", "ideal", "juice", "knife", "light", "march", "nerve",
    "olive", "peace", "quest", "round", "shore", "taste", "urban", "voice", "water", "xray",
    "young", "zesty", "alarm", "brave", "cabin", "delta", "entry", "fancy", "giant", "hover",
    "index", "joint", "karma", "lunch", "mango", "novel", "oasis", "piano", "quiet", "rally",
    "scope", "trust", "union", "vigor", "woven", "xerox", "yearn", "zonal", "amble", "blaze",
    "climb", "drift", "eager", "flood", "grasp", "harsh", "inbox", "jolly", "kneel", "liver",
    "moral", "nerdy", "offer", "pride", "quart", "rouge", "sheep", "tiger", "usage", "vocal"
};

        public List<string> wordsSix = new List<string>
{
    "animal", "bridge", "circle", "danger", "energy", "forest", "guitar", "helmet", "island", "jungle",
    "kitten", "letter", "monkey", "nature", "orange", "puzzle", "quartz", "rocket", "silver", "temple",
    "united", "violet", "wealth", "yellow", "zigzag", "admire", "bishop", "candle", "doctor", "effect",
    "fabric", "garden", "hunter", "insect", "jacket", "killer", "legend", "moment", "nation", "object",
    "palace", "questy", "rabbit", "simple", "travel", "unique", "volume", "window", "xenial", "yonder",
    "zenith", "arctic", "border", "career", "desert", "excite", "flight", "garage", "honest", "injury",
    "jungle", "kidnap", "length", "modern", "normal", "offend", "people", "quirky", "resist", "stream",
    "throw", "unfold", "violent", "wander", "yellow", "zipper", "aspect", "beware", "castle", "dinner",
    "empire", "fabric", "glance", "hunter", "ignore", "jockey", "kneel", "lumber", "method", "narrow",
    "output", "poetry", "quiver", "realm", "single", "tunnel", "unique", "valley", "wealth", "xenons"
};
 
    }
}
