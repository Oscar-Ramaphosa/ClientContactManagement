using Microsoft.EntityFrameworkCore;
using ClientContactManagement.Models;

namespace ClientContactManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<ClientContact> ClientContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // the client configuration
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.ClientCode)
                .IsUnique(); // this is where the ClientCode is unique in database

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Name); // Index on Name because i sort clients by Name ascending


            // CONTACT CONFIGURATION
            modelBuilder.Entity<Contact>()
                .HasIndex(c => c.Email)
                .IsUnique(); // making sure that the Email is unique in the database

            modelBuilder.Entity<Contact>()
                .HasIndex(c => new { c.Surname, c.Name }); // Composite index for sorting by Full Name (Surname, Name)

            // CLIENTCONTACT CONFIGURATION


            modelBuilder.Entity<ClientContact>()
                .HasIndex(cc => new { cc.ClientId, cc.ContactId })
                .IsUnique();
            // Prevent duplicate linking between same Client and Contact

            modelBuilder.Entity<ClientContact>()
                .HasOne(cc => cc.Client)
                .WithMany(c => c.ClientContacts)
                .HasForeignKey(cc => cc.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            // If Client is deleted, related ClientContacts are deleted

            modelBuilder.Entity<ClientContact>()
                .HasOne(cc => cc.Contact)
                .WithMany(c => c.ClientContacts)
                .HasForeignKey(cc => cc.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
            // If Contact is deleted, related ClientContacts are deleted
        }
    }
}
