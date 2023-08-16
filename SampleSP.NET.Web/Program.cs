using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SampleSP.NET.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // 創建主機構建器
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => 
                {
                    config.Sources.Clear();
                    // 獲取當前的環境
                    var env = hostingContext.HostingEnvironment;
                    // 加載 appsettings.json 配置文件，並根據當前環境加載相對應的配置文件
                    config.AddJsonFile("Configs/appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"Configs/appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
