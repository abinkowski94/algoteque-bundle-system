using System.CommandLine;
using AlgotequeCourseBundler.ConsoleApp.Binders;
using AlgotequeCourseBundler.ConsoleApp.Handlers;
using Microsoft.Extensions.DependencyInjection;

var topicsFilePathOption = new Option<string>(
    name: "--topics-path",
    description: "The file path to the topics JSON file.");

var rootCommand = new RootCommand("The topics bundler command.");
rootCommand.AddOption(topicsFilePathOption);

rootCommand.SetHandler(async (topicsFilePath, services) =>
{
    var handler = services.GetRequiredService<BundleTopicsHandler>();
    await handler.HandleAsync(topicsFilePath).ConfigureAwait(false);
},
topicsFilePathOption,
new ServiceProviderBinder());

await rootCommand.InvokeAsync(args).ConfigureAwait(false);