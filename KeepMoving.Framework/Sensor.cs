using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Lumia.Sense;
using Newtonsoft.Json;

namespace KeepMoving.Framework
{
    public class Sensor
    {
        public static async Task DoStuff()
        {
            var activityMonitor = await ActivityMonitor.GetDefaultAsync();

            var lastRead = new DateTimeOffset(DateTime.Now.Subtract(TimeSpan.FromHours(.5)));
            var history = await activityMonitor.GetActivityHistoryAsync(lastRead, TimeSpan.FromHours(.5));
            history = history.ToList();

            var timeSinceLastWalk = GetTimeSinceLastWalk(history, TimeSpan.FromSeconds(30));

            if (timeSinceLastWalk.TotalMinutes > 20)
            {
                Toast.SendStationaryNotification(timeSinceLastWalk);
            }
            else
            {
                Toast.ClearToast();
            }
        }

        //private static async void SendDiagnosticData(IList<ActivityMonitorReading> history)
        //{
        //    var historyJson = JsonConvert.SerializeObject(history);
        //    const string url = "http://keepmovingweb.azurewebsites.net/api/wakeevent/";

        //    var client = new HttpClient();
        //    var content = new StringContent("\"" + WebUtility.HtmlEncode(historyJson) + "\"");

        //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PostAsync(url, content);
        //}

        public static async Task<IEnumerable<ActivityMonitorReading>> GetHistory()
        {
            var activityMonitor = await ActivityMonitor.GetDefaultAsync();
            var history = await activityMonitor.GetActivityHistoryAsync(DateTime.Now.AddMinutes(-30), TimeSpan.FromHours(.5));


            return history;
        }

        public static TimeSpan GetTimeSinceLastWalk(IEnumerable<ActivityMonitorReading> history, TimeSpan minimumMoveTime)
        {
            var prevTimestamp = new DateTimeOffset();
            var lastMovedAt = new DateTimeOffset();
            ActivityMonitorReading currentActivity = null;

            var historyList = history.ToList();
            if (historyList.Count == 0)
                return TimeSpan.Zero;

            foreach (var reading in historyList)
            {
                //The first value is actually the previous activity outside the time range
                if (prevTimestamp == new DateTimeOffset())
                {
                    prevTimestamp = reading.Timestamp;
                    continue;
                }

                if ((reading.Mode == Activity.Moving ||
                     reading.Mode == Activity.Walking ||
                     reading.Mode == Activity.Running) &&
                    reading.Timestamp - prevTimestamp > minimumMoveTime)
                {
                    lastMovedAt = reading.Timestamp;
                }

                prevTimestamp = reading.Timestamp;
                currentActivity = reading;
            }

            if (currentActivity != null && (currentActivity.Mode != Activity.Stationary))
            {
                //the phone is sitting somewhere, or the person is currently moving
                return TimeSpan.Zero;
            }

            return DateTimeOffset.Now - lastMovedAt;
        }

        public static async Task<bool> CheckSensorCoreSupport()
        {
            var supported = await ActivityMonitor.IsSupportedAsync();
            if (!supported)
            {
                var dialog =
                    new MessageDialog(
                        "Your phone does not support the the SensorCore features, which are required for this app to function.");

                await dialog.ShowAsync();
            }

            return supported;
        }
    }
}
