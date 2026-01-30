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
    public async Task<ActionResult<IEnumerable<InccEntry>>> Get()
    {
        var inccEntries = await _inccRepository.GetAllAsync();

        if (inccEntries is null)
        {
            return NotFound();
        }

        return Ok(inccEntries);
    }
}
