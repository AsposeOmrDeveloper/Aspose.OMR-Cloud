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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using TemplateModel;
    using UndoRedo;
    using Utility;

    /// <summary>
    /// View model for the template view
    /// </summary>
    public class TemplateViewModel : TabViewModel
    {
        #region Fields
        
        /// <summary>
        /// Buffer to store copied elements
        /// </summary>
        private readonly ObservableCollection<BaseQuestionViewModel> copiedQuestionsBuffer;

        /// <summary>
        /// Current page elements
        /// </summary>
        private ObservableCollection<BaseQuestionViewModel> pageQuestions;

        /// <summary>
        /// Selected elements
        /// </summary>
        private ObservableCollection<BaseQuestionViewModel> selectedElements;

        /// <summary>
        /// List of warnings recieved from omr core after template finalization
        /// </summary>
        private ObservableCollection<string> warnings;

        /// <summary>
        /// Template image displayed to the user
        /// </summary>
        private BitmapImage templateImage;

        /// <summary>
        /// Indicates that user is currently in adding chocie box mode
        /// </summary>
        private bool isAddingChoiceBox;

        /// <summary>
        /// Indicates that user is currently in adding grid mode
        /// </summary>
        private bool isAddingGrid;

        /// <summary>
        /// Indicates that template finalization was complete with no warnings
        /// </summary>
        private bool finalizationComplete;

        /// <summary>
        /// True zoom level without koefficient
        /// </summary>
        private double zoomLevel;

        /// <summary>
        /// The template id used by omr core
        /// </summary>
        private string templateId;

        /// <summary>
        /// Indicates visibility of start panel with quick access commands
        /// </summary>
        private Visibility startPanelVisibility;

        /// <summary>
        /// Page width in pixels
        /// </summary>
        private double pageWidth;

        /// <summary>
        /// Page height in pixels
        /// </summary>
        private double pageHeight;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateViewModel"/> class.
        /// </summary>
        /// <param name="tabName">The tab name</param>
        public TemplateViewModel(string tabName)
        {
            this.TabName = tabName;

            this.Warnings = new ObservableCollection<string>();
            this.pageQuestions = new ObservableCollection<BaseQuestionViewModel>();
            this.selectedElements = new ObservableCollection<BaseQuestionViewModel>();
            this.copiedQuestionsBuffer = new ObservableCollection<BaseQuestionViewModel>();

            ZoomKoefficient = 1;
            this.zoomLevel = 1;

            this.InitCommands();
        }

        #region Properties

        /// <summary>
        /// Gets or sets zoom koefficient for better representation of large images (values from 0 to 1)
        /// </summary>
        public static double ZoomKoefficient { get; set; }

        /// <summary>
        /// Gets or sets the template id used by omr core
        /// </summary>
        public string TemplateId
        {
            get { return this.templateId; }
            set
            {
                this.templateId = value;
                this.FinalizeTemplateCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected elements
        /// </summary>
        public ObservableCollection<BaseQuestionViewModel> SelectedElements
        {
            get { return this.selectedElements; }
            set
            {
                this.selectedElements = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the page elements
        /// </summary>
        public ObservableCollection<BaseQuestionViewModel> PageQuestions
        {
            get { return this.pageQuestions; }
            private set
            {
                this.pageQuestions = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether adding choice box mode is enabled
        /// </summary>
        public bool IsAddingChoiceBox
        {
            get { return this.isAddingChoiceBox; }
            set
            {
                this.isAddingChoiceBox = value;
                if (value)
                {
                    this.IsAddingGrid = false;
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether adding grid mode is enabled
        /// </summary>
        public bool IsAddingGrid
        {
            get { return this.isAddingGrid; }
            set
            {
                this.isAddingGrid = value;
                if (value)
                {
                    this.IsAddingChoiceBox = false;
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether template finalization was complete with no warnings
        /// </summary>
        public bool FinalizationComplete
        {
            get { return this.finalizationComplete; }
            set
            {
                this.finalizationComplete = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the data context for properties view
        /// </summary>
        public BaseQuestionViewModel PropertiesContext
        {
            get { return this.SelectedElements.Count == 1 ? this.SelectedElements[0] : null; }
        }

        /// <summary>
        /// Gets the visual string representation of current page scale
        /// </summary>
        public string PageScaleDisplayString
        {
            get { return Math.Round(this.ZoomLevel * 100) + "%"; }
        }

        /// <summary>
        /// Gets scale used to scale visuals in template view with respect to scaling koefficient
        /// </summary>
        public double PageScale
        {
            get { return this.ZoomLevel * ZoomKoefficient; }
        }

        /// <summary>
        /// Gets the list of warnings recieved after template finalization
        /// </summary>
        public ObservableCollection<string> Warnings
        {
            get { return this.warnings; }
            private set
            {
                this.warnings = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets true zoom level without koefficient
        /// </summary>
        public double ZoomLevel
        {
            get { return this.zoomLevel; }
            set
            {
                if (this.TemplateImage != null)
                {
                    this.zoomLevel = value;
                    this.OnPropertyChanged();
                    this.OnPropertyChanged(nameof(this.PageScale));
                    this.OnPropertyChanged(nameof(this.PageScaleDisplayString));
                }
            }
        }

        /// <summary>
        /// Gets or sets the template page image displayed to the user
        /// </summary>
        public BitmapImage TemplateImage
        {
            get { return this.templateImage; }
            set
            {
                this.templateImage = value;
                this.WasUploaded = false;

                if (this.templateImage != null)
                {
                    this.StartPanelVisibility = Visibility.Collapsed;
                    this.PageWidth = this.templateImage.PixelWidth;
                    this.PageHeight = this.templateImage.PixelHeight;
                }
                else
                {
                    this.StartPanelVisibility = Visibility.Visible;
                }

                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether template image was already uploaded on cloud
        /// </summary>
        public bool WasUploaded { get; set; }

        /// <summary>
        /// Gets or sets page width
        /// </summary>
        public double PageWidth
        {
            get { return this.pageWidth; }
            set
            {
                this.pageWidth = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets page height
        /// </summary>
        public double PageHeight
        {
            get { return this.pageHeight; }
            set
            {
                this.pageHeight = value; 
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets visibility of start panel
        /// </summary>
        public Visibility StartPanelVisibility
        {
            get { return this.startPanelVisibility; }
            set
            {
                this.startPanelVisibility = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the template image size in bytes
        /// </summary>
        public long ImageSizeInBytes { get; private set; }

        /// <summary>
        /// Gets or sets the template image name
        /// </summary>
        public string TemplateImageName { get; set; }

        /// <summary>
        /// Gets or sets image file format
        /// </summary>
        public string ImageFileFormat { get; set; }

        #region Commands

        public RelayCommand LoadTemplateImageCommand { get; private set; }

        public RelayCommand DropPageImageCommand { get; private set; }

        public RelayCommand SaveTemplateCommand { get; private set; }

        public RelayCommand SelectAllElementsCommand { get; private set; }

        public RelayCommand CorrectTemplateCommand { get; private set; }

        public RelayCommand FinalizeTemplateCommand { get; private set; }

        public RelayCommand AddQuestionDebugCommand { get; private set; }

        public RelayCommand CopyElementsCommand { get; private set; }

        public RelayCommand PasteElementsCommand { get; private set; }

        public RelayCommand FitPageWidthCommand { get; private set; }

        public RelayCommand FitPageHeightCommand { get; private set; }

        public RelayCommand ZoomInCommand { get; private set; }

        public RelayCommand ZoomOutCommand { get; private set; }

        public RelayCommand ZoomOriginalCommand { get; private set; }

        public RelayCommand UndoCommand { get; private set; }

        public RelayCommand RedoCommand { get; private set; }

        public RelayCommand AlignLeftCommand { get; private set; }

        public RelayCommand AlignRightCommand { get; private set; }

        public RelayCommand AlignTopCommand { get; private set; }

        public RelayCommand AlignBottomCommand { get; private set; }

        public RelayCommand ApplyFormattingCommand { get; private set; }

        public RelayCommand ShrinkElementCommand { get; private set; }

        public RelayCommand MoveElementsHorizontal { get; private set; }

        public RelayCommand MoveElementsVertical { get; private set; }

        #endregion

        #endregion

        /// <summary>
        /// Unselect all elements
        /// </summary>
        public void ClearSelection()
        {
            foreach (BaseQuestionViewModel element in this.PageQuestions)
            {
                element.IsSelected = false;
            }
        }

        /// <summary>
        /// Adds group of questions to the template
        /// </summary>
        /// <param name="questions">Questinos to add</param>
        public void AddQuestions(IEnumerable<BaseQuestionViewModel> questions)
        {
            foreach (BaseQuestionViewModel question in questions)
            {
                this.OnAddQuestion(question);
            }
        }

        /// <summary>
        /// Add new element via selection rectangle
        /// </summary>
        /// <param name="area">Element area</param>
        public void AddQuestion(Rect area)
        {
            string nextName = NamingManager.GetNextAvailableElementName(this.PageQuestions);

            BaseQuestionViewModel newQuestion;

            if (this.IsAddingChoiceBox)
            {
                newQuestion = new ChoiceBoxViewModel(nextName, area);
                this.IsAddingChoiceBox = false;
            }
            else
            {
                newQuestion = new GridViewModel(nextName, area);
                this.IsAddingGrid = false;
            }

            this.OnAddQuestion(newQuestion);
            newQuestion.IsSelected = true;

            ActionTracker.TrackAction(new AddElementsAction(new[] { newQuestion }, this.PageQuestions));
        }

        /// <summary>
        /// Selects all elements
        /// </summary>
        private void OnSelectAllElements()
        {
            this.ClearSelection();
            foreach (BaseQuestionViewModel element in this.PageQuestions)
            {
                element.IsSelected = true;
            }
        }

        /// <summary>
        /// Loads main template image and calculates zoom koefficient
        /// </summary>
        private void OnLoadTemplateImage()
        {
            string imagePath = DialogManager.ShowOpenImageDialog();
            if (imagePath == null)
            {
                return;
            }

            this.LoadTemplateImageFromFile(imagePath);
        }

        /// <summary>
        /// Loads template image located by specified path
        /// </summary>
        /// <param name="path">Path to image</param>
        private void LoadTemplateImageFromFile(string path)
        {
            double monitorWidth, monitorHeight;
            ResolutionUtility.GetMonitorResolution(out monitorWidth, out monitorHeight);

            FileInfo fileInfo = new FileInfo(path);
            this.ImageSizeInBytes = fileInfo.Length;
            this.TemplateImageName = fileInfo.Name;
            this.ImageFileFormat = fileInfo.Extension;

            if (!ResolutionUtility.CheckImageSize(fileInfo))
            {
                return;
            }

            this.TemplateImage = new BitmapImage(new Uri("file://" + path));

            if (this.TemplateImage.PixelWidth < 1200 || this.TemplateImage.PixelHeight < 1700)
            {
                DialogManager.ShowImageSizeWarning();
            }

            ZoomKoefficient = monitorWidth / this.TemplateImage.PixelWidth < 1
                ? monitorWidth / this.TemplateImage.PixelWidth
                : 1;

            this.OnPropertyChanged(nameof(this.PageScale));
        }

        /// <summary>
        /// Removes selected elements
        /// </summary>
        private void OnRemoveElement()
        {
            BaseQuestionViewModel[] removedElements = new BaseQuestionViewModel[this.SelectedElements.Count];
            int index = 0;

            foreach (BaseQuestionViewModel selectedItem in this.SelectedElements)
            {
                this.PageQuestions.Remove(selectedItem);
                removedElements[index++] = selectedItem;
            }

            this.SelectedElements.Clear();

            ActionTracker.TrackAction(new RemoveElementsAction(removedElements, this.PageQuestions));

            this.OnPropertyChanged(nameof(this.PropertiesContext));
        }

        /// <summary>
        /// Add element to page elements collection
        /// </summary>
        /// <param name="question">Element to add</param>
        private void OnAddQuestion(BaseQuestionViewModel question)
        {
            // subscribe to property changed to track selection changes
            question.PropertyChanged += this.ElementPropertyChanged;
            this.PageQuestions.Add(question);
        }

        /// <summary>
        /// Handles questions selection logic
        /// </summary>
        /// <param name="sender">Question with changed property</param>
        /// <param name="e">The event args</param>
        private void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                BaseQuestionViewModel element = (BaseQuestionViewModel) sender;

                if (element.IsSelected)
                {
                    this.SelectedElements.Add(element);
                }
                else
                {
                    this.SelectedElements.Remove(element);
                }

                this.OnPropertyChanged(nameof(this.PropertiesContext));
            }
        }

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitCommands()
        {
            base.RemoveElementCommand = new RelayCommand(x => this.OnRemoveElement(), x => this.SelectedElements.Any());

            this.LoadTemplateImageCommand = new RelayCommand(x => this.OnLoadTemplateImage());
            this.DropPageImageCommand = new RelayCommand(x => this.LoadTemplateImageFromFile((string) x));
            this.SelectAllElementsCommand = new RelayCommand(x => this.OnSelectAllElements(), x => this.PageQuestions.Any());
            this.SaveTemplateCommand = new RelayCommand(x => this.OnSaveTemplate(), x => this.PageQuestions.Any());

            this.CorrectTemplateCommand = new RelayCommand(x => this.OnCorrectTemplate(), x => this.PageQuestions.Any());
            this.FinalizeTemplateCommand = new RelayCommand(x => this.OnFinilizeTemplate(), x => !string.IsNullOrEmpty(this.TemplateId));

            this.CopyElementsCommand = new RelayCommand(x => this.OnCopyQuestions(), x => this.SelectedElements.Any());

            this.PasteElementsCommand = new RelayCommand(x =>
            {
                if (x == null)
                    x = new Point(30, 30);
                this.OnPasteQuestions((Point) x);
            }, x => this.copiedQuestionsBuffer.Any());

            this.FitPageWidthCommand = new RelayCommand(x => this.OnFitPageWidth((double)x));
            this.FitPageHeightCommand = new RelayCommand(x => this.OnFitPageHeight((Size)x));
            this.ZoomInCommand = new RelayCommand(x => this.ZoomLevel = Math.Min(this.ZoomLevel + 0.1, 4));
            this.ZoomOutCommand = new RelayCommand(x => this.ZoomLevel = Math.Max(this.ZoomLevel - 0.1, 0.1));
            this.ZoomOriginalCommand = new RelayCommand(x => this.ZoomLevel = 1);

            this.UndoCommand = new RelayCommand(o => ActionTracker.Undo(1), o => ActionTracker.CanUndo());
            this.RedoCommand = new RelayCommand(o => ActionTracker.Redo(1), o => ActionTracker.CanRedo());

            this.AlignBottomCommand = new RelayCommand(x => AlignmentHelper.AlignBottom(this.SelectedElements), x => this.SelectedElements.Count > 1);
            this.AlignTopCommand = new RelayCommand(x => AlignmentHelper.AlignTop(this.SelectedElements), x => this.SelectedElements.Count > 1);
            this.AlignRightCommand = new RelayCommand(x => AlignmentHelper.AlignRight(this.SelectedElements), x => this.SelectedElements.Count > 1);
            this.AlignLeftCommand = new RelayCommand(x => AlignmentHelper.AlignLeft(this.SelectedElements), x => this.SelectedElements.Count > 1);

            this.ApplyFormattingCommand = new RelayCommand(x => this.OnApplyFormatting(), x => this.SelectedElements.Count == 1);
            this.ShrinkElementCommand = new RelayCommand(x => this.OnShrinkQuestionCommand(), x => this.SelectedElements.Count > 0);

            this.MoveElementsHorizontal = new RelayCommand(x => this.OnMoveElementsHorizontal((double)x), x => this.SelectedElements.Count > 0);
            this.MoveElementsVertical = new RelayCommand(x => this.OnMoveElementsVertical((double)x), x => this.SelectedElements.Count > 0);
        }

        /// <summary>
        /// Shrink selected elements
        /// </summary>
        private void OnShrinkQuestionCommand()
        {
            foreach (BaseQuestionViewModel element in this.SelectedElements)
            {
                element.Shrink();
            }
        }

        /// <summary>
        /// Move question horizontally
        /// </summary>
        /// <param name="distance">Distance to move elements</param>
        private void OnMoveElementsHorizontal(double distance)
        {
            foreach (BaseQuestionViewModel element in this.SelectedElements)
            {
                element.Left += distance;
            }
        }

        /// <summary>
        /// Move question vertically
        /// </summary>
        /// <param name="distance">Distance to move elements</param>
        private void OnMoveElementsVertical(double distance)
        {
            foreach (BaseQuestionViewModel element in this.SelectedElements)
            {
                element.Top += distance;
            }
        }

        /// <summary>
        /// Apply selected question's style to all other questions
        /// </summary>
        private void OnApplyFormatting()
        {
            BaseQuestionViewModel ethalon = this.SelectedElements[0];

            if (ethalon is ChoiceBoxViewModel)
            {
                foreach (var element in this.PageQuestions.Where(x => x is ChoiceBoxViewModel && x != ethalon))
                {
                    ((ChoiceBoxViewModel) element).ApplyStyle((ChoiceBoxViewModel) ethalon);
                }
            }
            else if (ethalon is GridViewModel)
            {
                foreach (var element in this.PageQuestions.Where(x => x is GridViewModel && x != ethalon))
                {
                    ((GridViewModel)element).ApplyStyle((GridViewModel)ethalon);
                }
            }
        }

        /// <summary>
        /// Runs template correction
        /// </summary>
        private void OnCorrectTemplate()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                string templateData = TemplateSerializer.TemplateToJson(this, false);

                byte[] imageData = TemplateSerializer.CompressImage(this.TemplateImage, this.ImageSizeInBytes);

                //string imageData = TemplateConverter.CheckAndCompressImage(this.TemplateImage, this.ImageFileFormat, this.ImageSizeInBytes);
                
                string additionalPars = string.Empty;

                try
                {
                    TemplateViewModel correctedTemplate = CoreApi.CorrectTemplate(this.TemplateImageName,
                        imageData,
                        templateData,
                        this.WasUploaded,
                        additionalPars);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.ClearSelection();
                        this.PageQuestions.Clear();

                        this.AddQuestions(correctedTemplate.PageQuestions);
                        this.TemplateId = correctedTemplate.TemplateId;

                        this.FinalizationComplete = false;

                        double koeff = this.TemplateImage.PixelWidth / correctedTemplate.PageWidth;
                        ZoomKoefficient *= koeff;
                        this.OnPropertyChanged(nameof(this.PageScale));

                        this.PageWidth = correctedTemplate.PageWidth;
                        this.PageHeight = correctedTemplate.PageHeight;
                        this.WasUploaded = true;

                        this.Warnings.Add("Template correction complete!");
                    });
                }
                catch (Exception e)
                {
                    DialogManager.ShowErrorDialog(e.Message);
                }
            };

            worker.RunWorkerCompleted += (sender, args) =>
            {
                BusyIndicatorManager.Disable();
            };

            BusyIndicatorManager.Enable();

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Runs template finalization
        /// </summary>
        private void OnFinilizeTemplate()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                string templateData = TemplateSerializer.TemplateToJson(this, false);
                string additionalPars = string.Empty;

                try
                {
                    string templateName = Path.GetFileNameWithoutExtension(this.TemplateImageName) + "_template.omr";
                    FinalizationData finalizationResult = CoreApi.FinalizeTemplate(templateName, Encoding.UTF8.GetBytes(templateData), this.TemplateId, additionalPars);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.ProcessFinalizationResponse(finalizationResult);
                    });
                }
                catch (Exception e)
                {
                    DialogManager.ShowErrorDialog(e.Message);
                }
            };

            worker.RunWorkerCompleted += (sender, args) =>
            {
                BusyIndicatorManager.Disable();
            };

            BusyIndicatorManager.Enable();

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Process finalization data and display warnings
        /// </summary>
        /// <param name="response">Response containing finalization data</param>
        private void ProcessFinalizationResponse(FinalizationData response)
        {
            // parse recognition results for template image
            ObservableCollection<RecognitionResult> recognitionResults = this.ParseAnswers(response.Answers);

            if (recognitionResults.Any(x => !string.IsNullOrEmpty(x.AnswerKey)))
            {
                new FinalizationResultsViewModel(recognitionResults);
            }

            // TODO
            this.FinalizationComplete = true;

            // check warnings
            if (response.Warnings.Length > 0)
            {
                foreach (var responseWarning in response.Warnings)
                {
                    this.Warnings.Add(responseWarning);
                }
            }
            else
            {
                this.Warnings.Add("Finalization complete!");
                this.FinalizationComplete = true;
            }
        }

        /// <summary>
        /// Adjust zoom level so that image fits page width
        /// </summary>
        /// <param name="viewportWidth">The viewport width</param>
        private void OnFitPageWidth(double viewportWidth)
        {
            // 40 pixels threshold for better visuals
            this.ZoomLevel = (viewportWidth - 40) / (this.PageWidth * ZoomKoefficient);
        }

        /// <summary>
        /// Adjust zoom level so that image fits page height
        /// </summary>
        /// <param name="viewportSize">The viewport size</param>
        private void OnFitPageHeight(Size viewportSize)
        {
            // 20 pixels threshold for better visuals
            this.ZoomLevel = (viewportSize.Height - 20) / (this.PageHeight * ZoomKoefficient);
        }

        /// <summary>
        /// Copy selected questions
        /// </summary>
        private void OnCopyQuestions()
        {
            this.copiedQuestionsBuffer.Clear();

            foreach (BaseQuestionViewModel element in this.SelectedElements)
            {
                this.copiedQuestionsBuffer.Add(element.CreateCopy());
            }
        }

        /// <summary>
        /// Paste questions in certain position
        /// </summary>
        /// <param name="position">Position to paste questions</param>
        private void OnPasteQuestions(Point position)
        {
            this.ClearSelection();

            // for the case when mouse position was out of UI change start position
            if (position.X < 0 || position.Y < 0)
            {
                position.X = position.Y = 30;
            }

            // calculate delta between topmost item and paste position
            BaseQuestionViewModel topmostItem = this.copiedQuestionsBuffer.OrderByDescending(x => x.Top).Last();
            double deltaTop = topmostItem.Top - position.Y;
            double deltaLeft = topmostItem.Left - position.X;

            // buffer for undo/redo
            BaseQuestionViewModel[] copiedElements = new BaseQuestionViewModel[this.copiedQuestionsBuffer.Count];

            for (int i = 0; i < this.copiedQuestionsBuffer.Count; i++)
            {
                // create new copy and update name and position
                BaseQuestionViewModel copy = this.copiedQuestionsBuffer[i].CreateCopy();
                copy.Name = NamingManager.GetNextAvailableElementName(this.PageQuestions);
                copy.Top -= deltaTop;
                copy.Left -= deltaLeft;

                this.OnAddQuestion(copy);
                copy.IsSelected = true;

                copiedElements[i] = copy;
            }

            ActionTracker.TrackAction(new AddElementsAction(copiedElements, this.PageQuestions));
        }

        /// <summary>
        /// Saves template
        /// </summary>
        private void OnSaveTemplate()
        {
            string savePath = DialogManager.ShowSaveTemplateDialog();
            if (savePath == null)
            {
                return;
            }

            string jsonRes = TemplateSerializer.TemplateToJson(this, true);
            File.WriteAllText(savePath, jsonRes);
        }
    }
}
