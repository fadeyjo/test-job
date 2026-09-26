using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestJob.DTOs.Elements;
using TestJob.Services;

namespace TestJob.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParseController(IParseService service, IValidator<ParseRequest> validator) : ControllerBase
{
    [HttpPost("elements")]
    public async Task<IActionResult> CreateElementAsync([FromBody]ParseRequest body)
    {
        var validationResult = await validator.ValidateAsync(body);
        
        var response = await service.CreateElementAsync(body, validationResult);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}