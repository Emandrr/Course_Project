using Course_Project.Domain.Models.CustomIdModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Contracts.DTO
{
    public class CustomSetOfId
    {
        public IdType IdType { get; set; }
        public string? Rule { get; set; } = "";
    }
}
