using System;
using System.IO;
using System.Threading.Tasks;
using Bromine.Automation.Core.Extensions;
using Bromine.Automation.Core.Helpers;
using Bromine.Automation.Core.Models;
using OpenQA.Selenium;

namespace Bromine.SeleniumWrapper.Tasks
{
  public class TakeScreenshotTask : BrowserTask
  {
    public override Task<TaskResult> Process(TaskInfo task)
    {
      //Always return true
      TearDownHandler(task.Parent.Name + "_" + task.TaskName);
      return new TaskResult(true).Result();
    }

    private void TearDownHandler(string fileName)
    {
      try
      {
        var ouputPath = Path.Combine(Environment.CurrentDirectory, "Screenshot");
        //Capture screen shot
        Directory.CreateDirectory(ouputPath);
        //Teardown - Capture screenshot
        var s = ((ITakesScreenshot)CurrentBrowser.Driver).GetScreenshot();
        s.SaveAsFile($"{ouputPath}\\{fileName}_{Utilities.GetCurrentTimeStamp()}.jpg",
          ScreenshotImageFormat.Jpeg);
      }
      catch (Exception ex)
      {
        Console.Write(ex.StackTrace);
      }
    }
  }
}
