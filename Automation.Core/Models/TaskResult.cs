namespace Bromine.Automation.Core.Models
{
  public class TaskResult
  {
    public bool IsError { get; set; }
    public bool Success { get; set; }

    public TaskResult(bool success)
    {
      Success = success;
    }
  }
}
