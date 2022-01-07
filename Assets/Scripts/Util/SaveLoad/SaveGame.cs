using System;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] public List<LoadVRM> vrms;
    [SerializeField] public bool autoLoadVRM = false;
    [SerializeField] public bool footIK = true;

    // Auto camera create setting

    [SerializeField] public int autoLoadSet = -1;

}
