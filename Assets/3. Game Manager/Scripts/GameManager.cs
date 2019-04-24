using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable] public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }

public class GameManager : Singleton<GameManager>
{
    public enum GameState { PREGAME, RUNNING, PAUSED }

    public GameObject[] SystemPrefabs;
    public EventGameState OnGameStateChanged;

    private List<GameObject> instancedSystemPrefabs;
    List<AsyncOperation> loadOperations;
    GameState currentGameState = GameState.PREGAME;

    private string currentLevelName = "";

    public GameState CurrentGameState
    {
        get { return currentGameState; }
        private set { currentGameState = value; }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        instancedSystemPrefabs = new List<GameObject>();
        loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (loadOperations.Contains(ao))
        {
            loadOperations.Remove(ao);

            if (loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);
            }
        }

        Debug.Log("Load Complete.");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete.");
    }

    void UpdateState(GameState state)
    {
        GameState previousGameState = currentGameState;
        currentGameState = state;

        switch (currentGameState)
        {
            case GameState.PREGAME:
                break;

            case GameState.RUNNING:
                break;

            case GameState.PAUSED:
                break;

            default:
                break;
        }

        OnGameStateChanged.Invoke(currentGameState, previousGameState);
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

    public void StartGame()
    {
        LoadLevel("Main");
    }
}
