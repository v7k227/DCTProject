using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DCTLib;

namespace DCTClient

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool canRunProc;

        private enum EUIState
        {
            MessageBox,
            NetworkDialog,
            FocusedUserId,
            FocusedPPCheckbox,
        }

        private delegate void UIThreadHandler(EUIState uiState, string msg);

        private event UIThreadHandler UIThreadEvent;

        public MainWindow()
        {
            InitializeComponent();
            canRunProc = true;
            this.Title = DCTDef.AppName;

            UIThreadEvent += MainWindow_UIThreadEvent;

            if (DCTRegistry.ExistOldVersion())
            {
                UpdateUIEvent(EUIState.MessageBox, "請至新增移除程式移除先前版本的DCT認證程式然後再試一次");
                Close();
                return;
            }

            DCTRegistry.InitialReg();

            if (DCTRegistry.IsLoggedIn())
            {
                NetWindow netWindow = new NetWindow();
                netWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                netWindow.ShowDialog();

                Close();
                return;
            }
        }

        private void UpdateUIEvent(EUIState uiState, string msg = "")
        {
            if (UIThreadEvent != null)
                UIThreadEvent(uiState, msg);
        }

        private void MainWindow_UIThreadEvent(EUIState uiState, string msg)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                switch (uiState)
                {
                    case EUIState.MessageBox:
                        MessageBox.Show(msg);
                        break;

                    case EUIState.NetworkDialog:
                        NetWindow netWindow = new NetWindow();
                        netWindow.Show();
                        this.Hide();
                        Close();
                        break;
                }
            }));
        }

        #region UI Events

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

        private SolidColorBrush normalColor = new SolidColorBrush(Color.FromRgb(255, 0, 79));
        private SolidColorBrush hoverColor = new SolidColorBrush(Color.FromRgb(128, 0, 40));

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            gdVerify.Background = hoverColor;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            gdVerify.Background = normalColor;
        }

        private void RunProc()
        {
            if (!canRunProc)
                return;

            canRunProc = false;

            bool isAnyFailure = false;

            Task.Factory.StartNew(new Action(() =>
            {
            })).ContinueWith(new Action<Task>((task) =>
            {
                if (!isAnyFailure) // 登入成功
                {
                    // 初始化環境
                    DCTFuncs.InitialGMEnv();

                    // 註冊TaskSchedluer for trigger DCTGM.exe
                    DCTTaskScheduler.CreateTask();

                    // 更新登入成功資訊
                    DCTRegistry.SetUserID();
                    //
                    UpdateUIEvent(EUIState.NetworkDialog);
                    canRunProc = true;
                }
            }));
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            RunProc();
        }

        #endregion UI Events

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunProc();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            foc.BorderThickness = new Thickness(1);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            foc.BorderThickness = new Thickness(0);
        }
    }
}