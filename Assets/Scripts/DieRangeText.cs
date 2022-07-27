using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DieRangeText : MonoBehaviour
{
    //config parameters
    [SerializeField] float fontShrinkRate = 0.5f;
    [SerializeField] float colorSpeed = 2f; 
    [SerializeField] public Color targetColor;
    [SerializeField] public float enlargedFontSize;
    
    //cached references
    float defaultFontSize;
    Color defaultColor;
    public TextMeshProUGUI myText;

    // Start is called before the first frame update
    void Start()
    {
        if (myText == null)
        {
            myText = GetComponent<TextMeshProUGUI>();
        }
        defaultColor = myText.color;
        defaultFontSize = myText.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(myText.fontSize > defaultFontSize)
        {
            myText.fontSize -= fontShrinkRate * Time.deltaTime;
            myText.color = Color.Lerp(myText.color, defaultColor, colorSpeed * Time.deltaTime);
        }
        else
        {
            myText.color = defaultColor;
        }
    }
}
