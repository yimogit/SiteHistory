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
        /// <summary>
        /// 运行 dotnet run 名称[必传] 网址[必传] 图片格式[jpg/png] 等待截图时间[默认为10，单位 秒] 保存目录[可选参数]
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length < 2 || args[1].IndexOf("http") != 0)
            {
                Console.WriteLine("请传入网站名称及其页面地址 例如： dotnet run baidu https://www.baidu.com");
                return;
            }
            try
            {
                //windows 需要将phantomjs.exe所在目录设置环境变量 或者 指定目录驱动目录
                //Linux 需要安装PhantomJS
                IWebDriver driver = new PhantomJSDriver();
                BeginTask(driver, args);
                driver.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常信息：{ex.Message}");
                Console.WriteLine($"堆栈信息：{ex.StackTrace}");
            }
        }
        static void BeginTask(IWebDriver driver, string[] args)
        {
            string siteName = args[0], sitePage = args[1], imgExt = "jpg", saveDirName = "download",appendjs = "";
            int waitTime = 10;
            if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
                imgExt = args[2];
            if (args.Length > 3 && !string.IsNullOrEmpty(args[3]))
                int.TryParse(args[3], out waitTime);
            if (args.Length > 4 && !string.IsNullOrEmpty(args[4]))
                saveDirName = args[4];
            if (args.Length > 5 && !string.IsNullOrEmpty(args[5]))
                appendjs = args[5];
            if (!Directory.Exists(saveDirName))
                Directory.CreateDirectory(saveDirName);
            StringBuilder builder = new StringBuilder();
            StringBuilder builderHtml = new StringBuilder();
            Console.WriteLine($"开始打开[{sitePage}]");
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(sitePage);
            string siteTitle = driver.Title;
            if (string.IsNullOrEmpty(siteTitle))
            {
                Console.WriteLine($"[{siteName}]打开失败");
                return;
            }
            if (!string.IsNullOrEmpty(appendjs))
                ((IJavaScriptExecutor)driver).ExecuteScript(appendjs);
            //分阶段滚动到底部
            var myScript = @"var ymtimer=setInterval(function(){
                                if (document.body.scrollHeight - 700 < document.body.scrollTop){
                                    window.scroll(0, document.body.scrollHeight)
                                        clearInterval(ymtimer);
                                    return;
                                }
                                window.scroll(0, document.body.scrollTop + 700)

                    }," + waitTime * 1000 / 10 + ");";
            //滚动到最底部再截图，触发图片懒加载
            ((IJavaScriptExecutor)driver).ExecuteScript(myScript);
            
            System.Threading.Thread.Sleep(1200 * waitTime);
            string saveName = $"{siteName}.{imgExt}";
            //截图保存
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile($"{ saveDirName }/{ saveName}", (imgExt == "jpg" ? ScreenshotImageFormat.Jpeg : ScreenshotImageFormat.Png));
            Console.WriteLine($"保存图片成功");
            //构造Readme.md
            builder.AppendLine($"### {siteName}");
            builder.AppendLine($"> 网站标题： {siteTitle}");
            builder.AppendLine($"![{siteName}](./{saveName})");
            //构造index.html
            builderHtml.AppendLine($"<a href='{sitePage}' title='{siteName}-{sitePage}'>{siteTitle}</a>");
            builderHtml.AppendLine($"<a href='{sitePage}' title='{siteName}-{sitePage}'><img src='./{saveName}' style='width:100%;'/></a>");

            //创建MD文件
            WriteFile($"{saveDirName}/index.html", builderHtml.ToString());
            WriteFile($"{saveDirName}/README.MD", builder.ToString());

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
