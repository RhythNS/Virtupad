using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serilizable save game.
/// </summary>
[Serializable]
public class SaveGame
{
    [Serializable]
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

    // vr controller
    [SerializeField] public float playerMovePerSecond = 1.5f;
    [SerializeField] public float playerRotatePerSecond = 90.0f;
    [SerializeField] public int playerMoveType = 1;
}
