using AppWorker;
using Configuration.Extensions;
using Security.Extensions;
using Services.Seeder;
using AppWorker.Mocking;
using AppWorker.Workers;
using Services.Music.Interfaces;
using Services.Music;
using Services.Cards.Interfaces;
using Services.Cards;
using Services.Employees;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Configuration.AddSecrets(builder.Environment);
builder.Services.AddEncryptionEngine(builder.Configuration);
builder.Services.AddVersionInfo();
builder.Services.AddEnvironmentInfo();

builder.Services.AddSeeder().MockMusic().MockLatin().MockQuote().MockEmployee();
builder.Services.AddSingleton<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IMusicGroupsService, MusicGroupsServiceWapi>();
builder.Services.AddTransient<IAlbumsService, AlbumsServiceWapi>();
builder.Services.AddTransient<IArtistsService, ArtistsServiceWapi>();
builder.Services.AddTransient<IPokerService, PokerService>();

//Worker and ints various modes
builder.Services.AddSingleton(builder.Configuration["Worker:Mode"]);
builder.Services.AddTransient<UsingSeeder>();
builder.Services.AddTransient<UsingWebApi>();
builder.Services.AddTransient<UsingEncryption>();
builder.Services.AddTransient<UsingPoker>();


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
