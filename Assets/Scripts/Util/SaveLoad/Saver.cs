using System;
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

        try
        {
            using (FileStream fileStream = File.Exists(toSaveTo) == true ? File.OpenWrite(toSaveTo) : File.Create(toSaveTo))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, saveGame);
                fileStream.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(string.Join(Environment.NewLine, e.Message, e.StackTrace));
        }
    }

    /// <summary>
    /// Loads a game from disk.
    /// </summary>
    public static SaveGame Load()
    {
        string toSaveTo = Application.persistentDataPath + "/save.game";

        try
        {
            if (File.Exists(toSaveTo) == false)
                return new SaveGame();

            using (FileStream fileStream = File.OpenRead(toSaveTo))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                SaveGame toRet = formatter.Deserialize(fileStream) as SaveGame;
                fileStream.Close();
                return toRet;

            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(string.Join(Environment.NewLine, e.Message, e.StackTrace));
            return new SaveGame();
        }
    }
}
