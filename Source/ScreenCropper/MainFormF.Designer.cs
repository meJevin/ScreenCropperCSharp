﻿namespace ScreenCropper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.trayIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.launchOnStartupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeCombinationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCombinationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trayNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayIconContextMenu
            // 
            this.trayIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchOnStartupMenuItem,
            this.changeCombinationMenuItem,
            this.showCombinationMenuItem,
            this.restartToolStripMenuItem,
            this.quitMenuItem});
            this.trayIconContextMenu.Name = "trayIconContextMenu";
            this.trayIconContextMenu.Size = new System.Drawing.Size(187, 114);
            // 
            // launchOnStartupMenuItem
            // 
            this.launchOnStartupMenuItem.CheckOnClick = true;
            this.launchOnStartupMenuItem.Name = "launchOnStartupMenuItem";
            this.launchOnStartupMenuItem.Size = new System.Drawing.Size(186, 22);
            this.launchOnStartupMenuItem.Text = "Launch on startup";
            this.launchOnStartupMenuItem.Click += new System.EventHandler(this.LaunchOnStartupMenuItem_Click);
            // 
            // changeCombinationMenuItem
            // 
            this.changeCombinationMenuItem.Name = "changeCombinationMenuItem";
            this.changeCombinationMenuItem.Size = new System.Drawing.Size(186, 22);
            this.changeCombinationMenuItem.Text = "Change combination";
            this.changeCombinationMenuItem.Click += new System.EventHandler(this.ChangeCombinationMenuItem_Click);
            // 
            // showCombinationMenuItem
            // 
            this.showCombinationMenuItem.Name = "showCombinationMenuItem";
            this.showCombinationMenuItem.Size = new System.Drawing.Size(186, 22);
            this.showCombinationMenuItem.Text = "Show combination";
            this.showCombinationMenuItem.Click += new System.EventHandler(this.ShowCombinationMenuItem_Click);
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.RestartMenuItem_Click);
            // 
            // quitMenuItem
            // 
            this.quitMenuItem.Name = "quitMenuItem";
            this.quitMenuItem.Size = new System.Drawing.Size(186, 22);
            this.quitMenuItem.Text = "Exit";
            this.quitMenuItem.Click += new System.EventHandler(this.QuitMenuItem_Click);
            // 
            // trayNotifyIcon
            // 
            this.trayNotifyIcon.Text = "Screen Cropper";
            this.trayNotifyIcon.Visible = true;
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayIconContextMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Screen Cropper";
            this.trayIcon.Visible = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "MainForm";
            this.Opacity = 0.25D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Screen Cropper";
            this.TopMost = true;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMain_Paint);
            this.trayIconContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip trayIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem launchOnStartupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeCombinationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCombinationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitMenuItem;
        private System.Windows.Forms.NotifyIcon trayNotifyIcon;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
    }
}