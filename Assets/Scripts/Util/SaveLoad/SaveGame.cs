using System;
using System.Collections.Generic;

/// <summary>
/// Serilizable save game.
/// </summary>
[System.Serializable]
public class SaveGame
{
    [System.Serializable]
    public struct LoadVRM
    {
        public string filepath;
        public int id;
        public DateTime lastLoaded;
        public int previewWidth, previewHeight;
    }

    public List<LoadVRM> vrms = new List<LoadVRM>();
}
