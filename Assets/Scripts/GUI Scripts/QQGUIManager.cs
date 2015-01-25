using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QQGUIManager : MonoBehaviour 
{
	public GameObject startScreen, winScreen, loseScreen, creditsScreen;
	private GameObject currentScreenShowing;

    [SerializeField]
    private GameObject scoreText;

	void Start()
	{
		currentScreenShowing = startScreen;
	}

    public void ChangeGameState(QQGameManager.GameState state)
    {
    	GameObject newScreen = null;

        bool showScore = false;

    	switch (state)
    	{
    		case QQGameManager.GameState.Menu: 		
                newScreen = startScreen;
                break;
            case QQGameManager.GameState.Win: 
                newScreen = winScreen;
                showScore = true; 
                break;
            case QQGameManager.GameState.Lose: 
                newScreen = loseScreen; 
                showScore = true; 
                break;
    		case QQGameManager.GameState.Credits:  
                newScreen = creditsScreen;
                break;
            default:
                showScore = true;
                break;
    	}

        if (scoreText != null)
            scoreText.gameObject.SetActive(showScore);

    	SwitchToScreen(newScreen);
    }
    
    private void SwitchToScreen(GameObject newScreen)
    {
    	if (currentScreenShowing != null)
    	{
    		currentScreenShowing.SetActive(false);
    		currentScreenShowing = null;
    	}

        if (newScreen != null)
        {
            newScreen.SetActive(true);
            currentScreenShowing = newScreen;
        }            
    }
    
	public void PulseText(Text textObj, float pulseRate)
	{
		Color newColor = textObj.color;
		newColor.a =  Mathf.Sin(Time.time * pulseRate);
		textObj.color = newColor;
	}

    public void SetScore(string score)
    {
        scoreText.GetComponentInChildren<Text>().text = "Score: " + score;
    }
}
