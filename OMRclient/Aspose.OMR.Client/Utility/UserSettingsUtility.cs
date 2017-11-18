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
namespace Aspose.OMR.Client.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Manages saving and loading user settings
    /// </summary>
    public static class UserSettingsUtility
    {
        /// <summary>
        /// Save custom mappings to settings
        /// </summary>
        /// <param name="customMappingDictionary">Custom mappings</param>
        public static void SaveCustomMappings(Dictionary<string, string[]> customMappingDictionary)
        {
            StringCollection collectionToSave = new StringCollection();

            foreach (var entry in customMappingDictionary)
            {
                var delimiter = "|";
                StringBuilder builder = new StringBuilder();
                builder.Append(entry.Key + delimiter);

                for (int i = 0; i < entry.Value.Length; i++)
                {
                    builder.Append(entry.Value[i] + delimiter);
                }

                // remove last delimiter for easy string.split on loading
                builder.Remove(builder.Length - 1, 1);
                collectionToSave.Add(builder.ToString());
            }

            // save mapping
            Properties.Settings.Default.Mappings = collectionToSave;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Load custom mapping from settings
        /// </summary>
        /// <returns>Loaded mappings dictionary</returns>
        public static Dictionary<string, string[]> LoadCustomMappings()
        {
            var customMappingDictionary = new Dictionary<string, string[]>();

            // get mappings from settings
            StringCollection mappingCollection = Properties.Settings.Default.Mappings;

            if (mappingCollection != null)
            {
                foreach (string entry in mappingCollection)
                {
                    string[] mapping = entry.Split('|');
                    customMappingDictionary.Add(mapping[0], mapping.Skip(1).ToArray());
                }
            }

            return customMappingDictionary;
        }

        /// <summary>
        /// Saves app key and app sid
        /// </summary>
        /// <param name="encryptedAppKey">Encrypted app key</param>
        /// <param name="encrpytedAppSid">Encrypted app sid</param>
        public static void SaveCredentials(byte[] encryptedAppKey, byte[] encrpytedAppSid)
        {
            string encryptedAppKeyString = Convert.ToBase64String(encryptedAppKey);
            string encryptedAppSidString = Convert.ToBase64String(encrpytedAppSid);

            Properties.Settings.Default.AppKey = encryptedAppKeyString;
            Properties.Settings.Default.AppSid = encryptedAppSidString;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Loads app key from settings
        /// </summary>
        /// <returns>Encrypted app key</returns>
        public static byte[] LoadAppKey()
        {
            string encryptedAppKeyString = Properties.Settings.Default.AppKey;
            if (!string.IsNullOrEmpty(encryptedAppKeyString))
            {
                return Convert.FromBase64String(encryptedAppKeyString);
            }

            return null;
        }

        /// <summary>
        /// Loads app sid from settings
        /// </summary>
        /// <returns>Encrypted app sid</returns>
        public static byte[] LoadAppSid()
        {
            string encryptedAppSidString = Properties.Settings.Default.AppSid;
            if (!string.IsNullOrEmpty(encryptedAppSidString))
            {
                return Convert.FromBase64String(encryptedAppSidString);
            }

            return null;
        }
    }
}
