using InccApi.DTOs;
using InccApi.DTOs.Mappings;
using InccApi.Models;
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
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> Get()
    {
        var inccEntries = await _inccRepository.GetAllAsync();

        if (inccEntries is null || !inccEntries.Any())
        {
            return NotFound();
        }

        var inccEntriesResponseDto = inccEntries.ToDto();

        return Ok(inccEntriesResponseDto);
    }
}
