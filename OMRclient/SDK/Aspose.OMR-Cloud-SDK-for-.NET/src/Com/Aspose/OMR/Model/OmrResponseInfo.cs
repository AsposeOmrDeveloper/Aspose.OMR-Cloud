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

using System.Text;

namespace Com.Aspose.OMR.Model
{
    public class OmrResponseInfo
    {
        private string responseVersion;
        private int processedTasksCount;
        private int successfulTasksCount;
        private OMRResponseDetails details;

        public string ResponseVersion
        {
            get { return this.responseVersion; }
            set { this.responseVersion = value; }
        }

        public int ProcessedTasksCount
        {
            get { return this.processedTasksCount; }
            set { this.processedTasksCount = value; }
        }

        public int SuccessfulTasksCount
        {
            get { return this.successfulTasksCount; }
            set { this.successfulTasksCount = value; }
        }

        public OMRResponseDetails Details
        {
            get { return this.details; }
            set { this.details = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class FilesInfo {\n");
            sb.Append("  ResponseVersion: ").Append(ResponseVersion).Append("\n");
            sb.Append("  ProcessedTasksCount: ").Append(ProcessedTasksCount).Append("\n");
            sb.Append("  SuccessfulTasksCount: ").Append(SuccessfulTasksCount).Append("\n");
            sb.Append("  Details: ").Append(Details).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
