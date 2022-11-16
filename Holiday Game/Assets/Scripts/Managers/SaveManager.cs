using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void SaveFile(string saveName, GameData data)
    {
        string destination = Application.persistentDataPath + "/" + saveName + ".dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public static GameData LoadFile(string saveName)
    {
        string destination = Application.persistentDataPath + "/" + saveName + ".dat";
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

    public static bool DoesFileExist(string saveName)
    {
        string destination = Application.persistentDataPath + "/" + saveName + ".dat";

        if (File.Exists(destination))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
