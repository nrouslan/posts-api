namespace UserService.EventProcessing
{
  public interface IEventProcessor
  {
    void ProcessEvent(string message);
  }
}