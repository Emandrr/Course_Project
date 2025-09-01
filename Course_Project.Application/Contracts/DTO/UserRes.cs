using Course_Project.Domain.Models.UserModels;
namespace Course_Project.Application.Contracts.DTO
{
    public class UserRes
    {
        public User? user { get; set; }
        public bool res { get; set; } = true;
    }
}
