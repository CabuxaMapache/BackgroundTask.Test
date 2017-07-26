using BackgroundTask.Test.Helper;
using Microsoft.Toolkit.Uwp;
using MyTestTask;
using System;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackgroundTask.Test
{
    public sealed partial class MainPage : Page
    {
        private const string EXECUTIONS_FILE = "EXECUTIONS_FILE";
        LocalObjectStorageHelper _settings;

        public MainPage()
        {
            InitializeComponent();
            _settings = new LocalObjectStorageHelper();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await RefreshListAsync();
        }

        private async Task RefreshListAsync()
        {
            if (await _settings.FileExistsAsync(EXECUTIONS_FILE))
                ItemsListView.ItemsSource = await _settings.ReadFileAsync<List<BackgroundTaskExecResult>>(EXECUTIONS_FILE);
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status != BackgroundAccessStatus.DeniedByUser
                    && status != BackgroundAccessStatus.DeniedBySystemPolicy
                    && status != BackgroundAccessStatus.Unspecified)
            {
                var timerTask = BackgroundTaskHelper.Register(
                            typeof(MyBackgroundTask),
                            new TimeTrigger(15, false),
                            true,
                            false,
                            null);

                if (timerTask == null)
                {
                    RegistrationResultTextBlock.Text = "Registration failed!";
                }
                else
                {
                    RegistrationResultTextBlock.Text = "Registration success!";
                }
            }
            else
            {
                RegistrationResultTextBlock.Text = "Registration not allowed!";
            }
        }

        private async void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await RefreshListAsync();
        }
    }
}
