using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Config.Net;
using Config.Net.Core;

namespace InoriDock.WPF.Services.Config.Helper
{
    public class JsonConfigStoreHelper : IConfigStore, IDisposable
    {
        private readonly string? _pathName;

        private JsonNode? _j;

        public string Name => "json";

        public bool CanRead => true;

        public bool CanWrite => _pathName != null;

        public JsonConfigStoreHelper(string name, bool isFilePath)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (isFilePath)
            {
                _pathName = Path.GetFullPath(name);
                _j = ReadJsonFile(_pathName);
            }
            else
            {
                _j = ReadJsonString(name);
            }
        }

        public void Dispose()
        {
        }

        public string? Read(string rawKey)
        {
            if (string.IsNullOrEmpty(rawKey) || _j == null)
            {
                return null;
            }

            string noLengthPath;
            bool flag = OptionPath.TryStripLength(rawKey, out noLengthPath);
            if (noLengthPath == null)
            {
                return null;
            }

            string[] array = noLengthPath.Split('.');
            if (array.Length == 0)
            {
                return null;
            }

            JsonNode jsonNode = _j;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string noIndexPath;
                int index;
                bool flag2 = OptionPath.TryStripIndex(array2[i], out noIndexPath, out index);
                if (noIndexPath == null)
                {
                    return null;
                }

                jsonNode = jsonNode[noIndexPath];
                if (jsonNode == null)
                {
                    return null;
                }

                if (flag2)
                {
                    if (!(jsonNode is JsonArray jsonArray))
                    {
                        return null;
                    }

                    if (index >= jsonArray.Count)
                    {
                        return null;
                    }

                    jsonNode = jsonArray[index];
                }
            }

            if (flag)
            {
                if (!(jsonNode is JsonArray { Count: var count }))
                {
                    return null;
                }

                return count.ToString();
            }

            return jsonNode.ToString();
        }

        public void Write(string key, string? value)
        {
            if (string.IsNullOrEmpty(_pathName))
            {
                throw new InvalidOperationException("please specify file name for writeable config");
            }

            if (_j == null)
            {
                _j = new JsonObject();
            }

            string[] array = key.Split('.');
            if (array.Length == 0)
            {
                return;
            }

            JsonNode jsonNode = _j;
            string propertyName = null;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string noIndexPath;
                int index;
                bool flag = OptionPath.TryStripIndex(array2[i], out noIndexPath, out index);
                if (noIndexPath == null)
                {
                    return;
                }

                propertyName = noIndexPath;
                JsonNode jsonNode2 = jsonNode[noIndexPath];
                if (flag)
                {
                    throw new NotImplementedException();
                }

                if (jsonNode2 == null)
                {
                    jsonNode2 = jsonNode[noIndexPath] = new JsonObject();
                }

                jsonNode = jsonNode2;
            }

            JsonObject obj = jsonNode.Parent as JsonObject;
            obj.Remove(propertyName);
            obj[propertyName] = JsonValue.Create(value);
            string contents = _j.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder=JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            FileInfo fileInfo = new FileInfo(_pathName);
            if (fileInfo != null && fileInfo.Directory != null)
            {
                fileInfo.Directory.Create();
            }

            File.WriteAllText(_pathName, contents);
        }

        private static JsonNode? ReadJsonFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                return ReadJsonString(File.ReadAllText(fileName));
            }

            return null;
        }

        private static JsonNode? ReadJsonString(string jsonString)
        {
            return JsonNode.Parse(jsonString);
        }
    }
}