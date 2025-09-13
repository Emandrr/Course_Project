using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface ISalesforceService
    {
        public Task<bool> EnsureAuthenticatedAsync();
        public Dictionary<string, string> Set(string clientId, string clientSecret, string username, string password, string token);
        public Task<bool> AuthenticateAsync(string clientId, string clientSecret, string username, string password, string token);
        public HttpRequestMessage CreateAccountAsync(string CompanyName);
        public HttpRequestMessage CreateContactAsync(string CompanyName, string ContactFirstName, string ContactLastName, string ContactEmail, string accountId);
        public Task<bool> CreateAccountWithContactAsync(string CompanyName, string ContactFirstName, string ContactLastName, string ContactEmail);
    }
}
