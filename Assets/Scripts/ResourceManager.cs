using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    //config parameters
    [SerializeField] int startingSun = 10;
    [SerializeField] int startingMoon = 10;
    [SerializeField] int startingStar = 10;
    [SerializeField] int startingCelestial = 0;
    [SerializeField] TextMeshProUGUI sunResourceText;
    [SerializeField] TextMeshProUGUI moonResourceText;
    [SerializeField] TextMeshProUGUI starResourceText;
    [SerializeField] TextMeshProUGUI celestialResourceText;

    //cached references
    public int currentSun;
    public int currentMoon;
    public int currentStar;
    public int currentCelestial;

    // Start is called before the first frame update
    void Start()
    {
        currentSun = startingSun;
        currentMoon = startingMoon;
        currentStar = startingStar;
        currentCelestial = startingCelestial;
    }

    // Update is called once per frame
    void Update()
    {
        sunResourceText.text = "Sunbeams\n" + currentSun.ToString();
        moonResourceText.text = "Moondrops\n" + currentMoon.ToString();
        starResourceText.text = "Stardust\n" + currentStar.ToString();
        if (currentCelestial < 0)
        {
            celestialResourceText.text = "Wisps\n0";
        }
        else
        {
            celestialResourceText.text = "Wisps\n" + currentCelestial.ToString();
        }
    }
}
