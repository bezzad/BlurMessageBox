using System.Drawing;

namespace BlurMessageBox
{
    class AnimateMsgBox
    {
        public Size FormSize;
        public AnimateStyle Style;

        public AnimateMsgBox(Size formSize, AnimateStyle style)
        {
            this.FormSize = formSize;
            this.Style = style;
        }
    }
}
