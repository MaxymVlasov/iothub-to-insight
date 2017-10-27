namespace IoTLoadEmulator.Models
{
    /// <summary>
    /// The device metric.
    /// </summary>
    public class DeviceMetric
    {
        /// <summary>
        /// Gets or sets the voltage 1.
        /// </summary>
        public decimal Voltage1 { get; set; }

        /// <summary>
        /// Gets or sets the voltage 2.
        /// </summary>
        public decimal Voltage2 { get; set; }

        /// <summary>
        /// Gets or sets the voltage 3.
        /// </summary>
        public decimal Voltage3 { get; set; }

        /// <summary>
        /// Gets or sets the currency 1.
        /// </summary>
        public decimal Currency1 { get; set; }

        /// <summary>
        /// Gets or sets the currency 2.
        /// </summary>
        public decimal Currency2 { get; set; }

        /// <summary>
        /// Gets or sets the currency 3.
        /// </summary>
        public decimal Currency3 { get; set; }

        /// <summary>
        /// Gets or sets the temperature.
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// Gets or sets the vibrations.
        /// </summary>
        public decimal Vibrations { get; set; }
    }
}