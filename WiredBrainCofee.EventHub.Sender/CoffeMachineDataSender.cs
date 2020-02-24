using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiredBrainCoffee.EventHub.Sender.Model;

namespace WiredBrainCofee.EventHub.Sender
{
    public interface ICoffeMachineDataSender
    {
        Task SendDataAsync(CoffeeMachineData data);

        Task SendDataAsync(IEnumerable<CoffeeMachineData> datas);
    }

    public class CoffeMachineDataSender : ICoffeMachineDataSender
    {
        private EventHubClient _eventHubClient;

        public CoffeMachineDataSender(string eventHubConnectionString)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
        }

        public async Task SendDataAsync(CoffeeMachineData data)
        {
            // SendAsync takes an EventData as an argument. EventData takes JSON as an argument
            EventData evenData = CreateEventData(data);
            await _eventHubClient.SendAsync(evenData);
        }

        // EventHubClient comes with an overloaded version of SendAsync that accepts IEnumerable 
        public async Task SendDataAsync(IEnumerable<CoffeeMachineData> datas)
        {
            var eventDatas = datas.Select(coffeeMachineData => CreateEventData(coffeeMachineData));
            // Creates an eventhub batch that prevents batches from exceeding the size limit of 256kb
            var eventDataBatch = _eventHubClient.CreateBatch();

            foreach (var eventData in eventDatas)
            {
                // handles if the event date would exceed the limit
                if(!eventDataBatch.TryAdd(eventData))
                {
                    await _eventHubClient.SendAsync(eventDataBatch.ToEnumerable());
                    eventDataBatch = _eventHubClient.CreateBatch();
                    eventDataBatch.TryAdd(eventData);
                }
            }

            if(eventDataBatch.Count > 0)
            {
                await _eventHubClient.SendAsync(eventDataBatch.ToEnumerable());
            }
        }

        private static EventData CreateEventData(CoffeeMachineData data)
        {
            string dataAsJson = JsonConvert.SerializeObject(data);
            EventData evenData = new EventData(Encoding.UTF8.GetBytes(dataAsJson));
            return evenData;
        }
    }
}