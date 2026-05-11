using LibraryApi.Data;
using LibraryApi.Repositories;
using LibraryApi.Repositories.Interfaces;
using LibraryApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;


try
{ 

    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    builder.Services.AddSingleton<ILoggerService, SqlLogger>();
    builder.Services.AddTransient<IDBManager, DBManager>();
    builder.Services.AddTransient<IBookRepository, BookRepository>();
    builder.Services.AddTransient<IUyeRepository, UyeRepository>();
    builder.Services.AddTransient<IOduncRepository, OduncRepository>();
    builder.Services.AddHttpClient();
    builder.Services.AddHostedService<LibraryBackgroundService>();
    builder.Services.AddControllers().AddNewtonsoftJson(); 
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(); 

    var app = builder.Build();

    var db = app.Services.GetRequiredService<IDBManager>();

    // if (app.Environment.IsDevelopment()) 
    // {
    app.UseSwagger();
    app.UseSwaggerUI();
    // }

    app.UseCors();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}