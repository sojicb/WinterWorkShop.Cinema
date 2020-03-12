using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class TagsServiceTests
    {
        public Mock<ITagRepository> _mockTagsRepository = new Mock<ITagRepository>();
        private Tag _tag;
        private TagDomainModel _tagDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _tag = new Tag
            {
                Id = 1,
                Value = "Naziv taga",

            };

            _tagDomainModel = new TagDomainModel
            {
                Id = 1,
                value = "Naziv taga"
            };
        }


        [TestMethod]
        public void TagService_GetAllAsync_ReturnListOfTags()
        {
            //Arrange
            int expectedResultCount = 1;
            List<Tag> tagModelList = new List<Tag>();
            tagModelList.Add(_tag);
            IEnumerable<Tag> tags = tagModelList;
            Task<IEnumerable<Tag>> responseTask = Task.FromResult(tags);

            _mockTagsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            TagService tagService = new TagService(_mockTagsRepository.Object);

            //Act
            var resultModel = tagService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<TagDomainModel>)resultModel;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_tag.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(TagDomainModel));
        }


        [TestMethod]
        public void TagService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Tag> tags = null;
            Task<IEnumerable<Tag>> responseTask = Task.FromResult(tags);

            _mockTagsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            TagService tagService = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TagService_CreateTag_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            Tag tags = null;
            Task<Tag> responseTask = Task.FromResult(tags);
            string expectedMessage = "Error occured while creating new tag, please try again.";

            _mockTagsRepository.Setup(x => x.Insert(responseTask.Result)).Returns(_tag);
            TagService tagsController = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagsController.AddTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }


        [TestMethod]
        public void TagService_CreateTag_InsertMocked_ReturnTags()
        {
            //Arrange
            Tag tags = _tag;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository = new Mock<ITagRepository>();

            _mockTagsRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(_tag);
            TagService tagService = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagService.AddTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_tag.Id, resultAction.Tag.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }

        [TestMethod]
        public void TagService_GetTagById_ReturnsTag()
        {
            //Arrange
            Tag tags = _tag;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository.Setup(x => x.GetByIdAsync(responseTask.Result.Id)).Returns(responseTask);
            TagService tagsController = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagsController.GetTagByIdAsync(responseTask.Result.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Id, responseTask.Result.Id);
        }


        [TestMethod]
        public void TagService_GetTagById_InsertedNull_ReturnsNull()
        {
            //Arrange
            Tag tags = null;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository = new Mock<ITagRepository>();

            _mockTagsRepository.Setup(x => x.Insert(It.IsAny<Tag>())).Returns(responseTask.Result);
            TagService tagService = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagService.GetTagByIdAsync(_tag.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TagService_DeleteTag_ReturnsDeletedTag()
        {
            //Arrange
            Tag tags = _tag;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository.Setup(x => x.Delete(responseTask.Result.Id)).Returns(responseTask.Result);
            TagService tagsController = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagsController.DeleteTag(_tagDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
        }


        [TestMethod]
        public void TagService_DeleteTag_InsertedNull_ReturnsNull()
        {
            //Arrange
            Tag tags = null;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository = new Mock<ITagRepository>();

            _mockTagsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(responseTask.Result);
            TagService tagService = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagService.DeleteTag(_tagDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void TagService_UpdateTag_ReturnsUpdatedTag()
        {
            //Arrange
            Tag tags = _tag;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository.Setup(x => x.Update(It.IsAny<Tag>())).Returns(responseTask.Result);
            TagService tagsController = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagsController.UpdateTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(responseTask.Result.Id, resultAction.Id);
        }


        [TestMethod]
        public void TagService_UpdateTag_InsertedNull_ReturnsNUll()
        {
            //Arrange
            Tag tags = null;
            Task<Tag> responseTask = Task.FromResult(tags);

            _mockTagsRepository = new Mock<ITagRepository>();

            _mockTagsRepository.Setup(x => x.Update(It.IsAny<Tag>())).Returns(responseTask.Result);
            TagService tagService = new TagService(_mockTagsRepository.Object);

            //Act
            var resultAction = tagService.UpdateTag(_tagDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }
    }
}
