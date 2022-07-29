using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DCTLib;

namespace DCTClient
{
    /// <summary>
    /// Interaction logic for PPWindow.xaml
    /// </summary>
    public partial class NetWindow : Window
    {
        private readonly string netId = "DCT";
        private readonly string netPw = "123qweasd";

        public NetWindow()
        {
            InitializeComponent();
            this.Title = DCTDef.AppName;
        }

        private Thread T;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pb.Visibility = Visibility.Visible;
            T = new Thread(PBRun);
            T.Start();
            Login(netId, netPw);
        }

        private void PBRun()
        {
            try
            {
                while (true)
                {
                    pb.Dispatcher.Invoke(() =>
                    {
                        if (pb.Value == pb.Minimum)
                            pb.Value = pb.Maximum;
                        else
                            pb.Value = pb.Minimum;
                    });

                    Thread.Sleep(100);
                }
            }
            catch
            {
            }
        }

        private async Task ShowRunningDiag()
        {
            await Task.Delay(3000);
            tbDis.Dispatcher.Invoke(() =>
            {
                pb.Visibility = Visibility.Collapsed;
                T.Abort();
                tbDis.Text =
                string.Format(@"
完成。
                        ", DCTDef.AppName);
            });
        }

        private async void Login(string userId, string password)
        {
            await ShowRunningDiag();
        }

        private static async Task<string> GetRequest(string url)
        {
            string output = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
                    output = await client.GetStringAsync(url);
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }

            return output;
        }

        private void gdClose_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}