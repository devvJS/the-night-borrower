using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int dayNumber = 1;
    public string lastSaveTimestamp;
    public List<SavedObjectState> objectStates = new List<SavedObjectState>();

    [System.Serializable]
    public class SavedObjectState
    {
        public string objectId;
        public bool hasBeenInspected;
        public int interactionCount;
    }
}
