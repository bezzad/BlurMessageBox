using System;

namespace BlurMessageBox
{
    public static class BlurWindows
    {
        private static System.Windows.Media.Effects.Effect effectBuffer;
        private static System.Windows.Media.Brush brushBuffer;

        /// <summary>
        /// Apply Blur Effect on the window
        /// </summary>
        /// <param name="win">parent window</param>
        public static void ApplyEffect(this System.Windows.Window win)
        {
            // Create new effective objects
            System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect { Radius = 5 };
            System.Windows.Media.SolidColorBrush mask = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGray) { Opacity = 50 };

            // Buffering ...
            effectBuffer = win.Effect;
            brushBuffer = win.OpacityMask;

            // in windows 10, the dispatcher way doesn't seem to work anymore
            if (Environment.OSVersion.Version.Major >= 10)
            {
                win.Effect = objBlur;
                win.OpacityMask = mask;
            }
            else
            {
                // Change this.win effective objects
                win.Dispatcher.Invoke(new Action(delegate { win.Effect = objBlur; }), System.Windows.Threading.DispatcherPriority.Normal);
                win.Dispatcher.Invoke(new Action(delegate { win.OpacityMask = mask; }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }
        /// <summary>
        /// Remove Blur Effects
        /// </summary>
        /// <param name="win">parent window</param>
        public static void ClearEffect(this System.Windows.Window win)
        {
            // in windows 10, the dispatcher way doesn't seem to work anymore
            if (Environment.OSVersion.Version.Major >= 10)
            {
                win.Effect = null;
                win.OpacityMask = null;
                win.Focus();
            }
            else
            {
                // Back changed effective objects
                win.Dispatcher.Invoke(new Action(delegate { win.Effect = effectBuffer; }), System.Windows.Threading.DispatcherPriority.Normal);
                win.Dispatcher.Invoke(new Action(delegate { win.OpacityMask = brushBuffer; }), System.Windows.Threading.DispatcherPriority.Normal);
                win.Dispatcher.Invoke(new Action(delegate { win.Focus(); }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }
    }

}
