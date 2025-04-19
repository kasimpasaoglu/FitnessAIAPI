public interface IUserService
{
    Task<UserDTO> LoginAsync(Guid id);
    Task<UserDTO> RegisterAsync(UserDTO user);
    Task<UserDTO> UpdateAsync(UserDTO user);
    Task DeleteAsync(Guid id);
}