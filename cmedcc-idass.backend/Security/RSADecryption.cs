using System.Security.Cryptography;
using System.Text;

namespace cmedcc_idass.backend.Security;


public static class RsaDecryption
{
    public static string Decrypt(string privateKeyPath, string encryptedData)
    {
        var base64Key = ReadPrivateKey(privateKeyPath);
        var privateKeyBytes = Convert.FromBase64String(base64Key);

        using var rsa = RSA.Create();
        try
        {
            // Try importing PKCS#1 format first
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        }
        catch (CryptographicException)
        {
            // If that fails, try importing PKCS#8 format
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        }

        // Use OAEP padding with SHA-256 for decryption
        var decryptedBytes = rsa.Decrypt(Convert.FromBase64String(encryptedData), RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public static string DecryptByHeaderPrivateKey(string stringPrivateKey, string encryptedData)
    {
        var base64Key = FormatPrivateKey(stringPrivateKey);
        var privateKeyBytes = Convert.FromBase64String(base64Key);

        using var rsa = RSA.Create();
        try
        {
            // Try importing PKCS#1 format first
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        }
        catch (CryptographicException)
        {
            // If that fails, try importing PKCS#8 format
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        }

        // Use OAEP padding with SHA-256 for decryption
        var decryptedBytes = rsa.Decrypt(Convert.FromBase64String(encryptedData), RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static string ReadPrivateKey(string privateKeyPath)
    {
        return FormatPrivateKey(File.ReadAllText(privateKeyPath));
    }

    private static string FormatPrivateKey(string pemContent)
    {
        // Remove PEM headers and any extra new lines/whitespace
        pemContent = pemContent.Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Trim(); // Remove any extra spaces

        return pemContent;
    }
}