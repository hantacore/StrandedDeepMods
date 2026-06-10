using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StrandedDeep2KModBundles
{
    internal static class CustomLogger
    {
        static string filePath = "";
        static StreamWriter logTarget = null;

        public static void InitCustomLogger(string directory)
        {
            filePath = Path.Combine(directory, "StrandedDeep2KLog.log");

            Debug.Log("Stranded Deep 2K Mod : placing custom log file in : " + filePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            logTarget = File.AppendText(filePath);
        }

        internal static void Log(string message, bool debug = true)
        {
            try
            {
                if (!debug)
                    return;

                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }

                logTarget.WriteLine(message);
                logTarget.Flush();
            }
            catch (Exception ex)
            {
                Debug.Log("Stranded Deep 2K Mod : error while writing custom log : " + ex + Environment.NewLine + message);
            }
            finally
            {

            }
        }
    }
}
