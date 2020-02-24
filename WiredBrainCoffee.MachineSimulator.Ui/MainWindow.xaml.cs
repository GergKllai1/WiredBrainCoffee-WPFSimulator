using System.Configuration;
using System.Windows;
using WiredBrainCofee.EventHub.Sender;
using WiredBrainCoffee.MachineSimulator.Ui.ViewModel;

namespace WiredBrainCoffee.MachineSimulator.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // using System.Configuration. Add the system configuration reference.
            // Search in references assembly.
            var eventHubConnectionString =
                ConfigurationManager.AppSettings["eventHubConnectionString"];
            DataContext = new MainViewModel(new CoffeMachineDataSender(eventHubConnectionString));
        }
    }
}