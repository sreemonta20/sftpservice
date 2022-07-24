using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Serilog;
using sftpservice;
using sftpservice.Core.IRepositories;
using sftpservice.Core.IServices;
using sftpservice.Core.Models.Common;
using sftpservice.Core.Repositories;
using sftpservice.Core.Services;
using sftpservice.Helper;
using sftpservice.Persistence.ORM;
using System.Text;
using System.Text.Json.Serialization;
using System.Data;

// Reading appsettings 
IConfiguration config = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile(ConstantSupplier.APP_SETTINGS_FILE_NAME)
                  .Build();

// Setup a static Log.Logger instance
Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .CreateLogger();

try
{
    // This defined the service introduction or title in logger

    StringBuilder sb = new StringBuilder();
    sb.AppendLine();
    sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FIRST);
    sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_SECOND_GATEWAY);
    sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_THIRD_VERSION);
    sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_FOURTH_COPYRIGHT);
    sb.AppendLine(ConstantSupplier.LOG_INFO_APPEND_LINE_END);
    Log.Logger.Information(sb.ToString());

    // Hosting is started and it is captured in the log. Configure Services started.
    // Add services to the container (ConfigureServices(IServiceCollection services) Method from the last .NET 5)
    Log.Information(ConstantSupplier.LOG_INFO_HOST_START_MSG);
    IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {

        IConfiguration configuration = hostContext.Configuration;

        // PostgreSql service added into the container
        var optionsBuilder = new DbContextOptionsBuilder<SFTPDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(ConstantSupplier.APP_CONFIG_SFTP_DB_CONN_NAME));//,
        services.AddEntityFrameworkNpgsql().AddScoped<SFTPDbContext>(s => new SFTPDbContext(optionsBuilder.Options));
        //services.AddEntityFrameworkNpgsql().AddDbContext<SFTPDbContext>(opt =>
        //opt.UseNpgsql(configuration.GetConnectionString(ConstantSupplier.APP_CONFIG_SFTP_DB_CONN_NAME))).AddScoped<SFTPDbContext>();

        //services.AddHostedService<Worker>();
        services.AddHostedService<TimedHostedService>();

        // Custom services injected.
        services.AddTransient<ILogService, LogService>();
        services.AddTransient<IRemoteService, RemoteService>();
        services.AddTransient<IGenericRepository<FileDetails>, GenericRepository<FileDetails>>();
        services.AddTransient<IDataService, DataService>();

        // To get the support of legacy datetime in the postgresql (apart from utc.now()
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    })
    .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
    .UseContentRoot(Directory.GetCurrentDirectory())
    .Build();


    await host.RunAsync();
}
catch (Exception Ex)
{
    Log.Fatal(Ex, ConstantSupplier.LOG_ERROR_HOST_TERMINATE_MSG);
    throw;
}

