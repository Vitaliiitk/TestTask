using AppDirectoryInfo;
using AppDirectoryInfo.Services;
using AppDirectoryInfo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((_, services) =>
{
	services.AddSingleton<IDirectoryProcessor, DirectoryProcessor>();
	services.AddSingleton<App>();
});

var app = builder.Build();

var application = app.Services.GetRequiredService<App>();
application.Run();