using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface IDropBoxService
    {
        public Task UploadFile(Stream stream, string fileName);
    }
}
