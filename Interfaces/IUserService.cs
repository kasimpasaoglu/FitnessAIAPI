public interface IUserService
{
    Task<UserDTO> LoginAsync(Guid id);
    Task<Guid> RegisterAsync(UserDTO user);
    Task<UserDTO> UpdateAsync(UserDTO user);
    Task DeleteAsync(Guid id);
}