using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonApp.Controllers;
using PokemonApp.Dto;
using PokemonApp.Interfaces;
using PokemonApp.Models;

namespace PokemonAppTests.ControllersTest
{
    public class ReviewControllerTests
    {
        private Fixture _fixture;
        private Mock<IReviewRepository> _mockReviewRepo;
        private Mock<IPokemonRepository> _mockPokemonRepo;
        private Mock<IReviewerRepository> _mockReviewerRepo;
        private Mock<IMapper> _mockMapper;
        private ReviewController _controller;

        public ReviewControllerTests()
        {
            _fixture = new Fixture();
            _mockReviewRepo = new Mock<IReviewRepository>();
            _mockPokemonRepo = new Mock<IPokemonRepository>();
            _mockReviewerRepo = new Mock<IReviewerRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ReviewController(
                _mockReviewRepo.Object, 
                _mockMapper.Object, 
                _mockPokemonRepo.Object, 
                _mockReviewerRepo.Object
                );
        }

        [Fact]
        public void GetReviews_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var reviews = _fixture.CreateMany<Review>(6).ToList();

            _mockReviewRepo.Setup(x => x.GetReviews()).Returns(reviews);
            _mockMapper.Setup(x => x.Map<List<ReviewDto>>(reviews)).Returns(reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Text = r.Text,
                Rating = r.Rating,
            }).ToList());

            // Act
            var result = _controller.GetReviews();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockReviewRepo.Verify(x => x.GetReviews(), Times.Once);
            _mockMapper.Verify(x => x.Map<List<ReviewDto>>(reviews), Times.Once);
        }

        [Fact]
        public void GetReview_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var review = _fixture.Create<Review>();

            _mockReviewRepo.Setup(x => x.ReviewExists(review.Id)).Returns(true);
            _mockReviewRepo.Setup(x => x.GetReview(review.Id)).Returns(review);
            _mockMapper.Setup(x => x.Map<ReviewDto>(review)).Returns((Review r) => new ReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Text = r.Text,
                Rating = r.Rating,
            });

            // Act
            var result = _controller.GetReview(review.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockReviewRepo.Verify(x => x.ReviewExists(review.Id), Times.Once);
            _mockReviewRepo.Verify(x => x.GetReview(review.Id), Times.Once);
            _mockMapper.Verify(x => x.Map<ReviewDto>(review), Times.Once);
        }

        [Fact]
        public void GetReviewOfAPokemon_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var pokemon = _fixture.Create<Pokemon>();

            _mockReviewRepo.Setup(x => x.GetReviewsOfAPokemon(pokemon.Id)).Returns(new List<Review>());

            // Act
            var result = _controller.GetReviewsOfAPokemon(pokemon.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockReviewRepo.Verify(x => x.GetReviewsOfAPokemon(pokemon.Id), Times.Once);
        }
    }
}
