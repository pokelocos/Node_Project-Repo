using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DataSystem
{
    /// <summary>
    /// It controls the saving and loading of information, it works using the "Data" class as a container.
    /// The generated files will be saved in the project's base path:
    /// EX: .../AppData/LocalLow/DefaultCompany/GaneName/...
    /// The files will have the extension ".dat" by default.
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// Crea un nuevo archivo en la ruta entrega y lo retorna, si el archivo ya exite intentara cargarlo.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T NewData<T>(string name,string extension = ".dat", string basePath = "")
        {
#if UNITY_WEBGL
            return NewDataToWebGL<T>(name, extension, basePath);
#else
            return NewDataToFile<T>(name, extension, basePath);
#endif

        }

        /// <summary>
        /// Create a new file with the name given by parameters, the system will use
        /// this function if the game is not compiled for "WebGL".
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        private static T NewDataToWebGL<T>(string name,string extension = ".dat", string basePath = "")
        {
            if (basePath.Equals(""))
                basePath = Application.persistentDataPath;

            string path = basePath + "/" + name + extension;
            T newData;

            if (File.Exists(path))
            {
                Debug.LogWarning("[A file with this path already exists]: " + path);
                return LoadData<T>(name);
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = File.Open(path, FileMode.Create);
                newData = Activator.CreateInstance<T>();
                formatter.Serialize(stream, newData);
                stream.Close();
                return newData;
            }

        }

        /// <summary>
        /// Creates a new file with the name given by parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        private static T NewDataToFile<T>(string name, string extension = ".dat", string basePath = "")
        {
            if (basePath.Equals(""))
                basePath = Application.persistentDataPath;

            string path = basePath + "/" + name + extension;
            T newData;

            if (File.Exists(path))
            {
                Debug.LogWarning("[A file with this path already exists]: " + path);
                return LoadData<T>(name);
            }
            else
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Create);
                newData = Activator.CreateInstance<T>();
                formatter.Serialize(stream, newData);
                stream.Close();
                return newData;
            }
        }

        /// <summary>
        /// Save a file, if it exists overwrites it, if not, create it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="name"></param>
        public static T SaveData<T>(T data, string name = "", string extension = ".dat", string basePath = "")
        {
            if (basePath.Equals(""))
                basePath = Application.persistentDataPath;

            string path = basePath + "/" + name + extension;

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Create);
                formatter.Serialize(stream, data);
                stream.Close();
                return data;
            }
            else
            {
                Debug.LogWarning("[there is no saved information]: " + path);
                return NewData<T>(name);
            }
        }

        /// <summary>
        /// Loads a saved file, if it does not exist or is corrupt, it returns the default or null value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T LoadData<T>(string name = "", string extension = ".dat", string basePath = "")
        {
            T instance;

            if (basePath.Equals(""))
                basePath = Application.persistentDataPath;

            string path = basePath + "/" + name + extension;

            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                instance = (T)formatter.Deserialize(stream);
                if (instance == null)
                {
                    instance = Activator.CreateInstance<T>();
                    Debug.LogWarning("[Corrupted file]: " + path);
                    return default(T);
                }
                stream.Close();
                return instance;
            }
            else
            {
                Debug.LogWarning("[The file does not exists]: " + path);
                return default(T);
            }
        }


    }
}