using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class TextComponent: BindableBase
    {
        private int id;
        private string description = string.Empty;
        private string textContent = string.Empty;

        public int Id { get => id; set => SetProperty(ref id, value); }
        [MaxLength(100)]
        public string  Description { get => description; set => SetProperty(ref description, value); }
        public string TextContent { get => textContent; set => SetProperty(ref textContent, value); }

    }
}
