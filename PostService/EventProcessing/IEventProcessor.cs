namespace PostService
{
  public interface IEventProcessor
  {
    void ProcessEvent(string message);
  }
}