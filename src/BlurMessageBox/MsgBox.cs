using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Application = System.Windows.Forms.Application;
using FlowDirection = System.Windows.Forms.FlowDirection;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using System.Text;

namespace BlurMessageBox
{
    [DebuggerStepThrough]
    public sealed class MsgBox : Form
    {
        #region Properties

        public static Font TitleFont = new System.Drawing.Font("Segoe UI", 18);
        public static Font MessageFont = new System.Drawing.Font("Segoe UI", 10);
        private static readonly object SyncLocker = new object();
        private static readonly Padding BoxPadding = new Padding(200, 0, 0, 200);

        private const int CS_DROPSHADOW = 0x00020000;
        private const int MinWidth = 350;
        private const int MinHeight = 250;
        private readonly Graphics _graphics;
        private const int cGrip = 16; // Grip size
        private const int cCaption = 32; // Caption bar height;

        private DialogResult _buttonResult;
        private Timer _timer;
        private Point _lastMousePos;

        private Panel _plHeader;
        private Panel _plFooter;
        private Panel _plIcon;
        private PictureBox _picIcon;
        private FlowLayoutPanel _flpButtons;
        private Label _lblTitle;
        private Label _lblMessage;
        private List<Button> _buttonCollection = new List<Button>();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        #endregion

        #region Constructors

        [DebuggerStepThrough]
        private MsgBox()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.BackColor = Color.FromArgb(45, 45, 48);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Width = 400;
            this._graphics = this.CreateGraphics();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            var culture = CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture;

            _lblTitle = new Label
            {
                ForeColor = Color.White,
                Font = TitleFont,
                Dock = DockStyle.Top,
                Height = 50,
                RightToLeft = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No
            };

            _lblMessage = new Label
            {
                ForeColor = Color.White,
                Font = MessageFont,
                Dock = DockStyle.Fill,
                RightToLeft = culture.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No
            };

            _flpButtons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };

            _plHeader = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            _plHeader.Controls.Add(_lblMessage);
            _plHeader.Controls.Add(_lblTitle);

