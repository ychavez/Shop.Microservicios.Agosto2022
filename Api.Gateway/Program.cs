using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Api.Gateway
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Host.ConfigureAppConfiguration((ctx, conf) =>
            {
                conf.AddJsonFile($"ocelot.{ctx.HostingEnvironment.EnvironmentName}.json");
            });

            builder.Services.AddOcelot(); 

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            await app.UseOcelot();

            app.Run();
        }
    }
}