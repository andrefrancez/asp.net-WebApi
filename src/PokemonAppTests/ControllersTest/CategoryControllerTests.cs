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
        private Mock<ICategoryRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private Fixture _fixture;

        public CategoryControllerTests()
        {
            _mockRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _fixture = new Fixture();
            _controller = new CategoryController(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetCategories_ReturnsOkResultWithCategories()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var categories = _fixture.CreateMany<Category>().ToList();

            _mockRepository.Setup(repository => repository.GetCategories()).Returns(categories);
            _mockMapper.Setup(mapper => mapper.Map<List<CategoryDto>>(categories)).Returns(categories.Select(c => new CategoryDto 
            { 
                Id = c.Id,
                Name = c.Name 
            }).ToList());

            var result = _controller.GetCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCategories = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okResult.Value);
            Assert.Equal(categories.Count, returnCategories.Count());
        }

        [Fact]
        public void GetCategory_ReturnsOkResultWithCategory()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();

            _mockRepository.Setup(repository => repository.CategoryExists(It.IsAny<int>())).Returns(true);
            _mockRepository.Setup(repository => repository.GetCategory(category.Id)).Returns(category);
            _mockMapper.Setup(mapper => mapper.Map<CategoryDto>(category)).Returns((Category c) => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });

            var result = _controller.GetCategory(category.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCategory = Assert.IsAssignableFrom<CategoryDto>(okResult.Value);
            Assert.Equal(category.Id, returnCategory.Id);
            Assert.Equal(category.Name, returnCategory.Name);
        }

        [Fact]
        public void GetCategoryByPokemon_ReturnsOkResultsWithCategory()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();
            var pokemons = _fixture.CreateMany<Pokemon>().ToList();

            _mockRepository.Setup(repository => repository.GetPokemonByCategory(category.Id)).Returns(pokemons);
            _mockMapper.Setup(mapper => mapper.Map<List<PokemonDto>>(pokemons)).Returns(pokemons.Select(p => new PokemonDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
            }).ToList());

            var result = _controller.GetPokemonByCategory(category.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnPokemons = Assert.IsAssignableFrom<IEnumerable<PokemonDto>>(okResult.Value);
            Assert.Equal(pokemons.Count, returnPokemons.Count());
        }

        [Fact]
        public void CreateCategory_ReturnsOkResultWithCategory()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var categoryCreate = _fixture.Create<CategoryDto>();
            var category = _fixture.Create<Category>();

            _mockRepository.Setup(repository => repository.GetCategories()).Returns(new List<Category>());
            _mockMapper.Setup(mapper => mapper.Map<Category>(categoryCreate)).Returns(category);
            _mockRepository.Setup(repository => repository.CreateCategory(It.IsAny<Category>())).Returns(true);

            var result = _controller.CreateCategory(categoryCreate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", okResult.Value);
        }

        [Fact]
        public void UpdateCategory_ReturnsNoContentResult()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var category = _fixture.Create<Category>();
            var updatedCategory = _fixture.Build<CategoryDto>().With(cd => cd.Id, category.Id).Create();

            if (category.Id != updatedCategory.Id)
                throw new InvalidOperationException("Category ID mismatch");

            _mockRepository.Setup(repository => repository.CategoryExists(category.Id)).Returns(true);
            _mockMapper.Setup(mapper => mapper.Map<Category>(updatedCategory)).Returns(new Category { Id = category.Id });
            _mockRepository.Setup(repository => repository.UpdateCategory(It.IsAny<Category>())).Returns(true);

            var result = _controller.UpdateCategory(category.Id, updatedCategory);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
