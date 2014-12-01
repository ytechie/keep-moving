using System;
using Windows.Storage;

namespace KeepMoving.Framework
{
    public class Settings
    {
        private const string DeviceSettingsContainerName = "DeviceSettings";
        private const string TrackingEnabledSettingName = "TrackingEnabled";
        private const string LastOpenTimeSettingName = "LastOpened";

        public static void SetTrackingEnabled(bool enabled)
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(DeviceSettingsContainerName,
    ApplicationDataCreateDisposition.Always);

            settingsContainer.Values[TrackingEnabledSettingName] = enabled;
        }

        public static bool GetTrackingEnabled()
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(DeviceSettingsContainerName,
                ApplicationDataCreateDisposition.Always);

            var settingObj = settingsContainer.Values[TrackingEnabledSettingName];

            if (settingObj == null)
            {
                return true; //default
            }

            return (bool) settingObj;
        }

        public static void UpdateApplicationOpenTime()
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(DeviceSettingsContainerName,
    ApplicationDataCreateDisposition.Always);

            settingsContainer.Values[LastOpenTimeSettingName] = DateTime.UtcNow;
        }

        //Gets the last time the app was run
        public static DateTime GetApplicationOpenTime()
        {
            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(DeviceSettingsContainerName,
    ApplicationDataCreateDisposition.Always);

            var settingObj = settingsContainer.Values[LastOpenTimeSettingName];

            if (settingObj == null)
            {
                return DateTime.UtcNow; //default
            }

            return (DateTime)settingObj;
        }
    }
}
