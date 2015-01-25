using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QQGUIManager : MonoBehaviour 
{
	public GameObject startScreen, winScreen, loseScreen, creditsScreen;
	private GameObject currentScreenShowing;

    public void ChangeGameState(QQGameManager.GameState state)
    {
    	GameObject newScreen = null;
    
    	switch (state)
    	{
    		case QQGameManager.GameState.Menu: 		newScreen = startScreen; break;
    		case QQGameManager.GameState.Win: 		newScreen = winScreen;	 break;
    		case QQGameManager.GameState.Lose: 		newScreen = loseScreen;	 break;
    		case QQGameManager.GameState.Credits:   newScreen = creditsScreen; break;
    	}	
    	
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
}
