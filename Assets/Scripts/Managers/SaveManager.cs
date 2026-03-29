using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private InputAction saveAction;
    private InputAction loadAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveAction = new InputAction("Save", InputActionType.Button, "<Keyboard>/f5");
        loadAction = new InputAction("Load", InputActionType.Button, "<Keyboard>/f9");
    }

    private void OnEnable()
    {
        saveAction.Enable();
        loadAction.Enable();
    }

    private void OnDisable()
    {
        saveAction.Disable();
        loadAction.Disable();
    }

    private void OnDestroy()
    {
        saveAction?.Dispose();
        loadAction?.Dispose();
    }

    private void Update()
    {
        if (saveAction.WasPressedThisFrame())
        {
            Save();
        }
        if (loadAction.WasPressedThisFrame())
        {
            Load();
        }
    }

    public bool Save()
    {
        try
        {
            SaveData saveData = new SaveData();
            saveData.dayNumber = 1;
            saveData.lastSaveTimestamp = System.DateTime.Now.ToString("o");

            InteractableObject[] interactables = FindObjectsOfType<InteractableObject>();
            saveData.objectStates = new List<SaveData.SavedObjectState>();

            foreach (InteractableObject obj in interactables)
            {
                SaveData.SavedObjectState state = new SaveData.SavedObjectState();
                state.objectId = obj.ObjectId;
                state.hasBeenInspected = false;
                state.interactionCount = 0;
                saveData.objectStates.Add(state);
            }

            string json = JsonUtility.ToJson(saveData, true);
            string path = Path.Combine(Application.persistentDataPath, GameConstants.SaveFileName);
            File.WriteAllText(path, json);

            DevLog.Log(LogCategory.Save,
                $"Saved: {GameConstants.SaveFileName} (size={json.Length}B, objects={saveData.objectStates.Count}) path={path}");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SaveManager: Save failed — {e.Message}");
            return false;
        }
    }

    public bool Load()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, GameConstants.SaveFileName);

            if (!File.Exists(path))
            {
                DevLog.Log(LogCategory.Save, "No save file found — starting fresh");
                return false;
            }

            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            DevLog.Log(LogCategory.Save,
                $"Loaded: {GameConstants.SaveFileName} (day={saveData.dayNumber}, objects={saveData.objectStates.Count})");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SaveManager: Load failed — {e.Message}");
            return false;
        }
    }

    public bool HasSave()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, GameConstants.SaveFileName));
    }

    public void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, GameConstants.SaveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
            DevLog.Log(LogCategory.Save, $"Deleted save file: {GameConstants.SaveFileName}");
        }
    }
}
