using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace BlurMessageBox
{
    public sealed class MsgBox : Form
    {
        #region Properties

        private const int CS_DROPSHADOW = 0x00020000;
        private static MsgBox _msgBox;
        private static DialogResult _buttonResult = new DialogResult();
        private static Timer _timer;
        private static Point _lastMousePos;

        private Panel _plHeader = new Panel();
        private Panel _plFooter = new Panel();
        private Panel _plIcon = new Panel();
        private PictureBox _picIcon = new PictureBox();
        private FlowLayoutPanel _flpButtons = new FlowLayoutPanel();
        private Label _lblTitle;
        private Label _lblMessage;
        private List<Button> _buttonCollection = new List<Button>();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        #endregion

        #region Constructors

        private MsgBox()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Width = 400;

            _lblTitle = new Label();
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Font = new System.Drawing.Font("Segoe UI", 18);
            _lblTitle.Dock = DockStyle.Top;
            _lblTitle.Height = 50;
            _lblTitle.RightToLeft = Properties.Localization.Culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;

            _lblMessage = new Label();
            _lblMessage.ForeColor = Color.White;
            _lblMessage.Font = new System.Drawing.Font("Segoe UI", 10);
            _lblMessage.Dock = DockStyle.Fill;
            _lblMessage.RightToLeft = Properties.Localization.Culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;

            _flpButtons.FlowDirection = FlowDirection.RightToLeft;
            _flpButtons.Dock = DockStyle.Fill;

            _plHeader.Dock = DockStyle.Fill;
            _plHeader.Padding = new Padding(20);
            _plHeader.Controls.Add(_lblMessage);
            _plHeader.Controls.Add(_lblTitle);

            _plFooter.Dock = DockStyle.Bottom;
            _plFooter.Padding = new Padding(20);
            _plFooter.BackColor = Color.FromArgb(37, 37, 38);
            _plFooter.Height = 80;
            _plFooter.Controls.Add(_flpButtons);

            _picIcon.Width = 32;
            _picIcon.Height = 32;
            _picIcon.Location = new Point(30, 50);

            _plIcon.Dock = DockStyle.Left;
            _plIcon.Padding = new Padding(20);
            _plIcon.Width = 70;
            _plIcon.Controls.Add(_picIcon);

            List<Control> controlCollection = new List<Control>();
            controlCollection.Add(this);
            controlCollection.Add(_lblTitle);
            controlCollection.Add(_lblMessage);
            controlCollection.Add(_flpButtons);
            controlCollection.Add(_plHeader);
            controlCollection.Add(_plFooter);
            controlCollection.Add(_plIcon);
            controlCollection.Add(_picIcon);

            foreach (Control control in controlCollection)
            {
                control.MouseDown += MsgBox_MouseDown;
                control.MouseMove += MsgBox_MouseMove;
            }

            this.Controls.Add(_plHeader);
            this.Controls.Add(_plIcon);
            this.Controls.Add(_plFooter);
        }

        #endregion

        #region Normal MessageBox.Show Methods
        public static DialogResult Show(string message)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox.Size = MsgBox.MessageSize(message);

            MsgBox.InitButtons(Buttons.OK);

            _msgBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox.Size = MsgBox.MessageSize(message);

            MsgBox.InitButtons(Buttons.OK);

            _msgBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox._plIcon.Hide();

            MsgBox.InitButtons(buttons);

            _msgBox.Size = MsgBox.MessageSize(message);
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            _msgBox.Size = MsgBox.MessageSize(message);
            _msgBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }

        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, AnimateStyle style)
        {
            _msgBox = new MsgBox();
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox.Height = 0;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            _timer = new Timer();
            Size formSize = MsgBox.MessageSize(message);

            switch (style)
            {
                case AnimateStyle.SlideDown:
                    _msgBox.Size = new Size(formSize.Width, 0);
                    _timer.Interval = 1;
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.FadeIn:
                    _msgBox.Size = formSize;
                    _msgBox.Opacity = 0;
                    _timer.Interval = 20;
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.ZoomIn:
                    _msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    _timer.Interval = 1;
                    break;
            }

            _timer.Tick += timer_Tick;
            _timer.Start();

            _msgBox.ShowDialog();
            MessageBeep(0);
            return _buttonResult;
        }
        #endregion
        #region BlurWindows MessageBox.Show Methods
        public static DialogResult Show(string message, System.Windows.Window win)
        {
            win.ApplyEffect();

            _msgBox = new MsgBox();

            _msgBox.StartPosition = FormStartPosition.CenterParent;

            _msgBox._lblMessage.Text = message;

            MsgBox.InitButtons(Buttons.OK);

            _msgBox.Size = MsgBox.MessageSize(message);

            _msgBox.ShowDialog();
            MessageBeep(0);

            win.ClearEffect();

            return _buttonResult;
        }
        public static DialogResult Show(string message, string title, System.Windows.Window win)
        {
            win.ApplyEffect();

            _msgBox = new MsgBox();
            _msgBox.StartPosition = FormStartPosition.CenterParent;
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox.Size = MsgBox.MessageSize(message);

            MsgBox.InitButtons(Buttons.OK);

            _msgBox.Size = MsgBox.MessageSize(message);

            _msgBox.ShowDialog();
            MessageBeep(0);


            win.ClearEffect();

            return _buttonResult;
        }
        public static DialogResult Show(string message, string title, Buttons buttons, System.Windows.Window win)
        {
            win.ApplyEffect();

            _msgBox = new MsgBox();
            _msgBox.StartPosition = FormStartPosition.CenterParent;
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox._plIcon.Hide();

            MsgBox.InitButtons(buttons);

            _msgBox.Size = MsgBox.MessageSize(message);

            _msgBox.ShowDialog();
            MessageBeep(0);

            win.ClearEffect();

            return _buttonResult;
        }
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, System.Windows.Window win)
        {
            win.ApplyEffect();

            _msgBox = new MsgBox();
            _msgBox.StartPosition = FormStartPosition.CenterParent;
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            _msgBox.Size = MsgBox.MessageSize(message);

            _msgBox.ShowDialog();
            MessageBeep(0);

            win.ClearEffect();

            return _buttonResult;
        }
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, AnimateStyle style, System.Windows.Window win)
        {
            win.ApplyEffect();

            _msgBox = new MsgBox();
            _msgBox.StartPosition = FormStartPosition.CenterParent;
            _msgBox._lblMessage.Text = message;
            _msgBox._lblTitle.Text = title;
            _msgBox.Height = 0;

            MsgBox.InitButtons(buttons);
            MsgBox.InitIcon(icon);

            _timer = new Timer();
            Size formSize = MsgBox.MessageSize(message);

            switch (style)
            {
                case AnimateStyle.SlideDown:
                    _msgBox.Size = new Size(formSize.Width, 0);
                    _timer.Interval = 1;
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.FadeIn:
                    _msgBox.Size = formSize;
                    _msgBox.Opacity = 0;
                    _timer.Interval = 20;
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.ZoomIn:
                    _msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    _timer.Tag = new AnimateMsgBox(formSize, style);
                    _timer.Interval = 1;
                    break;
            }
            _timer.Tick += timer_Tick;


            _timer.Start();
            _msgBox.ShowDialog();
            MessageBeep(0);

            win.ClearEffect();

            return _buttonResult;
        }
        #endregion

        #region Static Methods

        private static void MsgBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastMousePos = new Point(e.X, e.Y);
            }
        }

        private static void MsgBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _msgBox.Left += e.X - _lastMousePos.X;
                _msgBox.Top += e.Y - _lastMousePos.Y;
            }
        }

        private static void ButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            switch (btn.Name)
            {
                case "Abort":
                    _buttonResult = DialogResult.Abort;
                    break;

                case "Retry":
                    _buttonResult = DialogResult.Retry;
                    break;

                case "Ignore":
                    _buttonResult = DialogResult.Ignore;
                    break;

                case "OK":
                    _buttonResult = DialogResult.OK;
                    break;

                case "Cancel":
                    _buttonResult = DialogResult.Cancel;
                    break;

                case "Yes":
                    _buttonResult = DialogResult.Yes;
                    break;

                case "No":
                    _buttonResult = DialogResult.No;
                    break;
            }

            _msgBox.Dispose();
        }

        private static Size MessageSize(string message)
        {
            Graphics g = _msgBox.CreateGraphics();
            int width = 350;
            int height = 250;

            SizeF size = g.MeasureString(message, new System.Drawing.Font("Segoe UI", 10));

            if (message.Length < 150)
            {
                if ((int)size.Width > 350)
                {
                    width = (int)size.Width;
                }
            }
            else
            {
                string[] groups = (from Match m in Regex.Matches(message, ".{1,180}") select m.Value).ToArray();
                int maxLineLength = groups.Max(x => x.Length);
                int charachterLength = 6;
                int padding = 200;
                width = (width > padding + maxLineLength * charachterLength) ? width : padding + maxLineLength * charachterLength;
                height += (int)(size.Height);
            }
            return new Size(width, height);
        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            AnimateMsgBox animate = (AnimateMsgBox)timer.Tag;

            switch (animate.Style)
            {
                case AnimateStyle.SlideDown:
                    if (_msgBox.Height < animate.FormSize.Height)
                    {
                        _msgBox.Height += 17;
                        _msgBox.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case AnimateStyle.FadeIn:
                    if (_msgBox.Opacity < 1)
                    {
                        _msgBox.Opacity += 0.1;
                        _msgBox.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case AnimateStyle.ZoomIn:
                    if (_msgBox.Width > animate.FormSize.Width)
                    {
                        _msgBox.Width -= 17;
                        _msgBox.Invalidate();
                    }
                    if (_msgBox.Height > animate.FormSize.Height)
                    {
                        _msgBox.Height -= 17;
                        _msgBox.Invalidate();
                    }
                    if (_msgBox.Width <= animate.FormSize.Width && _msgBox.Height <= animate.FormSize.Height)
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;
            }
        }

        private static void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case Buttons.AbortRetryIgnore:
                    _msgBox.InitAbortRetryIgnoreButtons();
                    break;

                case Buttons.OK:
                    _msgBox.InitOkButton();
                    break;

                case Buttons.OKCancel:
                    _msgBox.InitOkCancelButtons();
                    break;

                case Buttons.RetryCancel:
                    _msgBox.InitRetryCancelButtons();
                    break;

                case Buttons.YesNo:
                    _msgBox.InitYesNoButtons();
                    break;

                case Buttons.YesNoCancel:
                    _msgBox.InitYesNoCancelButtons();
                    break;
            }

            foreach (Button btn in _msgBox._buttonCollection)
            {
                btn.ForeColor = Color.FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 8);
                btn.Padding = new Padding(3);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Height = 30;
                btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                _msgBox._flpButtons.Controls.Add(btn);
            }
        }

        private static void InitIcon(Icons icon)
        {
            switch (icon)
            {
                case Icons.Application:
                    _msgBox._picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case Icons.Exclamation:
                    _msgBox._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case Icons.Error:
                    _msgBox._picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case Icons.Info:
                    _msgBox._picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case Icons.Question:
                    _msgBox._picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case Icons.Shield:
                    _msgBox._picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case Icons.Warning:
                    _msgBox._picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }

        #endregion

        #region Methods

        private Button GetOkButton()
        {
            Button btnOk = new Button();
            btnOk.Name = "OK";
            btnOk.Text = Properties.Localization.BtnOK;
            btnOk.Click += ButtonClick;

            return btnOk;
        }

        private Button GetCancelButton()
        {
            Button btnCancel = new Button();
            btnCancel.Name = "Cancel";
            btnCancel.Text = Properties.Localization.BtnCancel;
            btnCancel.Click += ButtonClick;

            return btnCancel;
        }

        private Button GetRetryButton()
        {
            Button btnRetry = new Button();
            btnRetry.Name = "Retry";
            btnRetry.Text = Properties.Localization.BtnRetry;
            btnRetry.Click += ButtonClick;

            return btnRetry;
        }

        private Button GetAbortButton()
        {
            Button btnAbort = new Button();
            btnAbort.Name = "Abort";
            btnAbort.Text = Properties.Localization.BtnAbort;
            btnAbort.Click += ButtonClick;

            return btnAbort;
        }

        private Button GetIgnoreButton()
        {
            Button btnIgnore = new Button();
            btnIgnore.Name = "Ignore";
            btnIgnore.Text = Properties.Localization.BtnIgnore;
            btnIgnore.Click += ButtonClick;

            return btnIgnore;
        }

        private Button GetYesButton()
        {
            Button btnYes = new Button();
            btnYes.Name = "Yes";
            btnYes.Text = Properties.Localization.BtnYes;
            btnYes.Click += ButtonClick;

            return btnYes;
        }

        private Button GetNoButton()
        {
            Button btnNo = new Button();
            btnNo.Name = "No";
            btnNo.Text = Properties.Localization.BtnNo;
            btnNo.Click += ButtonClick;

            return btnNo;
        }


        private void InitOkButton()
        {
            _buttonCollection.Add(GetOkButton());
        }

        private void InitOkCancelButtons()
        {
            this._buttonCollection.Add(GetOkButton());
            this._buttonCollection.Add(GetCancelButton());
        }

        private void InitRetryCancelButtons()
        {
            this._buttonCollection.Add(GetRetryButton());
            this._buttonCollection.Add(GetCancelButton());
        }

        private void InitAbortRetryIgnoreButtons()
        {
            this._buttonCollection.Add(GetAbortButton());
            this._buttonCollection.Add(GetRetryButton());
            this._buttonCollection.Add(GetIgnoreButton());
        }

        private void InitYesNoButtons()
        {
            this._buttonCollection.Add(GetYesButton());
            this._buttonCollection.Add(GetNoButton());
        }

        private void InitYesNoCancelButtons()
        {
            this._buttonCollection.Add(GetYesButton());
            this._buttonCollection.Add(GetNoButton());
            this._buttonCollection.Add(GetCancelButton());
        }



        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle rect = new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1));
            Pen pen = new Pen(Color.FromArgb(0, 151, 251));

            g.DrawRectangle(pen, rect);
        }

        #endregion
    }
}
