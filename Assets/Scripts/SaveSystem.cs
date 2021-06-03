using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(Player player) {
        BinaryFormatter formatter = new BinaryFormatter();
        string saveName = PlayerPrefs.GetString("saveslot");

        string path = Application.persistentDataPath + "/" + saveName + ".kindle";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer() {
        string saveName = PlayerPrefs.GetString("saveslot");
        string path = Application.persistentDataPath + "/" + saveName + ".kindle";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            
            return data;
        } else {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeletePlayer() {
        string saveName = PlayerPrefs.GetString("saveslot");
        string path = Application.persistentDataPath + "/" + saveName + ".kindle";
        if (File.Exists(path)) {
            File.Delete(path);
        } else {
            Debug.LogError("Save file not found in " + path);
        }
    }
}
