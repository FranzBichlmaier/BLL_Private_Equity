using Prism.Mvvm;

namespace BLL_DataModels
{
    public class KeyEnumPair: BindableBase
    {
        private int key;
        private string description;

        public int Key { get => key; set => SetProperty(ref key, value); }
        public string Description { get => description; set => SetProperty(ref description, value); }
    }
}
