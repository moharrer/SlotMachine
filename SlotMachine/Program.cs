using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SlotMachine.Algo;
using SlotMachine.Infrastructure;
using SlotMachine.Services;
using SlotMachine.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoDbSettings>(serviceProvider =>
    serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
    DefaultMongoClient.GetMongoClient(serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value));

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetService<IMongoClient>();
    var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;

    return client.GetDatabase(settings.DatabaseName);
});


builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

builder.Services.AddSingleton(typeof(ISlotMachine), typeof(SlotMachine.Algo.SlotMachine));
builder.Services.AddSingleton(typeof(IWinStrategy), typeof(WinStrategy));

builder.Services.AddScoped(typeof(IConfigurationService), typeof(ConfigurationService));
builder.Services.AddScoped(typeof(IBetService), typeof(BetService));

builder.Services.AddScoped(typeof(IPlayerService), typeof(PlayerService));




var app = builder.Build();

app.UseMiddleware<UserExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
