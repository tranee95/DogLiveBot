using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Shared.Messages.Messages.Schedule.Model;

namespace Shared.Messages.Messages.Service.ServiceInterface;

public interface IMassTransitService
{
    Task Publish<TMessage>(TMessage message) where TMessage : class;
}