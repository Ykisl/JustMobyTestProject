using Game.Save.Data;
using System;
using System.IO;
using UnityEngine;
using Zenject;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Save
{
    public class SaveSystem : ISaveSystem, IInitializable, IDisposable
    {
        protected const string SAVE_FILE_NAME = "save.json";

        private ISavable[] _saveTargets;

        protected SaveStateData _lastLoadedData;

        public event Action OnSaveStateLoaded;

        [Inject]
        private void Construct(ISavable[] saveTargets)
        {
            _saveTargets = saveTargets;
        }

        public void Initialize()
        {
            foreach (var target in _saveTargets)
            {
                target.OnSaveRequested += HandleSaveRequested;
            }
        }

        public void Dispose()
        {
            foreach (var target in _saveTargets)
            {
                target.OnSaveRequested -= HandleSaveRequested;
            }
        }

        public void LoadState()
        {
            var filePath = GetSavePath();
            if(!File.Exists(filePath))
            {
                return;
            }

            var fileData = File.ReadAllText(filePath);
            if(string.IsNullOrEmpty(fileData))
            {
                return;
            }

            var loadedData = JsonUtility.FromJson<SaveStateData>(fileData);
            if(loadedData == null)
            {
                return;
            }

            _lastLoadedData = loadedData;
            var dataFormater = new JsonSaveItemFormater(loadedData);

            var sortedLoadTargets = _saveTargets.OrderBy(x => x.SaveDataLoadPriority);
            foreach(var target in sortedLoadTargets)
            {
                target.OnLoadState(dataFormater);
            }
        }

        public void SaveState()
        {
            var filePath = GetSavePath();

            var saveState = new SaveStateData()
            {
                SaveTimestap = DateTime.UtcNow.Ticks
            };

            var dataFormater = new JsonSaveItemFormater(saveState);
            foreach (var target in _saveTargets)
            {
                target.OnSaveState(dataFormater);
            }

            _lastLoadedData = saveState;

            var saveJson = JsonUtility.ToJson(saveState);
            File.WriteAllText(filePath, saveJson);
        }

#if UNITY_EDITOR
        [MenuItem("[Game] Tools/Reset save")]
#endif
        public static void ClearSaveFile()
        {
            var filePath = GetSavePath();
            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }

        private void HandleSaveRequested()
        {
            SaveState();
        }

        protected static string GetSavePath()
        {
            var dataPath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            return dataPath;
        }
    }
}
