using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using BGK.SaveSystem.Serialization;

//Bence's Game Kit
namespace BGK.SaveSystem
{
    public static class SaveSys
    {
        public static void Save(object input, SerializationSurrogate[] custom, string path, string name)
        {
            BinaryFormatter f = Formatter(custom);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream stream = new FileStream(path + "/" + name, FileMode.Create);
            f.Serialize(stream, input);
            stream.Close();
            RefreshSaveFileList(path, name);
        }

        public static void Load(out object output, SerializationSurrogate[] custom, string path, string name)
        {
            if (!File.Exists(path + "/" + name))
            {
                Debug.LogError("Save file does not exist at this location: " + path + "/" + name);
                output = null;
                return;
            }

            BinaryFormatter f = Formatter(custom);
            FileStream stream = new FileStream(path + "/" + name, FileMode.Open);

            try
            {
                output = f.Deserialize(stream);
            }
            catch
            {
                Debug.LogError("Failed to load save file at this location: " + path + "/" + name);
                output = null;
            }
            stream.Close();
        }

        public static bool CheckSaveFile(string path, string name)
        {
            return File.Exists(path + "/" + name);
        }

        public static void DeleteSaveFile(string path, string name)
        {
            if (!File.Exists(path + "/" + name))
            {
                Debug.LogError("Save file does not exist at this location: " + path + "/" + name);
                return;
            }

            try
            {
                File.Delete(path + "/" + name);
            }
            catch
            {
                Debug.LogError("Failed to delete save file at this location: " + path + "/" + name);
            }
            RefreshSaveFileList();
        }

        public static void DeleteAllSaveFile()
        {
            string[,] list = GetSaveFileList();
            for (int i = 0; i < list.GetLength(0); i++)
            {
                DeleteSaveFile(list[i, 0], list[i, 1]);
            }
            RefreshSaveFileList();
        }

        public static BinaryFormatter Formatter(SerializationSurrogate[] custom)
        {
            BinaryFormatter f = new BinaryFormatter();
            SurrogateSelector s = new SurrogateSelector();
            StreamingContext all = new StreamingContext(StreamingContextStates.All);

            s.AddSurrogate(typeof(Color), all, new ColorSS());
            s.AddSurrogate(typeof(Vector2), all, new Vector2SS());
            s.AddSurrogate(typeof(Vector3), all, new Vector3SS());
            s.AddSurrogate(typeof(Vector4), all, new Vector4SS());
            s.AddSurrogate(typeof(Quaternion), all, new QuaternionSS());
            s.AddSurrogate(typeof(Mesh), all, new MeshSS());

            if (custom != null)
            {
                foreach (SerializationSurrogate i in custom)
                {
                    try
                    {
                        s.AddSurrogate(i.type, all, i.surrogate);
                    }
                    catch
                    {
                        Debug.LogError("Failed to use custom Serialization Surrogate " + i.surrogate + " for " + i.type);
                    }
                }
            }

            f.SurrogateSelector = s;
            return f;
        }

        private static void RefreshSaveFileList(string path, string name)
        {
            BinaryFormatter f = Formatter(null);
            string[,] output = null;

            if (File.Exists(Application.persistentDataPath + "/BGK.dat"))
            {
                FileStream stream1 = new FileStream(Application.persistentDataPath + "/BGK.dat", FileMode.Open);

                try
                {
                    output = f.Deserialize(stream1) as string[,];
                }
                catch
                {
                    Debug.LogError("Failed to refresh save file list.");
                    stream1.Close();
                    return;
                }
                stream1.Close();
            }
            bool n = true;
            string[,] list;

            if (output != null)
            {
                for (int i = 0; i < output.GetLength(0); i++)
                {
                    if (output[i, 0] == path && output[i, 1] == name)
                    {
                        n = false;
                        break;
                    }
                }
            }

            if (n)
            {
                if (output != null)
                {
                    list = new string[output.GetLength(0) + 1, 2];
                    for (int i = 0; i < output.GetLength(0); i++)
                    {
                        list[i, 0] = output[i, 0];
                        list[i, 1] = output[i, 1];
                    }
                }
                else
                {
                    list = new string[1, 2];
                }
                list[list.GetLength(0) - 1, 0] = path;
                list[list.GetLength(0) - 1, 1] = name;
            }
            else
            {
                list = output;
            }

            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
            }
            FileStream stream = new FileStream(Application.persistentDataPath + "/BGK.dat", FileMode.Create);
            f.Serialize(stream, list);
            stream.Close();

