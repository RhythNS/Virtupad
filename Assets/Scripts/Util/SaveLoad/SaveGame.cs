using System;
using System.Collections.Generic;
using UnityEngine;
using Virtupad;

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

    [Serializable]
    public struct CamerasOnScene
    {
        public string sceneName;
        public List<StudioCamera.Definition> definitions;

        public CamerasOnScene(string sceneName, List<StudioCamera.Definition> definitions)
        {
            this.sceneName = sceneName;
            this.definitions = definitions;
        }
    }

    // Vrm
    [SerializeField] public List<LoadVRM> vrms;
    [SerializeField] public bool autoLoadVRM = false;
    [SerializeField] public bool footIK = true;
    [SerializeField] public string microphone = "";

    // Auto camera create setting
    [SerializeField] public int autoLoadSet = -1;
    [SerializeField] public List<CamerasOnScene> camerasOnScenes = new List<CamerasOnScene>();
    [SerializeField] public float cameraMovementSpeed = 1.0f;
    [SerializeField] public float cameraRotatePerSpeed = 1.0f;

    // ui settings
    [SerializeField] public string customNoCameraActivePath = "";

    // vr controller
    [SerializeField] public float playerMovePerSecond = 1.5f;
    [SerializeField] public float playerRotatePerSecond = 90.0f;
    [SerializeField] public int playerMoveType = 1;
}
