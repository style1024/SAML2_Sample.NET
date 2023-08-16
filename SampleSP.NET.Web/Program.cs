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

        // �ЫإD���c�ؾ�
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => 
                {
                    config.Sources.Clear();
                    // �����e������
                    var env = hostingContext.HostingEnvironment;
                    // �[�� appsettings.json �t�m���A�îھڷ�e���ҥ[���۹������t�m���
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
