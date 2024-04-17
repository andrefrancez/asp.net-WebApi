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

        [Fact]
        public void CreateReview_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var review = _fixture.Create<ReviewDto>();
            var reviewMapped = _fixture.Build<Review>()
                .With(x => x.Id, review.Id)
                .With(x => x.Title, review.Title)
                .With(x => x.Text, review.Text)
                .With(x => x.Rating, review.Rating)
                .Create();
            var pokemon = _fixture.Create<Pokemon>();
            var reviewer = _fixture.Create<Reviewer>();

            _mockReviewRepo.Setup(x => x.GetReviews()).Returns(new List<Review>());
            _mockMapper.Setup(x => x.Map<Review>(review)).Returns(reviewMapped);
            _mockPokemonRepo.Setup(x => x.GetPokemon(pokemon.Id)).Returns(pokemon);
            _mockReviewerRepo.Setup(x => x.GetReviewer(reviewer.Id)).Returns(reviewer);
            _mockReviewRepo.Setup(x => x.CreateReview(reviewMapped)).Returns(true);

            // Act
            var result = _controller.CreateReview(pokemon.Id, reviewer.Id, review);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", okResult.Value);

            _mockReviewRepo
                .Verify(x => x.GetReviews(), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Review>(review), Times.Once);

            _mockPokemonRepo
                .Verify(x => x.GetPokemon(pokemon.Id), Times.Once);

            _mockReviewerRepo
                .Verify(x => x.GetReviewer(reviewer.Id), Times.Once);

            _mockReviewRepo
                .Verify(x => x.CreateReview(reviewMapped), Times.Once);
        }

        [Fact]
        public void UpdateReview_ReturnsNoContent()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var review = _fixture.Create<Review>();
            var reviewUpdate = _fixture.Build<ReviewDto>()
                .With(r => r.Id, review.Id)
                .Create();

            _mockReviewRepo.Setup(x => x.ReviewExists(review.Id)).Returns(true);
            _mockReviewRepo.Setup(x => x.UpdateReview(It.IsAny<Review>())).Returns(true);

            // Act
            var result = _controller.UpdateReview(review.Id, reviewUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockReviewRepo
                .Verify(x => x.ReviewExists(review.Id), Times.Once);

            _mockReviewRepo
                .Verify(x => x.UpdateReview(It.IsAny<Review>()), Times.Once);
        }

        [Fact]
        public void DeleteReview_ReturnsNoContent()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var review = _fixture.Create<Review>();

            _mockReviewRepo.Setup(x => x.ReviewExists(review.Id)).Returns(true);
            _mockReviewRepo.Setup(x => x.GetReview(review.Id)).Returns(review);
            _mockReviewRepo.Setup(x => x.DeleteReview(review)).Returns(true);
            // Act
            var result = _controller.DeleteReview(review.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockReviewRepo
                .Verify(x => x.ReviewExists(review.Id), Times.Once);

            _mockReviewRepo
                .Verify(x => x.GetReview(review.Id), Times.Once);

            _mockReviewRepo
                .Verify(x => x.DeleteReview(review), Times.Once);
        }
    }
}
