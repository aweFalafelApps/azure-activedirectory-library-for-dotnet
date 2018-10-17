﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IPublicClientApplication _pca;
        private readonly static string ClientID = "0615b6ca-88d4-4884-8729-b178178f7c27";
        private readonly static string Authority = "https://login.microsoftonline.com/organizations"; // common will not work for WIA and U/P but it is a good test case
        private readonly static IEnumerable<string> Scopes = new[] { "user.read" };

        public MainPage()
        {
            this.InitializeComponent();

            _pca = new PublicClientApplication(ClientID, Authority);
            Telemetry.GetInstance().RegisterReceiver(TelemetryDelegate);
            logger = LogManager.Initialize(tenantToken: "356c5f7286974ece8d52964f7ad35643-6c8c6db0-888b-446e-a80c-e15e35b8cbcf-7507");
        }

        public void TelemetryDelegate(List<Dictionary<string, string>> events)
        {
            Console.WriteLine("{0} event(s) received", events.Count);
            foreach (var e in events)
            {
                Console.WriteLine("Event: {0}", e[EventBase.EventNameKey]);
                var eventData = new EventProperties(e[EventBase.EventNameKey]);
                foreach (var entry in e)
                {
                    eventData.SetProperty(entry.Key, entry.Value);
                    Console.WriteLine("  {0}: {1}", entry.Key, entry.Value);
                }
                logger.LogEvent(eventData);
            }
            LogManager.FlushAndTeardown();
        }

        private async void AcquireTokenIWA_ClickAsync(object sender, RoutedEventArgs e)
        {
            AuthenticationResult result = null;
            try
            {
                result = await _pca.AcquireTokenByIntegratedWindowsAuthAsync(Scopes);
                 // result = await _pca.AcquireTokenByIntegratedWindowsAuthAsync(Scopes, "bogavril@microsoft.com"); // can also use this overload
            }
            catch (Exception ex)
            {
                await DisplayError(ex);
                return;
            }

            await DisplayResult(result);

        }

        private async void ShowCacheCountAsync(object sender, RoutedEventArgs e)
        {
            var accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);
            await DisplayMessage(
                $"There are {accounts.Count()} in the token cache. " +
                Environment.NewLine +
                string.Join(", ", accounts.Select(a => a.Username)));
        }

        private async void ClearCacheAsync(object sender, RoutedEventArgs e)
        {
            var accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);
            foreach (var account in accounts)
            {
                await _pca.RemoveAsync(account);
            }
        }

        private async void AccessTokenSilentButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            var accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);

            AuthenticationResult result = null;
            try
            {
                result = await _pca.AcquireTokenSilentAsync(Scopes, accounts.FirstOrDefault());
            }
            catch (Exception ex)
            {
                await DisplayError(ex);
                return;
            }

            await DisplayResult(result);
        }

        private void AccessTokenButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task DisplayError(Exception ex)
        {
            await DisplayMessage(ex.Message);
        }

        private async Task DisplayResult(AuthenticationResult result)
        {
            await DisplayMessage("Signed in User - " + result.Account.Username + "\nAccessToken: \n" + result.AccessToken);
        }

        private async Task DisplayMessage(string message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       AccessToken.Text = message;
                   });
        }


    }
}
