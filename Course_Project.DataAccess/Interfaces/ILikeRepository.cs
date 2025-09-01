using Course_Project.Domain.Models.InventoryModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Interfaces
{
    public interface ILikeRepository
    {
        public Task SetLike(Like like);
        public Task SaveAllAsync();
        public Task RemoveLike(Like like);
    }
}
