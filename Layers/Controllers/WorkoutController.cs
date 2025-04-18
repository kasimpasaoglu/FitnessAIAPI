using System.Runtime.InteropServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/user/")]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutPlannerService _workoutPlannerService;
    private readonly IMapper _mapper;

    public WorkoutController(IWorkoutPlannerService workoutPlannerService, IMapper mapper)
    {
        _mapper = mapper;
        _workoutPlannerService = workoutPlannerService;
    }



    [HttpGet("{id}/workout")]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get workout plan for user",
        Description = "Generates a personalized workout plan based on the user's profile. This request can only be made once per user."
    )]
    [SwaggerResponse(200, "Successfully generated workout plan", typeof(ExerciseJsonModel))]
    [SwaggerResponse(400, "Invalid user ID")]
    [SwaggerResponse(404, "User not found or workout plan could not be created")]
    [SwaggerResponse(500, "Unexpected error occurred")]
    public async Task<ActionResult<ExerciseJsonModel>> CreateNewExercise(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("User ID cannot be empty");
        }

        try
        {
            var exerciseModel = await _workoutPlannerService.GetWorkoutByUserIdAsync(id);
            var jsonModel = exerciseModel.ExerciseJson;
            return Ok(jsonModel);
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException || ex is InvalidOperationException || ex is ExternalException)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
        }
    }

}