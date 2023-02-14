using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    private static string path = Application.persistentDataPath + "/" + "saves" + "/slot";

    public static void SaveFile(int slot, GameData data)
    {
        string destination = path + slot + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else 
        {
            //check if directory doesn't exit
            if (!Directory.Exists(Application.persistentDataPath + "/" + "saves"))
            {
                //if it doesn't, create it
                Directory.CreateDirectory(Application.persistentDataPath + "/" + "saves");

            }
            file = File.Create(destination);
        }

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public static GameData LoadFile(int slot)
    {
        string destination = path + slot + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }

        BinaryFormatter bf = new BinaryFormatter();
        GameData data = (GameData)bf.Deserialize(file);
        file.Close();

        return data;
    }

    public static bool DoesFileExist(int slot)
    {
        string destination = path + slot + ".dat";

        if (File.Exists(destination))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static FileInfo[] LoadAllSaves()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/" + "saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + "saves");
        }
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/" + "saves");
        FileInfo[] info = dir.GetFiles("*.dat");
        return info;
    }

    public static void Delete(int slot)
    {
        string destination = path + slot + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File " + destination + " not found");
            return;
        }

        FileInfo info = new FileInfo(file.Name);
        file.Close();
        info.Delete();
    }
}
