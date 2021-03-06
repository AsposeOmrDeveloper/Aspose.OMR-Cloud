﻿/*
 * Copyright (c) 2017 Aspose Pty Ltd. All Rights Reserved.
 *
 * Licensed under the MIT (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       https://github.com/asposecloud/Aspose.OMR-Cloud/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Text;

namespace Com.Aspose.OMR.Model
{
    public class ServerStat
    {
        private TimeSpan storageDownloadTime;
        private TimeSpan omrFunctionCallTime;

        public TimeSpan StorageDownloadTime
        {
            get { return this.storageDownloadTime; }
            set { this.storageDownloadTime = value; }
        }

        public TimeSpan OmrFunctionCallTime
        {
            get { return this.omrFunctionCallTime; }
            set { this.omrFunctionCallTime = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class OMRResponse {\n");
            sb.Append("  StorageDownloadTime: ").Append(StorageDownloadTime).Append("\n");
            sb.Append("  OmrFunctionCallTime: ").Append(OmrFunctionCallTime).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
