
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
public class ConstantsController : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get constant values",
        Description = "Returns predefined constants such as available workout days and fitness goals."
    )]
    [SwaggerResponse(200, "Successfully returned constants", typeof(ConstantsResponse))]
    public IActionResult Get() => Ok(new ConstantsResponse
    {
        Days = Constants.Days,
        Goals = Constants.Goals
    });
}