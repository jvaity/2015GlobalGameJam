using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QQFlashText : MonoBehaviour {

    private Text textObj;
    public float speed = 5.0f;

    void Start()
    {
        textObj = GetComponent<Text>();
    }

    void Update()
    {
        PulseText(textObj, speed);
    }

    public void PulseText(Text textObj, float pulseRate)
    {
        Color newColor = textObj.color;
        newColor.a = Mathf.Sin(Time.time * pulseRate);
        textObj.color = newColor;
    }
}
