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
        private Mock<ICountryRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private CountryController _controller;

        public CountryControllerTests()
        {
            _fixture = new Fixture();
            _mockRepository = new Mock<ICountryRepository>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CountryController(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public void GetCountries_ReturnsOkResultWithCountries()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var countries = _fixture.CreateMany<Country>(6).ToList();

            _mockRepository.Setup(repository => repository.GetCountries()).Returns(countries);
            _mockMapper.Setup(mapper => mapper.Map<List<CountryDto>>(countries)).Returns(countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList());

            var result = _controller.GetCountries();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetCountry_ReturnsOkResultWithCountry()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<Country>();

            _mockRepository.Setup(repository => repository.CountryExists(country.Id)).Returns(true);
            _mockRepository.Setup(repository => repository.GetCountry(country.Id)).Returns(country);
            _mockMapper.Setup(mapper => mapper.Map<CountryDto>(country)).Returns((Country c) => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
            });

            var result = _controller.GetCountry(country.Id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetCountryByOwner_ReturnsOkResultWithCountry()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var countries = _fixture.CreateMany<Country>().ToList();
            var randomCountry = countries[new Random().Next(countries.Count)];
            var owner = _fixture.Build<Owner>().Without(o => o.Id).With(o => o.Country, randomCountry).Create();

            _mockRepository.Setup(repository => repository.GetCountryByOwner(It.IsAny<int>()))
                   .Returns(randomCountry);

            _mockMapper.Setup(mapper => mapper.Map<CountryDto>(It.IsAny<Country>()))
               .Returns((Country c) => new CountryDto
               {
                   Id = c.Id,
                   Name = c.Name,
               });

            var result = _controller.GetCountryByOwner(owner.Id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CreateCountry_ReturnsOkResultWithCountry()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var countryDto = _fixture.Create<CountryDto>();

            _mockRepository.Setup(repository => repository.GetCountries()).Returns(new List<Country>());
            _mockRepository.Setup(repository => repository.CreateCountry(It.IsAny<Country>())).Returns(true);

            var result = _controller.CreateCountry(countryDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created", okResult.Value);
        }

        [Fact]
        public void UpdateCountry_ReturnsNoContentResult()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<Country>();
            var updatedCountry = _fixture.Build<CountryDto>().With(c => c.Id, country.Id).Create();

            _mockRepository.Setup(repository => repository.CountryExists(country.Id)).Returns(true);
            _mockRepository.Setup(repository => repository.UpdateCountry(It.IsAny<Country>())).Returns(true);

            var result = _controller.UpdateCountry(country.Id, updatedCountry);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCountry_ReturnsNoContentResult()
        {
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var country = _fixture.Create<Country>();

            _mockRepository.Setup(repository => repository.CountryExists(country.Id)).Returns(true);

            var result = _controller.DeleteCountry(country.Id);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