            RefreshSaveFileList();
        }

        private static void RefreshSaveFileList()
        {
            BinaryFormatter f = Formatter(null);
            string[,] output = null;

            if (File.Exists(Application.persistentDataPath + "/BGK.dat"))
            {
                FileStream stream1 = new FileStream(Application.persistentDataPath + "/BGK.dat", FileMode.Open);

                try
                {
                    output = f.Deserialize(stream1) as string[,];
                }
                catch
                {
                    Debug.LogError("Failed to refresh save file list.");
                    stream1.Close();
                    return;
                }
                stream1.Close();
            }

            if (output != null)
            {
                int num = 0;

                for (int i = 0; i < output.GetLength(0); i++)
                {
                    if (File.Exists(output[i, 0] + "/" + output[i, 1]))
                    {
                        num++;
                    }
                }

                string[,] list = new string[num, 2];
                num = 0;

                for (int i = 0; i < output.GetLength(0); i++)
                {
                    if (File.Exists(output[i, 0] + "/" + output[i, 1]))
                    {
                        list[num, 0] = output[i, 0];
                        list[num, 1] = output[i, 1];
                        num++;
                    }
                }

                if (!Directory.Exists(Application.persistentDataPath))
                {
                    Directory.CreateDirectory(Application.persistentDataPath);
                }
                FileStream stream = new FileStream(Application.persistentDataPath + "/BGK.dat", FileMode.Create);
                f.Serialize(stream, list);
                stream.Close();
            }
        }

        public static void GetSaveFileList(out string[,] output)
        {
            BinaryFormatter f = Formatter(null);

            if (!File.Exists(Application.persistentDataPath + "/BGK.dat"))
            {
                Debug.LogError("The list of save files does not exist.");
                output = null;
                return;
            }

            FileStream stream1 = new FileStream(Application.persistentDataPath + "/BGK.dat", FileMode.Open);

            try
            {
                output = f.Deserialize(stream1) as string[,];
            }
            catch
            {
                Debug.LogError("Failed to get save file list.");
                output = null;
            }
            stream1.Close();
        }

        public static string[,] GetSaveFileList()
        {
            string[,] a;
            GetSaveFileList(out a);
            return a;
        }

        public static bool CheckSaveFile(string name)
        {
            return CheckSaveFile(Application.persistentDataPath, name);
        }

        public static void DeleteSaveFile(string name)
        {
            DeleteSaveFile(Application.persistentDataPath, name);
        }

        public static void Save(object input, string path, string name)
        {
            Save(input, null, path, name);
        }

        public static void Load(out object output, string path, string name)
        {
            Load(out output, null, path, name);
        }

        public static void Save(object input, SerializationSurrogate[] custom, string name)
        {
            Save(input, custom, Application.persistentDataPath, name);
        }

        public static void Load(out object output, SerializationSurrogate[] custom, string name)
        {
            Load(out output, custom, Application.persistentDataPath, name);
        }

        public static object Load(SerializationSurrogate[] custom, string path, string name)
        {
            object output;
            Load(out output, custom, path, name);
            return output;
        }

        public static object Load(SerializationSurrogate[] custom, string name)
        {
            object output;
            Load(out output, custom, Application.persistentDataPath, name);
            return output;
        }

        public static void Save(object input, string name)
        {
            Save(input, Application.persistentDataPath, name);
        }

        public static void Load(out object output, string name)
        {
            Load(out output, Application.persistentDataPath, name);
        }

        public static object Load(string path, string name)
        {
            object output;
            Load(out output, path, name);
            return output;
        }

