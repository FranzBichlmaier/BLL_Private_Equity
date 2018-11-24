
using Prism.Mvvm;

namespace BLL_DataModels
{
    public class Gender: BindableBase
    {
        private string kurzform;
        private string langform;

        public string Kurzform { get => kurzform; set => SetProperty(ref kurzform, value); }
        public string Langform { get => langform; set => SetProperty(ref langform, value); }      

    }
}
