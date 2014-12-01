using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using KeepMoving.Framework;

namespace KeepMoving.Background
{
    public sealed class BackgroundReadTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _deferral;

        private const string BackgroundTaskName = "BackgroundReadTask";

        // The amount of time the app can be closed before needing to be
        // reopened to renew the background task
        private static readonly TimeSpan BackgroundTimeoutLimit = TimeSpan.FromDays(10);

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            if(!Settings.GetTrackingEnabled())
            {
                return;
            }

            _deferral = taskInstance.GetDeferral();

            await Framework.Sensor.DoStuff();

            _deferral.Complete();
        }

        public async static void Register()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == BackgroundTaskName)
                {
                    task.Value.Unregister(true);
                }
            }

            var builder = new BackgroundTaskBuilder
            {
                Name = BackgroundTaskName,
                TaskEntryPoint = typeof(BackgroundReadTask).Namespace + "." + typeof(BackgroundReadTask).Name
            };

            builder.SetTrigger(new TimeTrigger(15, false));

            try
            {
                var status = await BackgroundExecutionManager.RequestAccessAsync();
                var registration = builder.Register();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error while registering background task: ", ex.Message);
            }

        }

        private static void CheckLastOpenTime()
        {
            var lastOpened = Settings.GetApplicationOpenTime();

            if (lastOpened < DateTime.UtcNow.Subtract(BackgroundTimeoutLimit))
            {
                Toast.SendNotification("Please open Keep Moving so that it can continue to run in the background");
            }
        }
    }
}


