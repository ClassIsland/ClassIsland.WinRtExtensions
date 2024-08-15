using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Controls.CommonDialog;
using ClassIsland.Core.Enums.SettingsWindow;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Uwp.Notifications;
using Octokit;
using Windows.Security.Credentials;

namespace ClassIsland.WinRtExtensions.Views;

/// <summary>
/// DebugPage.xaml 的交互逻辑
/// </summary>
[SettingsPageInfo("classisland.winRtExtensions.debug", "Win App SDK 调试", PackIconKind.MicrosoftWindows, PackIconKind.MicrosoftWindows, SettingsPageCategory.Debug)]
public partial class DebugPage
{
    public Plugin Plugin { get; }

    public DebugPage(Plugin plugin)
    {
        Plugin = plugin;
        InitializeComponent();
        DataContext = this;
    }

    private void MenuItemToastMe_OnClick(object sender, RoutedEventArgs e)
    {// Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        new ToastContentBuilder()
            .AddText("测试Toast消息。")
            .AddText("「人们为何选择沉睡？我想…是因为害怕从『梦』中醒来。」")
            .AddHeroImage(new Uri(System.IO.Path.Combine(Plugin.Info.PluginFolderPath, "Assets/TestHeroImage.png"), UriKind.RelativeOrAbsolute))
            .AddComboBox("testComboBox", "测试 ComboBox", new ValueTuple<string, string>("test0", "test0"))
            .AddInputTextBox("testTextBox", "测试 TextBox")
            .AddButton("测试按钮!", ToastActivationType.Protocol, "classisland://app/test")
            .Show();
    }

    private async void MenuItemTestWindowHelloLogin_OnClick(object sender, RoutedEventArgs e)
    {
        new CommonDialogBuilder().AddConfirmAction().SetContent("输入用户名").HasInput(true).ShowDialog(out var username);
        var keyCreationResult =
            await KeyCredentialManager.RequestCreateAsync(username, KeyCredentialCreationOption.ReplaceExisting);
    }

}