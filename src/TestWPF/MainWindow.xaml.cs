using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
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
                        Environment.NewLine +
                        "Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String" +
                        Environment.NewLine +
                        "Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String Test Long String"
                        ,"Test Long Text Title in Blur Message Box",
                         Buttons.OKCancel, Icons.Shield, AnimateStyle.SlideDown);
        }

        private void BtTestMsg_Click(object sender, RoutedEventArgs e)
        {
            this.MessageBoxShow("This is a test messageBox",
                "Test Long Text Title in Blur Message Box", Buttons.YesNoCancel, Icons.Warning, AnimateStyle.FadeIn);
        }

        private void RbtnLanguage_Checked(object sender, RoutedEventArgs e)
        {
            var lang = ((RadioButton)sender).Content as string;

            switch (lang)
            {
                case "Persian":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");
                    }
                    break;
                case "English":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
                    }
                    break;
                case "Russian":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");
                    }
                    break;
                case "Azerbaijan":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("az");
                    }
                    break;
                case "French":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fr-FR");
                    }
                    break;
                case "Turkish":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");
                    }
                    break;
                case "Arabic":
                    {
                        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ar");
                    }
                    break;
            }
        }
    }
}
