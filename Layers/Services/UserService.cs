using System.Runtime.InteropServices;
using System.Text.Json;
using API.DMO;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;
    public UserService(IMapper mapper, IUnitOfWork unitOfWork, ILogService logService)
    {
        _logService = logService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserDTO> LoginAsync(Guid id)
    {
        var userDMO = await _unitOfWork.User.GetByUuidAsync(id);
        if (userDMO == null)
        {
            await _logService.LogError("User not found", "LoginUser", new DirectoryNotFoundException("User Not Found"), new { userId = id });
            return null;
        }
        await _logService.LogSuccess("User Logged In", "LoginUser", new
        {
            userId = userDMO.UserId.ToString(),
            userName = userDMO.Name,
            userSurname = userDMO.Surname
        });
        return _mapper.Map<UserDTO>(userDMO);
    }

    public async Task<UserDTO> RegisterAsync(UserDTO user)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            user.UserId = Guid.NewGuid(); // userId creation

            var userEntity = _mapper.Map<User>(user);

            var registeredUser = await _unitOfWork.User.AddAsync(userEntity); // user registration

            // Save changes to the database
            await _unitOfWork.SaveAsync();

            // Commit the transaction
            await _unitOfWork.CommitTransactionAsync();



            // Log success
            await _logService.LogSuccess("User Registered", "RegisterUser", new
            {
                userId = registeredUser.UserId.ToString(),
                userName = registeredUser.Name,
                userSurname = registeredUser.Surname
            });

            return _mapper.Map<UserDTO>(registeredUser);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            // Log error
            await _logService.LogError("User Registration Failed", "RegisterUser", ex, new
            {
                userName = user.Name,
                userSurname = user.Surname
            });

            throw new InvalidOperationException("An error occurred while registering the user", ex);
        }
    }

    public async Task<UserDTO> UpdateAsync(UserDTO user)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var userEntity = await _unitOfWork.User.GetByUuidAsync(user.UserId);
            if (userEntity == null)
            {
                throw new DirectoryNotFoundException("User not found");
            }

            userEntity.Name = user.Name;
            userEntity.Surname = user.Surname;
            userEntity.HeightCm = user.HeightCm;
            userEntity.WeightKg = user.WeightKg;
            userEntity.Gender = user.Gender.ToString();
            userEntity.Goal = user.Goal.ToString();
            userEntity.AvailableDays = JsonSerializer.Serialize(user.AvailableDays);
            userEntity.HasHealthIssues = user.HasHealthIssues;
            userEntity.MedicationsUsing = JsonSerializer.Serialize(user.MedicationsUsing);
            userEntity.ExperienceLevel = user.ExperienceLevel.ToString();

            var updateResult = await _unitOfWork.User.UpdateAsync(userEntity);
            if (updateResult == 0)
            {
                throw new ArgumentException("No changes made");
            }

            await _unitOfWork.CommitTransactionAsync(); // commiting update process

            // Log success
            await _logService.LogSuccess("User Updated", "Update User", new
            {
                userId = user.UserId.ToString(),
                userName = user.Name,
                userSurname = user.Surname
            });

            return _mapper.Map<UserDTO>(await _unitOfWork.User.GetByUuidAsync(user.UserId));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            // Log error
            await _logService.LogError("User Update Failed", "Update User", ex, new
            {
                userId = user.UserId.ToString(),
                userName = user.Name,
                userSurname = user.Surname
            });

            throw new InvalidOperationException("An error occurred while updating the user", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var userToDelete = await _unitOfWork.User.GetByUuidAsync(id);
            if (userToDelete == null)
            {
                throw new DirectoryNotFoundException("User not found");
            }

            var workoutToDelete = await _unitOfWork.WorkoutPlan.FindAsync(x => x.UserId == id);
            if (workoutToDelete != null)
            {
                await _unitOfWork.WorkoutPlan.DeleteAsync(workoutToDelete); // delete workout plan if exists
            }

            var userResult = await _unitOfWork.User.DeleteAsync(userToDelete);
            if (userResult == 0)
            {
                throw new InvalidOperationException("An error occurred while deleting the user");
            }
            await _unitOfWork.CommitTransactionAsync(); // commit deleting process

            // Log success
            await _logService.LogSuccess("User & Workout Plan Deleted", "DeleteUser", new
            {
                userId = userToDelete.UserId.ToString(),
                planId = workoutToDelete?.PlanId.ToString() ?? "NoPlan",
                userName = userToDelete.Name,
                userSurname = userToDelete.Surname
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            // Log error
            await _logService.LogError("User Deletion Attempt Failed", "DeleteUser", ex, new
            {
                userId = id.ToString(),
            });

            throw new InvalidOperationException("An error occurred while deleting the user", ex);
        }
    }
}

