using System;

namespace WiredBrainCoffee.EventHub.Sender.Model
{
    public class CoffeeMachineData
    {
        public string City { get; set; }
        public string SerialNumber { get; set; }
        public string SensorType { get; set; }
        public int SensorValue { get; set; }
        public DateTime RecordingTime { get; set; }

        // override for showing the log in the correct format
        public override string ToString()
        {
            return $"Time: {RecordingTime:HH:mm:ss} | {SensorType}: {SensorValue} | City: {City} | Serialnumber: {SerialNumber}";
        }
    }
}