using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Web.ViewModels;

namespace Course_Project.Web
{
    public static class Parser
    {
        // Mappers for easy-add new type
        private static readonly Dictionary<CustomFieldType, Func<CustomFieldInputViewModel, int, CustomField>> _parsers =
            new()
            {
                [CustomFieldType.SingleLine] = (field, id) => new SingleLineField
                {
                    Content = field.Value,
                    ItemId = id,
                    IsVisible = field.IsVisible,
                    FieldType = CustomFieldType.SingleLine
                },
                [CustomFieldType.MultiLine] = (field, id) => new MultiLineField
                {
                    Content = field.Value,
                    ItemId = id,
                    IsVisible = field.IsVisible,
                    FieldType = CustomFieldType.MultiLine
                },
                [CustomFieldType.Numeric] = (field, id) => new NumericField
                {
                    Value = (int)field.NumericValue,
                    ItemId = id,
                    IsVisible = field.IsVisible,
                    FieldType = CustomFieldType.Numeric
                },
                [CustomFieldType.Checkbox] = (field, id) => new CheckboxField
                {
                    IsSet = (bool)field.BoolValue,
                    ItemId = id,
                    IsVisible = field.IsVisible,
                    FieldType = CustomFieldType.Checkbox
                },
                [CustomFieldType.Link] = (field, id) => new DocumentField
                {
                    Link = field.UrlValue?.ToString(),
                    ItemId = id,
                    IsVisible = field.IsVisible,
                    FieldType = CustomFieldType.Link
                }
            };

        public static void ParseDifferentFields(List<CustomField> customFields, List<CustomFieldInputViewModel> customElems, int id)
        {
            foreach (var field in customElems)
            {
                if (_parsers.TryGetValue(field.FieldType, out var factory))
                {
                    customFields.Add(factory(field, id));
                }
            }
        }

        public static List<CustomField> ParseFields(List<CustomFieldInputViewModel> customElems, int id)
        {
            var customFields = new List<CustomField>();
            ParseDifferentFields(customFields, customElems, id);
            return customFields;
        }
    }

}
