using System;
using MySql.Data.MySqlClient; 

class Program
{
    static void Main()
    {
        string connectionString = "Server=localhost;Database=securedb;User ID=root;Password=root;";

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Conexiunea la MySQL a fost realizata cu succes!");
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Eroare la conectare: " + ex.Message);
        }
    }
}
