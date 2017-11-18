﻿/*
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
namespace Aspose.OMR.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Com.Aspose.OMR.Api;
    using Com.Aspose.OMR.Model;
    using Com.Aspose.Storage.Api;
    using Com.Aspose.Storage.Model;
    using TemplateModel;
    using Utility;
    using ViewModels;

    /// <summary>
    /// Provides API to communicate with Omr Core
    /// </summary>
    public static class CoreApi
    {
        /// <summary>
        /// The base path to call REST API
        /// </summary>
        private static readonly string Basepath = "http://api.aspose.cloud/v1.1";

        /// <summary>
        /// The app key
        /// </summary>
        private static string AppKey;

        /// <summary>
        /// The app sid
        /// </summary>
        private static string AppSid;

        /// <summary>
        /// Initializes static members of the <see cref="CoreApi"/> class.
        /// </summary>
        static CoreApi()
        {
            // try loading keys from settings
            byte[] appKey = UserSettingsUtility.LoadAppKey();
            byte[] appSid = UserSettingsUtility.LoadAppSid();

            // if any keys loaded, decrypt them
            if (appKey != null && appSid != null)
            {
                AppSid = Encoding.UTF8.GetString(SecurityUtility.Decrpyt(appSid));
                AppKey = Encoding.UTF8.GetString(SecurityUtility.Decrpyt(appKey));
            }
        }

        /// <summary>
        /// Gets a value indicating whether credentials keys were provided
        /// </summary>
        public static bool GotKeys
        {
            get { return !string.IsNullOrEmpty(AppKey) && !string.IsNullOrEmpty(AppSid); }
        }

        /// <summary>
        /// Updates user keys
        /// </summary>
        /// <param name="appKey">The app key</param>
        /// <param name="appSid">The app sid key</param>
        public static void UpdateKeys(string appKey, string appSid)
        {
            AppKey = appKey;
            AppSid = appSid;
        }

        /// <summary>
        /// Performs template correction
        /// </summary>
        /// <param name="imageName">The name of the template image</param>
        /// <param name="imageData">The template image data</param>
        /// <param name="templateData">The template data</param>
        /// <param name="wasUploaded">Indicates if image was already uploaded on cloud</param>
        /// <param name="additionalParams">The additional parameters</param>
        /// <returns>Corrected template</returns>
        public static TemplateViewModel CorrectTemplate(string imageName, byte[] imageData, string templateData, bool wasUploaded,  string additionalParams)
        {
            OMRResponse response = RunOmrTask("CorrectTemplate", imageName, imageData, templateData, wasUploaded, false, additionalParams);

            OmrResponseContent responseResult = response.Payload.Result;
            CheckTaskResult(response.Payload.Result);

            byte[] correctedTemplateData = responseResult.ResponseFiles
                .First(x => x.Name.Equals("corrected_template.dat"))
                .Data;

            TemplateViewModel templateViewModel = TemplateSerializer.JsonToTemplate(Encoding.UTF8.GetString(correctedTemplateData));
            templateViewModel.TemplateId = responseResult.TemplateId;

            return templateViewModel;
        }

        /// <summary>
        /// Performs template finalization
        /// </summary>
        /// <param name="templateName">The template file name</param>
        /// <param name="templateData">The template data</param>
        /// <param name="templateId">The template id</param>
        /// <param name="additionalParams">The additional parameters</param>
        /// <returns>Finalization data containing warnings</returns>
        public static FinalizationData FinalizeTemplate(string templateName, byte[] templateData, string templateId, string additionalParams)
        {
            OMRResponse response = RunOmrTask("FinalizeTemplate", templateName, templateData, templateId, false, false, additionalParams);
            OmrResponseContent responseResult = response.Payload.Result;
            CheckTaskResult(response.Payload.Result);

            FinalizationData data = new FinalizationData();
            data.Answers = Encoding.UTF8.GetString(responseResult.ResponseFiles[0].Data);
            data.Warnings = responseResult.Info.Details.TaskMessages;
            return data;
        }

        /// <summary>
        /// Performs OMR over single image
        /// </summary>
        /// <param name="imageName">Recognized image name</param>
        /// <param name="imageData">The image data</param>
        /// <param name="templateId">The template id</param>
        /// <param name="wasUploaded">Indicates if image was already uploaded on cloud</param>
        /// <param name="additionalParams">The additional parameters</param>
        /// <returns>Recognition results</returns>
        public static string RecognizeImage(string imageName, byte[] imageData, string templateId, bool wasUploaded, string additionalParams)
        {
            OMRResponse response = RunOmrTask("RecognizeImage", imageName, imageData, templateId, wasUploaded, true, additionalParams);

            OmrResponseContent responseResult = response.Payload.Result;
            if (responseResult.Info.SuccessfulTasksCount < 1 || responseResult.Info.Details.RecognitionStatistics[0].TaskResult != "Pass")
            {
                StringBuilder builder = new StringBuilder();
                foreach (string message in responseResult.Info.Details.RecognitionStatistics[0].TaskMessages)
                {
                    builder.AppendLine(message);
                }

                throw new Exception(builder.ToString());
            }

            string result = Encoding.UTF8.GetString(response.Payload.Result.ResponseFiles[0].Data);
            return result;
        }

        /// <summary>
        /// Put files to the cloud and call OMR task
        /// </summary>
        /// <param name="actionName">The action name string</param>
        /// <param name="fileName">The file name</param>
        /// <param name="fileData">The file data</param>
        /// <param name="functionParam">The function parameters</param>
        /// <param name="wasUploaded">Indicates if image was already uploaded on cloud</param>
        /// <param name="trackFile">Track file so that it can be deleted from cloud</param>
        /// <param name="additionalParam">The additional (debug) parameters</param>
        /// <returns>Task response</returns>
        public static OMRResponse RunOmrTask(string actionName, string fileName, byte[] fileData, string functionParam, bool wasUploaded, bool trackFile, string additionalParam)
        {
            if (string.IsNullOrEmpty(AppKey) || string.IsNullOrEmpty(AppSid))
            {
                throw new Exception("Please specify App Key and App SID in Settings->Credentials in order to use OMR functions.");
            }

            if (!wasUploaded)
            {
                BusyIndicatorManager.UpdateText("Uploading files...");

                if (trackFile)
                {
                    CloudStorageManager.TrackFileUpload(fileName);
                }

                StorageApi storageApi = new StorageApi(AppKey, AppSid, Basepath);
                storageApi.PutCreate(fileName, "", "", fileData);
            }

            OmrApi omrApi = new OmrApi(AppKey, AppSid, Basepath);

            OMRFunctionParam param = new OMRFunctionParam();
            param.FunctionParam = functionParam;
            param.AdditionalParam = additionalParam;

            BusyIndicatorManager.UpdateText("Processing task...");
            OMRResponse response = omrApi.PostRunOmrTask(fileName, actionName, param, null, null);
            CheckForError(response);
            return response;
        }

        /// <summary>
        /// Remove files from cloud storage
        /// </summary>
        /// <param name="files">List of file names to remove</param>
        public static void StorageCleanUp(List<string> files)
        {
            StorageApi storageApi = new StorageApi(AppKey, AppSid, Basepath);

            foreach (string file in files)
            {
                BusyIndicatorManager.UpdateText("Storage clean up...\n Deleting file: " + file);

                FileExistResponse existsResponse = storageApi.GetIsExist(file, "", "");
                if (existsResponse.FileExist.IsExist)
                {
                    RemoveFileResponse deleteResponse = storageApi.DeleteFile(file, "", "");
                }
            }
        }

        /// <summary>
        /// Check task result in response
        /// </summary>
        /// <param name="responseResult">Response to check</param>
        private static void CheckTaskResult(OmrResponseContent responseResult)
        {
            if (responseResult.Info.SuccessfulTasksCount < 1 || responseResult.Info.Details.TaskResult != "Pass")
            {
                StringBuilder builder = new StringBuilder();
                foreach (string message in responseResult.Info.Details.TaskMessages)
                {
                    builder.AppendLine(message);
                }

                throw new Exception(builder.ToString());
            }
        }

        /// <summary>
        /// Check response for error
        /// </summary>
        /// <param name="response">Response to check</param>
        private static void CheckForError(OMRResponse response)
        {
            if (response.ErrorCode != 0 || response.Code != "200")
            {
                throw new Exception(response.ErrorText);
            }
        }
    }
}
