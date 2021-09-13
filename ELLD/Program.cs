using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotinoNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELLD
{
    //class Program
    //{
    //    [STAThread]
    //    static void Main(string[] args)
    //    {
    //        // Window title declared here for visibility
    //        string windowTitle = "Photino for .NET Demo App";

    //        // Creating a new PhotinoWindow instance with the fluent API
    //        var window = new PhotinoWindow()
    //            .SetTitle(windowTitle)
    //            // Resize to a percentage of the main monitor work area
    //            .SetUseOsDefaultSize(false)
    //            .SetSize(new Size(600, 400))
    //            // Center window in the middle of the screen
    //            .Center()
    //            // Users can resize windows by default.
    //            // Let's make this one fixed instead.
    //            .SetResizable(false)
    //            .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
    //            {
    //                contentType = "text/javascript";
    //                return new MemoryStream(Encoding.UTF8.GetBytes(@"
    //                    (() =>{
    //                        window.setTimeout(() => {
    //                            alert(`🎉 Dynamically inserted JavaScript.`);
    //                        }, 1000);
    //                    })();
    //                "));
    //            })
    //            // Most event handlers can be registered after the
    //            // PhotinoWindow was instantiated by calling a registration 
    //            // method like the following RegisterWebMessageReceivedHandler.
    //            // This could be added in the PhotinoWindowOptions if preferred.
    //            .RegisterWebMessageReceivedHandler((object sender, string message) => {
    //                var window = (PhotinoWindow)sender;

    //                // The message argument is coming in from sendMessage.
    //                // "window.external.sendMessage(message: string)"
    //                string response = $"Received message: \"{message}\"";

    //                // Send a message back the to JavaScript event handler.
    //                // "window.external.receiveMessage(callback: Function)"
    //                window.SendWebMessage(response);
    //            })
    //            .Load("http://www.baidu.com"); // Can be used with relative path strings or "new URI()" instance to load a website.

    //        window.WaitForClose(); // Starts the application event loop
    //    }
    //}
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var result = MainImpl().Result;
            Environment.Exit(0);
        }

        public static async Task<int> MainImpl(string[] args = null)
        {
            Console.WriteLine("Begin Build Host");
            var host = CreateHostBuilder(args).Build();
            var applicationLifetime = host.Services.GetService(typeof(IHostApplicationLifetime)) as IHostApplicationLifetime;

            //[Special for DesktopLoveBlazorWeb]
            TaskCompletionSource<string> futureAddr = new TaskCompletionSource<string>();
            applicationLifetime?.ApplicationStarted.Register((futureAddrObj) =>
            {
                var server = host.Services.GetService(typeof(IServer)) as IServer;
                var logger = host.Services.GetService(typeof(ILogger<Program>)) as ILogger<Program>;

                var addressFeature = server.Features.Get<IServerAddressesFeature>();
                foreach (var addresses in addressFeature.Addresses)
                {
                    Console.WriteLine("Listening on address: " + addresses);
                }

                var addr = addressFeature.Addresses.First();
                (futureAddrObj as TaskCompletionSource<string>).SetResult(addr);
            }, futureAddr);

            //[Special for DesktopLoveBlazorWeb]
#pragma warning disable CS4014 // ���ڴ˵��ò���ȴ�������ڵ������ǰ������ִ�е�ǰ����
            host.RunAsync();
#pragma warning restore CS4014 // ���ڴ˵��ò���ȴ�������ڵ������ǰ������ִ�е�ǰ����

            //[Special for DesktopLoveBlazorWeb]
            OpenInLine(await futureAddr.Task);


            //TODO
            return 0;
        }

        public static void OpenInLine(string address)
        {
            Console.WriteLine($"Try to view blazor application on {address}");
            /*using(var webview = new Webview())
            {
                webview
                    .SetTitle("Blazor in webview_csharp")             
                    .SetSize(1024, 768, WebviewHint.None)
                    .SetSize(800, 600, WebviewHint.Min)
                    .Navigate(new UrlContent(address))
                    .Run();
            }*/
            {
                // Window title declared here for visibility
                string windowTitle = "汉德软件";

                // Creating a new PhotinoWindow instance with the fluent API
                var window = new PhotinoWindow()
                    .SetTitle(windowTitle)
                    //.SetFullScreen(true)
                    //.SetSize(new System.Drawing.Size(1280,800))
                    .SetMaximized(true)
                    // Resize to a percentage of the main monitor work area
                    //.Resize(100, 100, "%")
                    // Center window in the middle of the screen
                    .Center()
                    // Users can resize windows by default.
                    // Let's make this one fixed instead.
                    .SetResizable(true)
                    .RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
                    {
                        contentType = "text/javascript";
                        return new MemoryStream(Encoding.UTF8.GetBytes(@"
                            (() =>{
                                window.setTimeout(() => {
                                    alert(`🎉 Dynamically inserted JavaScript.`);
                                }, 1000);
                            })();
                        "));
                    })
                    // Most event handlers can be registered after the
                    // PhotinoWindow was instantiated by calling a registration 
                    // method like the following RegisterWebMessageReceivedHandler.
                    // This could be added in the PhotinoWindowOptions if preferred.
                    .RegisterWebMessageReceivedHandler((object sender, string message) =>
                    {
                        var window = (PhotinoWindow)sender;

                            // The message argument is coming in from sendMessage.
                            // "window.external.sendMessage(message: string)"
                            string response = $"Received message: \"{message}\"";

                            // Send a message back the to JavaScript event handler.
                            // "window.external.receiveMessage(callback: Function)"
                            window.SendWebMessage(response);
                    })
                    //.SetIconFile(System.IO.Path.Combine(Application.StartupPath, "mlmw.ico"))
                    //.Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.
                    .Load("http://127.0.0.1:34567");
                window.WaitForClose(); // Starts the application event loop
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://127.0.0.1:34567");
                });
    }
}
