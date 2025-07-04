
using System.Runtime.InteropServices;
using API.DMO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _mapper = mapper;
        _userService = userService;
    }

    [HttpPost]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Registers a new user to the system with the provided profile details. It does not generate a workout plan."
    )]
    [SwaggerResponse(200, "User successfully registered", typeof(UserVM))]
    [SwaggerResponse(400, "Invalid input or user already registered")]
    [SwaggerResponse(500, "Unexpected server error")]
    public async Task<IActionResult> POST([FromBody] RegisterRequest user)
    {
        Console.WriteLine($"Gelen GOAL: {user.Goal}"); // log
        if (user == null)
        {
            return BadRequest("User cannot be null"); // 400 bad req
        }

        var userDTO = _mapper.Map<UserDTO>(user);
        try
        {
            var result = await _userService.RegisterAsync(userDTO);
            return Ok(_mapper.Map<UserVM>(result)); // 200 ok
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message }); // 400 bad req
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message }); // 500 sikinti buyuk
        }
    }



    [HttpGet("{id}")]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Get user by ID",
        Description = "Retrieves user profile data for the specified user ID. This can be used to log in or display user information."
    )]
    [SwaggerResponse(200, "User found", typeof(UserVM))]
    [SwaggerResponse(400, "User not found")]
    [SwaggerResponse(500, "Unexpected server error")]
    public async Task<IActionResult> GET(Guid id)
    {
        try
        {
            var userDTO = await _userService.LoginAsync(id);
            if (userDTO == null)
            {
                return BadRequest("User not found");
            }

            var user = _mapper.Map<UserVM>(userDTO);

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message }); // 500 sikinti buyuk
        }

    }



    [HttpPut("{id}")]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Update user profile",
        Description = "Updates an existing user's profile data. Returns the updated user info if successful."
    )]
    [SwaggerResponse(200, "User successfully updated", typeof(UserVM))]
    [SwaggerResponse(400, "Invalid input or update failed")]
    [SwaggerResponse(500, "Unexpected server error")]
    public async Task<IActionResult> UpdateUser([FromBody] RegisterRequest user, Guid id)
    {
        if (user == null)
        {
            return BadRequest("User cannot be null"); // 400 bad req
        }
        // Map the incoming user data to a DTO

        var userDto = _mapper.Map<UserDTO>(user);
        userDto.UserId = id;
        try
        {
            var updatedDTO = await _userService.UpdateAsync(userDto);
            var updatedVM = _mapper.Map<UserVM>(updatedDTO);
            return Ok(updatedVM);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message }); // 400 bad req
        }
        catch (Exception ex)
        {
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message }); // 500 sikinti buyuk
            }
        }
    }


    [HttpDelete("{id}")]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Delete user by ID",
        Description = "Deletes the user with the specified ID, along with their associated workout plan if it exists."
    )]
    [SwaggerResponse(200, "User successfully deleted", typeof(object))] // {"message": "..."}
    [SwaggerResponse(400, "Invalid request or user could not be deleted")]
    [SwaggerResponse(500, "Unexpected server error")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message }); // 400 bad req
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message }); // 500 sikinti buyuk
        }
    }
}