        public static object Load(string name)
        {
            object output;
            Load(out output, Application.persistentDataPath, name);
            return output;
        }
    }

    [System.Serializable]
    public class SerializationSurrogate
    {
        public System.Type type;
        public ISerializationSurrogate surrogate;

        public SerializationSurrogate()
        {

        }
    }

    namespace Serialization
    {
        public class ColorSS : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Color c = (Color)obj;
                info.AddValue("r", c.r);
                info.AddValue("g", c.g);
                info.AddValue("b", c.b);
                info.AddValue("a", c.a);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Color c = (Color)obj;
                c.r = (float)info.GetValue("r", typeof(float));
                c.g = (float)info.GetValue("g", typeof(float));
                c.b = (float)info.GetValue("b", typeof(float));
                c.a = (float)info.GetValue("a", typeof(float));
                obj = c;
                return obj;
            }
        }

        public class Vector2SS : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector2 v2 = (Vector2)obj;
                info.AddValue("x", v2.x);
                info.AddValue("y", v2.y);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector2 v2 = (Vector2)obj;
                v2.x = (float)info.GetValue("x", typeof(float));
                v2.y = (float)info.GetValue("y", typeof(float));
                obj = v2;
                return obj;
            }
        }

        public class Vector3SS : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector3 v3 = (Vector3)obj;
                info.AddValue("x", v3.x);
                info.AddValue("y", v3.y);
                info.AddValue("z", v3.z);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector3 v3 = (Vector3)obj;
                v3.x = (float)info.GetValue("x", typeof(float));
                v3.y = (float)info.GetValue("y", typeof(float));
                v3.z = (float)info.GetValue("z", typeof(float));
                obj = v3;
                return obj;
            }
        }

        public class Vector4SS : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector4 q = (Vector4)obj;
                info.AddValue("x", q.x);
                info.AddValue("y", q.y);
                info.AddValue("z", q.z);
                info.AddValue("w", q.w);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Vector4 q = (Vector4)obj;
                q.x = (float)info.GetValue("x", typeof(float));
                q.y = (float)info.GetValue("y", typeof(float));
                q.z = (float)info.GetValue("z", typeof(float));
                q.w = (float)info.GetValue("w", typeof(float));
                obj = q;
                return obj;
            }
        }

        public class QuaternionSS : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Quaternion q = (Quaternion)obj;
                info.AddValue("x", q.x);
                info.AddValue("y", q.y);
                info.AddValue("z", q.z);
                info.AddValue("w", q.w);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Quaternion q = (Quaternion)obj;
                q.x = (float)info.GetValue("x", typeof(float));
                q.y = (float)info.GetValue("y", typeof(float));
                q.z = (float)info.GetValue("z", typeof(float));
                q.w = (float)info.GetValue("w", typeof(float));
                obj = q;
                return obj;
            }
        }

        public class MeshSS : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Mesh m = (Mesh)obj;
                info.AddValue("vertices", m.vertices);
                info.AddValue("uv", m.uv);
                info.AddValue("uv2", m.uv2);
                info.AddValue("uv3", m.uv3);
                info.AddValue("uv4", m.uv4);
                info.AddValue("uv5", m.uv5);
                info.AddValue("uv6", m.uv6);
                info.AddValue("uv7", m.uv7);
                info.AddValue("uv8", m.uv8);
                info.AddValue("triangles", m.triangles);
                info.AddValue("tangents", m.tangents);
                info.AddValue("colors", m.colors);
                info.AddValue("normals", m.normals);
                info.AddValue("name", m.name);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Mesh m = (Mesh)obj;
                m.Clear();
                m.vertices = (Vector3[])info.GetValue("vertices", typeof(Vector3[]));
                m.uv = (Vector2[])info.GetValue("uv", typeof(Vector2[]));
                m.uv2 = (Vector2[])info.GetValue("uv2", typeof(Vector2[]));
                m.uv3 = (Vector2[])info.GetValue("uv3", typeof(Vector2[]));
                m.uv4 = (Vector2[])info.GetValue("uv4", typeof(Vector2[]));
                m.uv5 = (Vector2[])info.GetValue("uv5", typeof(Vector2[]));
                m.uv6 = (Vector2[])info.GetValue("uv6", typeof(Vector2[]));
                m.uv7 = (Vector2[])info.GetValue("uv7", typeof(Vector2[]));
                m.uv8 = (Vector2[])info.GetValue("uv8", typeof(Vector2[]));
                m.triangles = (int[])info.GetValue("triangles", typeof(int[]));
                m.tangents = (Vector4[])info.GetValue("tangents", typeof(Vector4[]));
                m.colors = (Color[])info.GetValue("colors", typeof(Color[]));
                m.normals = (Vector3[])info.GetValue("normals", typeof(Vector3[]));
                m.name = (string)info.GetValue("name", typeof(string));
                return m;
            }
        }
    }
}
