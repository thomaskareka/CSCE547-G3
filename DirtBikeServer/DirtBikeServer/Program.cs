using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DirtBikeServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

           //swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // dirt bike project services
            builder.Services.AddDbContext<DirtBikeDbContext>(options =>
                options.UseLazyLoadingProxies()
                    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IParkRepository, ParkRepository>();

            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IParkService, ParkService>();

            builder.Services.AddControllers().AddJsonOptions(o =>
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // add CORS to allow requests from react frontend
            builder.Services.AddCors(options => {
                options.AddPolicy("ReactDev", policy =>
                    policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors("ReactDev");
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
