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
    public class RecognitionStatistics
    {
        private string name;
        private string[] taskMessages;
        private string taskResult;
        private double runSeconds;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string[] TaskMessages
        {
            get { return this.taskMessages; }
            set { this.taskMessages = value; }
        }

        public string TaskResult
        {
            get { return this.taskResult; }
            set { this.taskResult = value; }
        }

        public double RunSeconds
        {
            get { return this.runSeconds; }
            set { this.runSeconds = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class FilesInfo {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  TaskMessages: ").Append(TaskMessages).Append("\n");
            sb.Append("  TaskResult: ").Append(TaskResult).Append("\n");
            sb.Append("  RunSeconds: ").Append(RunSeconds).Append("\n");

            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
