using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonApp.Controllers;
using PokemonApp.Dto;
using PokemonApp.Interfaces;
using PokemonApp.Models;

namespace PokemonAppTests.ControllersTest
{
    public class PokemonControllerTests
    {
        private Fixture _fixture;
        private Mock<IPokemonRepository> _mockPokemonRepo;
        private Mock<IReviewRepository> _mockReviewRepo;
        private Mock<IMapper> _mockMapper;
        private PokemonController _controller;

        public PokemonControllerTests()
        {
            _fixture = new Fixture();
            _mockPokemonRepo = new Mock<IPokemonRepository>();
            _mockReviewRepo = new Mock<IReviewRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new PokemonController(_mockPokemonRepo.Object, _mockReviewRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetPokemons_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var pokemons = _fixture.CreateMany<Pokemon>(8).ToList();

            _mockPokemonRepo.Setup(x => x.GetPokemons()).Returns(pokemons);
            _mockMapper.Setup(x => x.Map<List<PokemonDto>>(pokemons)).Returns(pokemons.Select(p => new PokemonDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
            }).ToList());

            // Act
            var result = _controller.GetPokemons();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockPokemonRepo.Verify(x => x.GetPokemons(), Times.Once);
            _mockMapper.Verify(x => x.Map<List<PokemonDto>>(pokemons), Times.Once);
        }

        [Fact]
        public void GetPokemon_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var pokemon = _fixture.Create<Pokemon>();

            _mockPokemonRepo.Setup(x => x.PokemonExists(pokemon.Id)).Returns(true);
            _mockPokemonRepo.Setup(x => x.GetPokemon(pokemon.Id)).Returns(pokemon);
            _mockMapper.Setup(x => x.Map<PokemonDto>(pokemon)).Returns((Pokemon p) => new PokemonDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
            });

            // Act
            var result = _controller.GetPokemon(pokemon.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockPokemonRepo.Verify(x => x.PokemonExists(pokemon.Id), Times.Once);
            _mockPokemonRepo.Verify(x => x.GetPokemon(pokemon.Id), Times.Once);
            _mockMapper.Verify(x => x.Map<PokemonDto>(pokemon), Times.Once);
        }

        [Fact]
        public void GetPokemonRating_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var pokemon = _fixture
                .Build<Pokemon>()
                .Without(p => p.Reviews)
                .Do(p => p.Reviews = _fixture.CreateMany<Review>(7).ToList())
                .Create();

            _mockPokemonRepo.Setup(x => x.PokemonExists(pokemon.Id)).Returns(true);
            _mockPokemonRepo.Setup(x => x.GetPokemonRating(pokemon.Id)).Returns(It.IsAny<decimal>);

            // Act
            var result = _controller.GetPokemonRating(pokemon.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockPokemonRepo.Verify(x => x.PokemonExists(pokemon.Id), Times.Once);
            _mockPokemonRepo.Verify(x => x.GetPokemonRating(pokemon.Id), Times.Once);
        }

        [Fact]
        public void CreatePokemon_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var category = _fixture.Create<Category>();
            var pokemonDto = _fixture.Create<PokemonDto>();
            var pokemon = _fixture.Build<Pokemon>()
                .With(x => x.Id, pokemonDto.Id)
                .With(x => x.Name, pokemonDto.Name)
                .With(x => x.BirthDate, pokemonDto.BirthDate)
                .Create();
            var pokemons = _fixture.CreateMany<Pokemon>(5).ToList();

            _mockPokemonRepo.Setup(x => x.GetPokemons()).Returns(pokemons);
            _mockPokemonRepo.Setup(x => x.CreatePokemon(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Pokemon>())).Returns(true);


            // Act
            var result = _controller.CreatePokemon(owner.Id, category.Id, pokemonDto);

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", okResult.Value);

            _mockPokemonRepo
                .Verify(x => x.GetPokemons(), Times.Once);
            _mockPokemonRepo
                .Verify(x => x.CreatePokemon(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Pokemon>()), Times.Once);
        }

        [Fact]
        public void UpdateOwner_ReturnsNoContent()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var category = _fixture.Create<Category>();
            var pokemon = _fixture.Create<Pokemon>();
            var updatePokemon = _fixture.Build<PokemonDto>()
                .With(p => p.Id, pokemon.Id)
                .Create();

            _mockPokemonRepo.Setup(x => x.PokemonExists(pokemon.Id)).Returns(true);
            _mockPokemonRepo.Setup(x => x.UpdatePokemon(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Pokemon>())).Returns(true);

            // Act
            var result = _controller.UpdatePokemon(owner.Id, category.Id, pokemon.Id, updatePokemon);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockPokemonRepo
                .Verify(x => x.PokemonExists(pokemon.Id), Times.Once);
            _mockPokemonRepo
                .Verify(x => x.UpdatePokemon(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Pokemon>()), Times.Once);
        }

        [Fact]
        public void DeletePokemon_ReturnsNoContent()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var pokemon = _fixture.Create<Pokemon>();
            var reviews = _fixture.Create<Review>();

            _mockPokemonRepo.Setup(x => x.PokemonExists(pokemon.Id)).Returns(true);
            _mockPokemonRepo.Setup(x => x.DeletePokemon(pokemon)).Returns(true);
            _mockReviewRepo.Setup(x => x.GetReviewsOfAPokemon(pokemon.Id)).Returns(new List<Review>());

            // Act
            var result = _controller.DeletePokemon(pokemon.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockPokemonRepo
                .Verify(x => x.PokemonExists(pokemon.Id), Times.Once);
            _mockPokemonRepo
                .Verify(x => x.DeletePokemon(It.IsAny<Pokemon>()), Times.Once);
            _mockReviewRepo
                .Verify(x => x.GetReviewsOfAPokemon(pokemon.Id), Times.Once);
        }
    }
}
