using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlurMessageBox;

namespace TestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnShowMsg_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxShow("This is a test messageBox, Designed in WPF for WinForm and Window",
                "In the name of the god", Buttons.OK, Icons.Info, AnimateStyle.ZoomIn);
        }

        private void BtnLongTextMsg_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxShow("Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String" +
                        "Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String" +
                        "Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String" +
                        "Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String" +
                        "Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String.",
                        "Test Long Text",
                         Buttons.OKCancel, Icons.Shield, AnimateStyle.SlideDown);
        }

        private void BtTestMsg_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxShow("This is a test messageBox",
                "Test", Buttons.YesNoCancel, Icons.Warning, AnimateStyle.FadeIn);
        }

        private void RbtnLanguage_Checked(object sender, RoutedEventArgs e)
        {
            var lang = ((RadioButton)sender).Content as string;

            switch (lang)
            {
                case "Persian":
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("fa-IR");
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa-IR");
                    }
                    break;
                case "English":
                    {
                        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                        System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    }
                    break;
            }
        }
    }
}
