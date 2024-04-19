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
    public class ReviewerControllerTests
    {
        private Fixture _fixture;
        private Mock<IReviewerRepository> _mockReviewerRepo;
        private Mock<IMapper> _mockMapper;
        private ReviewerController _controller;

        public ReviewerControllerTests()
        {
            _fixture = new Fixture();
            _mockReviewerRepo = new Mock<IReviewerRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ReviewerController(_mockReviewerRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetReviewers_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var reviewers = _fixture.CreateMany<Reviewer>(5).ToList();

            _mockReviewerRepo.Setup(x => x.GetReviewers()).Returns(reviewers);
            _mockMapper.Setup(x => x.Map<List<ReviewerDto>>(reviewers)).Returns(reviewers.Select(r => new ReviewerDto
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
            }).ToList());

            // Act
            var result = _controller.GetReviewers();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockReviewerRepo
                .Verify(x => x.GetReviewers(), Times.Once);

            _mockMapper
                .Verify(x => x.Map<List<ReviewerDto>>(reviewers), Times.Once);
        }

        [Fact]
        public void GetReviewer_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            
            var reviewer = _fixture.Create<Reviewer>();

            _mockReviewerRepo.Setup(x => x.ReviewerExists(reviewer.Id)).Returns(true);
            _mockReviewerRepo.Setup(x => x.GetReviewer(reviewer.Id)).Returns(reviewer);
            _mockMapper.Setup(x => x.Map<ReviewerDto>(reviewer)).Returns((Reviewer r) => new ReviewerDto
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
            });

            // Act
            var result = _controller.GetReviewer(reviewer.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockReviewerRepo
                .Verify(x => x.ReviewerExists(reviewer.Id), Times.Once);

            _mockReviewerRepo
                .Verify(x => x.GetReviewer(reviewer.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<ReviewerDto>(reviewer), Times.Once);
        }

        [Fact]
        public void GetReviewsByReviewer_ReturnsOkResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var reviewer = _fixture.Create<Reviewer>();

            _mockReviewerRepo.Setup(x => x.ReviewerExists(reviewer.Id)).Returns(true);
            _mockReviewerRepo.Setup(x => x.GetReviewsByReviewer(reviewer.Id)).Returns(new List<Review>());

            // Act
            var result = _controller.GetReviewsByReviewer(reviewer.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockReviewerRepo
                .Verify(x => x.ReviewerExists(reviewer.Id), Times.Once);

            _mockReviewerRepo
                .Verify(x => x.GetReviewsByReviewer(reviewer.Id), Times.Once);
        }
    }
}
