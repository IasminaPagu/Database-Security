using System;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=localhost;Database=securedb;User ID=root;Password=root;";

        string name = "Ion Popescu";
        string cnp = "1980512345678";
        string card = "1234-5678-9012-3456";

        byte[] encryptedCNP = CryptoHelper.Encrypt(cnp);
        byte[] encryptedCard = CryptoHelper.Encrypt(card);

        try
        {
            using MySqlConnection connection = new(connectionString);
            connection.Open();

            string insertQuery = "INSERT INTO users (Name, EncryptedCNP, EncryptedCard) VALUES (@name, @cnp, @card)";
            using MySqlCommand cmd = new(insertQuery, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@cnp", encryptedCNP);
            cmd.Parameters.AddWithValue("@card", encryptedCard);
            cmd.ExecuteNonQuery();

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
