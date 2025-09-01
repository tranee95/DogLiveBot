using AutoMapper;
using DogLive.TelegramBot.BL.Handlers.MassTransitMessages.MassTransitMessageHandlerInterface;
using DogLive.TelegramBot.BL.Services.ServiceInterface.Schedule;
using DogLive.TelegramBot.Data.Context.Entity;
using Microsoft.Extensions.Logging;
using Shared.Messages.Messages.Schedule.Model;

namespace DogLive.TelegramBot.BL.Handlers.MassTransitMessages.MassTransitMessageHandlerImplementation;

public class ScheduleHandler : IMassTransitMessageHandler<IRMQScheduleSlot>
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ScheduleSlot, AvailableSlot>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.IsReserved, opt => opt.MapFrom(src => src.IsReserved));
        }
    }

    private readonly ILogger<ScheduleHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IScheduleService _scheduleService;

    public ScheduleHandler(
        ILogger<ScheduleHandler> logger,
        IMapper mapper,
        IScheduleService scheduleService)
    {
        _logger = logger;
        _mapper = mapper;
        _scheduleService = scheduleService;
    }

    public async Task Handle(IRMQScheduleSlot message, CancellationToken cancellationToken)
    {
        var scheduleSlot = message.ScheduleSlot;
        _logger.LogInformation($"Обработка расписания: {scheduleSlot.Count} элементов");

        var availableSlots = _mapper.Map<ICollection<ScheduleSlot>, ICollection<AvailableSlot>>(scheduleSlot);
        await _scheduleService.FillCalendar(availableSlots.ToArray(), cancellationToken);
    }
}