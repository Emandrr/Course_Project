using Course_Project.Domain.Models.CustomElemsModels;
using System.Buffers.Text;

namespace Course_Project.Web.ViewModels
{
    public class CustomFieldInputViewModel
    {
        public int Id { get; set; }
        public CustomFieldType FieldType { get; set; }
        public bool IsVisible { get;set; }
        public string? Value { get; set; }
        public int? NumericValue { get; set; }
        public bool? BoolValue { get; set; }
        public Uri? UrlValue { get; set; }

    }
}
