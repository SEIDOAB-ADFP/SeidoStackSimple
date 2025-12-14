using AppWorker;
using Configuration.Extensions;
using Security.Extensions;
using Services.Seeder;
using AppWorker.Mocking;
using AppWorker.Workers;
using AppWorker.Obfuscation;
using Services.Music.Interfaces;
using Services.Music;
using Services.Employees;
using Services.Encryptions;
using Models.Employees.Interfaces;
using Models.Employees;
using System.Text.RegularExpressions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Configuration.AddSecrets(builder.Environment);
builder.Services.AddEncryptionEngine(builder.Configuration);
builder.Services.AddVersionInfo();
builder.Services.AddEnvironmentInfo();

builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddSeeder().MockMusic().MockLatin().MockQuote().MockEmployee();

builder.Services.AddTransient<EncryptionService>(); //to be replaced by the extension method
//Example of adding obfuscation for Employee model
//builder.Services.AddEncryptionService().ObfuscateEmployee();



builder.Services.AddTransient<IMusicGroupsService, MusicGroupsServiceWapi>();
builder.Services.AddTransient<IAlbumsService, AlbumsServiceWapi>();
builder.Services.AddTransient<IArtistsService, ArtistsServiceWapi>();

//Worker and ints various modes
builder.Services.AddSingleton(builder.Configuration["Worker:Mode"]);
builder.Services.AddTransient<UsingSeeder>();
builder.Services.AddTransient<UsingWebApi>();
builder.Services.AddTransient<UsingEncryption>();

builder.Services.AddHttpClient(name: builder.Configuration["WebApi:Name"], configureClient: options =>
{
    options.BaseAddress = new Uri(builder.Configuration["WebApi:BaseUri"]);
    options.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
            mediaType: "application/json",
            quality: 1.0));
});

var host = builder.Build();
host.Run();
