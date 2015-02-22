using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using KeepMoving.Framework;
using Mindscape.Raygun4Net;

namespace KeepMoving.Background
{
    public sealed class BackgroundReadTask : IBackgroundTask
    {
        private static BackgroundTaskDeferral _deferral;

        private const string BackgroundTaskName = "BackgroundReadTask";

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

        private static void InitRaygun()
        {
            try
            {
                RaygunClient.Attach("fR87yXCJfg88Xi6rpV0k0g==");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error registering Raygun: " + ex.ToString());
            }
        }

        public async static void Register()
        {
            InitRaygun();

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
                if (status == BackgroundAccessStatus.Denied || status == BackgroundAccessStatus.Unspecified)
                {
                    throw new Exception("Invalid status when registering background task: " + status);
                }
                builder.Register();

#if DEBUG
                Toast.SendNotification("Background Task Registered");
#endif
            }
            catch (Exception ex)
            {
                RaygunClient.Current.Send(ex);
            }

        }
    }
}


