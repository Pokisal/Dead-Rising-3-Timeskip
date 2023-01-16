using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DR_RTM
{
    public class MyLabel : Label
    {
        public static Label Set(string Text = "", Font Font = null, Color ForeColor = new Color(), Color BackColor = new Color())
        {
            Label l = new Label();
            l.Text = Text;
            l.Font = SystemFonts.MessageBoxFont;
            l.ForeColor = (ForeColor == new Color()) ? Color.White : ForeColor;
            l.BackColor = (BackColor == new Color()) ? Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32))))) : BackColor;
            l.AutoSize = true;
            return l;
        }
    }
    public class MyButton : Button
    {
        public static Button Set(string Text = "", int Width = 102, int Height = 30, Font Font = null, Color ForeColor = new Color(), Color BackColor = new Color())
        {
            Button b = new Button();
            b.Text = Text;
            b.Width = Width;
            b.Height = Height;
            b.Font = (Font == null) ? new Font("Calibri", 12) : Font;
            b.ForeColor = (ForeColor == new Color()) ? Color.White : ForeColor;
            b.BackColor = (BackColor == new Color()) ? Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32))))) : BackColor;
            b.UseVisualStyleBackColor = (b.BackColor == SystemColors.Control);
            return b;
        }
    }
    public partial class MyMessageBox : Form
    {
        private MyMessageBox()
        {
            this.panText = new FlowLayoutPanel();
            this.panButtons = new FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // panText
            // 
            this.panText.Parent = this;
            this.panText.AutoScroll = true;
            this.panText.AutoSize = true;
            this.panText.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panText.Location = new Point(90, 90);
            this.panText.Margin = new Padding(0);
            this.panText.MaximumSize = new Size(500, 300);
            this.panText.MinimumSize = new Size(120, 50);
            this.panText.Size = new Size(108, 50);
            // 
            // panButtons
            // 
            this.panButtons.AutoSize = true;
            this.panButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.panButtons.FlowDirection = FlowDirection.RightToLeft;
            this.panButtons.Location = new Point(89, 89);
            this.panButtons.Margin = new Padding(0);
            this.panButtons.MaximumSize = new Size(580, 150);
            this.panButtons.MinimumSize = new Size(108, 0);
            this.panButtons.Size = new Size(108, 35);
            // 
            // MyMessageBox
            //
            this.BackColor = Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.AutoScaleDimensions = new SizeF(8F, 19F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(206, 133);
            this.Controls.Add(this.panButtons);
            this.Controls.Add(this.panText);
            this.Font = SystemFonts.MessageBoxFont;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Margin = new Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(168, 132);
            this.Name = "MyMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        public static string Show(Label Label, string Title = "", List<Button> Buttons = null, PictureBox Image = null)
        {
            List<Label> Labels = new List<Label>();
            Labels.Add(Label);
            return Show(Labels, Title, Buttons, Image);
        }
        public static string Show(string Label, string Title = "", List<Button> Buttons = null, PictureBox Image = null)
        {
            List<Label> Labels = new List<Label>();
            Labels.Add(MyLabel.Set(Label));
            return Show(Labels, Title, Buttons, Image);
        }
        public static string Show(List<Label> Labels = null, string Title = "", List<Button> Buttons = null, PictureBox Image = null)
        {
            if (Labels == null) Labels = new List<Label>();
            if (Labels.Count == 0) Labels.Add(MyLabel.Set(""));
            if (Buttons == null) Buttons = new List<Button>();
            if (Buttons.Count == 0) Buttons.Add(MyButton.Set("OK"));
            List<Button> buttons = new List<Button>(Buttons);
            buttons.Reverse();

            int ImageWidth = 0;
            int ImageHeight = 0;
            int LabelWidth = 0;
            int LabelHeight = 0;
            int ButtonWidth = 0;
            int ButtonHeight = 0;
            int TotalWidth = 0;
            int TotalHeight = 0;

            MyMessageBox mb = new MyMessageBox();

            mb.Text = Title;

            //Labels
            List<int> il = new List<int>();
            mb.panText.Location = new Point(9 + ImageWidth, 9);
            foreach (Label l in Labels)
            {
                mb.panText.Controls.Add(l);
                l.Location = new Point(200, 50);
                l.MaximumSize = new Size(480, 2000);
                il.Add(l.Width);
            }
            int mw = Labels.Max(x => x.Width);
            il.ToString();
            Labels.ForEach(l => l.MinimumSize = new Size(Labels.Max(x => x.Width), 1));
            mb.panText.Height = Labels.Sum(l => l.Height);
            mb.panText.MinimumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), ImageHeight);
            mb.panText.MaximumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), 300);
            LabelWidth = mb.panText.Width;
            LabelHeight = mb.panText.Height;

            //Buttons
            foreach (Button b in buttons)
            {
                mb.panButtons.Controls.Add(b);
                b.Location = new Point(3, 3);
                b.TabIndex = Buttons.FindIndex(i => i.Text == b.Text);
                b.Click += new EventHandler(mb.Button_Click);
            }
            ButtonWidth = mb.panButtons.Width;
            ButtonHeight = mb.panButtons.Height;

            //Set Widths
            if (ButtonWidth > ImageWidth + LabelWidth)
            {
                Labels.ForEach(l => l.MinimumSize = new Size(ButtonWidth - ImageWidth - mb.ScrollBarWidth(Labels), 1));
                mb.panText.Height = Labels.Sum(l => l.Height);
                mb.panText.MinimumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), ImageHeight);
                mb.panText.MaximumSize = new Size(Labels.Max(x => x.Width) + mb.ScrollBarWidth(Labels), 300);
                LabelWidth = mb.panText.Width;
                LabelHeight = mb.panText.Height;
            }
            TotalWidth = ImageWidth + LabelWidth;

            //Set Height
            TotalHeight = LabelHeight + ButtonHeight;

            mb.panButtons.Location = new Point(TotalWidth - ButtonWidth + 9, mb.panText.Location.Y + mb.panText.Height);

            mb.Size = new Size(TotalWidth + 25, TotalHeight + 47);
            mb.ShowDialog();
            return mb.Result;
        }

        private FlowLayoutPanel panText;
        private FlowLayoutPanel panButtons;
        private int ScrollBarWidth(List<Label> Labels)
        {
            return (Labels.Sum(l => l.Height) > 300) ? 23 : 6;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Result = ((Button)sender).Text;
            Close();
        }

        private string Result = "";
    }

    public class Form1 : Form
    {
        private delegate void TimeDisplayUpdateCallback(string text);

        private delegate void SplashTextUpdateCallback(string text);

        public static bool skipFlag;

        public static string PIDText;

        private Label PID;

        private Label label2;

        private Label timeDisplay;

        private Label label3;

        private Button button1;

        private RadioButton radioButton1;

        private RadioButton radiobutton2;

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private unsafe static extern bool WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, int nSize, void* lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        private static extern int CloseHandle(IntPtr hObject);

        public static IntPtr MemoryOpen(int ProcessID)
        {
            return OpenProcess(2035711u, bInheritHandle: false, ProcessID);
        }

        public unsafe void Write(uint mAddress, byte[] Buffer, int ProcessID)
        {
            if (!(MemoryOpen(ProcessID) == (IntPtr)0))
            {
                WriteProcessMemory(MemoryOpen(ProcessID), mAddress, Buffer, Buffer.Length, null);
            }
        }

        public void SplashTextUpdate(string text)
        {
            if (label3.InvokeRequired)
            {
                SplashTextUpdateCallback method = SplashTextUpdate;
                try
                {
                    Invoke(method, text);
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
            }
            label3.Text = text;
        }

        public void TimeDisplayUpdate(string text)
        {
            if (timeDisplay.InvokeRequired)
            {
                TimeDisplayUpdateCallback method = TimeDisplayUpdate;
                try
                {
                    Invoke(method, text);
                    return;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
            }
            timeDisplay.Text = text;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            AllSkips.UpdateTimer.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PID.Text = string.Empty;
        }

        public void disable_Radios(bool flag)
        {
            if (flag)
            {
                radioButton1.Enabled = false;
                radiobutton2.Enabled = false;
            }
            if (!flag)
            {
                radioButton1.Enabled = true;
                radiobutton2.Enabled = true;
            }
        }
        public void button1_Click(object sender, EventArgs e)
        {
            Process[] processesByName = Process.GetProcessesByName("deadrising3");
            if (processesByName.Length == 0)
            {
                MyMessageBox.Show("The game process was not detected!\n\nPlease open the game.\r\n ", "Error");
                return;
            }
            AllSkips.GameProcess = processesByName[0];
            PID.Text = string.Format("Connected to PID {0}", processesByName[0].Id.ToString("X8"));
            AllSkips.UpdateTimer.Elapsed += AllSkips.UpdateEvent;
            AllSkips.UpdateTimer.AutoReset = true;
            AllSkips.UpdateTimer.Enabled = true;
            AllSkips.form = this;
        }
        public void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            AllSkips.skipMode = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            AllSkips.skipMode = 1;
        }

        private void InitializeComponent()
        {
            this.PID = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.timeDisplay = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radiobutton2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // PID
            // 
            this.PID.AutoSize = true;
            this.PID.ForeColor = System.Drawing.Color.White;
            this.PID.Location = new System.Drawing.Point(81, 5);
            this.PID.Name = "PID";
            this.PID.Size = new System.Drawing.Size(0, 13);
            this.PID.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "The current time is:";
            // 
            // timeDisplay
            // 
            this.timeDisplay.AutoSize = true;
            this.timeDisplay.ForeColor = System.Drawing.Color.White;
            this.timeDisplay.Location = new System.Drawing.Point(12, 39);
            this.timeDisplay.Name = "timeDisplay";
            this.timeDisplay.Size = new System.Drawing.Size(53, 13);
            this.timeDisplay.TabIndex = 5;
            this.timeDisplay.Text = "<missing>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 223);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 6;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radioButton1.Checked = true;
            this.radioButton1.ForeColor = System.Drawing.Color.White;
            this.radioButton1.Location = new System.Drawing.Point(15, 72);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(69, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "TimeSkip";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radiobutton2
            // 
            this.radiobutton2.AutoSize = true;
            this.radiobutton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.radiobutton2.ForeColor = System.Drawing.Color.White;
            this.radiobutton2.Location = new System.Drawing.Point(15, 95);
            this.radiobutton2.Name = "radiobutton2";
            this.radiobutton2.Size = new System.Drawing.Size(45, 17);
            this.radiobutton2.TabIndex = 11;
            this.radiobutton2.TabStop = true;
            this.radiobutton2.Text = "AllBosses";
            this.radiobutton2.UseVisualStyleBackColor = true;
            this.radiobutton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(241, 126);
            this.Controls.Add(this.radiobutton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.timeDisplay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PID);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "DR3 Timeskip";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}