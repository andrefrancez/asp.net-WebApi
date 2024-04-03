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
    }
}
