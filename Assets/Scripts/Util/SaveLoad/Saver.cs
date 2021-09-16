using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Static methods for saving and loading a game.
/// </summary>
public static class Saver
{
    /// <summary>
    /// Saves a given game.
    /// </summary>
    /// <param name="saveGame">The game to be saved.</param>
    public static void Save(SaveGame saveGame)
    {
        string toSaveTo = Application.persistentDataPath + "/save.game";

        using (FileStream fileStream = File.Exists(toSaveTo) == true ? File.OpenWrite(toSaveTo) : File.Create(toSaveTo))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, saveGame);
            fileStream.Close();
        }
    }

    /// <summary>
    /// Loads a game from disk.
    /// </summary>
    public static SaveGame Load()
    {
        string toSaveTo = Application.persistentDataPath + "/save.game";

        if (File.Exists(toSaveTo) == false)
            return null;

        using (FileStream fileStream = File.OpenRead(toSaveTo))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SaveGame toRet = formatter.Deserialize(fileStream) as SaveGame;
            fileStream.Close();
            return toRet;
        }
    }
}
