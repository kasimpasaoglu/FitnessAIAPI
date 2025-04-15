
using System.Runtime.InteropServices;
using API.DMO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserVM user)
    {
        if (user == null)
        {
            return BadRequest("User cannot be null"); // 400 bad req
        }

        var userDTO = _mapper.Map<UserDTO>(user);
        try
        {
            var result = await _userService.RegisterAsync(userDTO);
            return Ok(new { userId = result });
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
    public async Task<IActionResult> LoginUser(Guid id)
    {
        try
        {
            var userDTO = await _userService.LoginAsync(id);
            if (userDTO == null)
            {
                return NotFound("User not found");
            }

            var user = _mapper.Map<UserVM>(userDTO);

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message }); // 500 sikinti buyuk
        }

    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UserVM user)
    {
        var userDto = _mapper.Map<UserDTO>(user);
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