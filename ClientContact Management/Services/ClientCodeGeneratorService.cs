using System.Text;
using ClientContactManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace ClientContactManagement.Services
{
    //Service responsible for generating a unique 6-character client code.(business Logic)
    public class ClientCodeGeneratorService : IClientCodeGeneratorService
    {
        private readonly ApplicationDbContext _context;

        public ClientCodeGeneratorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateClientCodeAsync(string clientName)
        {
            // making sure the name is not null and remove extra spaces
            clientName = clientName?.Trim() ?? string.Empty;

            //  Generate the 3-letter alphabetic prefix
            string alphaPart = GenerateAlphaPart(clientName);

            // Start numeric portion at 001
            int number = 1;

            while (true)
            {
                // Format number as 3 digits 001, 002,
                string numericPart = number.ToString("D3");

                // Combine alpha and numeric portions
                string finalCode = alphaPart + numericPart;

                // Check database to make sure there is uniqueness
                bool exists = await _context.Clients
                    .AnyAsync(c => c.ClientCode == finalCode);

                if (!exists)
                {
                    // If code does not exist, return it
                    return finalCode;
                }

                // Otherwise increment number and try again
                number++;
            }
        }

        //Generates the 3-letter alphabetic portion of the client code.
        private string GenerateAlphaPart(string originalName)
        {
            if (string.IsNullOrWhiteSpace(originalName))
                return "ABC"; // Fallback safety value

            // Split the name into words
            var words = originalName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // one that has musltiple words
            if (words.Length > 1)
            {
                StringBuilder initials = new StringBuilder();

                foreach (var word in words)
                {
                    // Take first letter of each word if it is alphabetic
                    if (char.IsLetter(word[0]))
                    {
                        initials.Append(char.ToUpper(word[0]));
                    }

                    // Stop once we reach 3 letters
                    if (initials.Length == 3)
                        break;
                }

                // If less than 3 letters, pad alphabetically
                char nextChar = 'A';
                while (initials.Length < 3)
                {
                    initials.Append(nextChar);
                    nextChar++;
                }

                return initials.ToString();
            }

            // this is one for the single word
            var cleaned = new string(originalName
                .Where(char.IsLetter)
                .ToArray())
                .ToUpper();

            if (cleaned.Length >= 3)
            {
                return cleaned.Substring(0, 3);
            }

            // If less than 3 letters → pad alphabetically
            StringBuilder sb = new StringBuilder(cleaned);
            char padChar = 'A';

            while (sb.Length < 3)
            {
                sb.Append(padChar);
                padChar++;
            }

            return sb.ToString();
        }
    }
}
