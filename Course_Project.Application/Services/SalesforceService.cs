using Course_Project.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Course_Project.Application.Services
{
    public class SalesforceService : ISalesforceService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private string _instanceUrl;
        private string _accessToken;

        public SalesforceService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12; 
        }

        public async Task<bool> EnsureAuthenticatedAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                return await AuthenticateAsync(
                    clientId: _config["SalClientId"],
                    clientSecret: _config["SalClientSecret"],
                    username: _config["SalUsername"],
                    password: _config["SalPassword"],
                    token: _config["SalSecurityToken"]
                );
            }
            return true;
        }
        public Dictionary<string,string> Set(string clientId, string clientSecret, string username, string password, string token)
        {
            return new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["username"] = username,
                ["password"] = password + token
            };
        }
        public async Task<bool> AuthenticateAsync(string clientId, string clientSecret, string username, string password, string token)
        {
            var content = new FormUrlEncodedContent(Set(clientId,clientSecret,username,password,token));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await _http.PostAsync("https://login.salesforce.com/services/oauth2/token", content);
            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            SetInBuiltArgs(json);
            return true;
        }
        private void SetInBuiltArgs(JsonElement json)
        {
            _accessToken = json.GetProperty("access_token").GetString();
            _instanceUrl = json.GetProperty("instance_url").GetString();
        }
        public HttpRequestMessage CreateAccountAsync(string CompanyName)
        {
            var account = new { Name = CompanyName };
            return new HttpRequestMessage(HttpMethod.Post, $"{_instanceUrl}/services/data/v58.0/sobjects/Account")
            {
                Headers = { { "Authorization", $"Bearer {_accessToken}" } },
                Content = JsonContent.Create(account)
            };
        }
        public HttpRequestMessage CreateContactAsync(string CompanyName, string ContactFirstName, string ContactLastName, string ContactEmail,string accountId)
        {
            return new HttpRequestMessage(HttpMethod.Post, $"{_instanceUrl}/services/data/v58.0/sobjects/Contact")
            {
                Headers = { { "Authorization", $"Bearer {_accessToken}" } },
                Content = JsonContent.Create(new {
                    FirstName = ContactFirstName,
                    LastName = ContactLastName,
                    Email = ContactEmail,
                    AccountId = accountId
                })
            };
        }
        public async Task<bool> CreateAccountWithContactAsync(string CompanyName, string ContactFirstName, string ContactLastName, string ContactEmail)
        {
            if (!await EnsureAuthenticatedAsync()) return false;
            var response = await _http.SendAsync(CreateAccountAsync(CompanyName));
            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var accountId = json.GetProperty("id").GetString();
            response = await _http.SendAsync(CreateContactAsync(CompanyName,ContactFirstName,ContactLastName,ContactEmail,accountId));
            return response.IsSuccessStatusCode;
        }
    }
}
