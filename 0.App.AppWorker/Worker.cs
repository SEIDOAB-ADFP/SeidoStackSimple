using Configuration.Options;
using Microsoft.Extensions.Options;
using Models;
using Services;
using Services.Seeder;
using AppWorker.Workers;
using Newtonsoft.Json;

namespace AppWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly UsingSeeder _usingSeeder;
    private readonly UsingWebApi _usingWebApi;
    private readonly UsingEncryption _usingEncryption;
    private readonly VersionOptions _versionOptions;
    private readonly EnvironmentOptions _environmentOptions;
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly string _workerMode;

    public Worker(ILogger<Worker> logger, 
            UsingSeeder usingSeeder, UsingWebApi usingWebApi, UsingEncryption usingEncryption,
            string workerMode,

            IHostApplicationLifetime hostLifetime, 
            IOptions<VersionOptions> versionOptions,
            IOptions<EnvironmentOptions> environmentOptions)
    {
        _logger = logger;
        _usingSeeder = usingSeeder;
        _usingWebApi = usingWebApi;
        _usingEncryption = usingEncryption;
        _workerMode = workerMode;
        _versionOptions = versionOptions.Value;
        _environmentOptions = environmentOptions.Value;
        _hostLifetime = hostLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Version: {Version}\n", Newtonsoft.Json.JsonConvert.SerializeObject(_versionOptions, Formatting.Indented));
        _logger.LogInformation("Environment: {Environment}\n", Newtonsoft.Json.JsonConvert.SerializeObject(_environmentOptions, Formatting.Indented));
        
        await (_workerMode.ToLower() switch
        {
            "seeder" => _usingSeeder.ExecuteAsync(),
            "webapi" => _usingWebApi.ExecuteAsync(),
            "encryption" =>  _usingEncryption.ExecuteAsync(),
            _ => throw new ArgumentException($"Unknown worker mode: {_workerMode}")
        });

        _hostLifetime.StopApplication();
    }
}
