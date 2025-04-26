using Konscious.Security.Cryptography;
using System.Text;

namespace ProjectManager.Encryption


//https://github.com/kmaragon/Konscious.Security.Cryptography

{
    public class Argon2Helper
    {

        // Define the configuration values as static fields
        private static int DegreeOfParallelism;
        private static int MemorySize;
        private static int Iterations;
        private static int HashLength;


        public static void Initialize(IConfiguration configuration)
        {
            var config = configuration.GetSection("Argon2Settings");

            DegreeOfParallelism = config.GetValue<int>("DegreeOfParallelism");
            MemorySize = config.GetValue<int>("MemorySize");
            Iterations = config.GetValue<int>("Iterations");
            HashLength = config.GetValue<int>("HashLength");
        }

        public static string HashPassword(string password, byte[] salt)
        {
            // Convert the password to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Create a new Argon2 instance for hashing
            using (var argon2 = new Argon2i(passwordBytes))
            {
                
                // Set the parameters for the hashing process 
                argon2.Salt = salt; // Random salt unique to each user
                argon2.DegreeOfParallelism = DegreeOfParallelism; // Number of threads
                argon2.MemorySize = MemorySize; // Memory cost (in KB)
                argon2.Iterations = Iterations; // Number of iterations

                // Generate the hash
                byte[] hashBytes = argon2.GetBytes(HashLength); // Length of the generated hash

                return Convert.ToBase64String(hashBytes); // Return the hash as a Base64 string
            }
        }

        // Verifying a password
        public static bool VerifyPassword(string password, string storedHash, byte[] salt)
        {
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            // Hash the password with the same settings and compare
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var argon2 = new Argon2i(passwordBytes))
            {
                argon2.Salt = salt; // Stored salt from DB
                argon2.DegreeOfParallelism = DegreeOfParallelism; // Number of threads
                argon2.MemorySize = MemorySize; // Memory cost (in KB)
                argon2.Iterations = Iterations; // Number of iterations

                byte[] hashBytes = argon2.GetBytes(HashLength);

                return storedHashBytes.Length == hashBytes.Length && storedHashBytes.SequenceEqual(hashBytes);
            }
        }
    }
}
