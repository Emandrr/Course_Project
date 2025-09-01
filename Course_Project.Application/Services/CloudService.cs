using Course_Project.Application.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
namespace Course_Project.Application.Services
{
    public class CloudService : ICloudService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _accessToken;
        private readonly string _refreshToken;
        private readonly string _applicationName;
        private readonly string _username;

        public CloudService(string clientId, string clientSecret, string accessToken, string refreshToken, string applicationName, string username)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _applicationName = applicationName;
            _username = username;
        }

        private DriveService GetService()
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken = _accessToken,
                RefreshToken = _refreshToken,
            };

            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = new[] { DriveService.Scope.Drive },
                DataStore = new FileDataStore(_applicationName)
            });

            var credential = new UserCredential(apiCodeFlow, _username, tokenResponse);

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
            return service;
        }

        public string UploadPhoto(string folder, IFormFile fileinfo)
        {
            DriveService service = GetService();


            using Stream file = fileinfo.OpenReadStream();
            var driveFile = new Google.Apis.Drive.v3.Data.File
            {
                Name = (GetFiles(folder).LongCount()+1).ToString()+fileinfo.Name.Split(".").Last(),
                MimeType = "image/"+ fileinfo.Name.Split(".").Last(),
                Parents = new string[] { folder }
            };
            var request = service.Files.Create(driveFile, file, driveFile.MimeType);
            request.Fields = "id";
            var response = request.Upload();
            if (response.Status != UploadStatus.Completed)
                throw response.Exception;
            return request.ResponseBody.Id;
        }

        public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string FolderId)
        {
            var service = GetService();
            var fileList = service.Files.List();
            fileList.Q = $"mimeType contains 'image/' and '{FolderId}' in parents";
            fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";

            var result = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {
                fileList.PageToken = pageToken;
                var filesResult = fileList.Execute();
                var files = filesResult.Files;
                pageToken = filesResult.NextPageToken;
                result.AddRange(files);
            } while (pageToken != null);
            return result;
        }

        public async Task<string> GetPhotoAsync(string fileId)
        {
            var service = GetService();
            var request = service.Files.Get(fileId);
            using (var stream = new MemoryStream())
            {
                await request.DownloadAsync(stream);
                stream.Position = 0;
                var bytes = stream.ToArray();
                var fileMetadata = await service.Files.Get(fileId).ExecuteAsync();
                return "data:"+$"{fileMetadata.MimeType};base64," +Convert.ToBase64String(bytes);
            }
        }

        public string UpdatePhoto(IFormFile fileinfo, string folder, string fileId)
        {
            var t = GetFiles(folder);
            foreach (var a in t)
            {
                if (a.Name == fileinfo.Name) fileId = a.Id;
            }
            DriveService service = GetService();
            using Stream file = fileinfo.OpenReadStream();
            var driveFile = new Google.Apis.Drive.v3.Data.File();
            var fileMetadata = service.Files.Get(fileId).Execute();
            driveFile.Name = fileMetadata.Name;
            driveFile.MimeType = "image/"+ fileinfo.Name.Split(".").Last();
            var request = service.Files.Update(driveFile, fileId, file, driveFile.MimeType);
            request.Fields = "id, name, webViewLink";
            var response = request.Upload();
            if (response.Status != UploadStatus.Completed)
                throw response.Exception;
            return driveFile.Name;
        }

        public void Delete(string fileId, string folder)
        {
            var service = GetService();
            var t = GetFiles(folder);
            foreach (var a in t)
            {
                if (a.Id == fileId)
                {
                    var command = service.Files.Delete(fileId);
                    command.Execute();
                }
            }
        }
    }
}
