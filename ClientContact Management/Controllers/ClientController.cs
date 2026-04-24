using ClientContactManagement.Data;
using ClientContactManagement.Models;
using ClientContactManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientContactManagement.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IClientCodeGeneratorService _codeService;

        public ClientController(ApplicationDbContext context,
                                IClientCodeGeneratorService codeService)
        {
            _context = context;
            _codeService = codeService;
        }

        // GET: /Client/Create
        public IActionResult Create()
        {
            // Return empty form; ClientCode not displayed yet
            return View();
        }

        // POST: /Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Client client)
        {
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState INVALID");
                // Validation failed, return form with errors
                return View(client);
            }
            

            // Generate unique ClientCode (This is from the ClientCodeGeneratorService5)
            client.ClientCode = await _codeService.GenerateClientCodeAsync(client.Name);

            // Save client
            _context.Add(client);
            await _context.SaveChangesAsync();

            // Redirect to Edit page to display ClientCode and Contacts tab
            return RedirectToAction(nameof(Edit), new { id = client.Id });
            

        }

        // GET: /Client/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _context.Clients
                .Include(c => c.ClientContacts)
                .ThenInclude(cc => cc.Contact)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
                return NotFound();

            //provide the dropdown of all contacts
            ViewBag.AllContacts = await _context.Contacts
                .OrderBy(c => c.Surname)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(client);
            
        }

        // Index - list all clients
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients
                .Include(c => c.ClientContacts)
                .ThenInclude(cc => cc.Contact)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(clients);
        }

        //the POST Method to link a contact to client

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkContact(int clientId, int contactId)
        {
            // Check if already linked
            bool alreadyLinked = await _context.ClientContacts
                .AnyAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);

            if (!alreadyLinked)// preventing duplicate links
            {
                _context.ClientContacts.Add(new ClientContact
                {
                    ClientId = clientId,
                    ContactId = contactId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = clientId });
           
        }


        //this is the POST method to unlinking the Contact 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            var link = await _context.ClientContacts
                .FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);

            if (link != null)
            {
                _context.ClientContacts.Remove(link);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new { id = clientId });
           
        }

        // GET: Client/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Client/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client != null)
            {
                // Remove any linked ClientContacts first
                _context.ClientContacts.RemoveRange(client.ClientContacts);

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
