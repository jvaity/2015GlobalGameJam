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

    private static QQGameManager instance;
    public static QQGameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private GameState currentState;

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

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        // Use this to Init any classes
    }
}
