using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class CryptoHelper
{
    private static readonly string keyString = "E9#xB1$eW7!qZ5@k";
    private static readonly string ivString = "L0ckD@tA12345678";

    public static byte[] Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(keyString);
        aes.IV = Encoding.UTF8.GetBytes(ivString);
        //transformam string-urile noastre din string in bytes

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using StreamWriter sw = new(cs);
        sw.Write(plainText);
        sw.Close();

        return ms.ToArray();
    }

    public static string Decrypt(byte[] cipherBytes)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(keyString);
        aes.IV = Encoding.UTF8.GetBytes(ivString);

        using MemoryStream ms = new(cipherBytes);
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return sr.ReadToEnd();
    }
}
