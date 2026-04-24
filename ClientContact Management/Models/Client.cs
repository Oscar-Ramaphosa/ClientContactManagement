using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace ClientContactManagement.Models
{
    public class Client
    {
        // Primary Key
        public int Id { get; set; }

        // Client name (required)
        [Required]
        public string Name { get; set; }

        // Auto-generated client code (unique)
        [MaxLength(6)]
        public string? ClientCode { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<ClientContact>? ClientContacts { get; set; } // help with understanding the relationship
    }
}
