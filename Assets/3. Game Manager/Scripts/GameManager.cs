using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject[] SystemPrefabs;

    private List<GameObject> instancedSystemPrefabs;
    List<AsyncOperation> loadOperations;

    private string currentLevelName = "";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        instancedSystemPrefabs = new List<GameObject>();
        loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        LoadLevel("Main");
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (loadOperations.Contains(ao))
        {
            loadOperations.Remove(ao);
        }

        Debug.Log("Load Complete.");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete.");
    }

    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;
        foreach (GameObject prefab in SystemPrefabs)
        {
            prefabInstance = Instantiate(prefab);
            instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null)
        {
            Debug.LogError("[Game Manager] Unable to load level " + levelName);
            return;
        }
        ao.completed += OnLoadOperationComplete;
        loadOperations.Add(ao);

        currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("[Game Manager] Unable to unload level " + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (GameObject instance in instancedSystemPrefabs)
        {
            Destroy(instance);
        }
        instancedSystemPrefabs.Clear();
    }
}
