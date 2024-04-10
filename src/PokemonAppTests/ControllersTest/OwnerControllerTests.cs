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
    }
}
