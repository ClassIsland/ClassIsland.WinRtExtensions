
using System.Reflection;
using System.Runtime.Loader;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.WinRtExtensions.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.IO.Path;

namespace ClassIsland.WinRtExtensions;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        if (Environment.OSVersion.Version.Build < 18362)
        {
            throw new PlatformNotSupportedException("此插件需要 Windows 10.0.18362.0 以及以上的系统才能工作。");
        }
        var asmContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
        asmContext?.LoadFromAssemblyPath(
            Combine(Info.PluginFolderPath, "Microsoft.Windows.SDK.NET.dll"));
        asmContext?.LoadFromAssemblyPath(Combine(Info.PluginFolderPath,
            "WinRT.Runtime.dll"));
        asmContext?.LoadFromAssemblyPath(Combine(Info.PluginFolderPath, "Microsoft.Toolkit.Uwp.Notifications.dll"));

        services.AddSettingsPage<DebugPage>();
    }
}