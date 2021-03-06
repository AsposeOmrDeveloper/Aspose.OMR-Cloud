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
namespace Aspose.OMR.Client.Utility
{
    /// <summary>
    /// String wrapper used for Custom Mapping view
    /// </summary>
    public class StringWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringWrapper"/> class
        /// </summary>
        /// <param name="stringValue">Wrapped string value</param>
        public StringWrapper(string stringValue)
        {
            this.StringValue = stringValue;
        }

        /// <summary>
        /// Gets or sets the wrapped string value
        /// </summary>
        public string StringValue { get; set; }
    }
}
