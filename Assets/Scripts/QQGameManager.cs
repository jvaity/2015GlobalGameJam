using UnityEngine;
using System.Collections;

public class QQGameManager : MonoBehaviour {

    public enum GameState
    {
        Menu,
        Game,
        Win,
        Lose,
        Credits
    }

    #region InspectorVariables

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Texture2D[] maps;
    [SerializeField]
    QQPlatformerController platformController;

    #endregion

    private static QQGameManager instance;
    public static QQGameManager Instance
    {
        get
        {
            return instance;
        }
    }

	[SerializeField]
    private GameState currentState;
    private QQLevelGenerator levelGenerator;

    private int currentLevel = 0;
    private int numberOfCollectiblesInLevel;

    private int score;

    #region Accessors

    public GameState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
        }
    }

    public GameObject TilePrefab
    {
        get
        {
            return tilePrefab;
        }
    }

    public QQLevelGenerator LevelGenerator
    {
        get
        {
            return levelGenerator;
        }
    }

    public QQPlatformerController PlatformController
    {
        get
        {
            return platformController;
        }
    }

    public int NumberOfCollectibilesInLevel
    {
        set
        {
            numberOfCollectiblesInLevel = value;
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
    }

    #endregion

    private void Update()
    {
        // Use this to call code from other classes
        switch (currentState)
        {
            case GameState.Menu:
                break;
            case GameState.Game:
                platformController.Move();
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            case GameState.Credits:
                break;
            default:
                break;
        }
    }

    public void CollectiblePickedUp()
    {
        numberOfCollectiblesInLevel--;
        if (numberOfCollectiblesInLevel < 1)
        {
            currentState = GameState.Win;
            Debug.Log("You Win");

            // debug
            StartNextLevel();
        }
    }

    public void blockDrilled(TileType type)
    {
        switch (type)
        {
            case TileType.Empty:
                break;
            case TileType.Block:
                score += 10;
                break;
            case TileType.Death:
                break;
            case TileType.Coin:
                score += 100;
                break;
            case TileType.Spawn:
                break;
            default:
                break;
        }
        Debug.Log("score: " + score);
    }

    public void GameOver()
    {
        currentState = GameState.Lose;
        Debug.Log("You Lose");
        // show game over screen

        //debug
        RestartLevel();
    }

    public void StartNextLevel()
    {
        ++currentLevel;
        if (currentLevel >= maps.Length)
        {
            Debug.Log("no more levels, go to credits");
        }
        else
            RestartLevel();        
    }

    public void RestartLevel()
    {
        if (levelGenerator != null)
            levelGenerator.Dispose();
        levelGenerator = new QQLevelGenerator(maps[currentLevel]);
        currentState = GameState.Game;
        score = 0;
        StartCoroutine(levelGenerator.ColumnGeneratorRoutine(levelGenerator.MapWidth));
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        Application.targetFrameRate = 60;

        // Create Classes
        levelGenerator = new QQLevelGenerator(maps[currentLevel]);

        // Use this to Init any classes
        RestartLevel();
    }
}
