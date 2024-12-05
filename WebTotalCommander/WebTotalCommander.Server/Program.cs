using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using WebTotalCommander.Repository.Files;
using WebTotalCommander.Repository.Folders;
using WebTotalCommander.Repository.Helpers.Interfaces;
using WebTotalCommander.Repository.Helpers.Services;
using WebTotalCommander.Repository.Settings;
using WebTotalCommander.Server.ActionHelpers;
using WebTotalCommander.Server.Configuration;
using WebTotalCommander.Service.Services.FileServices;
using WebTotalCommander.Service.Services.FolderServices;

namespace WebTotalCommander.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());

        // Folder Path get configuration data
        var mainFolderPath = builder.Configuration["MainFolderName"];
        var folderSettings = new FolderSettings(mainFolderPath);


        // DropBoxCloud get configuration data
        var dropBoxConfig = builder.Configuration.GetSection("DropBoxCloud:Login");
        var appName = dropBoxConfig["AppName"];
        var appKey = dropBoxConfig["AppKey"];
        var appSecret = dropBoxConfig["AppSecret"];
        var authUrl = dropBoxConfig["AuthUrl"];
        var refreshToken = dropBoxConfig["RefreshToken"];
        var dropBoxSettings = new DropBoxSettings(appName, appKey, appSecret, authUrl, refreshToken);


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.ConfigureCORSPolice();

        //Configure big file upload
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024 * 1024;
        });
        //====================
        builder.Services.AddSingleton<FolderSettings>(folderSettings);
        builder.Services.AddSingleton<DropBoxSettings>(dropBoxSettings);

        builder.Services.AddScoped<IFolderRepository, FolderDropboxRepository>();
        builder.Services.AddScoped<IFileRepository, FileDropboxRepository>();

        builder.Services.AddScoped<IFolderService, FolderService>();
        builder.Services.AddScoped<IFileService, FileService>();

        builder.Services.AddScoped<ISorter, Sorter>();
        builder.Services.AddScoped<IPaginator, Paginator>();
        builder.Services.AddScoped<IFilter, Filter>();

        //AutoMapper (for dependicy injection)
        builder.Services.AddAutoMapper(typeof(MapperConfiguration));

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "DataFolder")),
            RequestPath = "/DataFolder"
        });
        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("AllowSpecificOrigin");
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
