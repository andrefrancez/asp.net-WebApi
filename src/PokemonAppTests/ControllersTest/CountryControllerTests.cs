using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PokemonApp.Controllers;
using PokemonApp.Dto;
using PokemonApp.Interfaces;
using PokemonApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonAppTests.ControllersTest
{
    public class CountryControllerTests
    {
        private Fixture _fixture;
        private Mock<ICountryRepository> _mockCountryRepo;
        private Mock<IMapper> _mockMapper;
        private CountryController _controller;

        public CountryControllerTests()
        {
            _fixture = new Fixture();
            _mockCountryRepo = new Mock<ICountryRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CountryController(_mockCountryRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetCountries_ReturnsOkResultWithCountries()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var countries = _fixture.CreateMany<Country>(6).ToList();

            _mockCountryRepo.Setup(x => x.GetCountries()).Returns(countries);
            _mockMapper.Setup(x => x.Map<List<CountryDto>>(countries)).Returns(countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList());

            // Act
            var result = _controller.GetCountries();

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockCountryRepo
                .Verify(x => x.GetCountries(), Times.Once);

            _mockMapper
                .Verify(x => x.Map<List<CountryDto>>(countries), Times.Once);
        }

        [Fact]
        public void GetCountry_ReturnsOkResultWithCountry()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<Country>();

            _mockCountryRepo.Setup(x => x.CountryExists(country.Id)).Returns(true);
            _mockCountryRepo.Setup(x => x.GetCountry(country.Id)).Returns(country);
            _mockMapper.Setup(x => x.Map<CountryDto>(country)).Returns((Country c) => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
            });

            // Act
            var result = _controller.GetCountry(country.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockCountryRepo
                .Verify(x => x.CountryExists(country.Id), Times.Once);

            _mockCountryRepo
                .Verify(x => x.GetCountry(country.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<CountryDto>(country), Times.Once);
        }

        [Fact]
        public void GetCountryByOwner_ReturnsOkResultWithCountry()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var owner = _fixture.Create<Owner>();
            var countries = _fixture.CreateMany<Country>().ToList();

            _mockCountryRepo.Setup(x => x.GetCountryByOwner(owner.Id)).Returns(It.IsAny<Country>());

            // Act
            var result = _controller.GetCountryByOwner(owner.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            _mockCountryRepo
                .Verify(x => x.GetCountryByOwner(owner.Id), Times.Once);
        }

        [Fact]
        public void CreateCountry_ReturnsOkResultWithCountry()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<CountryDto>();
            var countryMapped = _fixture.Build<Country>()
                .With(c => c.Id, country.Id)
                .With(c => c.Name, country.Name)
                .Create();

            _mockCountryRepo.Setup(x => x.GetCountries()).Returns(new List<Country>());
            _mockMapper.Setup(x => x.Map<Country>(country)).Returns(countryMapped);
            _mockCountryRepo.Setup(x => x.CreateCountry(countryMapped)).Returns(true);

            // Act
            var result = _controller.CreateCountry(country);

            // Assert
            Assert.IsType<CreatedResult>(result);

            _mockCountryRepo
                .Verify(x => x.GetCountries(), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Country>(country), Times.Once);

            _mockCountryRepo
                .Verify(x => x.CreateCountry(countryMapped), Times.Once);
        }

        [Fact]
        public void UpdateCountry_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<Country>();
            var CountryUpdate = _fixture.Build<CountryDto>().With(c => c.Id, country.Id).Create();

            _mockCountryRepo.Setup(x => x.CountryExists(country.Id)).Returns(true);
            _mockMapper.Setup(x => x.Map<Country>(CountryUpdate)).Returns(country);
            _mockCountryRepo.Setup(x => x.UpdateCountry(It.IsAny<Country>())).Returns(true);

            // Act
            var result = _controller.UpdateCountry(country.Id, CountryUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockCountryRepo
                .Verify(x => x.CountryExists(country.Id), Times.Once);

            _mockMapper
                .Verify(x => x.Map<Country>(CountryUpdate), Times.Once);

            _mockCountryRepo
                .Verify(x => x.UpdateCountry(It.IsAny<Country>()), Times.Once);
        }

        [Fact]
        public void DeleteCountry_ReturnsNoContentResult()
        {
            // Arrange
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<Country>();

            _mockCountryRepo.Setup(x => x.CountryExists(country.Id)).Returns(true);
            _mockCountryRepo.Setup(x => x.GetCountry(country.Id)).Returns(country);
            _mockCountryRepo.Setup(x => x.DeleteCountry(country)).Returns(true);

            // Act
            var result = _controller.DeleteCountry(country.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _mockCountryRepo
                .Verify(x => x.CountryExists(country.Id), Times.Once);

            _mockCountryRepo
                .Verify(x => x.GetCountry(country.Id), Times.Once);

            _mockCountryRepo
                .Verify(x => x.DeleteCountry(country), Times.Once);
        }
    }
}
