using CsvReaderApp.Models;
using CsvReaderApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Contact_Manager.Controllers
{

    public class CsvImporterController : Controller
    {
        private readonly CsvService _csvService;
        private readonly ContactContext _dbContext;

        public CsvImporterController(CsvService csvService, ContactContext dbContext)
        {
            _csvService = csvService;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var contacts = _dbContext.Contacts.ToList();
            return View(contacts);
        }
        [HttpPost]
        public IActionResult Index(IFormFile csvFile)
        {
            if (csvFile != null && csvFile.Length > 0)
            {
                using (var stream = csvFile.OpenReadStream())
                {
                    try
                    {
                        var newContacts = _csvService.ReadCsvFile(stream).ToList();
                        _dbContext.Contacts.AddRange(newContacts);
                        _dbContext.SaveChanges();

                        var allContacts = _dbContext.Contacts.ToList();
                        return View(allContacts);
                    }
                    catch (ApplicationException ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please select a valid CSV file.");
            }

            var contacts = _dbContext.Contacts.ToList();
            return View(contacts);
        }
        public IActionResult Edit(int id)
        {
            var contact = _dbContext.Contacts.Find(id);

            if (contact == null)
                return RedirectToAction("Index");

            var contactDto = new ContactDto()
            {
                Name = contact.Name,
                DateOfBirth = contact.DateOfBirth,
                Married = contact.Married,
                Phone = contact.Phone,
                Salary = contact.Salary
            };

            ViewData["ContactId"] = contact.Id;

            return View(contactDto);

        }
        [HttpPost]
        public IActionResult Edit(int id, ContactDto contactDto) 
        {
            var contact = _dbContext.Contacts.Find(id);

            if (contact == null)
                return RedirectToAction("Index");
            if(!ModelState.IsValid)
            {
                ViewData["ContactId"] = contact.Id;
                return View(contactDto);
            }
            contact.Name = contactDto.Name;
            contact.DateOfBirth = contactDto.DateOfBirth;
            contact.Married = contactDto.Married;
            contact.Phone = contactDto.Phone;
            contact.Salary = contactDto.Salary;

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var contact = _dbContext.Contacts.Find(id);
            if(contact == null)
            {
                return RedirectToAction("Index");
            }
            _dbContext.Contacts.Remove(contact);
            _dbContext.SaveChanges(true);
            return RedirectToAction("Index");

        }
    }
}
