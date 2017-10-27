namespace IoTLoadEmulator
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using IoTLoadEmulator.Models;

    using Microsoft.Azure.Devices;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Common.Exceptions;

    using Newtonsoft.Json;

    using Message = Microsoft.Azure.Devices.Client.Message;
    using TransportType = Microsoft.Azure.Devices.Client.TransportType;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The generator.
        /// </summary>
        private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

        /// <summary>
        /// The log queue.
        /// </summary>
        private static readonly ConcurrentQueue<KeyValuePair<DateTime, string>> LogQueue = new ConcurrentQueue<KeyValuePair<DateTime, string>>();

        /// <summary>
        /// The settings.
        /// </summary>
        private static readonly Settings Settings = new Settings();

        /// <summary>
        /// The registry manager.
        /// </summary>
        private static RegistryManager registryManager;

        /// <summary>
        /// The main.
        /// </summary>
        internal static void Main()
        {
            // Creating log processing queue
            Task.Factory.StartNew(
                () =>
                    {
                        // Start infinite loop
                        while (true)
                        {
                            if (LogQueue.TryDequeue(out KeyValuePair<DateTime, string> value))
                            {
                                var oldColor = Console.ForegroundColor;
                                Console.Write($"{value.Key:s} > ");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(value.Value);
                                Console.ForegroundColor = oldColor;
                            }
                        }
                    });

            LogInfo("Connecting to hub");
            registryManager = RegistryManager.CreateFromConnectionString(Settings.ConnectionString);

            LogInfo("Generating device ids");
            var warehouses = Enumerable.Range(1, 1)
                .Select(
                    warehouse => new
                    {
                        WarehouseId = $"Warehouse{warehouse:D3}",
                        Devices = Enumerable.Range(1, 10)
                                             .Select(device => $"device{warehouse:D3}_{device:D3}").ToList()
                    })
                .ToDictionary(e => e.WarehouseId, e => e.Devices);
            LogInfo("Generating device ids complete");

            LogInfo("Generating device keys");

            var devices = warehouses.SelectMany(
                pair => pair.Value.Select(
                    deviceId => new
                    {
                        DeviceId = deviceId,
                        WarehouseId = pair.Key,
                        DeviceKey = AddDeviceAsync(deviceId).Result
                    })).ToList();

            LogInfo("Generating device keys complete");

            LogInfo("Start DDoSing");

            foreach (var device in devices)
            {
                var deviceClient = DeviceClient.Create(
                    Settings.HubUri,
                    new DeviceAuthenticationWithRegistrySymmetricKey(device.DeviceId, device.DeviceKey),
                    TransportType.Mqtt);
                Task.Factory.StartNew(
                    async () =>
                        {
                            // Start infinite loop
                            while (true)
                            {
                                var message = GetMessage(device.DeviceId, device.WarehouseId);
                                await SendDeviceToCloudMessagesAsync(deviceClient, message);
                                await Task.Delay(1000);
                            }
                        });
            }

            Console.ReadLine();
        }

        /// <summary>
        /// The log info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        internal static void LogInfo(string message)
        {
            LogQueue.Enqueue(new KeyValuePair<DateTime, string>(DateTime.UtcNow, message));
        }

        /// <summary>
        /// The get random message.
        /// </summary>
        /// <param name="deviceId">
        /// The device id.
        /// </param>
        /// <param name="warehouseId">
        /// The warehouse id.
        /// </param>
        /// <returns>
        /// The <see cref="MyEvent"/>.
        /// </returns>
        private static MyEvent GetMessage(string deviceId, string warehouseId)
        {
            var bytes = new byte[32];
            Generator.GetBytes(bytes);
            var result = new MyEvent
            {
                Properties = new DeviceProperty { DeviceId = deviceId, WarehouseId = warehouseId },
                Metrics =
                                     new DeviceMetric
                                     {
                                         Currency1 = bytes.GetInt(0),
                                         Currency2 = bytes.GetInt(4),
                                         Currency3 = bytes.GetInt(8),
                                         Temperature = bytes.GetInt(12),
                                         Vibrations = bytes.GetInt(16),
                                         Voltage1 = bytes.GetInt(20),
                                         Voltage2 = bytes.GetInt(24),
                                         Voltage3 = bytes.GetInt(28)
                                     }
            };
            return result;
        }

        /// <summary>
        /// The add device async.
        /// </summary>
        /// <param name="deviceId">
        /// The device Id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task<string> AddDeviceAsync(string deviceId)
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        /// <summary>
        /// The send device to cloud messages async.
        /// </summary>
        /// <param name="deviceClient">
        /// The device Client.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task SendDeviceToCloudMessagesAsync(DeviceClient deviceClient, MyEvent @event)
        {
            var messageString = JsonConvert.SerializeObject(@event);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await deviceClient.SendEventAsync(message);
            LogInfo($"Sending message: {messageString}");
        }
    }
}
