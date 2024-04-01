using Moq;
using PokemonApp.Interfaces;
using PokemonApp.Controllers;
using AutoMapper;
using PokemonApp.Models;
using PokemonApp.Dto;
using Microsoft.AspNetCore.Mvc;

namespace PokemonAppTests.ControllersTest
{
    public class CategoryControllerTests
    {
        [Fact]
        public void GetCategories_ReturnsOkResultWithCategories()
        {
            var mockRepository = new Mock<ICategoryRepository>();
            var mockMapper = new Mock<IMapper>();
            var controller = new CategoryController(mockRepository.Object, mockMapper.Object);

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Test 1"},
                new Category { Id = 2, Name = "Test 2"}
            };

            mockRepository.Setup(repository => repository.GetCategories()).Returns(categories);
            mockMapper.Setup(mapper => mapper.Map<List<CategoryDto>>(categories)).Returns(new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Test 1"},
                new CategoryDto { Id = 2, Name = "Test 2"}
            });

            var result = controller.GetCategories();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCategories = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okResult.Value);
            Assert.Equal(categories.Count, returnCategories.Count());
        }
    }
}
