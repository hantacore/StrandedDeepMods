using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace StrandedDeepMapper
{
    public static class MapParser
    {
        //private static string CleanupNew2(string input)
        //{
        //    List<char> toReplace = new List<char>();
        //    toReplace.Add('\u0001');
        //    toReplace.Add('\u0002');
        //    toReplace.Add('\u0005');
        //    toReplace.Add('\u0006');
        //    toReplace.Add('\u0011');
        //    toReplace.Add('\u0003');
        //    toReplace.Add('\u0004');
        //    toReplace.Add('\u0015');
        //    toReplace.Add('\u0016');
        //    toReplace.Add('\u000e');
        //    toReplace.Add('\u001a');
        //    toReplace.Add('\u0010');
        //    toReplace.Add('\u000f');
        //    toReplace.Add('\v');
        //    toReplace.Add('\n');
        //    toReplace.Add('\t');
        //    toReplace.Add('\u001e');
        //    toReplace.Add('\r');
        //    toReplace.Add('\f');
        //    toReplace.Add('\a');
        //    //toReplace.Add('\0');
        //    toReplace.Add('\b');
        //    toReplace.Add('\u0014');
        //    toReplace.Add('\u0013');
        //    toReplace.Add('\u0012');
        //    toReplace.Add('\u001b');

        //    string buffer = input;
        //    buffer = buffer.Replace("\0", "");
        //    foreach (char c in toReplace)
        //    {
        //        buffer = buffer.Replace(c, ';');
        //    }

        //    return buffer;
        //}

        private static string CleanupNew(string input)
        {
            List<char> toReplace = new List<char>();
            toReplace.Add('\u0001');
            toReplace.Add('\u0002');
            toReplace.Add('\u0003');
            toReplace.Add('\u0004');
            toReplace.Add('\u0005');
            toReplace.Add('\u0006');

            toReplace.Add('\u000e');
            toReplace.Add('\u000f');
            toReplace.Add('\u0010');
            toReplace.Add('\u0011');
            toReplace.Add('\u0012');
            toReplace.Add('\u0013');
            toReplace.Add('\u0014');
            toReplace.Add('\u0015');
            toReplace.Add('\u0016');

            toReplace.Add('\u0018');
            toReplace.Add('\u0019');
            toReplace.Add('\u001a');
            toReplace.Add('\u001b');
            toReplace.Add('\u001c');

            toReplace.Add('\u001e');

            // tests
            toReplace.Add('\u0007');
            toReplace.Add('\u0008');
            toReplace.Add('\u0009');
            toReplace.Add('\u0017');
            toReplace.Add('\u001d');
            toReplace.Add('\u001f');

            toReplace.Add('\v');
            toReplace.Add('\n');
            toReplace.Add('\t');
            toReplace.Add('\r');
            toReplace.Add('\f');
            toReplace.Add('\a');
            //toReplace.Add('\0');
            toReplace.Add('\b');

            string buffer = input;
            buffer = buffer.Replace("\0", "");
            //buffer = buffer.Replace(" ", "");
            // punkerich bug
            if (buffer.IndexOf(";") > 0)
            {
                buffer = buffer.Replace(";", "");
            }
            if (buffer.IndexOf("\'") > 0)
            {
                buffer = buffer.Replace("\'", "");
            }
            // fuzzd bug
            if (buffer.IndexOf("%") > 0)
            {
                buffer = buffer.Replace("%", "");
            }
            if (buffer.IndexOf("(") > 0)
            {
                buffer = buffer.Replace("(", "");
            }
            if (buffer.IndexOf(")") > 0)
            {
                buffer = buffer.Replace(")", "");
            }
            foreach (char c in toReplace)
            {
                buffer = buffer.Replace(c, ';');
            }

            return buffer;
        }

        //public static string Parse_EDITOR_FileNew(string mapFilePath)
        //{
        //    if (String.IsNullOrEmpty(mapFilePath) || !mapFilePath.Contains("EDITOR"))
        //    {
        //        return null;
        //    }

        //    string mapFileContent = File.ReadAllText(mapFilePath);
        //    Dictionary<string, string> mapCharacteristics = new Dictionary<string, string>();
        //    NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

        //    // debug
        //    //Console.WriteLine(mapFileContent);

        //    string buffer = CleanupNew(mapFileContent);
        //    string[] columns = buffer.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (columns.Length != 27 && columns.Length != 28 && columns.Length != 33 && columns.Length != 34)
        //    {
        //        if (columns.Length > 34 && columns.Length <= 45)
        //        {
        //            // Ultimate try
        //        }
        //        else
        //        {
        //            throw new FormatException("This maps seems incompatible, or parsing was wrong (step 2) : " + mapFilePath + " - If you have multiple lines in your description or special characters, parsing may fail");
        //        }
        //    }

        //    foreach (string column in columns)
        //    {
        //        if (column == "Id"
        //            || column == "VersionNumber"
        //            || column == "Name"
        //            || column == "Author"
        //            || column == "Description"
        //            || column == "DateCreated"
        //            || column == "DateEdited"
        //            || column == "GameVersionNumber")
        //        {
        //            mapCharacteristics.Add(column, "");
        //        }
        //    }

        //    if (columns.Length > 34 && columns.Length <= 45)
        //    {
        //        bool ok = true;

        //        // Ultimate try
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];

        //        try
        //        {
        //            Guid test = Guid.Parse(mapCharacteristics["Id"].Substring(1, mapCharacteristics["Id"].Length - 2));
        //        }
        //        catch { ok = false; }

        //        // try to read editor version
        //        double editorVersionNumberTest = 0;
        //        if (mapCharacteristics.ContainsKey("VersionNumber"))
        //        {
        //            string versionNumberString = mapCharacteristics["VersionNumber"];
        //            if (!String.IsNullOrEmpty(versionNumberString)
        //                && versionNumberString.Count(f => f == '.') == 2)
        //            {
        //                Double.TryParse(versionNumberString.Substring(0, versionNumberString.LastIndexOf('.')).Replace(".", nfi.NumberDecimalSeparator), out editorVersionNumberTest);
        //            }
        //        }
        //        if (editorVersionNumberTest == 0)
        //            ok = false;

        //        bool createdfound = false;
        //        bool editedfound = false;
        //        CultureInfo provider = CultureInfo.InvariantCulture;
        //        string format = "MM/dd/yyyy";
        //        for (int currentColumn = 31; currentColumn < columns.Length; currentColumn++)
        //        {
        //            try
        //            {
        //                DateTime test = DateTime.ParseExact(columns[currentColumn], format, provider);
        //                if (!createdfound)
        //                {
        //                    mapCharacteristics["DateCreated"] = columns[currentColumn];
        //                    createdfound = true;
        //                }
        //                else if (!editedfound)
        //                {
        //                    mapCharacteristics["DateEdited"] = columns[currentColumn];
        //                    editedfound = true;
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //            }
        //        }
        //        if (!createdfound || !editedfound)
        //            ok = false;

        //        if (!ok)
        //        {
        //            throw new FormatException("This maps seems incompatible, or parsing was wrong (step 2) : " + mapFilePath + "\nIf you have multiple lines in your description or special characters, parsing may fail");
        //        }
        //    }

        //    if (columns.Length == 27)
        //    {
        //        mapCharacteristics["Id"] = columns[16];
        //        mapCharacteristics["VersionNumber"] = columns[17];
        //        mapCharacteristics["Name"] = columns[21];
        //        mapCharacteristics["Author"] = columns[22];
        //        mapCharacteristics["Description"] = columns[23];
        //        mapCharacteristics["DateCreated"] = columns[24];
        //        mapCharacteristics["DateEdited"] = columns[25];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 28)
        //    {
        //        mapCharacteristics["Id"] = columns[16];
        //        mapCharacteristics["VersionNumber"] = columns[17];
        //        mapCharacteristics["Name"] = columns[21];
        //        mapCharacteristics["Author"] = columns[22];
        //        mapCharacteristics["Description"] = columns[23];
        //        mapCharacteristics["DateCreated"] = columns[24];
        //        mapCharacteristics["DateEdited"] = columns[25];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 33)
        //    {
        //        // Procedural ?
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];
        //        mapCharacteristics["DateCreated"] = columns[31];
        //        mapCharacteristics["DateEdited"] = columns[32];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 34)
        //    {
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];
        //        mapCharacteristics["DateCreated"] = columns[31];
        //        mapCharacteristics["DateEdited"] = columns[32];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    // try to read editor version
        //    double versionNumber = 1.0;
        //    if (mapCharacteristics.ContainsKey("VersionNumber"))
        //    {
        //        string versionNumberString = mapCharacteristics["VersionNumber"];
        //        if (!String.IsNullOrEmpty(versionNumberString)
        //            && versionNumberString.Count(f => f == '.') == 2)
        //        {
        //            Double.TryParse(versionNumberString.Substring(0, versionNumberString.LastIndexOf('.')).Replace(".", nfi.NumberDecimalSeparator), out versionNumber);
        //        }
        //    }

        //    string key = mapCharacteristics["Name"] + " (by " + mapCharacteristics["Author"] + ") " + mapCharacteristics["VersionNumber"];
        //    //Log("Map name found : " + key);
        //    return key;
        //}

        public static string Parse_EDITOR_File_revamp(string mapFilePath)
        {
            if (String.IsNullOrEmpty(mapFilePath) || !mapFilePath.Contains("EDITOR"))
            {
                return null;
            }

            string mapFileContent = File.ReadAllText(mapFilePath);
            Dictionary<string, string> mapCharacteristics = new Dictionary<string, string>();
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

            // debug
            //Console.WriteLine(mapFileContent);

            string buffer = CleanupNew(mapFileContent);
            string[] columns = buffer.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // iterate the values until finding the map ID
            try
            {
                // Get map characteristics columns
                Guid islandId = Guid.Empty;
                int idIndex = 0;
                foreach (string column in columns)
                {
                    string maybeGuid = column;
                    if (maybeGuid.Length >= 32)
                    {
                        maybeGuid = maybeGuid.TrimStart(new char[] { '$' });
                        if (maybeGuid.Length > 36)
                            maybeGuid = maybeGuid.Substring(0, 36);

                        if (Guid.TryParseExact(maybeGuid, "D", out islandId))
                        {
                            break;
                        }
                    }
                    // specific case for internal maps
                    if (maybeGuid.Contains("INTERNAL_PROCEDURAL"))
                    {
                        break;
                    }

                    if (!mapCharacteristics.ContainsKey(column))
                    {
                        mapCharacteristics.Add(column, "");
                    }
                    else
                    {

                    }
                    idIndex++;
                }

                int versionIndex = idIndex + 1;
                // indexes to look for
                int nameIndex = -1;
                int authorIndex = -1;
                int dateCreatedIndex = -1;
                int dateEditedIndex = -1;

                if (columns.Length > versionIndex
                    && mapCharacteristics.ContainsKey("Id"))
                    mapCharacteristics["Id"] = columns[idIndex];

                if (columns.Length > versionIndex
                    && mapCharacteristics.ContainsKey("VersionNumber"))
                    mapCharacteristics["VersionNumber"] = columns[versionIndex];

                // try to read editor version
                double versionNumber = 0.0;
                if (mapCharacteristics.ContainsKey("VersionNumber"))
                {
                    string versionNumberString = mapCharacteristics["VersionNumber"];
                    if (!String.IsNullOrEmpty(versionNumberString)
                        && versionNumberString.Count(f => f == '.') == 2)
                    {
                        Double.TryParse(versionNumberString.Substring(0, versionNumberString.LastIndexOf('.')).Replace(".", nfi.NumberDecimalSeparator), out versionNumber);
                    }
                }
                if (versionNumber == 0)
                    throw new FormatException();

                // second loop to get values
                bool createdfound = false;
                bool editedfound = false;
                CultureInfo provider = CultureInfo.InvariantCulture;
                string format = "MM/dd/yyyy";
                for (int currentColumn = idIndex; currentColumn < columns.Length; currentColumn++)
                {
                    try
                    {
                        // if it's match is 90% the name
                        string currentValue = columns[currentColumn];
                        string theRegex = "^(?=.*[A-Z])[A-Z 0-9:_\\-]+$";
                        string theNewRegex = "^(?=.*[A-Z])[A-Z 0-9:_\\-\\.\\,\\!ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝÞ]+$";
                        if (Regex.IsMatch(currentValue, theNewRegex)
                            && !currentValue.Contains("INTERNAL_PROCEDURAL") 
                            && !currentValue.Contains("DESC_PROCEDURAL"))
                        {
                            if (mapCharacteristics.ContainsKey("Name"))
                                mapCharacteristics["Name"] = currentValue;
                            nameIndex = currentColumn;
                            continue;
                        }

                        if (currentColumn == nameIndex + 1)
                        {
                            if (mapCharacteristics.ContainsKey("Author"))
                                mapCharacteristics["Author"] = currentValue;
                            authorIndex = currentColumn;
                            continue;
                        }

                        string maybeDate = currentValue;
                        if (maybeDate.Length == 9)
                        {
                            maybeDate = maybeDate.PadLeft(10, '0');
                        }
                        if (maybeDate.Length == 10)
                        {
                            DateTime test = DateTime.MinValue;
                            if (DateTime.TryParseExact(maybeDate, format, provider, DateTimeStyles.AssumeLocal, out test))
                            {
                                if (!createdfound)
                                {
                                    mapCharacteristics["DateCreated"] = maybeDate;
                                    createdfound = true;
                                    dateCreatedIndex = currentColumn;
                                }
                                else if (!editedfound)
                                {
                                    mapCharacteristics["DateEdited"] = maybeDate;
                                    editedfound = true;
                                    dateEditedIndex = currentColumn;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        throw new FormatException("This maps seems incompatible, or parsing was wrong : " + mapFilePath);
                    }
                }

                // special case for procedural
                if (mapCharacteristics["Name"].Contains("MAP_NAME_"))
                {
                    mapCharacteristics["Name"] = mapCharacteristics["Id"];
                }
            }
            catch (Exception e)
            {
                throw new FormatException("This maps seems incompatible, or parsing was wrong : " + mapFilePath);
            }

            if (String.IsNullOrEmpty(mapCharacteristics["Name"])
                || String.IsNullOrEmpty(mapCharacteristics["Author"]))
            {
                throw new FormatException("This maps seems incompatible, or parsing was wrong : " + mapFilePath);
            }

            string key = mapCharacteristics["Name"] + " (by " + mapCharacteristics["Author"] + ") " + mapCharacteristics["VersionNumber"];
            //Log("Map name found : " + key);
            return key;
        }

        public static string Parse_EDITOR_FileDeserialize(string mapFilePath)
        {
            try
            {
                Dictionary<string, string> mapCharacteristics = new Dictionary<string, string>();

                var surrSel = new SurrogateSelector();
                surrSel.AddSurrogate(typeof(ProxyMapEditorData),
                    new StreamingContext(StreamingContextStates.All), new SurrogateMapEditorDataConstructor());

                using (FileStream fileStream = new FileStream(mapFilePath, FileMode.Open))
                {
                    var formatter2 = new BinaryFormatter();
                    formatter2.Binder = new DeserializeBinder();
                    formatter2.SurrogateSelector = surrSel;
                    var deser = formatter2.Deserialize(fileStream) as ProxyMapEditorData;
                    foreach (var c in deser.Dump())
                    {
                        //Console.WriteLine("{0} = {1}", c.Key, c.Value);
                        mapCharacteristics.Add(c.Key, c.Value != null ? c.Value.ToString() : "");
                    }
                }

                string key = mapCharacteristics["Name"] + " (by " + mapCharacteristics["Author"] + ") " + mapCharacteristics["GameVersionNumber"];
                return key;
            }
            catch (Exception e)
            {
                throw new FormatException("This maps seems incompatible, or parsing was wrong : " + mapFilePath);
            }
        }

        #region code backup

        //public static string Parse_EDITOR_FileNew2(string mapFilePath)
        //{
        //    if (String.IsNullOrEmpty(mapFilePath) || !mapFilePath.Contains("EDITOR"))
        //    {
        //        return null;
        //    }

        //    string mapFileContent = File.ReadAllText(mapFilePath);
        //    Dictionary<string, string> mapCharacteristics = new Dictionary<string, string>();

        //    // debug
        //    //Console.WriteLine(mapFileContent);

        //    string buffer = CleanupNew(mapFileContent);
        //    string[] columns = buffer.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (columns.Length != 27 && columns.Length != 28 && columns.Length != 33 && columns.Length != 34)
        //    {
        //        //Log("This maps seems incompatible, or parsing was wrong (step 2) : " + mapFilePath);
        //        //return null;
        //        throw new FormatException("This maps seems incompatible, or parsing was wrong (step 2) : " + mapFilePath);
        //    }

        //    foreach (string column in columns)
        //    {
        //        if (column == "Id"
        //            || column == "VersionNumber"
        //            || column == "Name"
        //            || column == "Author"
        //            || column == "Description"
        //            || column == "DateCreated"
        //            || column == "DateEdited"
        //            || column == "GameVersionNumber")
        //        {
        //            mapCharacteristics.Add(column, "");
        //        }
        //    }

        //    if (columns.Length == 27)
        //    {
        //        mapCharacteristics["Id"] = columns[16];
        //        mapCharacteristics["VersionNumber"] = columns[17];
        //        mapCharacteristics["Name"] = columns[21];
        //        mapCharacteristics["Author"] = columns[22];
        //        mapCharacteristics["Description"] = columns[23];
        //        mapCharacteristics["DateCreated"] = columns[24];
        //        mapCharacteristics["DateEdited"] = columns[25];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 28)
        //    {
        //        mapCharacteristics["Id"] = columns[16];
        //        mapCharacteristics["VersionNumber"] = columns[17];
        //        mapCharacteristics["Name"] = columns[21];
        //        mapCharacteristics["Author"] = columns[22];
        //        mapCharacteristics["Description"] = columns[23];
        //        mapCharacteristics["DateCreated"] = columns[24];
        //        mapCharacteristics["DateEdited"] = columns[25];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 33)
        //    {
        //        // Procedural ?
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];
        //        mapCharacteristics["DateCreated"] = columns[31];
        //        mapCharacteristics["DateEdited"] = columns[32];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    else if (columns.Length == 34)
        //    {
        //        mapCharacteristics["Id"] = columns[19];
        //        mapCharacteristics["VersionNumber"] = columns[20];
        //        mapCharacteristics["Name"] = columns[28];
        //        mapCharacteristics["Author"] = columns[29];
        //        mapCharacteristics["Description"] = columns[30];
        //        mapCharacteristics["DateCreated"] = columns[31];
        //        mapCharacteristics["DateEdited"] = columns[32];
        //        //mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //    }
        //    // try to read editor version
        //    double versionNumber = 1.0;
        //    if (mapCharacteristics.ContainsKey("VersionNumber"))
        //    {
        //        string versionNumberString = mapCharacteristics["VersionNumber"];
        //        if (!String.IsNullOrEmpty(versionNumberString)
        //            && versionNumberString.Count(f => f == '.') == 2)
        //        {
        //            Double.TryParse(versionNumberString.Substring(0, versionNumberString.LastIndexOf('.')), out versionNumber);
        //        }
        //    }

        //    string key = mapCharacteristics["Name"] + " (by " + mapCharacteristics["Author"] + ") " + mapCharacteristics["VersionNumber"];
        //    //Log("Map name found : " + key);
        //    return key;
        //}

        //private string Cleanup(string input)
        //{
        //    #region old method

        //    string buffer = input;

        //    foreach (char c in input)
        //    {
        //        if (!Char.IsLetterOrDigit(c) && c != '-' && c != '.' && c != ' ' && c != '/' && c != '$' && c != '+' && c != '\'')
        //        {
        //            buffer = buffer.Replace(c, '*');
        //        }
        //    }

        //    #endregion

        //    return buffer;
        //}

        //private string Parse_EDITOR_File(string mapFilePath)
        //{
        //    if (String.IsNullOrEmpty(mapFilePath) || !mapFilePath.Contains("EDITOR"))
        //    {
        //        return null;
        //    }

        //    string mapFileContent = File.ReadAllText(mapFilePath);
        //    // debug
        //    //Console.WriteLine(mapFileContent);

        //    string[] chunks = mapFileContent.Split('$');
        //    if (chunks.Count() != 2 && chunks.Count() != 4)
        //    {
        //        Log("This maps seems incompatible, or parsing was wrong (step 1) : " + mapFilePath);
        //        return null;
        //    }
        //    else
        //    {
        //        Dictionary<string, string> mapCharacteristics = new Dictionary<string, string>();

        //        // Old editor format
        //        if (chunks.Count() == 2)
        //        {
        //            string buffer = Cleanup(chunks[0]);
        //            string[] columns = buffer.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
        //            foreach (string column in columns)
        //            {
        //                if (column.Contains("Assembly")
        //                    || column.Contains(" Version")
        //                    || column.Contains("0.0.0.0")
        //                    || column.Contains("Culture")
        //                    || column.Contains("neutral")
        //                    || column.Contains("null")
        //                    || column.Contains("PublicKeyToken")
        //                    || column.Contains("MapEditorData")
        //                    || column.Contains("LargePreviewImageBytes")
        //                    || column.Contains("Biome")
        //                    || column.Contains("Zone+BiomeType")
        //                    || column.Contains("Beam.Terrain.MapEditorData")
        //                    || column.Contains("Status")
        //                    )
        //                {
        //                    continue;
        //                }
        //                mapCharacteristics.Add(column, "");
        //            }

        //            buffer = Cleanup(chunks[1]);
        //            string[] values = buffer.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

        //            mapCharacteristics["Id"] = values[0];
        //            mapCharacteristics["VersionNumber"] = values[1];
        //            mapCharacteristics["Name"] = values[4];
        //            mapCharacteristics["Author"] = values[5];
        //            mapCharacteristics["Description"] = values[6];
        //            mapCharacteristics["DateCreated"] = values[7];
        //            mapCharacteristics["DateEdited"] = values[8];
        //            mapCharacteristics["GameVersionNumber"] = OLD_EDITOR;
        //        }
        //        // new editor format conversion
        //        else if (chunks.Count() == 4)
        //        {
        //            string buffer = Cleanup(chunks[0]);
        //            string[] columns = buffer.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
        //            foreach (string column in columns)
        //            {
        //                if (column.Contains("Assembly")
        //                    || column.Contains(" Version")
        //                    || column.Contains("0.0.0.0")
        //                    || column.Contains("Culture")
        //                    || column.Contains("neutral")
        //                    || column.Contains("null")
        //                    || column.Contains("PublicKeyToken")
        //                    || column.Contains("MapEditorData")
        //                    || column.Contains("LargePreviewImageBytes")
        //                    || column.Contains("Biome")
        //                    || column.Contains("Zone+BiomeType")
        //                    || column.Contains("Beam.Terrain.MapEditorData")
        //                    || column.Contains("Status")
        //                    )
        //                {
        //                    continue;
        //                }
        //                mapCharacteristics.Add(column, "");
        //            }

        //            buffer = Cleanup(chunks[1]);
        //            columns = buffer.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
        //            foreach (string column in columns)
        //            {
        //                if (column.Contains("Assembly")
        //                    || column.Contains(" Version")
        //                    || column.Contains("0.0.0.0")
        //                    || column.Contains("Culture")
        //                    || column.Contains("neutral")
        //                    || column.Contains("null")
        //                    || column.Contains("PublicKeyToken")
        //                    || column.Contains("MapEditorData")
        //                    || column.Contains("LargePreviewImageBytes")
        //                    || column.Contains("Biome")
        //                    || column.Contains("Zone+BiomeType")
        //                    || column.Contains("Beam.Terrain.MapEditorData")
        //                    || column.Contains("Status")
        //                    )
        //                {
        //                    continue;
        //                }
        //                mapCharacteristics.Add(column, "");
        //            }

        //            buffer = Cleanup(chunks[2]);
        //            string[] values = buffer.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

        //            mapCharacteristics["Id"] = values[0];
        //            mapCharacteristics["VersionNumber"] = values[1];

        //            buffer = Cleanup(chunks[3]);
        //            values = buffer.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

        //            mapCharacteristics["Name"] = values[4];
        //            mapCharacteristics["Author"] = values[5];
        //            mapCharacteristics["Description"] = values[6];
        //            mapCharacteristics["DateCreated"] = values[7];
        //            mapCharacteristics["DateEdited"] = values[8];
        //        }

        //        // try to read editor version
        //        double versionNumber = 1.0;
        //        if (mapCharacteristics.ContainsKey("VersionNumber"))
        //        {
        //            string versionNumberString = mapCharacteristics["VersionNumber"];
        //            if (!String.IsNullOrEmpty(versionNumberString)
        //                && versionNumberString.Count(f => f == '.') == 2)
        //            {
        //                Double.TryParse(versionNumberString.Substring(0, versionNumberString.LastIndexOf('.')), out versionNumber);
        //            }
        //        }
        //        // compatibility issue
        //        if (versionNumber >= 0.17)
        //        {
        //            string key = mapCharacteristics["Name"] + " (by " + mapCharacteristics["Author"] + ")" + mapCharacteristics["GameVersionNumber"];
        //            Log("Map name found : " + key);
        //            return key;
        //        }
        //        else
        //        {
        //            Log("This maps seems incompatible, or parsing was wrong (step 2) : " + mapFilePath);
        //            return null;
        //        }
        //    }
        //}

        #endregion
    }

    internal class ProxyMapEditorData
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Object GetData(string name)
        {
            if (data.ContainsKey(name))
            {
                return data[name];
            }
            return null;
        }
        public void SetData(string name, object value)
        {
            data[name] = value;
        }

        public IEnumerable<KeyValuePair<string, object>> Dump()
        {
            return data;
        }
    }

    internal class SurrogateMapEditorDataConstructor : ISerializationSurrogate
    {
        private ProxyMapEditorData mProxy;
        /// <summary>
        /// Populates the provided <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the object.
        /// </summary>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populates the object using the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo"/>.
        /// </summary>
        /// <returns>
        /// The populated deserialized object.
        /// </returns>
        /// <param name="obj">The object to populate. </param>
        /// <param name="info">The information to populate the object. </param>
        /// <param name="context">The source from which the object is deserialized. </param>
        /// <param name="selector">The surrogate selector where the search for a compatible surrogate begins. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            if (mProxy == null) mProxy = new ProxyMapEditorData();
            var en = info.GetEnumerator();
            while (en.MoveNext())
            {
                mProxy.SetData(en.Current.Name, en.Current.Value);
            }
            return mProxy;
        }
    }

    internal sealed class DeserializeBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            return typeof(ProxyMapEditorData);
        }
    }
}
