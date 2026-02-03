using InccApi.DTOs;
using InccApi.DTOs.Mappings;
using InccApi.Models;
using InccApi.Pagination;
using InccApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace InccApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class InccController : ControllerBase
{
    private readonly IInccRepository _inccRepository;

    public InccController(IInccRepository inccRepository)
    {
        _inccRepository = inccRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> GetPaginated(
        [FromQuery] PaginationParams paginationParams)
    {
        var (items, totalCount) = await _inccRepository.GetPaginatedAsync(paginationParams);

        var response = new InccPagedResponseDTO<InccResponseDTO>
        {
            Items = items.ToDtoList(),
            CurrentPage = paginationParams.PageNumber,
            TotalPages = (int)Math.Ceiling((double)totalCount / paginationParams.PageSize),
            TotalItems = totalCount
        };

        return Ok(response);
    }

    [HttpGet("{year:int:range(1995,2100)}/{month:int:range(1,12)}")]
    public async Task<ActionResult<InccResponseDTO>> Get(int year, int month)
    {
        var inccEntry = await _inccRepository.GetByDateAsync(year, month);

        if (inccEntry is null)
        {
            return NotFound();
        }

        var inccEntryResponseDto = inccEntry.ToDto();

        return Ok(inccEntryResponseDto);
    }

    [HttpGet("range")]
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> Get([FromQuery] InccFilter filter)
    {
        var start = filter.GetStartDate();

        if (start == null)
        {
            return BadRequest("Data inicial inválida");
        }

        if (!filter.IsValid())
        {
            return BadRequest("A data inicial não pode ser superior à final.");
        }

        var entries = await _inccRepository.GetRangeAsync(start.Value, filter.GetEndDate());

        if (entries == null || !entries.Any())
        {
            return NotFound();
        }

        return Ok(entries.ToDtoList());
    }
}
