using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace WPFEvents
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool isConnected;
        private string[] ports;
        private SerialPort port;
        public string points;
        private bool irCels = false;

        public MainWindow()
        {
            InitializeComponent();
            getAvailableComPorts();
            string path = Directory.GetCurrentDirectory();
            string compassImage = "file://" + path + "//Compass.jpg";
           
            if (File.Exists(compassImage))
            {
                imgPhoto.Source = new BitmapImage(new Uri(compassImage));
            }
            

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
                textBlockOutput.Text = (e.ToString());
                throw;
            }

            buttonConnect.Content = "Disconnect";
        }

        private void calcPath_Click(object sender, RoutedEventArgs e)
        {
            var platums = 4;
            var augstums = 6;
            var grid = new Labirints[platums, augstums];
            string ievads = inputTextFileContent.Text;
            ievads.Trim();
            string ievads2 = ievads.Remove(10, 5);
            char[] koordinatas = ievads2.ToCharArray(5, ievads2.Length - 5);
            for (int i = 0; i < koordinatas.Length - 1; i++)
            {
                if (!Char.IsNumber(koordinatas[i]))
                {
                    char temp = SwitchChar(koordinatas[i]);
                    koordinatas[i] = temp;
                }
            }

            textBlockOutput.Text = new string(koordinatas);

            int[] sienas = new int[(koordinatas.Length - 6) / 3];
            for (int i = 6; i < koordinatas.Length; i += 3)
            {
                sienas[(i - 6) / 3] = Convert.ToInt32(Char.GetNumericValue(koordinatas[i])) * 10 +
                                      Convert.ToInt32(Char.GetNumericValue(koordinatas[i + 1]));
            }

            Array.Sort(sienas);

            int sienuSkaititajs = 0;
            for (var x = 0; x < platums; x++)
            for (var y = 0; y < augstums; y++)
            {
                bool siena = false;

                if (sienuSkaititajs < sienas.Length && x + 1 == Math.Floor((double) (sienas[sienuSkaititajs] / 10)) &&
                    (y + 1 == sienas[sienuSkaititajs] - Math.Floor((double) (sienas[sienuSkaititajs] / 10) * 10)))
                {
                    siena = true;
                    sienuSkaititajs++;
                }

                grid[x, y] = new Labirints {Siena = siena, Platums = x, Augstums = y};
            }

            var x1 = Char.GetNumericValue(koordinatas[0]) - 1;
            var y1 = Char.GetNumericValue(koordinatas[1]) - 1;
            var x2 = Char.GetNumericValue(koordinatas[3]) - 1;
            var y2 = Char.GetNumericValue(koordinatas[4]) - 1;

            var aStar = new MySolver<Labirints, object>(grid);
            var path = aStar.Search(new Point(x1, y1), new Point(x2, y2), null);
            aStar.Search(new Point(x1, y1), new Point(x2, y2), null);

            points = "";

            if (path != null)
            {
                foreach (var node in path)
                {
                    textBlockOutput.Text = textBlockOutput.Text + "\t(" + (node.Platums + 1) + " " +
                                           (node.Augstums + 1) + ")";

                    points += node.Platums + 1;
                    points += node.Augstums + 1;
                }

                textBlockOutput.Text += " " + points;
                irCels = true;
            }
            else
            {
                textBlockOutput.Text = "Ceļa nav";

                irCels = false;
            }

            TextBlock[] labLauki =
            {
                txt11, txt12, txt13, txt14, txt15, txt16, txt21, txt22, txt23, txt24, txt25, txt26, 
                txt31, txt32, txt33, txt34, txt35, txt36, txt41, txt42, txt43, txt44, txt45, txt46
            };
            String[] laukuTeksts =
            {
                "A1", "A2", "A3", "A4", "A5", "A6", "B1", "B2", "B3", "B4", "B5", "B6", 
                "C1", "C2", "C3", "C4", "C5", "C6", "D1", "D2", "D3", "D4", "D5", "D6"

            };
            int skaits = 0;
            foreach (var lauks in labLauki)
            {
                lauks.Text = laukuTeksts[skaits];
                lauks.Background = Brushes.White;
                skaits++;
                mapColour(x1 + 1, y1 + 1, x2 + 1, y2 + 1, sienas, points, lauks);
            }
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

            public MySolver(TPathNode[,] inGrid) : base(inGrid)
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

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".txt";
            ofd.Filter = "Text Files (*.txt)|*.txt";

            if (ofd.ShowDialog() == true)
            {
                string dataFromTextFile = File.ReadAllText(ofd.FileName);
                inputTextFileContent.Text = dataFromTextFile;
            }
        }

        private void mapColour(double x1, double y1, double x2, double y2, int[] sienas, string points, TextBlock txt)
        {
            string name = txt.Name;
            double xPrim = Convert.ToInt32(Char.GetNumericValue(name[3]));
            double yPrim = Convert.ToInt32(Char.GetNumericValue(name[4]));
            if (x1 == xPrim && y1 == yPrim)
            {
                txt.Background = Brushes.LawnGreen;
                txt.Text = $"ST";
            }
            else if (x2 == xPrim && y2 == yPrim)
            {
                txt.Background = Brushes.Green;
                txt.Text = $"FN";
            }
            else
            {
                for (int i = 0; i < sienas.Length; i++)
                {
                    if (Math.Floor((double) (sienas[i] / 10)) == xPrim &&
                        sienas[i] - Math.Floor((double) (sienas[i] / 10) * 10) == yPrim)
                    {
                        txt.Background = Brushes.Black;
                    }
                }

                for (int i = 0; i < points.Length; i += 2)
                {
                    if (Convert.ToInt32(Char.GetNumericValue(points[i])) == xPrim &&
                        Convert.ToInt32(Char.GetNumericValue(points[i + 1])) == yPrim)
                    {
                        txt.Background = Brushes.Red;
                    }
                }
            }
        }

        private void ButtonSendData2_Click(object sender, RoutedEventArgs e)
        {
            if (irCels && isConnected)
            {
                port.WriteLine(points);
            }
        }

        private void comboBox_DropDownOpened(object sender, EventArgs e)
        {
            var value = comboBox.SelectedItem as string;
            getAvailableComPorts();
        }
    }
}