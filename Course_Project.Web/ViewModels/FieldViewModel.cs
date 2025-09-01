using Course_Project.Domain.Models.CustomElemsModels;

namespace Course_Project.Web.ViewModels
{
    public class FieldViewModel
    {
        public CustomFieldType FieldType { get; set; }
        public CustomFieldOption FieldData { get; set; } = new CustomFieldOption();
    }
}
