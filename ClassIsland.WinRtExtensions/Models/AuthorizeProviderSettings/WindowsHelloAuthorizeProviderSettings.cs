using CommunityToolkit.Mvvm.ComponentModel;
using WinRT.Interop;

namespace ClassIsland.WinRtExtensions.Models.AuthorizeProviderSettings;

public partial class WindowsHelloAuthorizeProviderSettings : ObservableObject
{
    [ObservableProperty]
    private Guid _userName = Guid.NewGuid();

    [ObservableProperty]
    private byte[] _publicKey = [];
}