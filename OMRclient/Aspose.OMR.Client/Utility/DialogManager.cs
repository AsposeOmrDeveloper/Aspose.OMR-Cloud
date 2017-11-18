/*
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
namespace Aspose.OMR.Client.Utility
{
    using System;
    using System.Windows;
    using System.Windows.Forms;
    using FileDialog = Microsoft.Win32.FileDialog;
    using MessageBox = System.Windows.MessageBox;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

    /// <summary>
    /// Manager to display dialogs to user
    /// </summary>
    public static class DialogManager
    {
        /// <summary>
        /// The filter string for the dialog that opens template images.
        /// </summary>
        private static readonly string ImageFilesFilterPrompt = "Image files |*.jpg; *.jpeg; *.png; *.gif; *.tif; *.tiff;";

        /// <summary>
        /// The filter string for the dialog that opens template files
        /// </summary>
        private static readonly string TemplateFilesFilterPrompt = "OMR Template Files (.omr)" + "| *.omr";

        /// <summary>
        /// The filter string for the dialog that saves recognition results
        /// </summary>
        private static readonly string DataExportFilesFilterPrompt = "Comma-Separated Values (*.csv)" + " | *.csv";

        /// <summary>
        /// Show folder selection dialog
        /// </summary>
        /// <returns>Path to selected folder</returns>
        public static string ShowFolderDialog()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = Environment.SpecialFolder.MyPictures.ToString();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Shows Open Image file dialog with multiple items select.
        /// </summary>
        /// <returns>Path to selected file, or <c>null</c> if no file was selected.</returns>
        public static string[] ShowOpenMultiselectDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            string[] fileName = null;

            dialog.Filter = ImageFilesFilterPrompt;
            dialog.RestoreDirectory = true;

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                fileName = dialog.FileNames;
            }

            return fileName;
        }

        /// <summary>
        /// Display error message box with provided message
        /// </summary>
        /// <param name="message">Error message</param>
        public static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, "Error occurred", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary>
        /// Display warning message box with provided message
        /// </summary>
        /// <param name="message">Error message</param>
        public static void ShowWarningDialog(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Display image size warning
        /// </summary>
        public static void ShowImageSizeWarning()
        {
            string message = "File is too small. We recommend using images at least 1200x1700 in size.";
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Shows Open Image file dialog.
        /// </summary>
        /// <returns>Path to selected file, or <c>null</c> if no file was selected.</returns>
        public static string ShowOpenImageDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            return ShowDialog(dialog, ImageFilesFilterPrompt);
        }

        /// <summary>
        /// Shows Open Template file dialog.
        /// </summary>
        /// <returns>Path to selected file, or <c>null</c> if no file was selected.</returns>
        public static string ShowOpenTemplateDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            return ShowDialog(dialog, TemplateFilesFilterPrompt);
        }

        /// <summary>
        /// Shows Save Template file dialog.
        /// </summary>
        /// <returns>Path to selected file, or <c>null</c> if no file was selected.</returns>
        public static string ShowSaveTemplateDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            return ShowDialog(dialog, TemplateFilesFilterPrompt);
        }

        /// <summary>
        /// Shows Save Template file dialog.
        /// </summary>
        /// <returns>Path to selected file, or <c>null</c> if no file was selected.</returns>
        public static string ShowSaveDataDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            return ShowDialog(dialog, DataExportFilesFilterPrompt);
        }

        /// <summary>
        /// Displayes recognition confirmation dialog
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <returns>Dialog result: true if yes, false otherwise</returns>
        public static bool ShowConfirmDialog(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Displays given dialog and returns its result as a <c>string</c>.
        /// </summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="filter">File type filter string.</param>
        /// <returns>Path to selected file, or <c>null</c> if no file was selected.</returns>
        private static string ShowDialog(FileDialog dialog, string filter)
        {
            string fileName = null;

            dialog.Filter = filter;
            dialog.RestoreDirectory = true;

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }
    }
}
