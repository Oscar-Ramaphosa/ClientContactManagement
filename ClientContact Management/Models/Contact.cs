using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace ClientContactManagement.Models
{
    public class Contact
    {
        // Primary Key
        public int Id { get; set; }

        // Contact first name (required)
        [Required]
        public string Name { get; set; } = string.Empty;

        // Contact surname (required)
        [Required]
        public string Surname { get; set; } = string.Empty;

        // Email must be unique and required
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Navigation property for many-to-many relationship
        public ICollection<ClientContact>? ClientContacts { get; set; } // make it nullable because a contact may not have a link.
    }
}
