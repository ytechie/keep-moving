using System;
using Windows.Data.Xml.Dom;
using Windows.Phone.Devices.Notification;
using Windows.UI.Notifications;

namespace KeepMoving.Framework
{
    public sealed class Toast
    {
        public static void SendNotification(string text)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

            var elements = toastXml.GetElementsByTagName("text");
            foreach (IXmlNode node in elements)
            {
                node.InnerText = text;
            }

            var notification = new ToastNotification(toastXml);

            ClearToast();
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        public static void ClearToast()
        {
            ToastNotificationManager.History.Clear();
        }

        public static void SendStationaryNotification(TimeSpan duration)
        {
            string msg;

            if (duration > TimeSpan.FromDays(1))
            {
                msg = "You've been inactive a while, get up and move!";
            }
            else
            {
                var minutes = (int)Math.Round(duration.TotalMinutes);
                msg = string.Format("You've been inactive {0} minutes, get up and move!", minutes);
            }

            SendNotification(msg);
        }
    }
}
