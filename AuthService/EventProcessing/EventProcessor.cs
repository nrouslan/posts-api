using System.Text.Json;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;
using EventBusSDK;

namespace AuthService.EventProcessing
{
  enum EventType
  {
    UserUpdate,

    UserDelete,

    Undetermined
  }

  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IMapper _mapper;

    public EventProcessor(
      IServiceScopeFactory scopeFactory,
      IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);

      switch (eventType)
      {
        case EventType.UserUpdate:
          UpdateUserAccount(message);
          break;
        case EventType.UserDelete:
          DeleteUserAccount(message);
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notifcationMessage)
    {
      Console.WriteLine("--> Determining Event...");

      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifcationMessage);

      switch (eventType.Event)
      {
        case "UserUpdate":
          Console.WriteLine("--> 'UserUpdate' Event Detected!");
          return EventType.UserUpdate;
        case "UserDelete":
          Console.WriteLine("--> 'UserDelete' Event Detected!");
          return EventType.UserDelete;
        default:
          Console.WriteLine("--> Could not determine the event type!");
          return EventType.Undetermined;
      }
    }

    private void UpdateUserAccount(string publishUserUpdateMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<IUserAccountRepo>();

        var publishUserUpdateDto = JsonSerializer.Deserialize<PublishUserUpdateDto>(publishUserUpdateMessage);

        try
        {
          var userAccount = _mapper.Map<UserAccount>(publishUserUpdateDto);

          repo.Update(userAccount);

          repo.SaveChanges();

          Console.WriteLine("--> User is updated!");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not update the user: {ex.Message}");
        }
      }
    }

    private void DeleteUserAccount(string publishUserDeleteMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<IUserAccountRepo>();

        var publishUserDeleteDto = JsonSerializer.Deserialize<PublishUserDeleteDto>(publishUserDeleteMessage);

        try
        {
          repo.Delete(publishUserDeleteDto.Id);

          repo.SaveChanges();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not delete the user: {ex.Message}");
        }
      }
    }
  }
}