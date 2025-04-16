
using System.Runtime.InteropServices;
using System.Text.Json;
using API.DMO;
using AutoMapper;
using MongoDB.Bson.IO;

public class WorkoutPlannerService : IWorkoutPlannerService
{
    private readonly IDeepSeekService _deepSeekService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;



    public WorkoutPlannerService(IDeepSeekService deepSeekService, IUnitOfWork unitOfWork, IMapper mapper, ILogService logService)
    {
        _deepSeekService = deepSeekService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logService = logService;
    }



    public async Task<WorkoutPlanDTO> GetWorkoutByUserIdAsync(Guid id)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = _mapper.Map<UserDTO>(await _unitOfWork.User.GetByUuidAsync(id)) ?? throw new DirectoryNotFoundException("User not found");

            var oldPlan = await _unitOfWork.WorkoutPlan.FindAsync(x => x.UserId == user.UserId);

            if (oldPlan != null)
            {
                await _logService.LogSuccess("Workout Plan Exists", "RequestExercise", new
                {
                    planId = oldPlan.PlanId.ToString(),
                    userId = oldPlan.UserId.ToString(),
                    userName = user.Name,
                    userSurname = user.Surname
                });

                var oldPlanDTO = _mapper.Map<WorkoutPlanDTO>(oldPlan);
                await transaction.RollbackAsync();

                return oldPlanDTO;
            }
            else
            {
                var exerciseModel = await _deepSeekService.RequestExerciseAsync(user);

                var workoutPlan = new WorkoutPlan
                {
                    PlanId = Guid.NewGuid(),
                    UserId = user.UserId,
                    ExerciseJson = JsonSerializer.Serialize(exerciseModel),
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.WorkoutPlan.AddAsync(workoutPlan);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                // Log success
                await _logService.LogSuccess("Workout Plan Created", "RequestExercise", new
                {
                    planId = workoutPlan.PlanId.ToString(),
                    userId = user.UserId.ToString(),
                    userName = user.Name,
                    userSurname = user.Surname
                });

                var workoutPlanDTO = _mapper.Map<WorkoutPlanDTO>(workoutPlan);
                return workoutPlanDTO;
            }
        }
        catch (ExternalException ex)
        {
            await transaction.RollbackAsync();
            // Log error
            await _logService.LogError("Workout Plan Creation Failed By AI", "RequestExercise", ex, new
            {
                userId = id.ToString(),
            });
            throw new ExternalException("AI Response Error while requesting exercise", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Log error
            await _logService.LogError("Workout Plan Creation Failed", "RequestExercise", ex, new
            {
                userId = id.ToString(),
            });
            throw new InvalidOperationException("Error while requesting exercise", ex);
        }
    }
}