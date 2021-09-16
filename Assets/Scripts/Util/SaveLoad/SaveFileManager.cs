using UnityEngine;

/// <summary>
/// Manages the save file.
/// </summary>
public class SaveFileManager : MonoBehaviour
{
    [SerializeField] public SaveGame saveGame;

    private void Awake()
    {
        saveGame = Saver.Load();
    }

    /// <summary>
    /// Saves the game.
    /// </summary>
    public void Save()
    {
        Saver.Save(saveGame);
    }
}
