
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class Country: BindableBase
    {
        private int id;
        private string countryName;

        public int Id { get => id; set => SetProperty(ref id, value); }
        [MaxLength(50)]
        public string CountryName { get => countryName; set => SetProperty(ref countryName,value); }
    }
}
