using API.DataAccessLayer;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;


Env.Load();

// DB CONNECTION STRING
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection String is null or empty!");
}

var mongoConn = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
if (string.IsNullOrEmpty(mongoConn))
{
    throw new InvalidOperationException("Mongo Connection String is missing!");
}



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// FLUENT VALIDATION
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();



// AUTOMAPPER
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());



// DB CONNECTION
builder.Services.AddDbContext<FitnessAIContext>(option =>
    option.UseSqlServer(connectionString));



//CORS
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});



//INJECTIONS
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddSingleton(_ => new MongoDBClient(mongoConn));
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseCors();
app.UseMiddleware<LoggingMiddleware>();
app.MapOpenApi();
app.MapControllers();
app.UseRouting();
app.UseHttpsRedirection();

app.Run();
