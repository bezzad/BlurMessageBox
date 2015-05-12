using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BlurMessageBox;

namespace TestWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnShowMessage_Click(object sender, EventArgs e)
        {
            MsgBox.Show("This is a test messageBox, Designed in WPF for WinForm and Window", 
                "In the name of the god", Buttons.OK, Icons.Info, AnimateStyle.ZoomIn);
        }

        private void btnLongText_Click(object sender, EventArgs e)
        {
            MsgBox.Show("Test Long String Test Long String Test Long String " + Environment.NewLine +
                        "Test Long String Test Long String Test Long String " + Environment.NewLine + 
                        "Test Long String Test Long String Test Long String.",
                        "Test Long Text",
                         Buttons.OKCancel, Icons.Shield, AnimateStyle.SlideDown);
        }

        private void btnTestAll_Click(object sender, EventArgs e)
        {
            MsgBox.Show("This is a test messageBox",
                "Test", Buttons.YesNoCancel, Icons.Warning, AnimateStyle.FadeIn);
        }
    }
}
