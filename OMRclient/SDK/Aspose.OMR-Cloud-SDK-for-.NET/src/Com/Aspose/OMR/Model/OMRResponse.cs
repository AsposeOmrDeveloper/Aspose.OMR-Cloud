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
    public class OMRResponse
    {
        private string code;
        private string status;
        private int errorCode;
        private string errorText;
        private Payload payload;
        private ServerStat serverStat;

        public string Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public int ErrorCode
        {
            get { return this.errorCode; }
            set { this.errorCode = value; }
        }

        public string ErrorText
        {
            get { return this.errorText; }
            set { this.errorText = value; }
        }


        public Payload Payload
        {
            get { return this.payload; }
            set { this.payload = value; }
        }

        public ServerStat ServerStat
        {
            get { return this.serverStat; }
            set { this.serverStat = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class OMRResponse {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  ErrorCode: ").Append(ErrorCode).Append("\n");
            sb.Append("  ErrorText: ").Append(ErrorText).Append("\n");
            sb.Append("  Payload: ").Append(Payload).Append("\n");
            sb.Append("  ServerStat: ").Append(ServerStat).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
