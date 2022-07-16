using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    //config parameters
    [SerializeField] int startingRed = 10;
    [SerializeField] int startingGreen = 10;
    [SerializeField] int startingBlue = 10;
    [SerializeField] int startingPremium = 0;
    [SerializeField] TextMeshProUGUI RedResourceText;
    [SerializeField] TextMeshProUGUI GreenResourceText;
    [SerializeField] TextMeshProUGUI BlueResourceText;
    [SerializeField] TextMeshProUGUI PremiumResourceText;

    //cached references
    public int currentRed;
    public int currentGreen;
    public int currentBlue;
    public int currentPremium;

    // Start is called before the first frame update
    void Start()
    {
        currentRed = startingRed;
        currentGreen = startingGreen;
        currentBlue = startingBlue;
        currentPremium = startingPremium;
    }

    // Update is called once per frame
    void Update()
    {
        RedResourceText.text = "Red: " + currentRed.ToString();
        GreenResourceText.text = "Green: " + currentGreen.ToString();
        BlueResourceText.text = "Blue: " + currentBlue.ToString();
        PremiumResourceText.text = "Premium: " + currentPremium.ToString();
    }
}
