using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QQGUIStartScreen : MonoBehaviour {

    [SerializeField]
    private Text continueText;

    public void pulseText(float pulseRate)
    {
        Color newColor = continueText.color;
        newColor.a =  Mathf.Sin(Time.time * pulseRate);
        continueText.color = newColor;
    }
}
