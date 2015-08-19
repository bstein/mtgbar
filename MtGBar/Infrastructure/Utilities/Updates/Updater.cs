using System;
using System.Deployment.Application;
using System.Threading;
using System.Windows.Threading;

namespace MtGBar.Infrastructure.Utilities.Updates
{
    public class Updater
    {
        #region Bad singleton bad
        private static Updater _Instance;
        public static Updater Instance 
        {
            get
            {
                if (_Instance == null) {
                    _Instance = new Updater();
                }
                return _Instance;
            }
        }
        #endregion

        #region Fields
        private DateTime? _TheyveBeenWarnedDate = null;
        private Version _TheyveBeenWarned = new Version(0, 0, 0, 0);
        private Timer _UpdateCheckTimer;
        #endregion

        #region Events
        public event UpdateFoundEventHandler UpdateFound;
        #endregion

        private Updater() 
        {
            _UpdateCheckTimer = new Timer(
                (ohGodItsTime) => {
                    CheckForAllUpdates();
                },
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromMinutes(5)
            );
        }

        private void CheckForAllUpdates()
        {
            // check for actual application update
            CheckForApplicationUpdate();

            // check for image data updates
            ImageManager.DownloadImageData();
        }

        private void CheckForApplicationUpdate()
        {
            AppState.Instance.LoggingNinja.LogMessage("Checking for update...");
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed) {
                try {
                    info = ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate();
                }
                catch (Exception) {
                    // LIKE I GIVE A FUCK
                    // SWAG OVERLOAD
                    AppState.Instance.LoggingNinja.LogMessage("Couldn't reach the update server. Is Azure being bad again?");
                }

                if (info != null && info.UpdateAvailable) {
                    AppState.Instance.LoggingNinja.LogMessage("Update found: " + info.AvailableVersion.ToString());
                    bool warnedAboutThisVersion = (_TheyveBeenWarned.Equals(info.AvailableVersion));
                    if(!warnedAboutThisVersion) {
                        _TheyveBeenWarnedDate = null;
                    }

                    if (!warnedAboutThisVersion || _TheyveBeenWarnedDate == null || DateTime.Now - _TheyveBeenWarnedDate.Value > TimeSpan.FromDays(1)) {
                        AppState.Instance.LoggingNinja.LogMessage("They haven't been warned about this version or it's been more than a day since they heard about it.");
                        if (UpdateFound != null) {
                            UpdateFound();

                            _TheyveBeenWarned = info.AvailableVersion;
                            _TheyveBeenWarnedDate = DateTime.Now;
                            AppState.Instance.LoggingNinja.LogMessage("Warned them.");
                        }
                    }
                }
                else {
                    AppState.Instance.LoggingNinja.LogMessage("No update available.");
                }
            }
            else {
                AppState.Instance.LoggingNinja.LogMessage("App isn't network deployed. Can't update. EVER.");
            }
        }
    }
}