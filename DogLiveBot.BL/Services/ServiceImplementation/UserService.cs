using DogLiveBot.BL.Services.ServiceInterface;
using DogLiveBot.Data.Context.Entity;
using DogLiveBot.Data.Repository.RepositoryInterfaces;

namespace DogLiveBot.BL.Services.ServiceImplementation;

public class UserService : IUserService
{
    private readonly IChangeRepository _changeRepository;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public UserService(
        IReadOnlyRepository readOnlyRepository, 
        IChangeRepository changeRepository)
    {
        _readOnlyRepository = readOnlyRepository;
        _changeRepository = changeRepository;
    }
    
    /// <inheritdoc />
    public async Task<bool> CreateIfNotExistAsync(User user, CancellationToken cancellationToken)
    {
        var ifExist = await _readOnlyRepository.IfExists<User>(
            filter: s => s.PhoneNumber == user.PhoneNumber, 
            cancellationToken: cancellationToken);

        if (ifExist)
        {
            return false;
        }

        await _changeRepository.Add(user, cancellationToken);
        return true;
    }
}