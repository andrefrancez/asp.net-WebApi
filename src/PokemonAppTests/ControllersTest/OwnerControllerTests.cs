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
        private Mock<IOwnerRepository> _mockOwnerRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<ICountryRepository> _mockCountryRepository;
        private OwnerController _controller;

        public OwnerControllerTests()
        {
            _fixture = new Fixture();
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockCountryRepository = new Mock<ICountryRepository>();
            _controller = new OwnerController(_mockOwnerRepository.Object, _mockCountryRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetOwners_ReturnsOkResultWithOwners()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owners = _fixture.CreateMany<Owner>(6).ToList();

            _mockOwnerRepository.Setup(repository => repository.GetOwners()).Returns(owners);
            _mockMapper.Setup(mapper => mapper.Map<List<OwnerDto>>(owners)).Returns(owners.Select(o => new OwnerDto
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Gym = o.Gym,
            }).ToList());

            var result = _controller.GetOwners();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetOwner_ReturnsOkResultWithOwner()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();

            _mockOwnerRepository.Setup(repository => repository.OwnerExists(owner.Id)).Returns(true);
            _mockOwnerRepository.Setup(repository => repository.GetOwner(owner.Id)).Returns(owner);
            _mockMapper.Setup(mapper => mapper.Map<OwnerDto>(owner)).Returns((Owner o) => new OwnerDto
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Gym = o.Gym,
            });

            var result = _controller.GetOwner(owner.Id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetPokemonByOwner_ReturnsOkResultWithPokemon()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var pokemons = _fixture.CreateMany<Pokemon>().ToList();

            _mockOwnerRepository.Setup(repository => repository.OwnerExists(owner.Id)).Returns(true);
            _mockMapper.Setup(mapper => mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemons.Select(p => new PokemonDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
            }).ToList());

            var result = _controller.GetPokemonByOwner(owner.Id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateOwner_ReturnsOkResultWithOwner()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var ownerUpdate = _fixture.Build<OwnerDto>().With(o => o.Id, owner.Id).Create();

            _mockOwnerRepository.Setup(repository => repository.OwnerExists(owner.Id)).Returns(true);
            _mockOwnerRepository.Setup(repository => repository.UpdateOwner(It.IsAny<Owner>())).Returns(true);

            var result = _controller.UpdateOwner(owner.Id, ownerUpdate);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteOwner_ReturnsNoContentResultWithOwner()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();

            _mockOwnerRepository.Setup(repository => repository.OwnerExists(owner.Id)).Returns(true);

            var result = _controller.DeleteOwner(owner.Id);
            Assert.IsType<NoContentResult>(result);
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

            _mockOwnerRepository
                .Setup(x => x.GetOwners())
                .Returns(ownerQueryReturnList);

            _mockMapper
                .Setup(x => x.Map<Owner>(ownerRequest))
                .Returns(ownerMapped);

            _mockCountryRepository
                .Setup(x => x.GetCountry(ownerRequest.Id))
                .Returns(_fixture.Create<Country>());

            _mockOwnerRepository
                .Setup(repository => repository.CreateOwner(ownerMapped))
                .Returns(true);

            // Act

            var result = _controller.CreateOwner(ownerRequest.Id, ownerRequest);


            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", okResult.Value);

            _mockOwnerRepository
                .Verify(x => x.GetOwners(), Times.Once);
            _mockOwnerRepository
                .Verify(x => x.CreateOwner(ownerMapped), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Owner>(ownerRequest), Times.Once);

            _mockCountryRepository
                .Verify(x => x.GetCountry(ownerRequest.Id), Times.Once);

        }
    }
}
