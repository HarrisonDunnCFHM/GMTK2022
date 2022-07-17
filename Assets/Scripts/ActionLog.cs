using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionLog : MonoBehaviour
{
    //config paramters
    [SerializeField] TextMeshProUGUI logText;
    
    //cached references
    public string myText;
    
    // Start is called before the first frame update
    void Start()
    {
        myText = "Good luck!\n";
    }

    // Update is called once per frame
    void Update()
    {
        logText.text = myText;
    }
}
