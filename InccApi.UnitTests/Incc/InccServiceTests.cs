using InccApi.DTOs;
using InccApi.Models;
using InccApi.Repositories;
using InccApi.Services;
using NSubstitute;

namespace InccApi.UnitTests.Incc;

public class InccServiceTests
{
    private readonly InccService _inccService;
    private readonly IInccRepository _inccRepositoryMock;

    public InccServiceTests()
    {
        _inccRepositoryMock = Substitute.For<IInccRepository>();
        _inccService = new InccService(_inccRepositoryMock);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task AccumulatedVariationAsync_Should_ReturnNull_When_StartOrEndOrBothEntriesIsNull(
        bool startExists, bool endExists)
    {
        // Arrange
        var paramsDto = new InccAccumulatedParams
        {
            StartYear = 2023,
            StartMonth = 1,
            EndYear = 2023,
            EndMonth = 2
        };

        var startEntry = startExists ? new InccEntry { Value = 100 } : null;
        var endEntry = endExists ? new InccEntry { Value = 110 } : null;

        _inccRepositoryMock
            .GetByDateAsync(paramsDto.StartYear, paramsDto.StartMonth)
            .Returns(startEntry);
        _inccRepositoryMock
            .GetByDateAsync(paramsDto.EndYear, paramsDto.EndMonth)
            .Returns(endEntry);

        // Act
        var result = await _inccService.AccumulatedVariationAsync(paramsDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task
         AccumulatedVariationAsync_Should_CalculateAdjustedValue_When_AmountIsProvided()
    {
        // Arrange
        var paramsDto = new InccAccumulatedParams
        {
            Amount = 1000,
            StartYear = 2023,
            StartMonth = 1,
            EndYear = 2023,
            EndMonth = 2
        };

        var startEntry = new InccEntry
        {
            Value = 100.0m,
            ReferenceDate = new DateTime(2023, 1, 1),
        };

        var endEntry = new InccEntry
        {
            Value = 105.0m,
            ReferenceDate = new DateTime(2023, 2, 1),
        };

        _inccRepositoryMock.GetByDateAsync(2023, 1)
            .Returns(startEntry);
        _inccRepositoryMock.GetByDateAsync(2023, 2)
            .Returns(endEntry);

        // Act
        var result = await _inccService.AccumulatedVariationAsync(
            paramsDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("01/2023", result.StartDate);
        Assert.Equal("02/2023", result.EndDate);
        Assert.Equal(5.0, result.AccumulatedVariation);
        Assert.Equal(1050.0m, result.AdjustedValue);
    }

    [Fact]
    public async Task
        AccumulatedVariationAsync_Should_Return_AdjustedValueNull_When_AmountIsMissing()
    {
        // Arrange
        var paramsDto = new InccAccumulatedParams
        {
            Amount = null,
            StartYear = 2023,
            StartMonth = 1,
            EndYear = 2023,
            EndMonth = 2
        };

        var startEntry = new InccEntry
        {
            Value = 100.0m,
            ReferenceDate = new DateTime(2023, 1, 1),
        };

        var endEntry = new InccEntry
        {
            Value = 105.0m,
            ReferenceDate = new DateTime(2023, 2, 1),
        };

        _inccRepositoryMock.GetByDateAsync(2023, 1)
            .Returns(startEntry);
        _inccRepositoryMock.GetByDateAsync(2023, 2)
            .Returns(endEntry);

        // Act
        var result = await _inccService.AccumulatedVariationAsync(
            paramsDto);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AdjustedValue);
    }

    [Fact]
    public async Task AccumulatedVariationAsync_Should_ThrowArgumentException_WhenStartValueIsZero()
    {
        // Arrange
        var paramsDto = new InccAccumulatedParams
        {
            Amount = 1000,
            StartYear = 2023,
            StartMonth = 1,
            EndYear = 2023,
            EndMonth = 2
        };

        _inccRepositoryMock.GetByDateAsync(paramsDto.StartYear, paramsDto.StartMonth)
            .Returns(new InccEntry
            {
                Value = 0
            });
        _inccRepositoryMock.GetByDateAsync(paramsDto.EndYear, paramsDto.EndMonth)
            .Returns(new InccEntry
            {
                Value = 100.0m,
                ReferenceDate = new DateTime(2023, 2, 1),
            });

        // Act and Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _inccService.AccumulatedVariationAsync(paramsDto));

        Assert.Equal("The initial INCC value cannot be zero.", exception.Message);
    }
}
