using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class ProjectionsServiceTests
    {
        private Mock<IProjectionsRepository> _mockProjectionsRepository = new Mock<IProjectionsRepository>();
        private Projection _projection;
        private ProjectionDomainModel _projectionDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _projection = new Projection
            {
                Id = new Guid("A3DB9208-FC16-4FF2-8489-0214E4EF512B"),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                DateTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };

            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };

            
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnListOfProjecrions()
        {
            //Arrange
            int expectedResultCount = 1;
            List<Projection> mockedProjections = new List<Projection>();
            mockedProjections.Add(_projection);
            IEnumerable<Projection> projections = mockedProjections;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);
            //Act
            var resultAction = projectionsController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult().ToList();
            var result = (List<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_projection.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(ProjectionDomainModel));
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Projection> projections = null;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return list with projections
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - true
        // return ErrorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_WithProjectionAtSameTime_ReturnErrorMessage() 
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            projectionsModelsList.Add(_projection);
            string expectedMessage = "Cannot create new projection, there are projections at same time alredy.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return null
        //  if (insertedProjection == null) - true
        // return CreateProjectionResultModel  with errorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            _projection = null;
            string expectedMessage = "Error occured while creating new projection, please try again.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return valid EntityEntry<Projection>
        //  if (insertedProjection == null) - false
        // return valid projection 
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMocked_ReturnProjection()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);
                    
            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_projection.Id, resultAction.Projection.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);   
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void Projectionervice_CreateProjection_When_Updating_Non_Existing_Movie()
        {
            // Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Throws(new DbUpdateException());
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ProjectionService_UpdateProjection_InsertedNull_ReturnNull()
        {
            //Arrange

            Projection projections = null;
            Task<Projection> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns(responseTask.Result);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);
            //Act
            var resultAction = projectionsController.UpdateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }
        

       [TestMethod]
        public void ProjectionService_UpdateProjection_ReturnUpdatedProjection()
        {
            //Arrange
            Projection projections = _projection;
            Task<Projection> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns(responseTask.Result);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.UpdateProjection(_projectionDomainModel)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(resultAction.Id, responseTask.Result.Id);
        }

        [TestMethod]
        public void ProjectionService_DeleteProjection_ReturnDeletedProjection()
        {
            //Arrange

            Projection projections = _projection;
            Task<Projection> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.Delete(responseTask.Result.Id)).Returns(responseTask.Result);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);
            //Act
            var resultAction = projectionsController.DeleteProjection(responseTask.Result.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(responseTask.Result.Id, resultAction.Id);
        }


        [TestMethod]
        public void ProjectionService_DeleteProjection_InsertNull_ReturnNull()
        {
            //Arrange
            Projection projections = null;
            Task<Projection> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(responseTask.Result);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.DeleteProjection(new Guid())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void ProjectionService_GetProjectionsById_ReturnProjections()
        {
            //Arrange
            Projection projections = _projection;
            Task<Projection> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(responseTask.Result.Id)).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);
            //Act
            var resultAction = projectionsController.GetProjectionByIdAsync(responseTask.Result.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(responseTask.Result.Id, resultAction.Id);
        }


        [TestMethod]
        public void ProjectionService_GetProjectionsById_InsertedMockedNull_ReturnsNull()
        {
            //Arrange
            Projection projections = null;
            Task<Projection> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.GetProjectionByIdAsync(new Guid())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        [TestMethod]
        public void ProjectionService_FilterProjections_ReturnFilteredProjections()
        {
            //Arrange
            int expectedResultCount = 1;
            List<Projection> mockedProjections = new List<Projection>();
            mockedProjections.Add(_projection);
            IEnumerable<Projection> projections = mockedProjections;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);
            //Act
            var resultAction = projectionsController.FilterProjections(1, 1, new Guid("A3DB9208-FC16-4FF2-8489-0214E4EF512B"), DateTime.Parse("1993-09-20 00:07:57.590"), DateTime.Parse("2022-09-20 00:07:57.590")).ConfigureAwait(false).GetAwaiter().GetResult().ToList();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedResultCount, resultAction.Count);
            Assert.IsTrue(resultAction.Any(x => x.Id.Equals(responseTask.Result.ElementAt(0).Id)));
        }


        [TestMethod]
        public void ProjectionService_FilterProjectionsInsertMockedNull_ReturnNull()
        {
            //Arrange 
            IEnumerable<Projection> projections = null;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object);

            //Act
            var resultAction = projectionsController.GetProjectionByIdAsync(new Guid())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }
    }
}
