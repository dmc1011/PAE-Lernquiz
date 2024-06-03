using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using Unity.VisualScripting.FullSerializer;

public class JsonDataService
{
    public static string CatalogueDirectory = "/Catalogue";

    public T LoadData<T>(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}");
            throw new FileNotFoundException($"{path} does not exist!");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            Debug.Log("Loaded Catalogue succesfully.");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public bool SaveData<T>(string RelativePath, T Data)
    {
        string path = Application.persistentDataPath + RelativePath;
        string backupPath = path + "/backup";
        try
        {
            if (File.Exists(path))
            {
                // create backup of the previous file
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
                File.Move(path, backupPath);
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));

            // delete backup as save was successful
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message}");

            // If an error occurs, restore the backup file
            if (File.Exists(backupPath))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                File.Move(backupPath, path);
            }

            return false;
        }
    }

    public int CountJsonFilesForDirectory(string RelativePath)
    {
        string path = Application.persistentDataPath + RelativePath;
        try
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }
            string[] jsonFiles = Directory.GetFiles(path, "*.json");
            Debug.Log(jsonFiles);
            return jsonFiles.Length;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}