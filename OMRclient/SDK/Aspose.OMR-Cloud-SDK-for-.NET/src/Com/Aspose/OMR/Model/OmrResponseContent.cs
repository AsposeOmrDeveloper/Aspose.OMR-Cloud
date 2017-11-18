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
    public class OmrResponseContent
    {
        private string templateId;
        private double executionTime;
        private FileInfo[] responseFiles;
        private OmrResponseInfo info;

        public string TemplateId
        {
            get { return this.templateId; }
            set { this.templateId = value; }
        }

        public double ExecutionTime
        {
            get { return this.executionTime; }
            set { this.executionTime = value; }
        }

        public FileInfo[] ResponseFiles
        {
            get { return this.responseFiles; }
            set { this.responseFiles = value; }
        }

        public OmrResponseInfo Info
        {
            get { return this.info; }
            set { this.info = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class Payload {\n");
            sb.Append("  CompletedTaskId: ").Append(TemplateId).Append("\n");
            sb.Append("  ExecutionTime: ").Append(ExecutionTime).Append("\n");
            sb.Append("  FilesInfo: ").Append(ResponseFiles).Append("\n");
            sb.Append("  ResultInfo: ").Append(Info).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
