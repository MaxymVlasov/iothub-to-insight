namespace IoTLoadEmulator
{
    /// <summary>
    /// The settings with sensitive values.
    /// </summary>
    public partial class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.ConnectionString =
                "HostName=MspDayTestHub.azure-devices.net;\
                SharedAccessKeyName=;\
                SharedAccessKey=";
            this.HubUri = "MspDayTestHub.azure-devices.net";
        }
    }
}
