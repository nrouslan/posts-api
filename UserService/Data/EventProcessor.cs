using System.Text.Json;
using AutoMapper;
using EventBusSDK;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Data
{
  public enum EventType
  {
    UserSignUp,
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
        case EventType.UserSignUp:
          AddUser(message);
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
        case "UserSignUp":
          Console.WriteLine("--> 'UserSignUp' Event Detected!");
          return EventType.UserSignUp;
        default:
          Console.WriteLine("--> Could not determine the event type!");
          return EventType.Undetermined;
      }
    }

    private void AddUser(string publishedUserMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

        var publishedUserDto = JsonSerializer.Deserialize<PublishedUserDto>(publishedUserMessage);

        try
        {
          var user = _mapper.Map<User>(publishedUserDto);

          if (repo.GetById(user.Id) == null)
          {
            repo.Insert(user);

            repo.Save();

            Console.WriteLine("--> User is added!");
          }
          else
          {
            Console.WriteLine("--> User already exisits...");
          }

        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not add User to DB {ex.Message}");
        }
      }
    }
  }
}