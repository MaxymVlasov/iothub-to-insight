namespace IoTLoadEmulator.Models
{
    /// <summary>
    /// The my event.
    /// </summary>
    public class MyEvent
    {
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public DeviceProperty Properties { get; set; }

        /// <summary>
        /// Gets or sets the metrics.
        /// </summary>
        public DeviceMetric Metrics { get; set; }
    }
}
