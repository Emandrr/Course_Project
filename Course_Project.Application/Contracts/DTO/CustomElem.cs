using Course_Project.Domain.Models.CustomElemsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Contracts.DTO
{
    public class CustomElem
    {
        public string Name { get; set; }
        public CustomFieldType FieldType { get; set; }
        public bool IsVisible { get; set; }
    }

}
