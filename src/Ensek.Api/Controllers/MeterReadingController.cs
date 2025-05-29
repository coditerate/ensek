using Ensek.Services;
using Ensek.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MeterReadingController(IMeterReadingService service) : ControllerBase
{
    [HttpPost("meter-reading-uploads")]
    [ProducesResponseType(typeof(UploadResult), 200)]
    public async Task<ActionResult<int>> Post(IFormFile? file, CancellationToken cancellationToken)
    {
        var result = await service.PostAsync(file, cancellationToken);
        return result.Success ? Ok(result.Result) : BadRequest(result.ErrorMessage);
    }
}
