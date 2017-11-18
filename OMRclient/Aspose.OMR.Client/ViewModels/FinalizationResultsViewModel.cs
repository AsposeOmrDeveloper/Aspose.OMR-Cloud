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
    using System.Collections.ObjectModel;
    using Utility;
    using Views;

    /// <summary>
    /// View model for recognition results recieved during finalization
    /// </summary>
    public class FinalizationResultsViewModel : ViewModelBase
    {
        /// <summary>
        /// View displayed to the user
        /// </summary>
        private readonly FinalizationResultsView view;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalizationResultsViewModel"/> class
        /// </summary>
        /// <param name="recognitionResults">recieved recognition results</param>
        public FinalizationResultsViewModel(ObservableCollection<RecognitionResult> recognitionResults)
        {
            this.RecognitionResults = recognitionResults;
            this.OkCommand = new RelayCommand(x => this.OnOkCommand());

            this.view = new FinalizationResultsView(this);
            this.view.ShowDialog();
        }

        /// <summary>
        /// Gets or sets the recognition results
        /// </summary>
        public ObservableCollection<RecognitionResult> RecognitionResults { get; set; }

        /// <summary>
        /// Gets or sets the Ok command
        /// </summary>
        public RelayCommand OkCommand { get; set; }

        /// <summary>
        /// Closes window
        /// </summary>
        private void OnOkCommand()
        {
            this.view.Close();
        }
    }
}
