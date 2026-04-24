using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientContactManagement.Models
{
    //junction entity
    public class ClientContact
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key to Client
        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        // Foreign Key to Contact
        public int ContactId { get; set; }

        public Contact Contact { get; set; } = null!;
    }
}
