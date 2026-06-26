using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PluginWithSettingsPage.Views.SettingsPages;

namespace ClassIslandCLIAddon;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSettingsPage<ExampleSettingsPage>();
        AppBase.Current.AppStarted += async (_, _) =>
            await CommonTaskDialogs.ShowDialog("Hello world!", "Hello from ClassIslandCLIAddon!");
    }
}