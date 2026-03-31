using InccApi.Authentication;
using InccApi.DTOs;
using InccApi.Extensions;
using InccApi.Pagination;
using InccApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InccApi.Controllers;


[EnableRateLimiting("per-user")]
[Route("api/[controller]")]
[ApiController]
public class InccController : ControllerBase
{
    private readonly IInccService _inccService;

    public InccController(IInccService inccService)
    {
        _inccService = inccService;
    }

    /// <summary>
    /// Obtém uma lista paginada de todos os registros do INCC-M.
    /// </summary>
    /// <param name="paginationParams">Parâmetros de paginação (PageNumber e
    /// PageSize).</param>
    /// <returns>Uma coleção paginada de registros do INCC-M.</returns>
    /// <response code="200">Retorna a lista de índices solicitada.</response>
    /// <response code="404">Nenhum registro encontrado para a página 
    /// especificada.
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> GetPaginated(
        [FromQuery] PaginationParams paginationParams)
    {
        var entries = await _inccService.GetPaginatedAsync(paginationParams);

        if (!entries.Items.Any())
        {
            return NotFound(new ProblemDetails
            {
                Title = "No entries found",
                Detail = "No entries found for the specified page",
                Status = StatusCodes.Status404NotFound
            });
        }

        Response.AddPaginationHeader(
            entries.CurrentPage, 
            entries.PageSize, 
            entries.TotalCount, 
            entries.TotalPages,
            entries.HasNext,
            entries.HasPrevious
        );

        return Ok(entries.Items);
    }

    /// <summary>
    /// Busca um índice específico do INCC-M baseado no ano e mês.
    /// </summary>
    /// <remarks>
    /// Exemplo de request: GET /api/incc/2023/5
    /// </remarks>
    /// <param name="year">Ano do índice</param>
    /// <param name="month">Mês do índice</param>
    /// <returns>Os dados do INCC-M para a data informada.</returns>
    /// <response code="200">Retorna o índice solicitado.</response>
    /// <response code="404">Nenhum índice encontrado para o ano e mês 
    /// especificados.</response>
    [HttpGet("{year:int:range(1994,2026)}/{month:int:range(1,12)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<InccResponseDTO>> Get(int year, int month)
    {
        var entry = await _inccService.GetByDateAsync(year, month);

        if (entry is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Entry not found",
                Detail = $"No entry found for {month}/{year}",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(entry);
    }

    /// <summary>
    /// Obtém registros do INCC-M dentro do intervalo de datas especificado.
    /// </summary>
    /// <param name="params">Parâmetros de intervalo e paginação.</param>
    /// <returns>Uma lista de registros dentro do período solicitado.</returns>
    /// <response code="200">Retorna os índices dentro do intervalo 
    /// especificado.</response>
    /// <response code="400">O intervalo de datas fornecido é inválido.</response>
    /// <response code="404">Nenhum índice encontrado para o intervalo 
    /// especificado.</response>
    [HttpGet("range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IEnumerable<InccResponseDTO>>> Get(
        [FromQuery] InccRangeParams @params)
    {
        var entries = await _inccService.GetRangeAsync(@params);

        if (!entries.Items.Any())
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

        return Ok(entries.Items);
    }

    /// <summary>
    /// Calcula a variação acumulada do INCC-M para um intervalo de datas.
    /// </summary>
    /// <param name="params">Datas de início e fim para o cálculo.</param>
    /// <returns>Variação acumulada e valor ajustado para o período.</returns>
    /// <response code="200">Retorna a variação acumulada e valor ajustado.</response>
    /// <response code="400">O intervalo de datas fornecido é inválido ou 
    /// o valor do INCC inicial é igual a zero.</response>
    /// <response code="404">Nenhum índice encontrado para o intervalo 
    /// especificado.</response>
    [HttpGet("accumulated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<InccAccumulatedResponseDTO>> Get(
        [FromQuery] InccAccumulatedParams @params)
    {
        InccAccumulatedResponseDTO? accumulatedDto;
        try
        {
            accumulatedDto = await _inccService.AccumulatedVariationAsync(@params);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Calculation Error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (accumulatedDto == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "No entries found",
                Detail = "No entries found for the specified range",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(accumulatedDto);
    }


    /// <summary>
    /// Cria um novo registro do INCC-M.
    /// </summary>
    /// <param name="createEntry">Registro para ser criado</param>
    /// <returns>O registro criado.</returns>
    /// <response code="201">Retorna o registro criado.</response>
    /// <response code="409">Já existe um registro para a data de referência.</response>
    [HttpPost]
    [ApiKey]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<InccResponseDTO>> Post(InccCreateDto createEntry)
    {
        var responseEntry = await _inccService.CreateAsync(createEntry);

        if (responseEntry is null)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Entry already exists",
                Detail = $"An entry for {createEntry.ReferenceDate:MM/yyyy} already exists",
                Status = StatusCodes.Status409Conflict
            });
        }

        return CreatedAtAction(
            nameof(Get),
            new
            {
                month = createEntry.ReferenceDate!.Value.Month,
                year = createEntry.ReferenceDate!.Value.Year
            },
            responseEntry);
    }
}
