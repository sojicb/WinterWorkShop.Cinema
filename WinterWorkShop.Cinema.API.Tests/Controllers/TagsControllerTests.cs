using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class TagsControllerTests
    {
        private Mock<ITagService> _tagService = new Mock<ITagService>();

        [TestMethod]
        public void TagsController_GetAsync_ReturnAllTags()
        {
            //Arrange
            List<TagDomainModel> tagDomainModelsList = new List<TagDomainModel>();
            TagDomainModel tagDomainModel = new TagDomainModel
            {
                Id = 1,
                value = "naziv taga"
            };

            tagDomainModelsList.Add(tagDomainModel);
            IEnumerable<TagDomainModel> tagDomainModels = tagDomainModelsList;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
;
            _tagService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var tagDomainModelResultList = (List<TagDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(tagDomainModelResultList);
            Assert.AreEqual(expectedResultCount, tagDomainModelResultList.Count);
            Assert.AreEqual(tagDomainModel.Id, tagDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void TagsConrtoller_GetAsync_ReturnEmptyList()
        {
            //Arrange
            IEnumerable<TagDomainModel> tagDomainModels = null;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _tagService = new Mock<ITagService>();
            _tagService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var tagDomainModelResultList = (List<TagDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(tagDomainModelResultList);
            Assert.AreEqual(expectedResultCount, tagDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetByIdAsync_Returns_Tag()
        {
            //Arrange
            List<TagDomainModel> tagDomainModelsList = new List<TagDomainModel>();
            TagDomainModel tagDomainModel = new TagDomainModel
            {
                Id = 1,
                value = "naziv taga"
            };

            tagDomainModelsList.Add(tagDomainModel);
            Task<TagDomainModel> responseTask = Task.FromResult(tagDomainModel);
            int expectedStatusCode = 200;

            _tagService.Setup(x => x.GetTagByIdAsync(tagDomainModel.Id)).Returns(responseTask);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.GetByIdAsync(tagDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var tagDomainModelResultList = (TagDomainModel)resultList;

            //Assert
            Assert.IsNotNull(tagDomainModelResultList);
            Assert.AreEqual(tagDomainModel.Id, tagDomainModelResultList.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void TagsConrtoller_GetByIdAsync_ReturnTagNotFound()
        {

            //Arrange
            int Id = 1;

            TagDomainModel tagDomainModel = null;
            Task<TagDomainModel> responseTask = Task.FromResult(tagDomainModel);
            int expectedStatusCode = 404;

            _tagService.Setup(x => x.GetTagByIdAsync(Id)).Returns(responseTask);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.GetByIdAsync(Id).ConfigureAwait(false).GetAwaiter().GetResult().Result;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            Assert.AreEqual(expectedStatusCode, ((NotFoundObjectResult)result).StatusCode);
        }


        [TestMethod]
        public void UsersConrtoller_GetAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<TagDomainModel> tagDomainModels = null;
            Task<IEnumerable<TagDomainModel>> responseTask = Task.FromResult(tagDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _tagService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var tagDomainModelResultList = (List<TagDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(tagDomainModelResultList);
            Assert.AreEqual(expectedResultCount, tagDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_createTagResultModel_IsSuccessful_True_Tag()
        {
            //Arrange
            int expectedStatusCode = 201;

            TagModel tagModel = new TagModel()
            {
                Id = 1,
                Value = "Naziv taga"
            };

            CreateTagResultModel createTagResultModel = new CreateTagResultModel
            {
                Tag = new TagDomainModel
                {
                    Id = 1,
                    value = "Naziv taga"
                },
                IsSuccessful = true
            };

            Task<CreateTagResultModel> responseTask = Task.FromResult(createTagResultModel);

            _tagService = new Mock<ITagService>();
            _tagService.Setup(x => x.AddTag(It.IsAny<TagDomainModel>())).Returns(responseTask);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.Post(tagModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var tagDomainModel = (TagDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(tagDomainModel);
            Assert.AreEqual(tagModel.Id, tagModel.Id);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Tag()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            TagModel tagModel = new TagModel()
            {
                Id = 1,
                Value = "Naziv taga"
            };

            CreateTagResultModel createTagResultModel = new CreateTagResultModel
            {
                Tag = new TagDomainModel
                {
                    Id = 1,
                    value = "Naziv taga"
                },
                IsSuccessful = true
            };

            Task<CreateTagResultModel> responseTask = Task.FromResult(createTagResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _tagService = new Mock<ITagService>();
            _tagService.Setup(x => x.AddTag(It.IsAny<TagDomainModel>())).Throws(dbUpdateException);
            TagsController tagsController = new TagsController(_tagService.Object);

            //Act
            var result = tagsController.Post(tagModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedErrorMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            TagModel tagModel = new TagModel()
            {
                Id = 1,
                Value = "Naziv taga"
            };

            _tagService = new Mock<ITagService>();
            TagsController tagsController = new TagsController(_tagService.Object);
            tagsController.ModelState.AddModelError("key","Invalid Model State");

            //Act
            var result = tagsController.Post(tagModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedErrorMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);


        }



    }
}
