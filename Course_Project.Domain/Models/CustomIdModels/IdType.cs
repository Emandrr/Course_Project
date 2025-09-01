using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.CustomIdModels
{
    public enum IdType
    {
        Text,
        Rand20Bit,
        Rand32Bit,
        Rand6Digit,
        Rand9Digit,
        GUID,
        DateTime,
        Sequence
    }
}
