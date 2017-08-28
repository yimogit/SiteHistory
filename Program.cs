using System;
using System.Collections.Generic;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.IO;
using System.Drawing.Imaging;
using System.Text;

namespace SiteHistory
{
    class Program
    {

        static Dictionary<string, string> pagesConfig = new Dictionary<string, string>() { };
        static Program()
        {
            // pagesConfig.Add("youtube", "http://www.youtube.com/");
            // pagesConfig.Add("google", "https://www.google.com");
        }
        static void Main(string[] args)
        {
            //获取传入参数 dotnet run https://www.baidu.com https://github.com/
            for (int i = 0; i < args.Length; i++)
            {
                pagesConfig.Add(args[i], args[i]);
            }
            if (pagesConfig.Count == 0)
            {
                return;
            }
            //windows 需要将phantomjs.exe所在目录设置环境变量 或者 指定目录驱动目录
            //Linux 需要安装PhantomJS
            using (var driver = new PhantomJSDriver())
            {
                const int timeout= 60;
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, timeout);
                var saveDir = $"SaveImgs";
                StringBuilder builder = new StringBuilder();
                foreach (var item in pagesConfig)
                {
                    try
                    {
                        Console.WriteLine($"开始前往[{item.Value}]");
                        driver.Navigate().GoToUrl(item.Value);
                        if (string.IsNullOrEmpty(driver.Title))
                        {
                            Console.WriteLine($"[{item.Key}]打开失败");
                            return;
                        }
                        if (!Directory.Exists(saveDir))
                        {
                            Directory.CreateDirectory(saveDir);
                        }
                        var saveName = $"{item.Key.Replace(".", "_").Replace("http://", "").Replace("https://", "").Replace("/", "")}.jpg";
                        var savePath = $"{ saveDir }/{ saveName}";
                        System.Threading.Thread.Sleep(3000);
                        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(savePath, ScreenshotImageFormat.Jpeg);
                        builder.AppendLine($"![图片](./{saveName})");
                        builder.AppendLine($"### {item.Key}");
                        Console.WriteLine($"图片保存至：{savePath}");
                        driver.ExecuteScript("document.body.innerHTML=''");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[{item.Key}]异常：{ex.Message}");
                    }
                }
                //创建MD文件
                WriteFile($"{saveDir}/README.MD", builder.ToString());
                driver.Quit();
            }
        }
        static void WriteFile(string fileName, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write,
                                                  FileShare.Write, 1024, FileOptions.Asynchronous))
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(input + "\r\n");
                IAsyncResult writeResult = fs.BeginWrite(buffer, 0, buffer.Length,
                    (asyncResult) =>
                    {
                        var fStream = (FileStream)asyncResult.AsyncState;
                        fStream.EndWrite(asyncResult);
                    },
                    fs);
                fs.Close();
            }

        }
    }
}
