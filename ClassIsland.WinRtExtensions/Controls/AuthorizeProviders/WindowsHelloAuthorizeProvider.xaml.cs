using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
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
using MaterialDesignThemes.Wpf;
using Octokit;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
using ABI.Windows.Data.Json;
using ClassIsland.Core.Controls.CommonDialog;
using System.Security.Principal;
using Windows.Security.Cryptography;

namespace ClassIsland.WinRtExtensions.Controls.AuthorizeProviders;

/// <summary>
/// WindowsHelloAuthorizeProvider.xaml 的交互逻辑
/// </summary>
[AuthorizeProviderInfo("classisland.authProviders.windowsHello", "Windows Hello", PackIconKind.MicrosoftWindows)]
public partial class WindowsHelloAuthorizeProvider
{
    public static readonly DependencyProperty ErrorDetailsProperty = DependencyProperty.Register(
        nameof(ErrorDetails), typeof(string), typeof(WindowsHelloAuthorizeProvider), new PropertyMetadata(default(string)));

    public string ErrorDetails
    {
        get { return (string)GetValue(ErrorDetailsProperty); }
        set { SetValue(ErrorDetailsProperty, value); }
    }

    public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register(
        nameof(IsError), typeof(bool), typeof(WindowsHelloAuthorizeProvider), new PropertyMetadata(default(bool)));

    public bool IsError
    {
        get { return (bool)GetValue(IsErrorProperty); }
        set { SetValue(IsErrorProperty, value); }
    }

    public static readonly DependencyProperty IsSuccessProperty = DependencyProperty.Register(
        nameof(IsSuccess), typeof(bool), typeof(WindowsHelloAuthorizeProvider), new PropertyMetadata(default(bool)));

    public bool IsSuccess
    {
        get { return (bool)GetValue(IsSuccessProperty); }
        set { SetValue(IsSuccessProperty, value); }
    }

    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register(
        nameof(IsWorking), typeof(bool), typeof(WindowsHelloAuthorizeProvider), new PropertyMetadata(default(bool)));

    public bool IsWorking
    {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }

    public WindowsHelloAuthorizeProvider()
    {
        InitializeComponent();
    }

    private async void ButtonSetupWindowsHello_OnClick(object sender, RoutedEventArgs e)
    {
        IsError = IsSuccess = false;
        IsWorking = true;

        var keyCreationResult = await KeyCredentialManager.RequestCreateAsync(Settings.UserName.ToString(), KeyCredentialCreationOption.ReplaceExisting);
        if (keyCreationResult.Status != KeyCredentialStatus.Success)
        {
            ErrorDetails = $"无法创建密钥：{keyCreationResult.Status}";
            IsError = true;
            IsWorking = false;
            return;
        }
        var publicKey = keyCreationResult.Credential.RetrievePublicKey().ToArray();
        Settings.PublicKey = publicKey;
        IsSuccess = true;
        IsWorking = false;
    }

    private async void ButtonAuthenticateWindowsHello_OnClick(object sender, RoutedEventArgs e)
    {
        IsError = IsSuccess = false;
        IsWorking = true;

        var openKeyResult = await KeyCredentialManager.OpenAsync(Settings.UserName.ToString());
        if (openKeyResult.Status != KeyCredentialStatus.Success)
        {
            ErrorDetails = $"无法打开密钥：{openKeyResult.Status}";
            IsError = true;
            IsWorking = false;
            return;
        }
        if (await RequestSignAsync(Settings.UserName, openKeyResult))
        {
            CompleteAuthorize();
        }
        else
        {
            IsError = true;
            ErrorDetails = "认证失败。";
        }

        IsWorking = false;
    }

    private static async Task<bool> RequestSignAsync(Guid userId, KeyCredentialRetrievalResult openKeyResult)
    {
        var challengeMessage = CryptographicBuffer.ConvertStringToBinary(Guid.NewGuid().ToString(), BinaryStringEncoding.Utf8);
        var userKey = openKeyResult.Credential;
        var signResult = await userKey.RequestSignAsync(challengeMessage);

        if (signResult.Status == KeyCredentialStatus.Success)
        {
            // If the challenge from the server is signed successfully
            // send the signed challenge back to the server and await the servers response
            return true;
        }
        else if (signResult.Status == KeyCredentialStatus.UserCanceled)
        {
            // User cancelled the Windows Hello PIN entry.
        }
        else if (signResult.Status == KeyCredentialStatus.NotFound)
        {
            // Must recreate Windows Hello key
        }
        else if (signResult.Status == KeyCredentialStatus.SecurityDeviceLocked)
        {
            // Can't use Windows Hello right now, remember that hardware failed and suggest restart
        }
        else if (signResult.Status == KeyCredentialStatus.UnknownError)
        {
            // Can't use Windows Hello right now, try again later
        }

        return false;
    }
}