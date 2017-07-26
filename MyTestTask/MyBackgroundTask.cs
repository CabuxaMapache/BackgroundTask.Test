using BackgroundTask.Test.Helper;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace MyTestTask
{
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        private const string LAST_EXECUTION = "LAST_EXECUTION";
        private const string EXECUTIONS_FILE = "EXECUTIONS_FILE";
        BackgroundTaskDeferral _deferral;
        LocalObjectStorageHelper _settings;
        volatile bool _cancelRequested = false;
        List<BackgroundTaskExecResult> _executions;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _settings = new LocalObjectStorageHelper();
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            _deferral = taskInstance.GetDeferral();

            try
            {
                if (!_cancelRequested)
                {
                    _settings.Save(LAST_EXECUTION, DateTime.Now);

                    if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
                    {
                        throw new Exception("No internet connection!");
                    }

                    var jsonImgs = await HttpUtil.GetContentAsync("https://www.bing.com//HPImageArchive.aspx?format=js&idx=0&n=100&mkt=en-US");

                    // OK!
                    await SaveTaskResult("OK", jsonImgs);
                }
            }
            catch (Exception e)
            {
                await SaveTaskResult($"ERROR: {e.Message}", e.ToString());
            }
            finally
            {
                _deferral.Complete();
            }
        }

        private async void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            await SaveTaskResult($"BackgroundTask cancelled by: {reason.ToString()}");
        }

        private async Task SaveTaskResult(string text, string details = null)
        {
            await LoadExecutions();

            _executions.Add(new BackgroundTaskExecResult()
            {
                Result = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}] {text}",
                Details = details
            });
            await _settings.SaveFileAsync(EXECUTIONS_FILE, _executions);
        }

        private async Task LoadExecutions()
        {
            if (_executions == null)
            {
                if (await _settings.FileExistsAsync(EXECUTIONS_FILE))
                {
                    _executions = await _settings.ReadFileAsync<List<BackgroundTaskExecResult>>(EXECUTIONS_FILE);
                }
                else
                {
                    _executions = new List<BackgroundTaskExecResult>();
                }
            }
        }
    }
}
