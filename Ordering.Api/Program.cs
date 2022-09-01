using EventBusMessages.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Api.EventBusConsumer;
using Ordering.Application;
using Ordering.Application.Contracts;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplicationServices();

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<CheckoutConsumer>();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue,
                        c =>
                        {
                            c.ConfigureConsumer<CheckoutConsumer>(ctx);
                        });
                });

            });

            builder.Services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(builder
                    .Configuration
                    .GetConnectionString("OrderingConnectionString"));
            });

            builder.Services.AddScoped(typeof(IRepositoryBase<>),
               typeof(RepositoryBase<>));

            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}