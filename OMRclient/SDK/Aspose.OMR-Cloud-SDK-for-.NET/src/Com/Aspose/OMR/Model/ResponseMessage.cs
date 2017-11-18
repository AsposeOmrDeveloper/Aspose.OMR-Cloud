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

/**
 *
 * @author Imran Anwar
 * This class is added for the response to be separated in Json and InputStream
 */
using System;
using System.Text;

namespace Com.Aspose.OMR.Model
{
    public class ResponseMessage
    {
        private int code;
        private string status;
        private string message;
        private byte[] responseStream;

        public ResponseMessage() { }

        public ResponseMessage(byte[] ResponseStream, int Code, String Status )
        {
            this.Code = Code;
            this.Status = Status;
            this.ResponseStream = ResponseStream;
        }

        public int Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        public byte[] ResponseStream
        {
            get { return this.responseStream; }
            set { this.responseStream = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class ResponseMessage {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}


