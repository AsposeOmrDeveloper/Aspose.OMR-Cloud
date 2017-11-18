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
    public class OMRFunctionParam
    {
        private string functionParam;
        private string additionalParam;

        public string FunctionParam
        {
            get { return this.functionParam; }
            set { this.functionParam = value; }
        }

        public string AdditionalParam
        {
            get { return this.additionalParam; }
            set { this.additionalParam = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class OMRResponse {\n");
            sb.Append("  Code: ").Append(FunctionParam).Append("\n");
            sb.Append("  Status: ").Append(AdditionalParam).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
