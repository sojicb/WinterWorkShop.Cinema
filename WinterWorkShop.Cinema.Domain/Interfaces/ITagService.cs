using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ITagService
    {
        Task<TagDomainModel> GetTagByIdAsync(int id);

        Task<IEnumerable<TagDomainModel>> GetAllAsync();

        Task<CreateTagResultModel> AddTag(TagDomainModel domainModel);

        Task<TagDomainModel> UpdateTag(TagDomainModel tagToUpdate);

        Task<TagDomainModel> DeleteTag(int id);
    }
}
