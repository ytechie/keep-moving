using Windows.Storage;

namespace KeepMoving.Framework
{
    public class Settings
    {
        private const string DeviceSettingsContainerName = "DeviceSettings";
        private const string TrackingEnabledSettingName = "TrackingEnabled";

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
    }
}
