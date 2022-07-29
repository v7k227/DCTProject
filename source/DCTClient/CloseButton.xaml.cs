using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DCTClient
{
    /// <summary>
    /// Interaction logic for CloseButton.xaml
    /// </summary>
    public partial class CloseButton : UserControl
    {
        public CloseButton()
        {
            InitializeComponent();

            this.MouseEnter += gdClose_MouseEnter;
            this.MouseLeave += gdClose_MouseLeave;
            this.MouseDown += gdClose_MouseDown;
        }

        private void gdClose_MouseEnter(object sender, MouseEventArgs e)
        {
            GridCloseStateChange(img_close_h);
        }

        private void gdClose_MouseLeave(object sender, MouseEventArgs e)
        {
            GridCloseStateChange(img_close_n);
        }

        private void gdClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GridCloseStateChange(img_close_p);
        }

        private void GridCloseStateChange(Image visImg)
        {
            img_close_n.Visibility = Visibility.Collapsed;
            img_close_h.Visibility = Visibility.Collapsed;
            img_close_p.Visibility = Visibility.Collapsed;

            visImg.Visibility = Visibility.Visible;
        }
    }
}