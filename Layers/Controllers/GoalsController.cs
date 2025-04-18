using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
public class GoalsController : ControllerBase
{
    public GoalsController() { }

    [HttpGet]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get all fitness goal ENUMS",
        Description = "Retrieves a list of all available fitness goals as ENUM."
    )]
    [SwaggerResponse(200, "Fitness goals found", typeof(IEnumerable<FitnessGoal>))]
    [SwaggerResponse(404, "Fitness goals not found")]
    public IActionResult GetAllFitnessGoals()
    {
        var fitnessGoals = Enum.GetValues(typeof(FitnessGoal))
            .Cast<FitnessGoal>()
            .Where(g => g != FitnessGoal.Unknown)
            .ToList();

        return Ok(fitnessGoals);
    }
}
