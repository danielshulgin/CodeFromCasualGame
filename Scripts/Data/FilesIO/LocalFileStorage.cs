using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LocalFileStorage : MonoBehaviour
{
    [SerializeField] private string path = "/player.json";

    [SerializeField] private DefaultPlayerSettings defaultPlayerSettings;
    
    [SerializeField] private bool loadDefault;

    private static bool _loadedDefaults;
    
    
    public static LocalFileStorage Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            Debug.Log("Multiple LocalFileStorage instances");
        }
    }

    public void Save(PlayerDAO playerData)
    {
        DebugConsole.Instance.Log("Save");
        try
        {
            var fullPath = Application.persistentDataPath + path;
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath).Dispose();
            }
            DebugConsole.Instance.Log("Save1");
            if (playerData == null)
            {
                playerData = new PlayerDAO(defaultPlayerSettings);
            }
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = JsonConvert.SerializeObject(playerData, settings);
            var writer = new StreamWriter(fullPath, false);
            writer.Write(json);
            writer.Close();
        }
        catch (Exception e)
        {
            DebugConsole.Instance.Log(e.Message + e.Source);
            throw;
        }
        DebugConsole.Instance.Log("writer.Close();");
    }

    public PlayerDAO Load()
    {
        DebugConsole.Instance.Log("Load");
        if (loadDefault && !_loadedDefaults)
        {
            _loadedDefaults = true;
            return new PlayerDAO(defaultPlayerSettings);
        }
        
        var fullPath = Application.persistentDataPath + path;
        if (!File.Exists(fullPath))
        {
            File.Create(fullPath);
            var playerData = new PlayerDAO(defaultPlayerSettings);
            DebugConsole.Instance.Log("File.Create(fullPath);");
            return playerData;
        }
        else
        {
            var reader = new StreamReader(fullPath);
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var json = reader.ReadToEnd();
            reader.Close();
            
            var playerData = JsonConvert.DeserializeObject<PlayerDAO>(json, settings);
            DebugConsole.Instance.Log("defaultPlayerSettings");
            if (playerData == null)
                return new PlayerDAO(defaultPlayerSettings);
            
            playerData.AddNewId(defaultPlayerSettings);
            DebugConsole.Instance.Log("AddNewId");
            return  playerData;
        }
    }
}
