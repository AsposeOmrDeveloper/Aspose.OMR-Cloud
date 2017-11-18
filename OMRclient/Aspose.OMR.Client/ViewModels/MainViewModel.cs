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
namespace Aspose.OMR.Client.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Controls;
    using UndoRedo;
    using Utility;
    using Views;

    /// <summary>
    /// View model for main view
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Collection of tab items
        /// </summary>
        private ObservableCollection<TabViewModel> tabViewModels;

        /// <summary>
        /// Currently selected tab
        /// </summary>
        private TabViewModel selectedTab;

        /// <summary>
        /// Flag indicating if busy indicator should be enabled
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// Indicates visibility of start panel with quick access commands
        /// </summary>
        private Visibility startPanelVisibility;

        /// <summary>
        /// Indicates if drop functionality is enabled
        /// </summary>
        private bool canDrop;

        /// <summary>
        /// Busy indication message
        /// </summary>
        private string busyIndicationMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            BusyIndicatorManager.EnabledChanged += this.BusyChanged;
            this.tabViewModels = new ObservableCollection<TabViewModel>();

            this.CanDrop = true;
            this.StartPanelVisibility = Visibility.Visible;

            this.InitCommands();
        }

        #region Commands

        public RelayCommand NewTemplateCommand { get; set; }

        public RelayCommand LoadTemplateCommand { get; set; }

        public RelayCommand DropTemplateCommand { get; set; }

        public RelayCommand SaveTemplateCommand { get; private set; }

        public RelayCommand StartRecognitionCommand { get; set; }

        public RelayCommand CloseTabCommand { get; set; }

        public RelayCommand UndoCommand { get; set; }

        public RelayCommand RedoCommand { get; set; }

        public RelayCommand LoadTemplateImageCommand { get; set; }

        public RelayCommand CopyCommand { get; set; }

        public RelayCommand PasteCommand { get; set; }

        public RelayCommand DeleteCommand { get; set; }

        public RelayCommand SelectAllCommand { get; set; }

        public RelayCommand ZoomInCommand { get; set; }

        public RelayCommand ZoomOutCommand { get; set; }

        public RelayCommand ZoomOriginalCommand { get; set; }

        public RelayCommand ExitCommand { get; set; }

        public RelayCommand ShowCredentialsSettingsCommand { get; set; }

        public RelayCommand ShowAboutCommand { get; set; }

        #endregion

        /// <summary>
        /// Gets the visibility of start panel
        /// </summary>
        public Visibility StartPanelVisibility
        {
            get { return this.startPanelVisibility; }
            private set
            {
                this.startPanelVisibility = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether busy indicator should be enabled
        /// </summary>
        public bool IsBusy
        {
            get { return this.isBusy; }
            private set
            {
                this.isBusy = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether recognition tab is available
        /// </summary>
        public bool IsRecognitionAvailable
        {
            get { return !(this.SelectedTab is ResultsViewModel); }
        }

        /// <summary>
        /// Gets or sets the selected tab
        /// </summary>
        public TabViewModel SelectedTab
        {
            get { return this.selectedTab; }
            set
            {
                // unselect old tab
                if (this.selectedTab != null)
                {
                    this.selectedTab.IsSelected = false;
                }

                // select new tab
                this.selectedTab = value;
                if (value != null)
                {
                    this.selectedTab.IsSelected = true;
                    this.StartPanelVisibility = Visibility.Collapsed;
                    this.CanDrop = false;
                }
                else
                {
                    this.StartPanelVisibility = Visibility.Visible;
                    this.CanDrop = true;
                }

                this.OnPropertyChanged();
                this.OnPropertyChanged("IsRecognitionAvailable");
            }
        }

        /// <summary>
        /// Gets a value indicating whether template finalization was complete with no warnings
        /// </summary>
        public bool FinalizationComplete
        {
            get
            {
                var templateViewModel = this.TabViewModels.FirstOrDefault(x => x is TemplateViewModel);
                if (templateViewModel != null)
                {
                    return ((TemplateViewModel) templateViewModel).FinalizationComplete;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the collection of TabViewModels
        /// </summary>
        public ObservableCollection<TabViewModel> TabViewModels
        {
            get { return this.tabViewModels; }
            private set
            {
                this.tabViewModels = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drop can be performed on main window
        /// </summary>
        public bool CanDrop
        {
            get { return this.canDrop; }
            set
            {
                this.canDrop = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets message displayed during busy indication
        /// </summary>
        public string BusyIndicationMessage
        {
            get { return this.busyIndicationMessage; }
            set
            {
                this.busyIndicationMessage = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Busy indicator manager busy state changed
        /// </summary>
        /// <param name="isEnabled">Indicates if busy state is enabled</param>
        /// <param name="message">Busy indication message</param>
        private void BusyChanged(bool isEnabled, string message)
        {
            this.IsBusy = isEnabled;
            this.BusyIndicationMessage = message;
        }

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitCommands()
        {
            this.NewTemplateCommand = new RelayCommand(x => this.OnNewTemplate());
            this.LoadTemplateCommand = new RelayCommand(x => this.OnLoadTemplate());
            this.DropTemplateCommand = new RelayCommand(x => this.LoadTemplateFromFile((string)x));
            this.SaveTemplateCommand = new RelayCommand(x => this.OnSaveTemplate(), x => this.CanSaveTemplate());

            this.StartRecognitionCommand = new RelayCommand(x => this.OnStartRecognition());

            this.CloseTabCommand = new RelayCommand(x => this.OnCloseTab());

            // shortcuts commands
            this.UndoCommand = new RelayCommand(x => this.OnUndoCommand(), x => this.CanUndo());
            this.RedoCommand = new RelayCommand(x => this.OnRedoCommand(), x => this.CanRedo());

            this.LoadTemplateImageCommand = new RelayCommand(x => this.LoadTemplateImage(), x => this.CanLoadTemplateImage());

            this.CopyCommand = new RelayCommand(x => this.OnCopyCommand(), x => this.CanCopy());
            this.PasteCommand = new RelayCommand(x => this.OnPasteCommand(x), x => this.CanPaste());
            this.DeleteCommand = new RelayCommand(x => this.OnDeleteCommand(), x => this.CanDelete());

            this.SelectAllCommand = new RelayCommand(x => this.OnSelectAllCommand(), x => this.CanSelectAll());
            this.ZoomInCommand = new RelayCommand(x => this.OnZoomInCommand());
            this.ZoomOutCommand = new RelayCommand(x => this.OnZoomOutCommand());
            this.ZoomOriginalCommand = new RelayCommand(x => this.OnZoomOriginalCommand());

            this.ShowCredentialsSettingsCommand = new RelayCommand(x => new CredentialsViewModel());
            this.ShowAboutCommand = new RelayCommand(x => new AboutView().ShowDialog());

            this.ExitCommand = new RelayCommand(x => this.Exit());
        }

        #region Shortcuts commands

        private void OnSaveTemplate()
        {
            ((TemplateViewModel) this.SelectedTab).SaveTemplateCommand.Execute(null);
        }

        private bool CanSaveTemplate()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.SaveTemplateCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void LoadTemplateImage()
        {
            ((TemplateViewModel) this.SelectedTab).LoadTemplateImageCommand.Execute(null);
        }

        private bool CanLoadTemplateImage()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.LoadTemplateImageCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void OnRedoCommand()
        {
            ((TemplateViewModel) this.SelectedTab).RedoCommand.Execute(null);
        }

        private bool CanRedo()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.RedoCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void OnUndoCommand()
        {
            ((TemplateViewModel) this.SelectedTab).UndoCommand.Execute(null);
        }

        private bool CanUndo()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.UndoCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void OnZoomOutCommand()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.ZoomOutCommand.CanExecute(null))
            {
                selectedTemplate.ZoomOutCommand.Execute(null);
            }
        }

        private void OnZoomInCommand()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.ZoomInCommand.CanExecute(null))
            {
                selectedTemplate.ZoomInCommand.Execute(null);
            }
        }

        private void OnZoomOriginalCommand()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.ZoomOriginalCommand.CanExecute(null))
            {
                selectedTemplate.ZoomOriginalCommand.Execute(null);
            }
        }

        private void OnSelectAllCommand()
        {
            ((TemplateViewModel) this.SelectedTab).SelectAllElementsCommand.Execute(null);
        }

        private bool CanSelectAll()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.SelectAllElementsCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void OnCopyCommand()
        {
            ((TemplateViewModel) this.SelectedTab).CopyElementsCommand.Execute(null);
        }

        private bool CanCopy()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.CopyElementsCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void OnPasteCommand(object control)
        {
            // find parent canvas
            UIElement uiItem = (UIElement)control;
            var canvas = uiItem.FindChild<CustomCanvas>(Properties.Resources.RootCanvasName);

            // get mouse position to use for paste questions
            Point mousePos = Mouse.GetPosition(canvas);

            ((TemplateViewModel) this.SelectedTab).PasteElementsCommand.Execute(mousePos);
        }

        private bool CanPaste()
        {
            var selectedTemplate = this.SelectedTab as TemplateViewModel;
            if (selectedTemplate != null && selectedTemplate.PasteElementsCommand.CanExecute(null))
            {
                return true;
            }

            return false;
        }

        private void OnDeleteCommand()
        {
            this.SelectedTab.RemoveElementCommand.Execute(null);
        }

        private bool CanDelete()
        {
            if (this.SelectedTab != null)
            {
                return this.SelectedTab.RemoveElementCommand.CanExecute(null);
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void Exit()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Loads template from disk
        /// </summary>
        private void OnLoadTemplate()
        {
            string templatePath = DialogManager.ShowOpenTemplateDialog();
            if (string.IsNullOrEmpty(templatePath))
            {
                return;
            }

            this.LoadTemplateFromFile(templatePath);
        }

        /// <summary>
        /// Loads template by specified path
        /// </summary>
        /// <param name="file">Path to template file</param>
        private void LoadTemplateFromFile(string file)
        {
            string jsonString = File.ReadAllText(file);
            TemplateViewModel tempalteViewModel = TemplateSerializer.JsonToTemplate(jsonString);

            this.TabViewModels.Add(tempalteViewModel);
            this.SelectedTab = tempalteViewModel;
        }

        /// <summary>
        /// Closes tab
        /// </summary>
        private void OnCloseTab()
        {
            this.TabViewModels.Remove(this.SelectedTab);
            if (this.TabViewModels.Count > 0)
            {
                this.SelectedTab = this.TabViewModels.Last();
            }
            else
            {
                this.SelectedTab = null;
                ActionTracker.ClearCommands();
            }
        }
        
        /// <summary>
        /// Create new template
        /// </summary>
        private void OnNewTemplate()
        {
            TemplateViewModel templateVm;

            if (this.TabViewModels.Any(x => x is TemplateViewModel))
            {
                templateVm = this.TabViewModels.First(x => x is TemplateViewModel) as TemplateViewModel;
                this.SelectedTab = templateVm;
                this.OnCloseTab();
            }

            string tabName = "Template";
            templateVm = new TemplateViewModel(tabName);
            this.TabViewModels.Add(templateVm);
            this.SelectedTab = templateVm;
        }

        /// <summary>
        /// Opens recognition view
        /// </summary>
        private void OnStartRecognition()
        {
            if (!this.FinalizationComplete)
            {
                string message = "There is no finalized template to work with!";
                MessageBox.Show(message, "Oops", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string tabName = "Recognition";
            string id = ((TemplateViewModel)this.TabViewModels.First(x => x is TemplateViewModel)).TemplateId;

            ResultsViewModel resultsVm = new ResultsViewModel(tabName, id);
            this.TabViewModels.Add(resultsVm);
            this.SelectedTab = resultsVm;
        }
    }
}
