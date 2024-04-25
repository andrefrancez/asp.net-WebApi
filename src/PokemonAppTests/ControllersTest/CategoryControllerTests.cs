using Moq;
using PokemonApp.Interfaces;
using PokemonApp.Controllers;
using AutoMapper;
using PokemonApp.Models;
using PokemonApp.Dto;
using Microsoft.AspNetCore.Mvc;
using AutoFixture;

namespace PokemonAppTests.ControllersTest
{
    public class CategoryControllerTests
    {

        private CategoryController _controller;
        private Mock<ICategoryRepository> _mockCategoryRepo;
        private Mock<IMapper> _mockMapper;
        private Fixture _fixture;

        public CategoryControllerTests()
        {
            _mockCategoryRepo = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();
            _controller = new CategoryController(_mockCategoryRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetCategories_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var categories = _fixture.CreateMany<Category>().ToList();

            _mockCategoryRepo.Setup(x => x.GetCategories()).Returns(categories);
            _mockMapper.Setup(x => x.Map<List<CategoryDto>>(categories)).Returns(categories.Select(c => new CategoryDto 
            { 
                Id = c.Id,
                Name = c.Name 
            }).ToList());

            // Act
            var result = _controller.GetCategories();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockCategoryRepo
                .Verify(x => x.GetCategories(), Times.Once);

            _mockMapper
                .Verify(x => x.Map<List<CategoryDto>>(categories), Times.Once);
        }

        [Fact]
        public void GetCategory_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();

            _mockCategoryRepo.Setup(x => x.CategoryExists(category.Id)).Returns(true);
            _mockCategoryRepo.Setup(x => x.GetCategory(category.Id)).Returns(category);
            _mockMapper.Setup(x => x.Map<CategoryDto>(category)).Returns((Category c) => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });

            // Act
            var result = _controller.GetCategory(category.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockCategoryRepo
                .Verify(x => x.CategoryExists(category.Id), Times.Once);

            _mockCategoryRepo
                .Verify(x => x.GetCategory(category.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<CategoryDto>(category), Times.Once);
        }

        [Fact]
        public void GetPokemonByCategory_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();
            var pokemons = _fixture.CreateMany<Pokemon>().ToList();

            _mockCategoryRepo.Setup(x => x.GetPokemonByCategory(category.Id)).Returns(pokemons);
            _mockMapper.Setup(x => x.Map<List<PokemonDto>>(pokemons)).Returns(pokemons.Select(p => new PokemonDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
            }).ToList());

            // Act
            var result = _controller.GetPokemonByCategory(category.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockCategoryRepo
                .Verify(x => x.GetPokemonByCategory(category.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<List<PokemonDto>>(pokemons), Times.Once);
        }

        [Fact]
        public void CreateCategory_ReturnsCreatedResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<CategoryDto>();
            var categoryMapped = _fixture.Build<Category>()
                .With(c => c.Id, category.Id)
                .With(c => c.Name, category.Name)
                .Create();

            _mockCategoryRepo.Setup(x => x.GetCategories()).Returns(new List<Category>());
            _mockMapper.Setup(x => x.Map<Category>(category)).Returns(categoryMapped);
            _mockCategoryRepo.Setup(x => x.CreateCategory(categoryMapped)).Returns(true);

            //Act
            var result = _controller.CreateCategory(category);

            // Assert
            Assert.IsType<CreatedResult>(result);

            _mockCategoryRepo
                .Verify(x => x.GetCategories(), Times.Once);

            _mockCategoryRepo
                .Verify(x => x.CreateCategory(categoryMapped), Times.Once);
        }

        [Fact]
        public void UpdateCategory_ReturnsNoContent()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();
            var categoryUpdate = _fixture.Build<CategoryDto>()
                .With(cd => cd.Id, category.Id)
                .Create();

            _mockCategoryRepo.Setup(x => x.CategoryExists(category.Id)).Returns(true);
            _mockMapper.Setup(x => x.Map<Category>(categoryUpdate)).Returns(category);
            _mockCategoryRepo.Setup(x => x.UpdateCategory(It.IsAny<Category>())).Returns(true);

            // Act
            var result = _controller.UpdateCategory(category.Id, categoryUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockCategoryRepo
                .Verify(x => x.CategoryExists(category.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Category>(categoryUpdate), Times.Once);

            _mockCategoryRepo
                .Verify(x => x.UpdateCategory(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public void DeleteCategory_ReturnsNoContent()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();

            _mockCategoryRepo.Setup(x => x.CategoryExists(category.Id)).Returns(true);
            _mockCategoryRepo.Setup(x => x.GetCategory(category.Id)).Returns(category);
            _mockCategoryRepo.Setup(x => x.DeleteCategory(category)).Returns(true);

            // Act
            var result = _controller.DeleteCategory(category.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockCategoryRepo
                .Verify(x => x.CategoryExists(category.Id), Times.Once);

            _mockCategoryRepo
                .Verify(x => x.GetCategory(category.Id), Times.Once);

            _mockCategoryRepo
                .Verify(x => x.DeleteCategory(category), Times.Once);
        }
    }
}
