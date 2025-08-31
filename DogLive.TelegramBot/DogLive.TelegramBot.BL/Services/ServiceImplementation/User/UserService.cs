using DogLive.TelegramBot.BL.Services.ServiceInterface.User;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;

namespace DogLive.TelegramBot.BL.Services.ServiceImplementation.User;

public class UserService : IUserService
{
    private readonly IRepository _repository;
    private readonly IReadOnlyRepository _readOnlyRepository;

    public UserService(
        IReadOnlyRepository readOnlyRepository, 
        IRepository repository)
    {
        _readOnlyRepository = readOnlyRepository;
        _repository = repository;
    }
    
    /// <inheritdoc />
    public async Task<bool> CreateIfNotExistAsync(Data.Context.Entity.User user, CancellationToken cancellationToken)
    {
        var ifExist = await _readOnlyRepository.IfExists<Data.Context.Entity.User>(
            filter: s => s.PhoneNumber == user.PhoneNumber, 
            cancellationToken: cancellationToken);

        if (ifExist)
        {
            return false;
        }

        await _repository.Add(user, cancellationToken);
        return true;
    }
}