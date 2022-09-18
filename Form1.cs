using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Drawing;
using Microsoft.Win32;
using Gma.System.MouseKeyHook;
using CustomTouckKeyboard.actions;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace CustomTouckKeyboard
{
    public partial class Form1 : Form
    {
        static string TabTipFilePath = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
      
        Timer t;
        Process x;

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
         [DllImport("user32.dll", EntryPoint = "SetParent")]
        static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_MAXIMIZE = 0xF030;
        bool isExit = false;
        void embedTool()
        {
            //Process p = Process.Start("Notepad");
            //p.WaitForInputIdle();
            //SetParent(p.MainWindowHandle, panel2.Handle);
            //SendMessage(p.MainWindowHandle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);

        }
        public Form1()
        {
            // = false;
            this.components = new System.ComponentModel.Container();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();

            // Initialize contextMenu1
            this.contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.menuItem1 ,this.menuItem2});

            // Initialize menuItem1
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            this.menuItem2.Index = 1;
            this.menuItem2.Text = "Startup";
            this.menuItem2.Click += MenuItem1_Click;
            this.menuItem2.Checked = isStartup();
            // Set up how the form should be displayed.
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.Visible = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Text = "TouchKey";
            this.FormBorderStyle = FormBorderStyle.None;
            // Create the NotifyIcon.
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.Shown += Form1_Shown;
            this.FormClosing += Form1_FormClosing;
            // The Icon property sets the icon that will appear
            // in the systray for this application.
            notifyIcon1.Icon = new Icon("coding.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon1.ContextMenu = this.contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "Pin keyboard";
            notifyIcon1.Visible = true;

            // Handle the DoubleClick event to activate the form.
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            //InitializeComponent();

            //Process[] keys = Process.GetProcessesByName("TabTip");
            //SetParent(keys[0].MainWindowHandle, this.Handle);
            // Open();
            Process.Start(TabTipFilePath);
            Thread.Sleep(200);
            inputPane = (IFrameworkInputPane)new FrameworkInputPane();
            inputPane.Location(out rect);
            t  = new Timer();
            t.Interval = 2000;
            t.Tick += T_Tick;
            t.Start();
            //Hook.GlobalEvents().MouseDown += MouseClickAll;
            //MouseHook.Start();
            //MouseHook.MouseAction += MouseHook_MouseAction;
            //Touch.FrameReported += Touch_FrameReported;

        }
        IFrameworkInputPane inputPane;
        Rectangle rect;
        private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            Console.WriteLine("Touched");
        }

        private void MouseHook_MouseAction(object sender, EventArgs e)
        {
            //trackingInfo.userActivityPerMinute.incrementMouseClick();
            Console.WriteLine(CursorControl.GetCursorState());
            string cursorState = CursorControl.GetCursorState();

            if (cursorState == "65541" || cursorState == "6948113")
            {
               // Console.WriteLine("Mouse Click");
                Open();

            }
            
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        int changed = 0;
        private void MouseClickAll(object sender, MouseEventArgs e)
        {
            Console.WriteLine(CursorControl.GetCursorState());
            if (CursorControl.GetCursorState() == "65541")
            {
                Open();
            }
            
        }
        bool isHidden = false;
        
        private void T_Tick(object sender, EventArgs e)
        {
            try
            {
                if(!isHidden)
                foreach (var x in Process.GetProcessesByName("TabTip"))
                {
                    x.Kill();
                }
                //t.Interval = 5000; Process.Start(TabTipFilePath);





                inputPane.Location(out rect);

                Console.WriteLine((rect.Width == 0 && rect.Height == 0) ? "Keyboard not visible " : "Keyboard visible");
                Console.WriteLine(isHidden + " " + Process.GetProcessesByName("TabTip").Length);
                
                if (rect.Width == 0 && !isHidden)
                {
                    isHidden = true;
                    Process.Start(TabTipFilePath);
                    Thread.Sleep(200);
                    var uiHostNoLaunch = new UIHostNoLaunch();
                    var tipInvocation = (ITipInvocation)uiHostNoLaunch;
                    tipInvocation.Toggle(GetDesktopWindow());
                    Marshal.ReleaseComObject(uiHostNoLaunch);
                    
                }else if(rect.Width != 0)
                    isHidden = false;
            }
            catch (Exception ex)
            {

            }

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void MenuItem1_Click(object sender, EventArgs e)
        {
            
            ToggleStartup();

        }

        int SC_RESTORE = 0xF120;
        bool checkup = false;
        private void ToggleStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!isStartup())
                rk.SetValue("TouchKey", Application.ExecutablePath);
            else
                rk.DeleteValue("TouchKey", false);
            this.menuItem2.Checked = isStartup();
        }

        private bool isStartup ()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
               ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return rk.GetValue("TouchKey") != null;

        }
        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            // Activate the form.
            this.Activate();
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            notifyIcon1.Icon.Dispose();

            notifyIcon1.Dispose();
            this.Close();
        }
        
       

        public void Open()
        {
            try
            {
                Process.Start(TabTipFilePath);
                Thread.Sleep(200);
                var inputPane = (IFrameworkInputPane)new FrameworkInputPane();
                inputPane.Location(out var rect);

                // Console.WriteLine((rect.Width == 0 && rect.Height == 0) ? "Keyboard not visible" : "Keyboard visible");
                if (rect.Width == 0 )
                {
                    var uiHostNoLaunch = new UIHostNoLaunch();
                    var tipInvocation = (ITipInvocation)uiHostNoLaunch;
                    tipInvocation.Toggle(GetDesktopWindow());
                    Marshal.ReleaseComObject(uiHostNoLaunch);
                }
               
            }
            catch (Exception ex)
            {

            }

            
        }

        public void show()
        {

        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);


        private void X_Exited(object sender, EventArgs e)
        {
            //Console.WriteLine("sa");
        }

        [ComImport, Guid("4ce576fa-83dc-4F88-951c-9d0782b4e376")]      
        class UIHostNoLaunch
        {
        }

        [ComImport, Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ITipInvocation
        {
            void Toggle(IntPtr hwnd);
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();


        ////////////////////////////
        ///
        [ComImport, Guid("D5120AA3-46BA-44C5-822D-CA8092C1FC72")]
        public class FrameworkInputPane
        {
        }

        [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("5752238B-24F0-495A-82F1-2FD593056796")]
        public interface IFrameworkInputPane
        {
            [PreserveSig]
            int Advise(
                [MarshalAs(UnmanagedType.IUnknown)] object pWindow,
                [MarshalAs(UnmanagedType.IUnknown)] object pHandler,
                out int pdwCookie
                );

            [PreserveSig]
            int AdviseWithHWND(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.IUnknown)] object pHandler,
                out int pdwCookie
                );

            [PreserveSig]
            int Unadvise(
                int pdwCookie
                );

            [PreserveSig]
            int Location(
                out Rectangle prcInputPaneScreenLocation
                );
        }
        bool attrib = true;

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

        [Flags]
        public enum DwmWindowAttribute : uint
        {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        }
    }


    [ComImport]
    [Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        internal static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern int GetWindowLong(int hWnd, int nIndex);

        internal const int GWL_STYLE = -16;
        internal const int GWL_EXSTYLE = -20;
        internal const int WM_SYSCOMMAND = 0x0112;
        internal const int SC_CLOSE = 0xF060;

        internal const int WS_DISABLED = 0x08000000;

        internal const int WS_VISIBLE = 0x10000000;

    }
}
