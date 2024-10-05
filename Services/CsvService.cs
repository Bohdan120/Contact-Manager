using CsvHelper;
using CsvHelper.TypeConversion;
using CsvReaderApp.Models;
using System.Globalization;

namespace CsvReaderApp.Services
{
    public class CsvService
    {
        private readonly ContactContext _context;

        public CsvService(ContactContext context)
        {
            _context = context;
        }

        public IEnumerable<Contact> ReadCsvFile(Stream fileStream)
        {
            try
            {
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Contact>().ToList();
                    foreach (var contact in records)
                    {
                        contact.Id = 0; 
                    }
                    return records;
                }
            }
            catch (HeaderValidationException ex)
            {
                throw new ApplicationException("CSV file header is invalid.", ex);
            }
            catch (TypeConverterException ex)
            {
                throw new ApplicationException("CSV file contains invalid data format.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error reading CSV file", ex);
            }
        }

        public void SaveContactsToDatabase(IEnumerable<Contact> contacts)
        {
            _context.Contacts.AddRange(contacts);
            _context.SaveChanges();
        }
    }
}
