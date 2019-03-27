﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ScreenCropper
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            InitializeScreenCropper();

            MouseHookProcedure = MouseHookCallback;
            MouseHookID = WinAPIHelper.SetGlobalMouseHook(MouseHookProcedure);

            KeyboardHookProcedure = KeyboardHookCallback;
            KeyboardHookID = WinAPIHelper.SetGlobalKeyboardHook(KeyboardHookProcedure);

            // Get device contexts for out screen
            screenDC = GetDC(IntPtr.Zero);
            screenCompatibleDC = CreateCompatibleDC(screenDC);

            // This clears the BG initially
            WinAPIHelper.DrawWindowRectangle(SystemInformation.VirtualScreen, this.Handle);
        }

        ~frmMain()
        {
            // Release the hooks
            WinAPIHelper.UnhookWindowsHookEx(MouseHookID);
            WinAPIHelper.UnhookWindowsHookEx(KeyboardHookID);

            // And the screen device context
            ReleaseDC(IntPtr.Zero, screenDC);
        }

        #region Private Variables

        // Current key combination that activated Screen Cropper
        private List<Keys> CurrentCombination = new List<Keys>() { Keys.LControlKey, Keys.LMenu, Keys.C };

        // This pointer is required to perform unhooking cleanup
        private IntPtr KeyboardHookID = IntPtr.Zero;
        private static LowLevelHookProcedure KeyboardHookProcedure;

        private IntPtr MouseHookID = IntPtr.Zero;
        private static LowLevelHookProcedure MouseHookProcedure;

        // Used for selection drawing
        private Point selectionStartPoint = new Point();
        private Point lastCursorPosition = new Point();

        private Pen selectionRectangleBorderPen = new Pen(new SolidBrush(Color.Black), 1.5f);

        private bool isTakingScreenshot = false;
        private bool overlayVisible = false;

        private IntPtr screenDC;
        private IntPtr screenCompatibleDC;

        #endregion

        #region Privates Methods

        #region Hook callbacks
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(MouseHookID, nCode, wParam, lParam);
            }

            MouseMessages mouseMessage = (MouseMessages)wParam;

            LowLevelMouseHookStructure mouseInfo = (LowLevelMouseHookStructure)Marshal.PtrToStructure(lParam, typeof(LowLevelMouseHookStructure));

            if (mouseMessage == MouseMessages.WM_LBUTTONDOWN)
            {
                if (!isTakingScreenshot && overlayVisible)
                {
                    StartTakingScreenshot(new Point(mouseInfo.point.x, mouseInfo.point.y));
                }
            }
            else if (mouseMessage == MouseMessages.WM_LBUTTONUP)
            {
                Console.WriteLine(DateTime.Now.ToBinary() + ": Mouse event");
                if (isTakingScreenshot)
                {
                    StopTakingScreenshot();

                    CopySelectedAreaToClipBoard();
                }
            }
            else if (mouseMessage == MouseMessages.WM_MOUSEMOVE)
            {
                if (isTakingScreenshot)
                {
                    HandleScreenshotSelectionChange(new Point(mouseInfo.point.x, mouseInfo.point.y));
                    Invalidate();
                }
            }

            return WinAPIHelper.CallNextHookEx(MouseHookID, nCode, wParam, lParam);
        }

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
            }

            if (Keyboard.IsKeyDown(Key.Escape))
            {
                StopTakingScreenshot();
            }
            else if (IsCombinationPressed())
            {
                ShowScreenshotOverlay();
            }

            return WinAPIHelper.CallNextHookEx(KeyboardHookID, nCode, wParam, lParam);
        }
        #endregion

        #region Screenshot Related

        /// <summary>
        /// Black overlay which is a system-on-top fullscreen form
        /// </summary>
        private void ShowScreenshotOverlay()
        {
            if (this.Size != SystemInformation.VirtualScreen.Size)
            {
                this.Location = new System.Drawing.Point(0, 0);
                this.Size = SystemInformation.VirtualScreen.Size;
                this.Bounds = new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
            this.Opacity = 0.5;
            Visible = true;
            overlayVisible = true;

            WinAPIHelper.DrawWindowRectangle(SystemInformation.VirtualScreen, this.Handle);
        }

        private void StartTakingScreenshot(Point currentMousePosition)
        {
            selectionStartPoint = currentMousePosition;
            isTakingScreenshot = true;
        }

        private void StopTakingScreenshot()
        {
            this.Opacity = 0.25;
            isTakingScreenshot = false;
            Visible = false;
            overlayVisible = false;
            BackColor = Color.Black;
        }

        /// <summary>
        /// Occurs when user moves the mouse while selecting the desired area of screenshot
        /// </summary>
        /// <param name="currentMousePosition">New mouse position</param>
        private void HandleScreenshotSelectionChange(Point currentMousePosition)
        {
            overlayVisible = false;

            if (BackColor != Color.White)
            {
                BackColor = Color.White;
            }

            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, currentMousePosition);

            // Look at the defenition for more details
            WinAPIHelper.DrawWindowRectangle(selectionRect, this.Handle);

            lastCursorPosition = currentMousePosition;
        }

        private void CopySelectedAreaToClipBoard()
        {
            Rectangle selectionRect = Utils.RectangleFromTwoPoints(selectionStartPoint, lastCursorPosition);

            IntPtr screenBitmap = CreateCompatibleBitmap(screenDC, selectionRect.Width, selectionRect.Height);
            IntPtr tempScreenBitmap = SelectObject(screenCompatibleDC, screenBitmap);

            BitBlt(screenCompatibleDC, 0, 0, selectionRect.Width, selectionRect.Height, screenDC, selectionRect.X, selectionRect.Y, TernaryRasterOperations.SRCCOPY);

            screenBitmap = SelectObject(screenCompatibleDC, tempScreenBitmap);

            OpenClipboard(IntPtr.Zero);
            EmptyClipboard();
            SetClipboardData(ClipFormat.CF_BITMAP, screenBitmap);
            CloseClipboard();
        }

        #endregion

        #region Combination Related

        private bool IsCombinationPressed()
        {
            bool combinationPressed = true;
            foreach (var key in CurrentCombination)
            {
                if (!Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)key)))
                {
                    combinationPressed = false;
                    break;
                }
            }

            return combinationPressed;
        }

        private void ShowCurrentCombinationInTrayIcon()
        {
            if (trayIcon.Icon == null)
            {
                trayIcon.Icon = new Icon("ScreenCropper.ico");
            }

            trayIcon.BalloonTipTitle = "Your combination";
            trayIcon.BalloonTipText = Utils.GetCombinationString(CurrentCombination);
            trayIcon.Visible = true;
            trayIcon.ShowBalloonTip(3000);
        }

        #endregion

        #region Application Related

        /// <summary>
        /// The combination is stored in the registry
        /// </summary>
        private void LoadCombinationFromRegistry()
        {
            RegistryKey screenCropperKey = OpenScreenCropperRegKey();

            var loadedCombinationValue = screenCropperKey.GetValue("ActivationCombination");

            if (loadedCombinationValue != null)
            {
                // Succesful load of combination
                string loadedCombinationString = loadedCombinationValue as string;
                loadedCombinationString = Utils.NullTerminate(loadedCombinationString);

                CurrentCombination = Utils.ParseCombination(loadedCombinationString);
            }
            else
            {
                // It has disappeared (??), let's create it again and default it out
                screenCropperKey.SetValue("ActivationCombination", Utils.SerializeCombination(CurrentCombination));
            }
        }

        private void CheckStartup()
        {
            var windowsStartupAppsKey = OpenWindowsStartupAppsKey();
            var screenCropperStartupValue = windowsStartupAppsKey.GetValue("ScreenCropper");

            if (screenCropperStartupValue == null)
            {
                // We're not registered in the windows startup, let's ask the user whether he wants the application to be launched at startup

                DialogResult result = MessageBox.Show("Do you want Screen Cropper to be launch at startup?", "Screen Cropper", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    windowsStartupAppsKey.SetValue("ScreenCropper", Application.ExecutablePath);
                    launchOnStartupMenuItem.Checked = true;
                }
                else
                {
                    windowsStartupAppsKey.SetValue("ScreenCropper", "None");
                    launchOnStartupMenuItem.Checked = false;
                }
            }
            else
            {
                // Initizize from current settings
                string loadedStartupPath = screenCropperStartupValue as string;
                loadedStartupPath = Utils.NullTerminate(loadedStartupPath);

                // Make sure the value in that Key is equal to the current .exe ("None means that the user has chosen not to launch the application at startup)
                if (loadedStartupPath == "None")
                {
                    launchOnStartupMenuItem.Checked = false;
                }
                else
                {
                    if (Application.ExecutablePath != loadedStartupPath)
                    {
                        windowsStartupAppsKey.SetValue("ScreenCropper", Application.ExecutablePath);
                    }

                    launchOnStartupMenuItem.Checked = true;
                }
            }
        }

        /// <summary>
        /// Checks regestry, initializes combination, startup etc.
        /// </summary>
        private void InitializeScreenCropper()
        {
            var screenCropperKey = OpenScreenCropperRegKey();

            if (screenCropperKey == null)
            {
                // First launch
                screenCropperKey = Registry.CurrentUser.CreateSubKey(@"Software\ScreenCropper", true);

                screenCropperKey.SetValue("ActivationCombination", Utils.SerializeCombination(CurrentCombination));

                CheckStartup();
            }
            else
            {
                // Initizize current settings

                LoadCombinationFromRegistry();

                CheckStartup();
            }
        }

        private RegistryKey OpenScreenCropperRegKey()
        {
            var currentUserRegKey = Registry.CurrentUser;

            return currentUserRegKey.OpenSubKey(@"Software\ScreenCropper", true); ;
        }

        private RegistryKey OpenWindowsStartupAppsKey()
        {
            var currentUserRegKey = Registry.CurrentUser;

            return currentUserRegKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        #endregion

        #region Form events
        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            // For some reason the clip rectangle is not the same as the one that I use to draw a specified region of this form
            // it's one pixel wider and higher, so we have to create a new one
            e.Graphics.DrawRectangle(selectionRectangleBorderPen, new Rectangle(e.ClipRectangle.Location, new Size(e.ClipRectangle.Width - 1, e.ClipRectangle.Height - 1)));
        }
        #endregion

        #endregion

        #region DLL Imports
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetClipboardData(ClipFormat uFormat, IntPtr hMem);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("user32.dll")]
        static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject", SetLastError = true)]
        static public extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        #endregion

        #region Tray Icon Context Menu Item Click Events

        private void quitMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit Screen Cropper?", "Qutting Screen Cropper", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        private void launchOnStartupMenuItem_Click(object sender, EventArgs e)
        {
            var windowsStartupAppsKey = OpenWindowsStartupAppsKey();

            if (launchOnStartupMenuItem.Checked)
            {
                windowsStartupAppsKey.SetValue("ScreenCropper", Application.ExecutablePath);
            }
            else
            {
                windowsStartupAppsKey.SetValue("ScreenCropper", "None");
            }
        }

        private void showCombinationMenuItem_Click(object sender, EventArgs e)
        {
            ShowCurrentCombinationInTrayIcon();
        }

        #endregion

    }
}
