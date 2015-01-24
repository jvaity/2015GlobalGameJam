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

    #endregion

    private static QQGameManager instance;
    public static QQGameManager Instance
    {
        get
        {
            return instance;
        }
    }


    private GameState currentState;
    private QQLevelGenerator levelGenerator;


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

    #endregion

    private void Update()
    {
        // Use this to call code from other classes
        switch (currentState)
        {
            case GameState.Menu:
                break;
            case GameState.Game:
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

    public void RemoveBlock(bool immediatley)
    {
        
    }

    public void PickUpCollectible()
    {

    }

    public void GameOver()
    {

    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        // Create Classes
        levelGenerator = new QQLevelGenerator(maps[0]);

        // Use this to Init any classes
    }
}
