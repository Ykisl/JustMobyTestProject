using Game.Save.Context;
using Game.Save.Data;
using System.Linq;

namespace Game.Save
{
    public abstract class SaveItemFormater : ISaveContext, ILoadContext
    {
        protected SaveStateData _saveData;

        public long SaveTimestamp => _saveData.SaveTimestap;

        public SaveItemFormater(SaveStateData saveStateData)
        {
            _saveData = saveStateData;
            _saveData ??= new SaveStateData();
        }

        public void SetData<T>(T data)
        {
            var dataItem = GetDataItem<T>();
            if(dataItem == null) 
            {
                var type = typeof(T);
                var key = type.FullName;
                dataItem = new SaveStateDataItem()
                {
                    Name = key,
                };

                _saveData.DataItems.Add(dataItem);
            }

            dataItem.Data = GetStringFromData<T>(data);
        }

        public T GetData<T>()
        {
            var dataItem = GetDataItem<T>();
            if(dataItem == null)
            {
                return default;
            }

            return GetDataFromString<T>(dataItem.Data);
        }

        public SaveStateData GetSaveState()
        {
            return _saveData;
        }

        protected SaveStateDataItem GetDataItem<T>()
        {
            var type = typeof(T);
            var key = type.FullName;

            var dataItem = _saveData.DataItems.FirstOrDefault(x => x.Name == key);
            return dataItem;
        }

        protected abstract T GetDataFromString<T>(string data);

        protected abstract string GetStringFromData<T>(T data);
    }
}
