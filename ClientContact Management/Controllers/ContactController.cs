using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientContactManagement.Data;
using ClientContactManagement.Models;

namespace ClientContactManagement.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Surname,Email")] Contact contact)
        {
            if (!ModelState.IsValid)
                return View(contact);

            _context.Add(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = contact.Id });
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.ClientContacts)
                .ThenInclude(cc => cc.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            ViewBag.AllClients = await _context.Clients
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(contact);
        }

        // Index - list all contacts
        public async Task<IActionResult> Index()
        {
            var contacts = await _context.Contacts
                .Include(c => c.ClientContacts)
                .ThenInclude(cc => cc.Client)
                .OrderBy(c => c.Surname)
                .ThenBy(c => c.Name)
                .ToListAsync();
            return View(contacts);
        }

        // POST: Link a client to a contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkClient(int contactId, int clientId)
        {
            // Prevent duplicates
            bool alreadyLinked = await _context.ClientContacts
                .AnyAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);

            if (!alreadyLinked)
            {
                _context.ClientContacts.Add(new ClientContact
                {
                    ClientId = clientId,
                    ContactId = contactId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = contactId });
        }

        // POST: Unlink a client from a contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkClient(int contactId, int clientId)
        {
            var link = await _context.ClientContacts
                .FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);

            if (link != null)
            {
                _context.ClientContacts.Remove(link);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = contactId });
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.Contacts
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null) return NotFound();

            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact != null)
            {
                // Remove linked ClientContacts first
                _context.ClientContacts.RemoveRange(contact.ClientContacts);

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
