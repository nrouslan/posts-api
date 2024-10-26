using System.Text.Json;
using AutoMapper;
using PostService.Data;
using PostService.Dtos;
using PostService.Models;

namespace PostService.EventProcessing
{
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
        case EventType.Publish_User:
          AddUser(message);
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determining Event...");

      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType.Event)
      {
        case "Publish_User":
          Console.WriteLine("--> Publish_User event detected");

          return EventType.Publish_User;
        default:
          Console.WriteLine("--> Could not determine the event type");

          return EventType.Undetermined;
      }
    }

    private void AddUser(string publishUserMessage)
    {
      using var scope = _scopeFactory.CreateScope();

      var repo = scope.ServiceProvider.GetRequiredService<IPostRepo>();

      var publishUserDto = JsonSerializer.Deserialize<PublishUserDto>(publishUserMessage);

      try
      {
        var user = _mapper.Map<User>(publishUserDto);

        if (!repo.IsExternalUserExists(user.ExternalId))
        {
          repo.CreateUser(user);

          repo.SaveChanges();

          System.Console.WriteLine("--> User is added!");
        }
        else
        {
          Console.WriteLine("--> User already exists");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not add User to the database {ex.Message}");
      }
    }
  }

  enum EventType
  {
    Publish_User,
    Undetermined
  }
}