            _plFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(37, 37, 38),
                Height = 80
            };
            _plFooter.Controls.Add(_flpButtons);

            _picIcon = new PictureBox { Width = 32, Height = 32, Location = new Point(30, 50) };

            _plIcon = new Panel { Dock = DockStyle.Left, Padding = new Padding(20), Width = 70 };
            _plIcon.Controls.Add(_picIcon);

            var controlCollection = new List<Control>
            {
                this,
                _lblTitle,
                _lblMessage,
                _flpButtons,
                _plHeader,
                _plFooter,
                _plIcon,
                _picIcon
            };

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

        [DebuggerStepThrough]
        public static DialogResult Show(string message)
        {
            var msgBox = new MsgBox();
            msgBox._lblMessage.Text = message;
            msgBox.Size = msgBox.MessageSize(message, "");

            msgBox.InitButtons(Buttons.OK);

            ShowForm(msgBox);

            MessageBeep(0);
            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title)
        {
            var msgBox = new MsgBox
            {
                _lblMessage = { Text = message },
                _lblTitle = { Text = title },
            };
            msgBox.Size = msgBox.MessageSize(message, title);

            msgBox.InitButtons(Buttons.OK);

            ShowForm(msgBox);

            MessageBeep(0);
            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons)
        {
            var msgBox = new MsgBox { _lblMessage = { Text = message }, _lblTitle = { Text = title } };
            msgBox._plIcon.Hide();

            msgBox.InitButtons(buttons);

            msgBox.Size = msgBox.MessageSize(message, title);

            ShowForm(msgBox);

            MessageBeep(0);
            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon)
        {
            var msgBox = new MsgBox { _lblMessage = { Text = message }, _lblTitle = { Text = title } };

            msgBox.InitButtons(buttons);
            msgBox.InitIcon(icon);

            msgBox.Size = msgBox.MessageSize(message, title);

            ShowForm(msgBox);

            MessageBeep(0);
            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, AnimateStyle style)
        {
            var msgBox = new MsgBox { _lblMessage = { Text = message }, _lblTitle = { Text = title }, Height = 0 };

            msgBox.InitButtons(buttons);
            msgBox.InitIcon(icon);

            msgBox._timer = new Timer();
            Size formSize = msgBox.MessageSize(message, title);

            switch (style)
            {
                case AnimateStyle.SlideDown:
                    msgBox.Size = new Size(formSize.Width, 0);
                    msgBox._timer.Interval = 1;
                    msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.FadeIn:
                    msgBox.Size = formSize;
                    msgBox.Opacity = 0;
                    msgBox._timer.Interval = 20;
                    msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.ZoomIn:
                    msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                    msgBox._timer.Interval = 1;
                    break;
            }

            msgBox.CenterToParent(formSize);

            msgBox._timer.Tick += msgBox.timer_Tick;
            msgBox._timer.Start();

            ShowForm(msgBox);

            MessageBeep(0);
            return msgBox._buttonResult;
        }

        #endregion

        #region BlurWindows MessageBox.Show Methods

        [DebuggerStepThrough]
        public static DialogResult Show(string message, System.Windows.Window win)
        {
            win.ApplyEffect();

            var msgBox = new MsgBox { StartPosition = FormStartPosition.CenterParent, _lblMessage = { Text = message } };

            msgBox.InitButtons(Buttons.OK);

            msgBox.Size = msgBox.MessageSize(message, "");

            ShowForm(msgBox);

            MessageBeep(0);

            win.ClearEffect();

            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, System.Windows.Window win)
        {
            win.ApplyEffect();

            var msgBox = new MsgBox
            {
                StartPosition = FormStartPosition.CenterParent,
                _lblMessage = { Text = message },
                _lblTitle = { Text = title },
            };
            msgBox.Size = msgBox.MessageSize(message, title);

            msgBox.InitButtons(Buttons.OK);

            msgBox.Size = msgBox.MessageSize(message, title);

            ShowForm(msgBox);

            MessageBeep(0);

            win.ClearEffect();

            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, System.Windows.Window win)
        {
            win.ApplyEffect();

            var msgBox = new MsgBox
            {
                StartPosition = FormStartPosition.CenterParent,
                _lblMessage = { Text = message },
                _lblTitle = { Text = title }
            };
            msgBox._plIcon.Hide();

            msgBox.InitButtons(buttons);

            msgBox.Size = msgBox.MessageSize(message, title);

            ShowForm(msgBox);

            MessageBeep(0);

            win.ClearEffect();

            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, System.Windows.Window win)
        {
            win.ApplyEffect();

            var msgBox = new MsgBox
            {
                StartPosition = FormStartPosition.CenterParent,
                _lblMessage = { Text = message },
                _lblTitle = { Text = title }
            };

            msgBox.InitButtons(buttons);
            msgBox.InitIcon(icon);

            msgBox.Size = msgBox.MessageSize(message, title);

            ShowForm(msgBox);

            MessageBeep(0);

            win.ClearEffect();

            return msgBox._buttonResult;
        }

        [DebuggerStepThrough]
        public static DialogResult Show(string message, string title, Buttons buttons, Icons icon, AnimateStyle style, System.Windows.Window win)
        {
            win.ApplyEffect();

            var msgBox = new MsgBox { _lblMessage = { Text = message }, _lblTitle = { Text = title } };
            Size formSize = msgBox.MessageSize(message, title);
            msgBox.Size = formSize;
            msgBox.StartPosition = FormStartPosition.CenterParent;
            msgBox.Height = 0;

            msgBox.InitButtons(buttons);
            msgBox.InitIcon(icon);

            msgBox._timer = new Timer();
            switch (style)
            {
                case AnimateStyle.SlideDown:
                    msgBox.Size = new Size(formSize.Width, 0);
                    msgBox._timer.Interval = 1;
                    msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.FadeIn:
                    msgBox.Size = formSize;
                    msgBox.Opacity = 0;
                    msgBox._timer.Interval = 20;
                    msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                    break;

                case AnimateStyle.ZoomIn:
                    msgBox.Size = new Size(formSize.Width + 100, formSize.Height + 100);
                    msgBox._timer.Tag = new AnimateMsgBox(formSize, style);
                    msgBox._timer.Interval = 1;
                    break;
            }
            msgBox._timer.Tick += msgBox.timer_Tick;
            msgBox._timer.Start();

            ShowForm(msgBox);

            MessageBeep(0);

            win.ClearEffect();

            return msgBox._buttonResult;
        }

        #endregion

        #region Methods

        [DebuggerStepThrough]
        private void MsgBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastMousePos = new Point(e.X, e.Y);
            }
        }

        [DebuggerStepThrough]
        private void MsgBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - _lastMousePos.X;
                this.Top += e.Y - _lastMousePos.Y;
            }
        }

        [DebuggerStepThrough]
        private void ButtonClick(object sender, EventArgs e)
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

            this.Dispose();
        }

        [DebuggerStepThrough]
        private Size MessageSize(string message, string title)
        {
            var msgSize = GetTextSize(message, MessageFont);
            var titleSize = GetTextSize(title, TitleFont);

            var w = (int)(Math.Max(msgSize.Width, titleSize.Width) + BoxPadding.Left);
            var h = (int)(Math.Max(msgSize.Height, titleSize.Height) + BoxPadding.Bottom);

            var boxSize = new Size()
            {
                Width = MinWidth > w ? MinWidth : w,
                Height = MinHeight > h ? MinHeight : h
            };

            return boxSize;
        }

        private SizeF GetTextSize(string p, Font f)
        {
            // uncomment in vb.net:

            //var sb = new StringBuilder(p);
            //sb.Replace( vbCr, Environment.NewLine); 
            //sb.Replace(vbCrLf, Environment.NewLine); 
            //p = sb.ToString();
            //return _graphics.MeasureString(p, f);

            return _graphics.MeasureString(p, f);
        }

        [DebuggerStepThrough]
        private void timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            AnimateMsgBox animate = (AnimateMsgBox)timer.Tag;

            switch (animate.Style)
            {
                case AnimateStyle.SlideDown:
                    if (this.Height < animate.FormSize.Height)
                    {
                        this.Height += 17;
                        this.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case AnimateStyle.FadeIn:
                    if (this.Opacity < 1)
                    {
                        this.Opacity += 0.1;
                        this.Invalidate();
                    }
                    else
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;

                case AnimateStyle.ZoomIn:
                    if (this.Width > animate.FormSize.Width)
                    {
                        this.Width -= 17;
                        this.Invalidate();
                    }
                    if (this.Height > animate.FormSize.Height)
                    {
                        this.Height -= 17;
                        this.Invalidate();
                    }
                    if (this.Width <= animate.FormSize.Width && this.Height <= animate.FormSize.Height)
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    break;
            }
        }

        [DebuggerStepThrough]
        private void InitButtons(Buttons buttons)
        {
            switch (buttons)
            {
                case Buttons.AbortRetryIgnore:
                    this.InitAbortRetryIgnoreButtons();
                    break;

                case Buttons.OK:
                    this.InitOkButton();
                    break;

                case Buttons.OKCancel:
                    this.InitOkCancelButtons();
                    break;

                case Buttons.RetryCancel:
                    this.InitRetryCancelButtons();
                    break;

                case Buttons.YesNo:
                    this.InitYesNoButtons();
                    break;

                case Buttons.YesNoCancel:
                    this.InitYesNoCancelButtons();
                    break;
            }

            foreach (Button btn in this._buttonCollection)
            {
                btn.ForeColor = Color.FromArgb(170, 170, 170);
                btn.Font = new System.Drawing.Font("Segoe UI", 8);
                btn.Padding = new Padding(3);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Height = 30;
                btn.FlatAppearance.BorderColor = Color.FromArgb(99, 99, 98);

                this._flpButtons.Controls.Add(btn);
            }
        }

        [DebuggerStepThrough]
        private void InitIcon(Icons icon)
        {
            switch (icon)
            {
                case Icons.Application:
                    this._picIcon.Image = SystemIcons.Application.ToBitmap();
                    break;

                case Icons.Exclamation:
                    this._picIcon.Image = SystemIcons.Exclamation.ToBitmap();
                    break;

                case Icons.Error:
                    this._picIcon.Image = SystemIcons.Error.ToBitmap();
                    break;

                case Icons.Info:
                    this._picIcon.Image = SystemIcons.Information.ToBitmap();
                    break;

                case Icons.Question:
                    this._picIcon.Image = SystemIcons.Question.ToBitmap();
                    break;

                case Icons.Shield:
                    this._picIcon.Image = SystemIcons.Shield.ToBitmap();
                    break;

                case Icons.Warning:
                    this._picIcon.Image = SystemIcons.Warning.ToBitmap();
                    break;
            }
        }




        [DebuggerStepperBoundary]
        [DebuggerStepThrough]
        private static void ShowForm(MsgBox msgBox)
        {
            lock (SyncLocker)
            {
                if (msgBox.IsHandleCreated)
                {
                    msgBox.BeginInvoke(new Action(() => msgBox.ShowDialog()));
                }
            }
        }

        public Form GetParentForm()
        {
            Form parent = ActiveForm;

            if (parent != null) return parent;
            else if (Application.OpenForms.Count > 0)
            {
                parent = Application.OpenForms[0];
            }

            return parent;
        }

        /// <summary>
        /// Center Message Form To WinForm Parent
        /// </summary>
        public void CenterToParent(Size finalSize)
        {
            this.StartPosition = FormStartPosition.Manual;
            Point centerPos = new Point();

            var parent = GetParentForm();
            if (parent != null) // Center to Parent
            {
                var pW = parent.Size.Width;
                var pH = parent.Size.Height;
                var pX = parent.Location.X;
                var pY = parent.Location.Y;

                centerPos.X = pX + ((pW - finalSize.Width) / 2);
                centerPos.Y = pY + ((pH - finalSize.Height) / 2);
            }
            else // Center to Screen
            {
                this.StartPosition = FormStartPosition.CenterParent;
            }

            this.Location = centerPos;
        }

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

            // Draw Resize rectangle:
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }

        #endregion
    }
}
