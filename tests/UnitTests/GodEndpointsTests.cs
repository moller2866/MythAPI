using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using MythApi.Common.Database.Models;
using MythApi.Endpoints.v1;
using MythApi.Gods.Interfaces;
using MythApi.Gods.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests
{
    public class GodEndpointsTests
    {
        private Mock<IGodRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IGodRepository>();
        }

        [Test]
        public async Task GetAllGods_ShouldReturnAllGods()
        {
            var gods = new List<God>
            {
                new God { Name = "Zeus", MythologyId = 1, Description = "God of the sky" },
                new God { Name = "Hera", MythologyId = 1, Description = "Goddess of marriage" }
            };
            _mockRepository.Setup(repo => repo.GetAllGodsAsync()).ReturnsAsync(gods);

            var result = await MythApi.Endpoints.v1.Gods.GetAlllGods(_mockRepository.Object);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task AddOrUpdateGods_ShouldAddNewGod()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = 1, Description = "God of the sky" }
            };
            var gods = new List<God>
            {
                new God { Name = "Zeus", MythologyId = 1, Description = "God of the sky" }
            };
            _mockRepository.Setup(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>())).ReturnsAsync(gods);

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            var okResult = result as IValueHttpResult;
            Assert.That(okResult, Is.Not.Null);
            var godList = okResult!.Value as List<God>;
            Assert.That(godList, Is.Not.Null);
            Assert.That(godList!.Count, Is.EqualTo(1));
            Assert.That(godList.First().Name, Is.EqualTo("Zeus"));
        }

        [Test]
        public async Task AddOrUpdateGods_EmptyName_ShouldReturnValidationProblem()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "", MythologyId = 1, Description = "Some description" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.InstanceOf<IResult>());
            // Repository should not be called when validation fails
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_NameTooLong_ShouldReturnValidationProblem()
        {
            var longName = new string('A', 101);
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = longName, MythologyId = 1, Description = "Some description" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.InstanceOf<IResult>());
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_InvalidCharactersInName_ShouldReturnValidationProblem()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus123", MythologyId = 1, Description = "Some description" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.InstanceOf<IResult>());
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_ValidNameWithHyphenAndApostrophe_ShouldSucceed()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "O'Brien-Thor", MythologyId = 1, Description = "A valid god name with hyphen and apostrophe" }
            };
            var gods = new List<God>
            {
                new God { Name = "O'Brien-Thor", MythologyId = 1, Description = "A valid god name with hyphen and apostrophe" }
            };
            _mockRepository.Setup(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>())).ReturnsAsync(gods);

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            var okResult = result as IValueHttpResult;
            Assert.That(okResult, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Once);
        }

        [Test]
        public async Task AddOrUpdateGods_InvalidMythologyId_ShouldReturnValidationProblem()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = 0, Description = "God of the sky" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.InstanceOf<IResult>());
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_EmptyDescription_ShouldReturnValidationProblem()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = 1, Description = "" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.InstanceOf<IResult>());
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }
    }
}
