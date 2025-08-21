using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cmedcc_idass.backend.Utilize;

public class Utilize
{
    // --- Helper methods for password hashing (simplified for demonstration) ---
    public static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    
}