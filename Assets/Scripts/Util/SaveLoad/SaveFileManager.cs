using UnityEngine;

/// <summary>
/// Manages the save file.
/// </summary>
public class SaveFileManager : MonoBehaviour
{
    public static SaveFileManager Instance { get; private set; }


    [SerializeField] public SaveGame saveGame;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("SaveFileManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;

        saveGame = Saver.Load();
    }

    /// <summary>
    /// Saves the game.
    /// </summary>
    public void Save()
    {
        Saver.Save(saveGame);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
