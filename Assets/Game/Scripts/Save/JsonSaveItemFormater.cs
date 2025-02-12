using Game.Save.Data;
using UnityEngine;

namespace Game.Save
{
    public class JsonSaveItemFormater : SaveItemFormater
    {
        public JsonSaveItemFormater(SaveStateData saveStateData) : base(saveStateData) { }

        protected override T GetDataFromString<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }

        protected override string GetStringFromData<T>(T data)
        {
            return JsonUtility.ToJson(data);
        }
    }
}
