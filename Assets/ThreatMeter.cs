using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThreatMeter : MonoBehaviour
{
    //config parameters
    [SerializeField] List<DieStats> allDice;
    [SerializeField] TextMeshProUGUI maxValueText;
    [SerializeField] TextMeshProUGUI currentValueText;

    //cached references
    Slider mySlider;
    
    // Start is called before the first frame update
    void Start()
    {
        mySlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        mySlider.maxValue = allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue;
        mySlider.value = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;

        maxValueText.text = mySlider.maxValue.ToString();
        currentValueText.text = mySlider.value.ToString();

    }
}
