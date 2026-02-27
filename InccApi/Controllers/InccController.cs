using InccApi.DTOs;
using InccApi.DTOs.Mappings;
using InccApi.Extensions;
using InccApi.Pagination;
using InccApi.Repositories;
using InccApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InccApi.Controllers;


[EnableRateLimiting("per-user")]
[Route("api/[controller]")]
[ApiController]
public class InccController : ControllerBase
{
    private readonly IInccRepository _inccRepository;
    private readonly IInccService _inccService;

    public InccController(IInccRepository inccRepository, IInccService inccService)
    {
        _inccRepository = inccRepository;
        _inccService = inccService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> GetPaginated(
        [FromQuery] PaginationParams paginationParams)
    {
        var entries = await _inccRepository.GetPaginatedAsync(paginationParams);

        if (entries == null || !entries.Any())
        {
            return NotFound();
        }

        Response.AddPaginationHeader(
            entries.CurrentPage, 
            entries.PageSize, 
            entries.TotalCount, 
            entries.TotalPages,
            entries.HasNext,
            entries.HasPrevious
        );

        return Ok(entries.ToDtoList());
    }

    [HttpGet("{year:int:range(1994,2026)}/{month:int:range(1,12)}")]
    public async Task<ActionResult<InccResponseDTO>> Get(int year, int month)
    {
        var inccEntry = await _inccRepository.GetByDateAsync(year, month);

        if (inccEntry is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Entry not found",
                Detail = $"No entry found for {month}/{year}",
                Status = StatusCodes.Status404NotFound
            });
        }

        var inccEntryResponseDto = inccEntry.ToDto();

        return Ok(inccEntryResponseDto);
    }

    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> Get(
        [FromQuery] InccRangeParams @params)
    {
        var start = @params.GetStartDate();
        var end = @params.GetEndDate();

        if (!@params.IsValid())
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid date range",
                Detail = "Start date can't be greater than end date",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var entries = await _inccRepository.GetRangeAsync(@params, start, end);

        if (entries == null || !entries.Any())
            return NotFound(new ProblemDetails
            {
                Title = "No entries found",
                Detail = "No entries found for the specified range",
                Status = StatusCodes.Status404NotFound
            });

        Response.AddPaginationHeader(
            entries.CurrentPage,
            entries.PageSize,
            entries.TotalCount,
            entries.TotalPages,
            entries.HasNext,
            entries.HasPrevious
        );

        return Ok(entries.ToDtoList());
    }

    [HttpGet("accumulated")]
    public async Task<ActionResult<InccAccumulatedResponseDTO>> Get(
        [FromQuery] InccAccumulatedParams @params)
    {
        if (!@params.IsValid())
        {
            return BadRequest("A data inicial não pode ser superior à final");
        }

        var accumulatedDto = await _inccService.AccumulatedVariationAsync(@params);

        if (accumulatedDto == null)
        {
            return NotFound();
        }

        return Ok(accumulatedDto);
    }
}
