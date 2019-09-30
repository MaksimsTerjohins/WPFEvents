using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO.Ports;

namespace WPFEvents
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isConnected = false;
        String[] ports;
        SerialPort port;

        public MainWindow()
        {
            InitializeComponent();
            getAvailableComPorts();

            foreach (string port in ports)
            {
                comboBox.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null)
                {
                    comboBox.SelectedItem = ports[0];
                }
            }
            
        }



        private void InputTextFileContent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (inputTextFileContent.Text == "")
            {
                inputTextFileContent.Text = "Input your data";
            }

        }

        private void InputTextFileContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (inputTextFileContent.Text == "Input your data")
            {
                inputTextFileContent.Text = null;
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string value = comboBox.SelectedItem as string;
            getAvailableComPorts();


        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                connectToArduino();
            }
            else
            {
                disconnectFromArduino();
            }
        }

        private void disconnectFromArduino()
        {
            isConnected = false;
            port.WriteLine("4"); //STOP visiem motoriem
            port.Close();
            buttonConnect.Content = "Connect";
        }

        void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = comboBox.SelectedValue.ToString();
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            try //medz buut ka USB tika atvienots
            {
                port.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            buttonConnect.Content = "Disconnect";
     }

        private void ButtonSendData_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                string inputText = inputTextFileContent.Text; //nolasam no TextBoxa datus
                sendCommand(inputText);  //metode , kas apstrādā datus un sūta uz Arduino
            }
        }

        private async void sendCommand (string inputText)
        {
            char[] cArray = new char[inputText.Length];
            for (int i = 0; i < inputText.Length; i++)
            {
                cArray[i] = inputText[i]; //ielikam visas komandas char tipa massīvā
            }
            var commandChars = new List<char>(); //jauns massivs (List) ar atsēvišķiem chariem 
            for (int i = 0; i < inputText.Length; i++)
            {
                if (cArray[i] ==  '1' || cArray[i] ==  '2' || cArray[i] == '3' || cArray[i] == '4' || cArray[i] == '5' )
                {  // ^ filtrējam, ja lietotājs ievadīs nepareizus datus
                    commandChars.Add(cArray[i]); 
                }
            }

            foreach (var c in commandChars)
            {
                textBlockOutput.Text += " " + c.ToString(); //izvadām uz Output textBox rezultātu (tikai lai ērti redzēt kas notiek)
                port.WriteLine(c.ToString()); //sūtam uz arduino komandas pa vienai
                await Task.Delay(2000); // ar 2 sek pauzi
            }


        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
