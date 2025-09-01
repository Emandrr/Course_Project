using Course_Project.DataAccess.Data;
using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetAllCategoriesAcync();
        public Task<Category> GetCategoryByIdAsync(int id);
    }
}
