using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TestJob.DTOs.Elements;
using TestJob.Models;
using TestJob.Services;

namespace TestJob.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParseController(IParseService service) : ControllerBase
{
    [HttpPost("elements")]
    public async Task<IActionResult> CreateElementAsync([FromBody]ParseRequest body)
    {
        var response = await service.CreateElementAsync(body);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}