using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryData.Models
{
    public class Patron

    {
        public int Id { get; set; }
        public string FirstName  { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public string TelephoneNumber { get; set; }

        public virtual LibraryCard LibraryCard { get; set; }
        public virtual LibraryBranch HomeLibraryBranch { get; set; }

    }
}
