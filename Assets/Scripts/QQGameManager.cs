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
    [SerializeField]
    ParticleSystem	particle;

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

	private QQGUIManager guiManager;
	
    #region Accessors

    public GameState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
        	if (currentState != value)
        	{
            	currentState = value;          	
            	guiManager.ChangeGameState(currentState);
            }
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
        set
        {
            if (score != value)
            {
                score = value;
                guiManager.SetScore(score.ToString());
            }
        }
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
			if (Input.GetKeyDown(KeyCode.Space))
				RestartLevel();
			break;
			case GameState.Game:
                platformController.Move();
                break;
            case GameState.Win:
            //Start next Level
                if (Input.GetKeyDown(KeyCode.Space))
                    StartNextLevel();
                break;
            case GameState.Lose:
            if (Input.GetKeyDown(KeyCode.Space))
            	RestartLevel();
                break;
            case GameState.Credits:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentLevel = 0;
                    CurrentState = GameState.Menu;
                }
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
            CurrentState = GameState.Win;
            Debug.Log("You Win");

            // debug
            //StartNextLevel();
        }
    }

    public void blockDrilled(TileType type)
    {
        switch (type)
        {
            case TileType.Empty:
                break;
            case TileType.Block:
                Score += 10;
                break;
            case TileType.Death:
                break;
            case TileType.Coin:
                Score += 100;
                break;
            case TileType.Spawn:
                break;
            default:
                break;
        }
        //Debug.Log("score: " + Score);
        
        StartCoroutine(DrillParticle(platformController.transform.position + Vector3.right * 2f, type));
    }

	private IEnumerator DrillParticle(Vector3 pos, TileType type)
	{
		GameObject newParticle = GameObject.Instantiate(particle.gameObject) as GameObject;
		newParticle.transform.position = pos;
		ParticleSystem system = newParticle.GetComponent<ParticleSystem>();
		if (type == TileType.Coin)
			system.startColor = Color.cyan;
		else
			system.startColor = new Color32(148, 48, 0, 255);
		
		yield return new WaitForSeconds(system.duration);
		GameObject.Destroy(newParticle);
	}

    public void GameOver()
    {
        CurrentState = GameState.Lose;
        Debug.Log("You Lose");
        // show game over screen

        //debug
        //RestartLevel();
    }

    public void StartNextLevel()
    {
        ++currentLevel;
        if (currentLevel >= maps.Length)
        {
            Debug.Log("no more levels, go to credits");
            CurrentState = GameState.Credits;
        }
        else
            RestartLevel();        
    }

    public void RestartLevel()
    {
        if (levelGenerator != null)
            levelGenerator.Dispose();
        levelGenerator = new QQLevelGenerator(maps[currentLevel]);
        CurrentState = GameState.Game;
        Score = 0;
        StopAllCoroutines();
        StartCoroutine(levelGenerator.ColumnGeneratorRoutine(levelGenerator.MapWidth));
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        Application.targetFrameRate = 60;

        // Create Classes
        //levelGenerator = new QQLevelGenerator(maps[currentLevel]);

		guiManager = GetComponent<QQGUIManager>();
        // Use this to Init any classes
        //RestartLevel();
    }
}
