using InccApi.DTOs;
using InccApi.Models;
using InccApi.Pagination;
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

    [Fact]
    public async Task GetByDateAsync_Should_ReturnDto_When_EntryExists()
    {
        // Arrange
        var year = 2025;
        var month = 1;
        var entry = new InccEntry
        {
            ReferenceDate = new DateTime(year, month, 1),
            Value = 100.0m,
            MonthlyVariation = 0.5
        };

        _inccRepositoryMock.GetByDateAsync(year, month).Returns(entry);

        // Act
        var result = await _inccService.GetByDateAsync(year, month);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("01/2025", result.MonthYear);
        Assert.Equal(100.0m, result.Value);
    }

    [Fact]
    public async Task GetByDateAsync_Should_ReturnNull_When_EntryDoesNotExist()
    {
        // Arrange
        _inccRepositoryMock.GetByDateAsync(Arg.Any<int>(), Arg.Any<int>())
                            .Returns(Task.FromResult<InccEntry>(null));
        
        // Act
        var result = await _inccService.GetByDateAsync(1, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPaginatedAsync_Should_ReturnPagedDtos_WithCorrectMetadata()
    {
        // Arrange
        var paginationParams = new PaginationParams { PageNumber = 1, PageSize = 5 };
        var entries = new List<InccEntry>
        {
            new InccEntry { ReferenceDate = new DateTime(2023, 1, 1), Value = 100.0m },
            new InccEntry { ReferenceDate = new DateTime(2024, 1, 1), Value = 200.0m },
        };
        var pagedEntries = new PagedList<InccEntry>(entries, 2, 1, 5);

        _inccRepositoryMock.GetPaginatedAsync(paginationParams).Returns(pagedEntries);

        // Act
        var result = await _inccService.GetPaginatedAsync(paginationParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.IsType<InccResponseDTO>(result.Items.First());
        Assert.Equal(100.0m, result.Items.First().Value);
    }

    [Fact]
    public async Task GetPaginatedAsync_Should_ReturnEmptyPagedList_IfNoEntriesFound()
    {
        // Arrange
        var paginationParams = new PaginationParams();
        var emptyPagedList = new PagedList<InccEntry>(new List<InccEntry>(), 0, 1, 10);

        _inccRepositoryMock.GetPaginatedAsync(paginationParams).Returns(emptyPagedList);

        // Act
        var result = await _inccService.GetPaginatedAsync(paginationParams);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetRangeAsync_Should_ReturnPagedDtos_WithCorrectMetadata()
    {
        // Arrange
        var rangeParams = new InccRangeParams
        {
            StartYear = 2025,
            StartMonth = 1
        };

        var entries = new List<InccEntry>
        {
            new InccEntry { ReferenceDate = new DateTime(2023, 1, 1), Value = 100.0m },
            new InccEntry { ReferenceDate = new DateTime(2024, 1, 1), Value = 200.0m },
        };

        var pagedList = new PagedList<InccEntry>(entries, 2, 1, 10);

        _inccRepositoryMock.GetRangeAsync(rangeParams, Arg.Any<DateTime>(), Arg.Any<DateTime?>())
            .Returns(pagedList);

        // Act
        var result = await _inccService.GetRangeAsync(rangeParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.IsType<InccResponseDTO>(result.Items.First());
        Assert.Equal(100.0m, result.Items.First().Value);
    }

    [Fact]
    public async Task GetRangeAsync_Should_ReturnEmptyPagedList_IfNoEntriesFound()
    {
        // Arrange
        var rangeParams = new InccRangeParams
        {
            StartYear = 2025,
            StartMonth = 1
        };
        var emptyPagedList = new PagedList<InccEntry>(new List<InccEntry>(), 0, 1, 10);

        _inccRepositoryMock.GetRangeAsync(rangeParams, Arg.Any<DateTime>(), Arg.Any<DateTime?>())
            .Returns(emptyPagedList);

        // Act
        var result = await _inccService.GetRangeAsync(rangeParams);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task CreateAsync_Should_NormalizeDateAndReturnDto_WhenEntryIsNew()
    {
        // Arrange
        var inputDate = new DateTime(2026, 1, 15);
        var expectedNormalizedDate = new DateTime(2026, 1, 1);

        var entryCreateDto = new InccCreateDto
        {
            MonthlyVariation = 123,
            ReferenceDate = inputDate,
            Value = 456.78m,
        };

        _inccRepositoryMock.GetByDateAsync(2026, 1)
            .Returns(Task.FromResult<InccEntry?>(null));

        _inccRepositoryMock.CreateAsync(Arg.Any<InccEntry>())
            .Returns(args => (InccEntry)args[0]);

        // Act
        var result = await _inccService.CreateAsync(entryCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<InccResponseDTO>(result);
        Assert.Equal(456.78m, result.Value);
        Assert.Equal("01/2026", result.MonthYear);

        await _inccRepositoryMock.Received(1).CreateAsync(Arg.Is<InccEntry>(e => 
            e.ReferenceDate == expectedNormalizedDate && e.Value == 456.78m));
    }

    [Fact]
    public async Task CreateAsync_Should_ReturnNull_WhenEntryAlreadyExists()
    {
        // Arrange
        var entryCreateDto = new InccCreateDto
        {
           ReferenceDate = new DateTime(2026, 1, 1),
        };

        _inccRepositoryMock.GetByDateAsync(2026, 1)
            .Returns(new InccEntry());

        // Act
        var result = await _inccService.CreateAsync(entryCreateDto);

        // Assert
        Assert.Null(result);
    }
}
