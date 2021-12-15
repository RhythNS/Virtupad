using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LastLoadedVRMs : MonoBehaviour
{
    [System.Serializable]
    public class LoadVRM : IComparer<LoadVRM>
    {
        public string filepath;
        public int id;
        public Texture2D previewTexture;
        public bool isResponsibleForCleanUp;
        public DateTime lastLoaded;
        public int previewWidth, previewHeight;

        public string Name => string.IsNullOrEmpty(filepath) ?
            "" : Path.GetFileNameWithoutExtension(filepath);

        public int Compare(LoadVRM x, LoadVRM y) => x.lastLoaded > y.lastLoaded ? 1 : -1;

        public void Free()
        {
            if (previewTexture == null || isResponsibleForCleanUp == false)
                return;

            Destroy(previewTexture);
            previewTexture = null;
        }
    }

    public List<LoadVRM> LastLoaded => lastLoaded;
    private List<LoadVRM> lastLoaded = new List<LoadVRM>();
    private string vrmFolder;

    public void Init(List<SaveGame.LoadVRM> savedlastLoaded)
    {
        vrmFolder = Application.persistentDataPath + "lastLoadedVRMs";

        FreeAll();
        lastLoaded.Clear();

        if (File.Exists(vrmFolder) == false)
            Directory.CreateDirectory(vrmFolder);

        List<string> pngs = Directory.GetFiles(vrmFolder, "*.png", SearchOption.TopDirectoryOnly).ToList();

        for (int i = 0; i < savedlastLoaded.Count; i++)
        {
            for (int j = 0; j < pngs.Count; j++)
            {
                if (Path.GetFileName(pngs[j]) == savedlastLoaded[i].id + ".png" == false)
                    continue;

                if (IsValid(savedlastLoaded[i].filepath) == false)
                    break;

                LoadVRM loaded = new LoadVRM()
                {
                    filepath = savedlastLoaded[i].filepath,
                    id = savedlastLoaded[i].id,
                    lastLoaded = savedlastLoaded[i].lastLoaded,
                    previewWidth = savedlastLoaded[i].previewWidth,
                    previewHeight = savedlastLoaded[i].previewHeight,
                    isResponsibleForCleanUp = true
                };

                /*
                if (TryLoadPreview(ref loaded) == false)
                    break;
                 */

                lastLoaded.Add(loaded);
                pngs.RemoveAt(j);

                break;
            }
        }

        lastLoaded.Sort();

        for (int i = 0; i < pngs.Count; i++)
        {
            File.Delete(pngs[i]);
        }

        OnUpdateSaveGame();
    }

    public void OnLoaded(LoadVRM vrm)
    {
        int index = lastLoaded.IndexOf(vrm);
        if (index == -1)
            return;

        OnLoaded(index);
    }

    public void OnLoaded(int index)
    {
        if (index < 0 || index >= lastLoaded.Count)
            return;

        lastLoaded[index].lastLoaded = DateTime.Now;

        OnUpdateSaveGame();
    }

    public void Add(string filepath, Texture2D previewTexture)
    {
        int index = lastLoaded.FindIndex(x => x.filepath == filepath);
        if (index != -1)
        {
            OnLoaded(index);
            return;
        }

        int id = GetNextFreeID();
        if (id == -1)
            return;

        if (TrySavePreview(Path.Combine(vrmFolder, id.ToString()), previewTexture) == false)
            return;

        LoadVRM loadVRM = new LoadVRM()
        {
            id = id,
            filepath = filepath,
            isResponsibleForCleanUp = false,
            lastLoaded = DateTime.Now,
            previewHeight = previewTexture.height,
            previewWidth = previewTexture.width,
            previewTexture = previewTexture
        };

        lastLoaded.Add(loadVRM);
    }

    private int GetNextFreeID()
    {
        List<string> pngs = Directory.GetFiles(vrmFolder, "*.png", SearchOption.TopDirectoryOnly).ToList();
        for (int i = 0; i < int.MaxValue; i++)
        {
            if (Path.GetFileNameWithoutExtension(pngs[i]).Equals(i.ToString()) == false)
                return i;
        }

        return -1;
    }

    public bool IsValid(string filepath)
    {
        return File.Exists(filepath) && File.Exists(vrmFolder);
    }

    public void DeleteFromHistory(LoadVRM loadVRM)
    {
        int index = lastLoaded.IndexOf(loadVRM);
        if (index == -1)
            return;

        try
        {
            File.Delete(Path.Combine(vrmFolder, loadVRM.id.ToString()));
        }
        catch (Exception)
        {
        }

        loadVRM.Free();
        lastLoaded.RemoveAt(index);

        OnUpdateSaveGame();
    }

    private bool TrySavePreview(string path, Texture2D texture)
    {
        try
        {
            byte[] write = texture.EncodeToPNG();
            File.WriteAllBytes(path, write);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IEnumerator RequestPreviewImages(int fromInclusive, int toExclusive)
    {
        for (int i = fromInclusive; i < toExclusive; i++)
        {
            if (IsValid(lastLoaded[i].filepath) == false)
                continue;

            Task<byte[]> byteTask = ReadBytes(lastLoaded[i].filepath);
            yield return new WaitUntil(() => byteTask.IsCompleted);

            byte[] read = byteTask.Result;
            if (read == null)
                continue;

            lastLoaded[i].previewTexture = new Texture2D(lastLoaded[i].previewWidth, lastLoaded[i].previewHeight);
            if (lastLoaded[i].previewTexture.LoadImage(read) == false)
            {
                Destroy(lastLoaded[i].previewTexture);
                lastLoaded[i] = null;
                continue;
            }
        }
    }

    private async Task<byte[]> ReadBytes(string filename)
    {
        try
        {
            byte[] result;

            using (FileStream SourceStream = File.Open(filename, FileMode.Open))
            {
                result = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
            }

            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /*
    private bool TryLoadPreview(ref LoadVRM loadVRM)
    {
        if (IsValid(loadVRM.filepath) == false)
            return false;

        byte[] read;
        try
        {
            read = File.ReadAllBytes(loadVRM.filepath);
        }
        catch (Exception)
        {
            return false;
        }

        loadVRM.previewTexture = new Texture2D(loadVRM.previewWidth, loadVRM.previewHeight);
        if (loadVRM.previewTexture.LoadImage(read) == false)
        {
            Destroy(loadVRM.previewTexture);
            return false;
        }

        return true;
    }
     */

    public void FreeAll()
    {
        for (int i = 0; i < lastLoaded.Count; i++)
        {
            lastLoaded[i].Free();
        }
    }

    private void OnUpdateSaveGame()
    {
        List<SaveGame.LoadVRM> toSave = new List<SaveGame.LoadVRM>();
        for (int i = 0; i < lastLoaded.Count; i++)
        {
            toSave.Add(new SaveGame.LoadVRM()
            {
                filepath = lastLoaded[i].filepath,
                id = lastLoaded[i].id,
                lastLoaded = lastLoaded[i].lastLoaded,
                previewHeight = lastLoaded[i].previewHeight,
                previewWidth = lastLoaded[i].previewWidth,
            });
        }
        SaveFileManager saveFileManager = SaveFileManager.Instance;

        if (saveFileManager == null)
            return;

        saveFileManager.saveGame.vrms = toSave;
        saveFileManager.Save();
    }

    private void OnDestroy()
    {
        FreeAll();
    }
}
