using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrandedDeep2KModBundles
{
    static partial class Main
    {
        private static string configFileName = "StrandedDeep2KMod.config";

        private static void ReadConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                if (System.IO.File.Exists(configFilePath))
                {
                    string[] config = System.IO.File.ReadAllLines(configFilePath);
                    foreach (string line in config)
                    {
                        string[] tokens = line.Split(new string[] { "=", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length == 2)
                        {
                            if (tokens[0].StartsWith("enableModel_"))
                            {
                                string key = tokens[0].Replace("enableModel_", "");
                                foreach (ModelInfo mi in _modelInfos)
                                {
                                    if (mi.ConfigKey.CompareTo(key) == 0)
                                    {
                                        mi.ConfigEnabled = bool.Parse(tokens[1]);
                                    }
                                }
                            }
                            else if (tokens[0].Contains("replaceBushes"))
                            {
                                replaceBushes = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("replaceFicus"))
                            {
                                replaceFicus = bool.Parse(tokens[1]);
                            }
                            else if (tokens[0].Contains("newModels"))
                            {
                                newModels = bool.Parse(tokens[1]);
                            }
                        }
                    }
                }
            }
        }

        private static void WriteConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("replaceBushes=" + replaceBushes + ";");
                sb.AppendLine("replaceFicus=" + replaceFicus + ";");
                sb.AppendLine("newModels=" + newModels + ";");

                foreach (ModelInfo mi in _modelInfos)
                {
                    sb.AppendLine("enableModel_"+mi.ConfigKey+"=" + mi.ConfigEnabled + ";");
                }

                System.IO.File.WriteAllText(configFilePath, sb.ToString(), Encoding.UTF8);
            }
        }

        private static bool ExistsConfig()
        {
            string dataDirectory = FilePath.SAVE_FOLDER;//System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow"), @"Beam Team Games\Stranded Deep\Data\");
            if (System.IO.Directory.Exists(dataDirectory))
            {
                string configFilePath = System.IO.Path.Combine(dataDirectory, configFileName);
                if (System.IO.File.Exists(configFilePath))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
