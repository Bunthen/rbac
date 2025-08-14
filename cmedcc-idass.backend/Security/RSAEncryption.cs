using System.Security.Cryptography;
using System.Text;

namespace cmedcc_idass.backend.Security;


public static class RsaEncryption
{
    public static string Encrypt(string publicKeyPath, string data)
    {
        var base64Key = ReadPublicKey(publicKeyPath);
        var publicKeyBytes = Convert.FromBase64String(base64Key);

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        // Use OAEP padding with SHA-256
        var encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(data), RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encryptedBytes);
    }

    private static string ReadPublicKey(string publicKeyPath)
    {
        var pemContent = File.ReadAllText(publicKeyPath);

        // Remove PEM headers
        pemContent = pemContent.Replace("-----BEGIN PUBLIC KEY-----", "")
            .Replace("-----END PUBLIC KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim(); // Remove extra spaces

        return pemContent;
    }
}