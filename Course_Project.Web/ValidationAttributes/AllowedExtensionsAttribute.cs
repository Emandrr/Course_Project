using System.ComponentModel.DataAnnotations;

namespace Course_Project.Web.ValidationAttributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"Разрешены только файлы: {string.Join(", ", _extensions)}");
                }
            }
            return ValidationResult.Success;
        }
    }

   
}
