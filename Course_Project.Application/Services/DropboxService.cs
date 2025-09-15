using Dropbox.Api;
using Dropbox.Api.Files;
using Course_Project.Application.Interfaces;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Services
{
    public class DropboxService : IDropBoxService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        private async Task<string> GetAccessTokenAsync()
        {
            var appKey = _config["DId"];
            var appSecret = _config["DSecret"];
            var refreshToken = _config["token"];

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropbox.com/oauth2/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken},
                {"client_id", appKey},
                {"client_secret", appSecret}
            });

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            return data["access_token"].ToString();
        }

        public DropboxService(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task UploadFile(Stream stream, string fileName)
        {
            var accessToken = await GetAccessTokenAsync();
            using var dbx = new DropboxClient(accessToken);
            stream.Position = 0;
            await dbx.Files.UploadAsync($"/{fileName}", WriteMode.Overwrite.Instance, body: stream);
        }
    }
}
