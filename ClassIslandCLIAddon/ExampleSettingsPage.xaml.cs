using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;

namespace PluginWithSettingsPage.Views.SettingsPages;

[SettingsPageInfo("examples.exampleSettingsPage", "示例设置页面")]
public partial class ExampleSettingsPage : SettingsPageBase
{
    public ExampleSettingsPage()
    {
        InitializeComponent();
    }
}