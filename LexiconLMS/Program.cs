using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexiconLMS.Data;
using LexiconLMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LexiconLMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();

                var config = host.Services.GetRequiredService<IConfiguration>();

                //Behöver sättas via kommandotolken i projectkatalogen
                //dotnet user-secrets set "LMS:Teacher1PW" "Teacher_1"
                //dotnet user-secrets set "LMS:Teacher2PW" "Teacher_2"
                //dotnet user-secrets set "LMS:Teacher3PW" "Teacher_3"
                //dotnet user-secrets set "LMS:Teacher4PW" "Teacher_4"
                //dotnet user-secrets set "LMS:Student1PW" "Student_1"
                //dotnet user-secrets set "LMS:Student2PW" "Student_2"
                //dotnet user-secrets set "LMS:Student3PW" "Student_3"
                //dotnet user-secrets set "LMS:Student4PW" "Student_4"
                //dotnet user-secrets set "LMS:Student5PW" "Student_5"
                //dotnet user-secrets set "LMS:Student6PW" "Student_6"
                //dotnet user-secrets set "LMS:Student7PW" "Student_7"
                //dotnet user-secrets set "LMS:Student8PW" "Student_8"
                //dotnet user-secrets set "LMS:Student9PW" "Student_9"
                //dotnet user-secrets set "LMS:Student10PW" "Student_10"

                var Teacher1PW = config["LMS:Teacher1PW"];
                var Teacher2PW = config["LMS:Teacher2PW"];
                var Teacher3PW = config["LMS:Teacher3PW"];
                var Teacher4PW = config["LMS:Teacher4PW"];
                var Student1PW = config["LMS:Student1PW"];
                var Student2PW = config["LMS:Student2PW"];
                var Student3PW = config["LMS:Student3PW"];
                var Student4PW = config["LMS:Student4PW"];
                var Student5PW = config["LMS:Student5PW"];
                var Student6PW = config["LMS:Student6PW"];
                var Student7PW = config["LMS:Student7PW"];
                var Student8PW = config["LMS:Student8PW"];
                var Student9PW = config["LMS:Student9PW"];
                var Student10PW = config["LMS:Student10PW"];

                List<string> users = new List<string>();

                users.Add(Teacher1PW);
                users.Add(Teacher2PW);
                users.Add(Teacher3PW);
                users.Add(Teacher4PW);
                users.Add(Student1PW);
                users.Add(Student2PW);
                users.Add(Student3PW);
                users.Add(Student4PW);
                users.Add(Student5PW);
                users.Add(Student6PW);
                users.Add(Student7PW);
                users.Add(Student8PW);
                users.Add(Student9PW);
                users.Add(Student10PW);


                try
                {
                    SeedData.InitializeAsync(services, users).Wait();
                }
                catch (Exception ex)
                {

                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "Seed Fail");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
