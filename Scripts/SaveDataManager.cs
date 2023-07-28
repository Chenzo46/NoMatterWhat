using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField] private WorldManager worldManager;
    [SerializeField] private int nonLevelCount = 1;
    [SerializeField] public GameStateVariables gameStateVariables { get; private set; }

    private string saveDataPath;
    private string gameVariablePath;

    public static SaveDataManager Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
        saveDataPath = Application.persistentDataPath + "/nomatterwhat.json";
        gameVariablePath = Application.persistentDataPath + "/nomatterwhatvariables.json";
        Debug.Log(saveDataPath);
        loadData();
        Debug.Log(gameStateVariables.ToString());
        DontDestroyOnLoad(gameObject);
    }


    private void generateLevelDataPath()
    {
        string save_to_json = JsonUtility.ToJson(worldManager, true);
        System.IO.File.WriteAllText(saveDataPath, save_to_json);
    }

    private void generateVariableDataPath()
    {
        string save_to_json = JsonUtility.ToJson(gameStateVariables, true);
        System.IO.File.WriteAllText(gameVariablePath, save_to_json);
    }

    public void saveData()
    {
        if (Singleton == null) { return; }
        // Save Level Data
        string save_to_json = JsonUtility.ToJson(worldManager, true);
        System.IO.File.WriteAllText(saveDataPath, save_to_json);

        Debug.Log("Level Data Saved");

        // Save variable data
        save_to_json = JsonUtility.ToJson(gameStateVariables, true);
        System.IO.File.WriteAllText(gameVariablePath, save_to_json);

        Debug.Log("Varibale Data Saved");
    }

    public void loadData()
    {
        if (Singleton == null) { return; }
        // Level Data loader
        try
        {
            string save_from_json = System.IO.File.ReadAllText(saveDataPath);
            
            WorldManager n_worldManager = JsonUtility.FromJson<WorldManager>(save_from_json);

            if (worldManager.isManagerLengthSame(n_worldManager))
            {
                worldManager = n_worldManager;
            }
            else
            {
                Debug.Log("Old save data detected. Creating new save data...");
                generateLevelDataPath();
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("Save data not found with exception: " + e.Message + ". Creating new save data...");
        }

        // Variable Data Loader
        try
        {
            string data_from_json = System.IO.File.ReadAllText(gameVariablePath);
            GameStateVariables n_gameStateVariables = JsonUtility.FromJson<GameStateVariables>(data_from_json);
            gameStateVariables = n_gameStateVariables;
        }
        catch (System.Exception e) 
        {
            Debug.Log("Variable data not found with exception: " + e.Message + ". Creating new variable data...");
            gameStateVariables = new GameStateVariables();
            gameStateVariables.createNewVariableDictionaries();
            generateVariableDataPath();
        }
        saveData();
    }
    public void clearData()
    {
        worldManager.newLevelManagers();
        saveData();
    }

    public void clearVariableData()
    {
        gameStateVariables.clearAllVariables();
        saveData();
    }

    public Level getCurrentLevelData()
    {
        return worldManager.currentLevelManager(0).currentLevel(SceneManager.GetActiveScene().buildIndex - nonLevelCount);
    }

    #region Classes

    [System.Serializable]
    public class WorldManager
    {
        [SerializeField] private List<LevelManager> levelManagers;

        public void newLevelManagers()
        {
            levelManagers = new List<LevelManager>();
            foreach(LevelManager lv in levelManagers)
            {
                lv.newLevels();
            }
        }

        public LevelManager currentLevelManager(int world)
        {
            return levelManagers[world];
        }

        public int getWorldCount()
        {
            return levelManagers.Count;
        }

        public List<LevelManager> getLevelManagers()
        {
            return levelManagers;
        }

        public bool isManagerLengthSame(WorldManager wm)
        {

            if(getWorldCount() != wm.getWorldCount())
            {
                return false;
            }
            else
            {
                for(int i = 0; i < getWorldCount(); i++)
                {
                    if (levelManagers[i].getManagerLength() != wm.getLevelManagers()[i].getManagerLength())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }

    [Serializable]
    public class LevelManager 
    {
        [SerializeField]
        private List<Level> levels = new List<Level>();

        public void newLevels()
        {
            levels = new List<Level>();
        }

        public int getManagerLength()
        {
            return levels.Count;
        }

        public Level currentLevel(int id)
        {
            return levels[id];
        }
    }
    [Serializable]
    public class Level
    {
        [SerializeField] private bool hasBeatLevel = false;
        [SerializeField] private bool hasReachedCheckpoint = false;

        public bool getBeatLevel()
        {
            return hasBeatLevel;
        }

        public bool getReachedCheckpoint()
        {
            return hasReachedCheckpoint;
        }

        public void beatLevel()
        {
            hasBeatLevel = true;
        }

        public void setCheckpoint(bool set)
        {
            hasReachedCheckpoint = set;
        }
           
    }

    [Serializable]
    public class GameStateVariables
    {
        [SerializeField] private DataBank<int> saved_ints = new DataBank<int>();
        [SerializeField] private DataBank<float> saved_floats = new DataBank<float>();

        public void createNewVariableDictionaries()
        {
            saved_floats = new DataBank<float>();
            saved_ints = new DataBank<int>();
        }

        public float getFloat(string key)
        {
            try
            {
                float num = saved_floats.Get(key);
                return num;
            }
            catch (Exception e)
            {
                Debug.Log($"Method 'getFloat(string key)' failed with the following error message: {e.Message} Making new key...");
                addKey(key, 0.5f);
                return 0.5f;
            }
        }

        public float getInt(string key)
        {
            try
            {
                int num = saved_ints.Get(key);
                return num;
            }
            catch
            {
                Debug.Log("Saved int key not found, making new key...");
                addKey(key, 0);
                return 0;
            }
        }

        public void addKey(string key, int value)
        {
            if (!saved_ints.ContainsKey(key))
            {
                saved_ints.Add(key, value);
            }
            else
            {
                saved_floats.Overwrite(key, value);
            }
            
        }
        public void addKey(string key, float value) 
        {
            if (!saved_floats.ContainsKey(key))
            {
                saved_floats.Add(key, value);
            }
            else
            {
                saved_floats.Overwrite(key, value);
            }
        }
        public void removeKey(string key)
        {
            if (saved_ints.ContainsKey(key))
            {
                saved_ints.Remove(key);
            }
            else if (saved_floats.ContainsKey(key))
            {
                saved_floats.Remove(key);
            }
        }
        public void clearAllVariables()
        {
            saved_floats = new DataBank<float>();
            saved_ints = new DataBank<int>();
        }

        public override string ToString()
        {
            string message = "All saved ints:";

            //Compile all integers
            if(saved_ints.Count() >= 1)
            {
                for(int i = 0; i < saved_ints.Count(); i++)
                {
                    message += $"\n- ({saved_ints.saveVariables[i].key}, {saved_ints.saveVariables[i].value})";
                }
            }
            else
            {
                message += "\n- No saved integers...";
            }
            //Complie all floats
            message += "\nAll saved floats:";

            if (saved_floats.Count() >= 1)
            {
                for (int i = 0; i < saved_floats.Count(); i++)
                {
                    message += $"\n- ({saved_floats.saveVariables[i].key}, {saved_floats.saveVariables[i].value})";
                }
            }
            else
            {
                message += "\n- No saved floats...";
            }

            return message;
        }
    }

    [Serializable]
    public class DataBank<D>
    {
        private D dataType;

        public List<SaveVariable<D>> saveVariables = new List<SaveVariable<D>>();

        public D Get(string key)
        {
            foreach(SaveVariable<D> sv in saveVariables)
            {
                if(sv.key == key)
                {
                    return sv.value;
                }
            }
            throw new Exception($"Game state variable of key '{key}' not found.");
        }

        public int Count()
        {
            return saveVariables.Count;
        }

        public void Add(string key, D value)
        {
            SaveVariable<D> sv = new SaveVariable<D>(key,value);
            saveVariables.Add(sv);
        }

        public void Overwrite(string key, D value)
        {
            foreach(SaveVariable<D> sv in saveVariables)
            {
                if(sv.key == key)
                {
                    sv.value = value;
                }
            }
        }

        public void Remove(string key)
        {
            foreach(SaveVariable<D> sv in saveVariables)
            {
                if(sv.key == key)
                {
                    saveVariables.Remove(sv);
                    return;
                }
            }
        }

        public bool ContainsKey(string key)
        {
            bool defaultBool = false;

            foreach(SaveVariable<D> sv in saveVariables)
            {
                if(sv.key == key)
                {
                    defaultBool = true;
                    break;
                }
            }

            return defaultBool;
        }
        

    }
    [Serializable]
    public class SaveVariable<D>
    {
        public string key;
        public D value;

        public SaveVariable(string key, D data){
            value = data;
            this.key = key;
        }
    }
    
    #endregion 

}
