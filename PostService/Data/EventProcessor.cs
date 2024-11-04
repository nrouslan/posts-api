using System.Text.Json;
using EventBusSDK;
using PostService.Dtos;

namespace PostService.Data
{
  enum EventType
  {
    UserDelete,

    Undetermined
  }

  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IServiceScopeFactory scopeFactory)
    {
      _scopeFactory = scopeFactory;
    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);

      switch (eventType)
      {
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
        default:
          Console.WriteLine("--> Could not determine the event type!");
          return EventType.Undetermined;
      }
    }

    private void DeletePostsForUser(string publishUserDeleteMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<IPostRepo>();

        var publishUserDeleteDto = JsonSerializer.Deserialize<PublishUserDeleteDto>(publishUserDeleteMessage);

        try
        {
          foreach (var post in repo.GetPostsForUser(publishUserDeleteDto.Id))
          {
            repo.Delete(publishUserDeleteDto.Id, post.Id);
          }

          repo.SaveChanges();

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