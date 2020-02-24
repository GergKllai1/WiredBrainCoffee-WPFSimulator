using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WiredBrainCofee.EventHub.Sender;
using WiredBrainCoffee.EventHub.Sender.Model;

namespace WiredBrainCoffee.MachineSimulator.Ui.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private int _counterCappuccino;
        private int _counterEspresso;
        private string _city;
        private string _serialNumber;
        private int _boilerTemp;
        private int _beanlevel;
        private bool _isSendingPeriodically;
        private ICoffeMachineDataSender _coffeeMachineDataSender;
        private DispatcherTimer _dispatcherTimer;

        public MainViewModel(ICoffeMachineDataSender coffeeMachineDatSender)
        {
            _coffeeMachineDataSender = coffeeMachineDatSender;
            SerialNumber = Guid.NewGuid().ToString().Substring(0, 8);
            makeCappuccinoCommand = new DelegateCommand(MakeCappuccino);
            makeEspressoCommand = new DelegateCommand(MakeEspresso);
            Logs = new ObservableCollection<string>();
            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _dispatcherTimer.Tick += DispatchTimer_Tick;
        }

        private async void DispatchTimer_Tick(object sender, EventArgs e)
        {
            var boilerTermpData = CreateCoffeMachineData(nameof(BoilerTemp), BoilerTemp);
            var beanLevelData = CreateCoffeMachineData(nameof(BeanLevel), BeanLevel);
            // batching the data together to reduce network calls
            await SendDataAsync(new[] { boilerTermpData, beanLevelData });
        }

        public ICommand makeCappuccinoCommand { get; }
        public ICommand makeEspressoCommand { get; }
        public ObservableCollection<string> Logs { get; }

        public int BoilerTemp
        {
            get { return _boilerTemp; }
            set
            {
                _boilerTemp = value;
                RaisePropertyChanged();
            }
        }

        public int BeanLevel
        {
            get { return _beanlevel; }
            set
            {
                _beanlevel = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSendingPeriodically
        {
            get { return _isSendingPeriodically; }
            set
            {
                if (_isSendingPeriodically != value)
                {
                    _isSendingPeriodically = value;
                    if (_isSendingPeriodically)
                    {
                        _dispatcherTimer.Start();
                    }
                    else
                    {
                        _dispatcherTimer.Stop();
                    }
                    RaisePropertyChanged();
                }
            }
        }

        public int CounterCappuccino
        {
            get { return _counterCappuccino; }
            set
            {
                _counterCappuccino = value;
                RaisePropertyChanged();
            }
        }

        public int CounterEspresso
        {
            get { return _counterEspresso; }
            set
            {
                _counterEspresso = value;
                RaisePropertyChanged();
            }
        }

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged();
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
                RaisePropertyChanged();
            }
        }

        private async void MakeCappuccino()
        {
            CounterCappuccino++;
            var coffeeMachineData = CreateCoffeMachineData(nameof(CounterCappuccino), CounterCappuccino);
            await SendDataAsync(coffeeMachineData);
        }

        private async void MakeEspresso()
        {
            CounterEspresso++;
            var coffeeMachineData = CreateCoffeMachineData(nameof(CounterEspresso), CounterEspresso);
            await SendDataAsync(coffeeMachineData);
        }

        private CoffeeMachineData CreateCoffeMachineData(string sensorType, int sensorVaule)
        {
            return new CoffeeMachineData
            {
                City = City,
                SerialNumber = SerialNumber,
                SensorType = sensorType,
                SensorValue = sensorVaule,
                RecordingTime = DateTime.Now
            };
        }

        private async Task SendDataAsync(CoffeeMachineData coffeeMachineData)
        {
            try
            {
                await _coffeeMachineDataSender.SendDataAsync(coffeeMachineData);
                WriteLog($"Send data: {coffeeMachineData}");
            }
            catch (Exception ex)
            {
                WriteLog($"Execption : {ex.Message}");
            }
        }

        private async Task SendDataAsync(IEnumerable<CoffeeMachineData> coffeeMachineDatas)
        {
            try
            {
                await _coffeeMachineDataSender.SendDataAsync(coffeeMachineDatas);
                foreach (var coffeeMachineData in coffeeMachineDatas)
                {
                    WriteLog($"Send data: {coffeeMachineData}");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Execption : {ex.Message}");
            }
        }

        private void WriteLog(string logMessage)
        {
            Logs.Insert(0, logMessage);
        }
    }
}