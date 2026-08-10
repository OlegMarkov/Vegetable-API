namespace Vegetable.Core.Services
{
    /// <summary>
    /// Bound from the <c>FcmPushOptions</c> section of appsettings.json, the
    /// same way <c>GeTuiPushOptions</c> is.
    /// </summary>
    public class FcmPushOptions
    {
        /// <summary>
        /// Path to the Firebase service account key, absolute or relative to
        /// the content root. This file is a credential — it must not be
        /// committed, and it is not the same thing as the app's
        /// google-services.json, which is public and ships inside the APK.
        /// </summary>
        public string ServiceAccountJsonPath { get; set; }

        /// <summary>
        /// Optional. The service account file already names the project; set
        /// this only to override it.
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Name of the FirebaseApp instance. Only matters because FirebaseApp
        /// is process-global and throws on a duplicate name.
        /// </summary>
        public string AppName { get; set; } = "vegetable";
    }
}
