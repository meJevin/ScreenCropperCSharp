﻿/*

Screen Cropper Project - A lightweight screenshot taking program
Copyright (C) 2019 MICHAEL NAIFIELD

*/

using Squirrel;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCropper
{
    static class Program
    {
        private static MainForm MainForm = new MainForm();

        private static Task UpdateTask = new Task(CheckForUpdates);

        public static UpdateManager UpdManager;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledExpection);
            AppDomain.CurrentDomain.ProcessExit += OnExit;

            if (Utils.IsAlreadyRunning())
            {
                MessageBox.Show("Screen Cropper is already running!");
                return;
            }

            Task.Run(() => InitUpdateManager()).Wait();

            UpdateTask.Start();

            Application.EnableVisualStyles();

            Application.Run();
        }

        public static async Task InitUpdateManager()
        {
            try
            {
                UpdManager = await UpdateManager.GitHubUpdateManager(@"https://github.com/meJevin/ScreenCropperCSharp");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize update manager!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static async void CheckForUpdates()
        {
            try
            {
                var updateInfo = await UpdManager.CheckForUpdate();

                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    var releaseEntry = await UpdManager.UpdateApp();
                    
                    if (releaseEntry != null)
                    {
                        MessageBox.Show($"Screen Cropper version {releaseEntry.Version.ToString()} has been downloaded!" +
                            $"\nUpdates will take effect after restrat!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not update app!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnExit(object sender, EventArgs e)
        {
            UpdManager?.Dispose();
        }

        static void OnUnhandledExpection(object sender, UnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.ExceptionObject.ToString(), "Screen Cropper error!");
        }
    }
}
