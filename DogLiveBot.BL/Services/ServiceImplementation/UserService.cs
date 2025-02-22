using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Entity;
using DogLiveBot.Data.Repository.RepositoryInterfaces;

namespace DogLiveBot.BL.Services.ServiceImplementation;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
    
    /// <inheritdoc />
    public async Task<bool> CreateIfNotExistAsync(User user, CancellationToken cancellationToken)
    {
        var ifExist = await _userRepository.IfExists(s => s.PhoneNumber == user.PhoneNumber, cancellationToken);
        if (ifExist)
        {
            return false;
        }

        await _userRepository.Add(user, cancellationToken);
        return true;
    }
}