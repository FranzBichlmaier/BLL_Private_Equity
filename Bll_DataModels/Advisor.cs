using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BLL_DataModels
{
    public class Advisor: BindableBase
    {
        private int id;
        private string title = string.Empty;
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string fullName = string.Empty;
        private string extension = string.Empty;
        private string faxExtension = string.Empty;
        private string emailAddress = string.Empty;
        private bool privateEquityExpert = false;
        private int? reportsToId;

        [ForeignKey ("ReportsToId")]
        [JsonIgnore]
        public virtual Advisor ReportsTo { get; set; }
       public string FullName { get => fullName; set => SetProperty(ref fullName, value); }
        public string EmailAddress { get => emailAddress; set => SetProperty(ref emailAddress, value); }
        public string FaxExtension { get => faxExtension; set => SetProperty(ref faxExtension, value); }
        public int Id { get => id; set => SetProperty(ref id, value); }
        public int? ReportsToId { get => reportsToId; set => SetProperty(ref reportsToId, value); }
        public string Extension { get => extension; set => SetProperty(ref extension, value); } 
        [MaxLength (50)]
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }     
        [MaxLength(50)]
        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }      
        [MaxLength(10)]
        public string Title { get => title; set => SetProperty(ref title, value); }
        public bool PrivateEquityExpert { get => privateEquityExpert; set => SetProperty(ref privateEquityExpert, value); }

        public Advisor()
        {

        }

        public Advisor(Advisor old)
        {
            Id = old.Id;
            Title = old.Title;
            FirstName = old.FirstName;
            LastName = old.LastName;
            FullName = old.FullName;
            Extension = old.Extension;
            FaxExtension = old.FaxExtension;
            EmailAddress = old.EmailAddress;
            PrivateEquityExpert = old.PrivateEquityExpert;
            ReportsToId = old.ReportsToId;
        }
    }
}
