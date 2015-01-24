//using UnityEngine;
//using System.Collections;
//
//public enum SceneState
//{
//    Running,
//    Paused,
//    Won
//}
//
//public class GameManager : MonoBehaviour 
//{
//    private static GameManager instance;
//    public static GameManager Instance
//    {
//        get { return instance; }
//    }
//
//    public SceneState sceneState;
//    public int numCollectablesLeft = 0;
//
//    private int collectablesCollected = 0;
//
//    void Awake()
//    {
//        if (instance == null)
//            instance = this;
//        else
//            Destroy(this);
//    }
//
//    public void GetCollectable()
//    {
//        numCollectablesLeft--;
//
//        if (numCollectablesLeft <= 0)
//        {
//            sceneState = SceneState.Won;
//            EndLevel();
//        }
//    }
//
//    public void TogglePause()
//    {
//        switch(sceneState)
//        {
//            case SceneState.Running:
//                sceneState = SceneState.Paused;
//                Time.timeScale = 0.0f;
//                break;
//
//            case SceneState.Paused:
//                sceneState = SceneState.Running;
//                Time.timeScale = 1.0f;
//                break;
//        }
//    }
//
//    public void collect()
//    {
//        collectablesCollected++;
//        Debug.Log("collectibles: "+collectablesCollected);
//    }
//
//    private void EndLevel()
//    {
//        //Show Win UI
//    }
//}
