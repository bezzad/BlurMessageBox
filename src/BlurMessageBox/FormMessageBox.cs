using System.Windows.Forms;

namespace BlurMessageBox
{
    public static class FormMessageBox
    {
        #region BlurWindows MessageBox.Show Methods

        public static DialogResult MessageBoxShow(this System.Windows.Window win, string message)
        {
            return MsgBox.Show(message, win);
        }
        public static DialogResult MessageBoxShow(this System.Windows.DependencyObject dependencyObject, string message)
        {
            return MsgBox.Show(message, System.Windows.Window.GetWindow(dependencyObject));
        }

        public static DialogResult MessageBoxShow(this System.Windows.Window win, string message, string title)
        {
            return MsgBox.Show(message, title, win);
        }
        public static DialogResult MessageBoxShow(this System.Windows.DependencyObject dependencyObject, string message, string title)
        {
            return MsgBox.Show(message, title, System.Windows.Window.GetWindow(dependencyObject));
        }

        public static DialogResult MessageBoxShow(this System.Windows.Window win, string message, string title, Buttons buttons)
        {
            return MsgBox.Show(message, title, buttons, win);
        }
        public static DialogResult MessageBoxShow(this System.Windows.DependencyObject dependencyObject, string message, string title, Buttons buttons)
        {
            return MsgBox.Show(message, title, buttons, System.Windows.Window.GetWindow(dependencyObject));
        }

        public static DialogResult MessageBoxShow(this System.Windows.Window win, string message, string title, Buttons buttons, Icons icon)
        {
            return MsgBox.Show(message, title, buttons, icon, win);
        }
        public static DialogResult MessageBoxShow(this System.Windows.DependencyObject dependencyObject, string message, string title, Buttons buttons, Icons icon)
        {
            return MsgBox.Show(message, title, buttons, icon, System.Windows.Window.GetWindow(dependencyObject));
        }

        public static DialogResult MessageBoxShow(this System.Windows.Window win, string message, string title, Buttons buttons, Icons icon, AnimateStyle style)
        {
            return MsgBox.Show(message, title, buttons, icon, style, win);
        }
        public static DialogResult MessageBoxShow(this System.Windows.DependencyObject dependencyObject, string message, string title, Buttons buttons, Icons icon, AnimateStyle style)
        {
            return MsgBox.Show(message, title, buttons, icon, style, System.Windows.Window.GetWindow(dependencyObject));
        }
        #endregion
    }
}
