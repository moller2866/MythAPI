using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MythApi.Common.Database.Models;
using MythApi.Common.Models;
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
        public async Task GetAllGods_WithoutPagination_ShouldReturnAllGods()
        {
            var gods = new List<God>
            {
                new God { Name = "Zeus", MythologyId = 1, Description = "God of the sky" },
                new God { Name = "Hera", MythologyId = 1, Description = "Goddess of marriage" }
            };
            _mockRepository.Setup(repo => repo.GetAllGodsAsync()).ReturnsAsync(gods);

            var result = await MythApi.Endpoints.v1.Gods.GetAllGods(_mockRepository.Object, null, null);

            Assert.That(result, Is.InstanceOf<Ok<IList<God>>>());
            var okResult = result as Ok<IList<God>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.Value.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllGods_WithPagination_ShouldReturnPagedResult()
        {
            var pagedResult = new PagedResult<God>
            {
                Items = new List<God>
                {
                    new God { Name = "Zeus", MythologyId = 1, Description = "God of the sky" }
                },
                Page = 1,
                PageSize = 1,
                TotalCount = 2
            };
            _mockRepository.Setup(repo => repo.GetAllGodsAsync(It.IsAny<PaginationParameters>()))
                .ReturnsAsync(pagedResult);

            var result = await MythApi.Endpoints.v1.Gods.GetAllGods(_mockRepository.Object, 1, 1);

            Assert.That(result, Is.InstanceOf<Ok<PagedResult<God>>>());
            var okResult = result as Ok<PagedResult<God>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.Value.Items.Count, Is.EqualTo(1));
            Assert.That(okResult.Value.TotalCount, Is.EqualTo(2));
            Assert.That(okResult.Value.TotalPages, Is.EqualTo(2));
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

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Zeus"));
        }
    }
}
