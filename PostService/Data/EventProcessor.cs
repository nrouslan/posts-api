using System.Text.Json;
using AutoMapper;
using EventBusSDK;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Data
{
  enum EventType
  {
    UserDelete,

    UserUpdate,

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
          UpdateUser(message);
          break;
        case EventType.UserDelete:
          DeletePostsForUser(message);
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
        case "UserDelete":
          Console.WriteLine("--> 'UserDelete' Event Detected!");
          return EventType.UserDelete;
        case "UserUpdate":
          Console.WriteLine("--> 'UserUpdate' Event Detected!");
          return EventType.UserUpdate;
        default:
          Console.WriteLine("--> Could not determine the event type!");
          return EventType.Undetermined;
      }
    }

    private void DeletePostsForUser(string publishUserDeleteMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var postRepo = scope.ServiceProvider.GetRequiredService<IPostRepo>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

        var publishUserDeleteDto = JsonSerializer.Deserialize<PublishUserDeleteDto>(publishUserDeleteMessage);

        try
        {
          foreach (var post in postRepo.GetPostsForUser(publishUserDeleteDto.Id))
          {
            postRepo.Delete(publishUserDeleteDto.Id, post.Id);
          }

          postRepo.SaveChanges();

          Console.WriteLine($"--> Deleted posts for the user (userId = {publishUserDeleteDto.Id})!");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not delete posts for the user: {ex.Message}");
        }

        try
        {
          userRepo.Delete(publishUserDeleteDto.Id);

          userRepo.SaveChanges();

          Console.WriteLine($"--> Deleted the user (userId = {publishUserDeleteDto.Id})!");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not delete the user!");
        }
      }
    }

    private void UpdateUser(string publishUserUpdateMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();

        var publishUserUpdateDto = JsonSerializer.Deserialize<PublishUserUpdateDto>(publishUserUpdateMessage);

        try
        {
          userRepo.Update(_mapper.Map<User>(publishUserUpdateDto));

          userRepo.SaveChanges();

          Console.WriteLine($"--> Deleted posts for the user!");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"--> Could not delete posts for the user: {ex.Message}");
        }
      }
    }
  }
}