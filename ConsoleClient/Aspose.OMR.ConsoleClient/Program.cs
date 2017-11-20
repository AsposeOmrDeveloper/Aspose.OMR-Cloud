/*
 * Copyright (c) 2017 Aspose Pty Ltd. All Rights Reserved.
 *
 * Licensed under the MIT (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      https://github.com/aspose-omr/Aspose.OMR-for-Cloud/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Com.Aspose.OMR.Api;
using Com.Aspose.OMR.Model;
using Com.Aspose.Storage.Api;

namespace Aspose.OMR.ConsoleClient
{
    class Program
    {
        // provide your own keys recieved by registrating at Aspose Cloud Dashboard (https://dashboard.aspose.cloud/)
        private static string APIKEY = "xxxxx";
        private static string APPSID = "xxxxx";

        /// <summary>
        /// Production base path
        /// </summary>
        private static string BASEPATH = "https://api.aspose.cloud/v1.1";
        
        /// <summary>
        /// Path to test data
        /// </summary>
        static string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestExamples\");

        static void Main(string[] args)
        {
            // 0. Create template (using Aspose.OMR.Client)
            // 1. Run template correction
            OMRResponse response = RunOmrTask("CorrectTemplate", "AsposeTestExample.jpg", File.ReadAllText(path + "AsposeTestExample.omr"));

            // get template id, which will be used during finalization and recognition
            string templateId = response.Payload.Result.TemplateId;

            // save corrected template data to file
            File.WriteAllBytes(path + "AsposeTestCorrectedTemplate.omr", response.Payload.Result.ResponseFiles[0].Data);

            // 2. Run template Finalization
            RunOmrTask("FinalizeTemplate", "AsposeTestCorrectedTemplate.omr", templateId);

            // 3. Recognize image
            OMRResponse recognitionResponse = RunOmrTask("RecognizeImage", "1.jpg", templateId);

            // get recognition results as string
            string recognitionResults = Encoding.UTF8.GetString(recognitionResponse.Payload.Result.ResponseFiles[0].Data);

            Console.WriteLine(recognitionResults);
            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        private static Com.Aspose.OMR.Model.OMRResponse RunOmrTask(string actionName, string inputFileName, string functionParam)
        {
            // Instantiate Aspose Storage Cloud API SDK
            OmrApi target = new OmrApi(APIKEY, APPSID, BASEPATH);

            // Instantiate Aspose OMR Cloud API SDK
            StorageApi storageApi = new StorageApi(APIKEY, APPSID, BASEPATH);

            // Init function parameters
            OMRFunctionParam param = new OMRFunctionParam();
            param.FunctionParam = functionParam;

            // Set 3rd party cloud storage server (if any)
            string storage = null;
            string folder = null;

            // Upload source file to aspose cloud storage
            storageApi.PutCreate(inputFileName, "", "", System.IO.File.ReadAllBytes(path + inputFileName));

            // Invoke Aspose.OMR Cloud SDK API
            Com.Aspose.OMR.Model.OMRResponse response = target.PostRunOmrTask(inputFileName, actionName, param, storage, folder);
            return response;
        }
    }
}
