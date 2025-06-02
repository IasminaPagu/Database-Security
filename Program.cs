using System;
using System.IO;
using System.Text.Json;
using MySql.Data.MySqlClient;

class DbConfig
{
    public string? Server { get; set; }
    public string? Database { get; set; }
    public string? UserID { get; set; }
    public string? Password { get; set; }
}

class Program
{
    static void Main()
    {
        // 1. Citește fișierul JSON
        string configText = File.ReadAllText("config.json");
        DbConfig? config = JsonSerializer.Deserialize<DbConfig>(configText);

        // 2. Verifică dacă a fost citit corect
        if (config == null || config.Server == null || config.Database == null || config.UserID == null || config.Password == null)
        {
            Console.WriteLine("Eroare: config.json lipsește sau conține date invalide.");
            return;
        }

        // 3. Construiește connection string-ul
        string connectionString = $"Server={config.Server};Database={config.Database};User ID={config.UserID};Password={config.Password};";

        // 4. Date de test
        string name = "Iasmina";
        string cnp = "11111111111";
        string card = "0123-4567-8912-1010";

        // 5. Criptare
        byte[] encryptedCNP = CryptoHelper.Encrypt(cnp);
        byte[] encryptedCard = CryptoHelper.Encrypt(card);

        try
        {
            using MySqlConnection connection = new(connectionString);
            connection.Open();

            // 6. Inserare date criptate
            string insertQuery = "INSERT INTO users (Name, EncryptedCNP, EncryptedCard) VALUES (@name, @cnp, @card)";
            using MySqlCommand cmd = new(insertQuery, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@cnp", encryptedCNP);
            cmd.Parameters.AddWithValue("@card", encryptedCard);
            cmd.ExecuteNonQuery();

            // 7. Citire din DB
            string selectQuery = "SELECT Name, EncryptedCNP, EncryptedCard FROM users";
            using MySqlCommand selectCmd = new(selectQuery, connection);
            using MySqlDataReader reader = selectCmd.ExecuteReader();

            while (reader.Read())
            {
                string dbName = reader.GetString("Name");
                byte[] dbCNP = (byte[])reader["EncryptedCNP"];
                byte[] dbCard = (byte[])reader["EncryptedCard"];

                string decryptedCNP = CryptoHelper.Decrypt(dbCNP);
                string decryptedCard = CryptoHelper.Decrypt(dbCard);

                Console.WriteLine($"Nume: {dbName}, CNP: {decryptedCNP}, Card: {decryptedCard}");
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Eroare: " + ex.Message);
        }
    }
}
