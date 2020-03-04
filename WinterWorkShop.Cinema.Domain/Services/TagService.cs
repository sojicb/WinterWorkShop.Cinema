using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagsRepository)
        {
            _tagRepository = tagsRepository;
        }

        public async Task<IEnumerable<TagDomainModel>> GetAllAsync()
        {
            var data = await _tagRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<TagDomainModel> result = new List<TagDomainModel>();
            TagDomainModel model;
            foreach (var item in data)
            {
                model = new TagDomainModel
                {
                   Id = item.Id,
                   value = item.Value
                   
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<TagDomainModel> GetTagByIdAsync(int id)
        {
            var data = await _tagRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            TagDomainModel domainModel = new TagDomainModel
            {
                Id = data.Id,
                value = data.Value
            };

            return domainModel;
        }

        public async Task<CreateTagResultModel> AddTag(TagDomainModel newTag)
        {
            Tag tagToCreate = new Tag()
            {
                Id = newTag.Id,
                Value = newTag.value
               
            };

            var createdTag = _tagRepository.Insert(tagToCreate);

            if(createdTag == null)
            {
                return new CreateTagResultModel
                {
                    IsSuccessful = false,

                    ErrorMessage = Messages.TAG_CREATION_ERROR
                }; 
            }

            _tagRepository.Save();

            CreateTagResultModel resultModel = new CreateTagResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Tag = new TagDomainModel
                {
                    Id = createdTag.Id,
                    value = createdTag.Value
                    
                }
            };

            return resultModel;
        }

        public async Task<TagDomainModel> UpdateTag(TagDomainModel tagToUpdate)
        {
            Tag tag = new Tag()
            {
                Id = tagToUpdate.Id,
                Value = tagToUpdate.value
                
            };

            var data = _tagRepository.Update(tag);

            if (data == null)
            {
                return null;
            }
             _tagRepository.Save();

            TagDomainModel domainModel = new TagDomainModel()
            {
                Id = data.Id,
                value = data.Value
            };

            return domainModel;
        }

        public async Task<TagDomainModel> DeleteTag(int id)
        {
            var data = _tagRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _tagRepository.Save();

            TagDomainModel domainModel = new TagDomainModel
            {
                Id = data.Id,
                value = data.Value

            };

            return domainModel;
        }
    }
}
