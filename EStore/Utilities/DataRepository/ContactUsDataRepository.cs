using EStore.Models;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace EStore.Utilities.DataRepository
{
    public class ContactUsDataRepository : SqlDbConnection, IContactUsDataRepository
    {
        public ContactUs GetContactDetails()
        {
            try
            {
                ContactUs contact = _dbContext.ContactUsModel.First();
                if(contact != null)
                {
                    return contact;
                }
            } 
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return new ContactUs();
        }

        public bool UpdateContactDetails(ContactUs contactUs)
        {
            try
            {
                ContactUs contact = _dbContext.ContactUsModel.FirstOrDefault();
                if (contact != null)
                {
                    contact = contactUs;
                } else
                {
                    _dbContext.ContactUsModel.Add(contactUs);
                }
                _dbContext.SaveChanges();

                return true;
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // Log or handle the validation error details
                        System.Diagnostics.Debug.WriteLine($"Entity: {validationErrors.Entry.Entity.GetType().Name}, Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                    }
                }
            }

            return false;
        }
    }
}