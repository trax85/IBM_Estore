using System;
using System.Security.Cryptography;
using System.Text;

namespace EStore.Utilities
{
    public class TokenGenerator
    {
        private const string SecretKey = "UXSVWYSUDJJDKKD";

        public string GenerateToken(string userEmail)
        {
            // Combine user-specific information and current timestamp.
            string dataToHash = userEmail + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            // Add the secret key or salt to enhance security.
            dataToHash += SecretKey;
            // Generate the token using SHA256 hashing algorithm.
            string token = ComputeSha256Hash(dataToHash);

            return token;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                // Convert the byte array to a hexadecimal string.
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}