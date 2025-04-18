using System.Text.Json;
using System.Text.Json.Serialization;
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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});




builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // BU DOĞRU
        opt.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(namingPolicy: null));
    });



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
builder.Services.AddHttpClient<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWorkoutPlannerService, WorkoutPlannerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FitnessMentorAI API V1");
});


app.UseCors();
app.UseMiddleware<LoggingMiddleware>();
app.MapOpenApi();
app.MapControllers();
app.UseRouting();
app.UseHttpsRedirection();

app.Run();
