using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonApp.Controllers;
using PokemonApp.Dto;
using PokemonApp.Interfaces;
using PokemonApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonAppTests.ControllersTest
{
    public class OwnerControllerTests
    {
        private Fixture _fixture;
        private Mock<IOwnerRepository> _mockOwnerRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ICountryRepository> _mockCountryRepo;
        private OwnerController _controller;

        public OwnerControllerTests()
        {
            _fixture = new Fixture();
            _mockOwnerRepo = new Mock<IOwnerRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCountryRepo = new Mock<ICountryRepository>();
            _controller = new OwnerController(_mockOwnerRepo.Object, _mockCountryRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetOwners_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owners = _fixture.CreateMany<Owner>(6).ToList();

            _mockOwnerRepo.Setup(x => x.GetOwners()).Returns(owners);
            _mockMapper.Setup(x => x.Map<List<OwnerDto>>(owners)).Returns(owners.Select(o => new OwnerDto
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Gym = o.Gym,
            }).ToList());

            // Act
            var result = _controller.GetOwners();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockOwnerRepo
                .Verify(x => x.GetOwners(), Times.Once);

            _mockMapper
                .Verify(x => x.Map<List<OwnerDto>>(owners), Times.Once);
        }

        [Fact]
        public void GetOwner_ReturnsOkResult()
        {
            //Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();

            _mockOwnerRepo.Setup(x => x.OwnerExists(owner.Id)).Returns(true);
            _mockOwnerRepo.Setup(x => x.GetOwner(owner.Id)).Returns(owner);
            _mockMapper.Setup(x => x.Map<OwnerDto>(owner)).Returns((Owner o) => new OwnerDto
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Gym = o.Gym,
            });

            // Act
            var result = _controller.GetOwner(owner.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockOwnerRepo
                .Verify(x => x.OwnerExists(owner.Id), Times.Once);

            _mockOwnerRepo
                .Verify(x => x.GetOwner(owner.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<OwnerDto>(owner), Times.Once);
        }

        [Fact]
        public void GetPokemonByOwner_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var pokemons = _fixture.CreateMany<Pokemon>().ToList();

            _mockOwnerRepo.Setup(x => x.OwnerExists(owner.Id)).Returns(true);
            _mockOwnerRepo.Setup(x => x.GetPokemonByOwner(owner.Id)).Returns(pokemons);
            _mockMapper.Setup(x => x.Map<List<PokemonDto>>(pokemons)).Returns(pokemons.Select(p => new PokemonDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
            }).ToList());

            // Act
            var result = _controller.GetPokemonByOwner(owner.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockOwnerRepo
                .Verify(x => x.OwnerExists(owner.Id), Times.Once);

            _mockOwnerRepo
                .Verify(x => x.GetPokemonByOwner(owner.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<List<PokemonDto>>(pokemons), Times.Once);
        }

        [Fact]
        public void UpdateOwner_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var ownerUpdate = _fixture.Build<OwnerDto>()
                .With(o => o.Id, owner.Id)
                .Create();

            _mockOwnerRepo.Setup(x => x.OwnerExists(owner.Id)).Returns(true);
            _mockMapper.Setup(x => x.Map<Owner>(ownerUpdate)).Returns(owner);
            _mockOwnerRepo.Setup(x => x.UpdateOwner(It.IsAny<Owner>())).Returns(true);

            // Act
            var result = _controller.UpdateOwner(owner.Id, ownerUpdate);
            
            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockOwnerRepo
                .Verify(x => x.OwnerExists(owner.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Owner>(ownerUpdate), Times.Once);

            _mockOwnerRepo
                .Verify(x => x.UpdateOwner(It.IsAny<Owner>()), Times.Once);
        }

        [Fact]
        public void DeleteOwner_ReturnsNoContentResultWithOwner()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();

            _mockOwnerRepo.Setup(x => x.OwnerExists(owner.Id)).Returns(true);
            _mockOwnerRepo.Setup(x => x.GetOwner(owner.Id)).Returns(owner);
            _mockOwnerRepo.Setup(x => x.DeleteOwner(owner)).Returns(true);

            // Act
            var result = _controller.DeleteOwner(owner.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockOwnerRepo
                .Verify(x => x.OwnerExists(owner.Id), Times.Once);

            _mockOwnerRepo
                .Verify(x => x.GetOwner(owner.Id), Times.Once);

            _mockOwnerRepo
                .Verify(x => x.DeleteOwner(owner), Times.Once);
        }

        [Fact]
        public void CreateOwner_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var ownerRequest = _fixture.Create<OwnerDto>();
            var ownerQueryReturnList = _fixture.CreateMany<Owner>(2).ToList();

            var ownerMapped = _fixture.Build<Owner>()
                .With(x => x.Id, ownerRequest.Id)
                .With(x => x.FirstName, ownerRequest.FirstName)
                .With(x => x.LastName, ownerRequest.LastName)
                .With(x => x.Gym, ownerRequest.Gym)
                .Create();

            _mockOwnerRepo
                .Setup(x => x.GetOwners())
                .Returns(ownerQueryReturnList);

            _mockMapper
                .Setup(x => x.Map<Owner>(ownerRequest))
                .Returns(ownerMapped);

            _mockCountryRepo
                .Setup(x => x.GetCountry(ownerRequest.Id))
                .Returns(_fixture.Create<Country>());

            _mockOwnerRepo
                .Setup(x => x.CreateOwner(ownerMapped))
                .Returns(true);

            // Act
            var result = _controller.CreateOwner(ownerRequest.Id, ownerRequest);

            // Assert
            Assert.IsType<CreatedResult>(result);

            _mockOwnerRepo
                .Verify(x => x.GetOwners(), Times.Once);

            _mockOwnerRepo
                .Verify(x => x.CreateOwner(ownerMapped), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Owner>(ownerRequest), Times.Once);

            _mockCountryRepo
                .Verify(x => x.GetCountry(ownerRequest.Id), Times.Once);
        }
    }
}
