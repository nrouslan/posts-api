namespace EventBusSDK
{
  public interface IEventProcessor
  {
    void ProcessEvent(string message);
  }
}