using System;
using System.Collections.Generic;

namespace Game.Save.Data
{
    [Serializable]
    public class SaveStateDataItem
    {
        public string Name;
        public string Data;
    }

    [Serializable]
    public class SaveStateData
    {
        public long SaveTimestap;
        public List<SaveStateDataItem> DataItems = new List<SaveStateDataItem>();
    }
}
