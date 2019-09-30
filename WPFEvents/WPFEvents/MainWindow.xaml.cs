﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFEvents
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isConnected;
        private string[] ports;
        private SerialPort port;

        public MainWindow()
        {
            InitializeComponent();
            getAvailableComPorts();

            foreach (var port in ports)
            {
                comboBox.Items.Add(port);
                Console.WriteLine(port);
                if (ports[0] != null) comboBox.SelectedItem = ports[0];
            }
        }


        private void InputTextFileContent_LostFocus(object sender, RoutedEventArgs e)
        {
            if (inputTextFileContent.Text == "") inputTextFileContent.Text = "Input your data";
        }

        private void InputTextFileContent_GotFocus(object sender, RoutedEventArgs e)
        {
            if (inputTextFileContent.Text == "Input your data") inputTextFileContent.Text = null;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = comboBox.SelectedItem as string;
            getAvailableComPorts();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
                connectToArduino();
            else
                disconnectFromArduino();
        }

        private void disconnectFromArduino()
        {
            isConnected = false;
            port.WriteLine("4"); //STOP visiem motoriem
            port.Close();
            buttonConnect.Content = "Connect";
        }

        private void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void connectToArduino()
        {
            isConnected = true;
            var selectedPort = comboBox.SelectedValue.ToString();
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
                var inputText = inputTextFileContent.Text; //nolasam no TextBoxa datus
                sendCommand(inputText); //metode , kas apstrādā datus un sūta uz Arduino
            }

            var platums = 4;
            var augstums = 6;
            var grid = new Labirints[platums, augstums];
            string ievads = inputTextFileContent.Text;
            ievads.Trim();
            string ievads2 = ievads.Remove(10, 3);
            char[] koordinatas = ievads2.ToCharArray(5, ievads2.Length - 5);
            for (int i = 0; i < koordinatas.Length-1; i ++)
            {
                if (!Char.IsNumber(koordinatas[i]))
                {
                    char temp = SwitchChar(koordinatas[i]);
                    koordinatas[i] = temp;
                }
                
            }
            textBlockOutput.Text = new string(koordinatas);

            int[] sienas = new int[(koordinatas.Length - 6)/3];
            for (int i = 6; i < koordinatas.Length; i+=3)
            {
                sienas[(i - 6)/3] = Convert.ToInt32(Char.GetNumericValue(koordinatas[i]))*10 + Convert.ToInt32(Char.GetNumericValue(koordinatas[i+1]));
            }
            Array.Sort(sienas);

            int sienuSkaititajs = 0;
            for (var x = 0; x < platums; x++)
            for (var y = 0; y < augstums; y++)
            {
                
                bool siena = false;
                

                if (sienuSkaititajs < sienas.Length && x +1 == Math.Floor((double) (sienas[sienuSkaititajs]/10)) && (y+1  == sienas[sienuSkaititajs]- Math.Floor((double)(sienas[sienuSkaititajs] / 10)*10)))
                {
                    siena = true;
                    sienuSkaititajs ++;
                }

                grid[x, y] = new Labirints
                {
                    Siena = siena,
                    Platums = x,
                    Augstums = y
                };
                
            }

            var x1 = Char.GetNumericValue(koordinatas[0])-1;
            var y1 = Char.GetNumericValue(koordinatas[1])-1;
            var x2 = Char.GetNumericValue(koordinatas[3])-1;
            var y2 = Char.GetNumericValue(koordinatas[4])-1;
            
            var aStar = new MySolver<Labirints, object>(grid);
            var path = aStar.Search(new Point(x1, y1), new Point(x2, y2), null);
            aStar.Search(new Point(x1, y1), new Point(x2, y2), null);

            if (path != null)
                foreach (var node in path)
                    textBlockOutput.Text = textBlockOutput.Text + "\t(" + (node.Platums+1) + " " + (node.Augstums+1) + ")";
            else
                textBlockOutput.Text = "Ceļa nav";
        }

        private async void sendCommand(string inputText)
        {
            var cArray = new char[inputText.Length];
            for (var i = 0; i < inputText.Length; i++)
                cArray[i] = inputText[i]; //ielikam visas komandas char tipa massīvā
            var commandChars = new List<char>(); //jauns massivs (List) ar atsēvišķiem chariem 
            for (var i = 0; i < inputText.Length; i++)
                if (cArray[i] == '1' || cArray[i] == '2' || cArray[i] == '3' || cArray[i] == '4' || cArray[i] == '5')
                    // ^ filtrējam, ja lietotājs ievadīs nepareizus datus
                    commandChars.Add(cArray[i]);

            foreach (var c in commandChars)
            {
                textBlockOutput.Text +=
                    " " + c; //izvadām uz Output textBox rezultātu (tikai lai ērti redzēt kas notiek)
                port.WriteLine(c.ToString()); //sūtam uz arduino komandas pa vienai
                await Task.Delay(2000); // ar 2 sek pauzi
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        public class MySolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext>
            where TPathNode : IPathNode<TUserContext>
        {
            protected override double Heuristic(PathNode inStart, PathNode inEnd)
            {
                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
            }

            protected override double NeighborDistance(PathNode inStart, PathNode inEnd)
            {
                return Heuristic(inStart, inEnd);
            }

            public MySolver(TPathNode[,] inGrid)
                : base(inGrid)
            {
            }
        }

        static char SwitchChar(char input)
        {
            switch (input)
            {
                case 'A':
                {
                    return '1';
                }
                case 'B':
                {
                    return '2';
                }
                case 'C':
                {
                    return '3';
                }
                case 'D':
                {
                    return '4';
                }
                case '/':
                case ',':
                case '_':
                case 'u':
                case '.':
                {
                    return ' ';
                }
                default:
                {
                    return ' ';
                }

            }
        }
}
}