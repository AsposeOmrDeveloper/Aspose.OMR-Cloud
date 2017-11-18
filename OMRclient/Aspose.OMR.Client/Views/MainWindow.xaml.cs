﻿/*
 * Copyright (c) 2017 Aspose Pty Ltd. All Rights Reserved.
 *
 * Licensed under the MIT (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       https://github.com/aspose-omr/Aspose.OMR-for-Cloud/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Aspose.OMR.Client.Views
{
    using System.Windows;
    using ViewModels;
    using Utility;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.OnLoaded;
            this.Closing += this.OnWindowClosing;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int filesCount = CloudStorageManager.UploadedFilesCount;

            if (filesCount > 0)
            {
                if (DialogManager.ShowConfirmDialog("During Your session " + filesCount +
                                                    " files were uploaded for recognition to Aspose.Cloud Storage. Would like to delete them?"))
                {
                    try
                    {
                        CloudStorageManager.CleanUpStorage();
                        e.Cancel = true;
                    }
                    catch
                    {
                        DialogManager.ShowErrorDialog("An error occured while attempting to delete files from storage. Please visit Cloud Dashboard to manage storage files.");
                    }
                }
            }
        }

        /// <summary>
        /// Fires when main window is loaded
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="routedEventArgs">The event args</param>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // check if keys were loaded
            if (!CoreApi.GotKeys)
            {
                // if not, display credentials dialog
                MainViewModel context = this.DataContext as MainViewModel;
                context.ShowCredentialsSettingsCommand.Execute(null);
            }
        }

        /// <summary>
        /// Handle template file drop
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event args</param>
        private void OnTemplateDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            var context = this.DataContext as MainViewModel;

            if (context == null || files == null || files.Length < 1)
            {
                return;
            }

            // take first string (if several files were dropped, choose one anyway)
            string file = files[0];

            // get format
            string format = System.IO.Path.GetExtension(file);

            if (format != null && format.ToLower().Equals(".omr"))
            {
                context.DropTemplateCommand.Execute(file);
            }

            // bring client window on top
            GetWindow(this).Activate();
        }
    }
}
