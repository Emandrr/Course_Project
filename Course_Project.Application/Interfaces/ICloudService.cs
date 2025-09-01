using Google.Apis.Drive.v3;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface ICloudService
    {
        public string UploadPhoto(string folder, IFormFile fileinfo);

        public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string FolderId);

        public Task<string> GetPhotoAsync(string fileId);

        public string UpdatePhoto(IFormFile fileinfo,string folder, string fileId);
        public void Delete(string fileId, string folder);
    }
}
