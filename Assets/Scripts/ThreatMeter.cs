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
    [SerializeField] TextMeshProUGUI sumValueText;
    [SerializeField] TextMeshProUGUI currentThreatValueText;
    [SerializeField] GameObject sumTracker;
    [SerializeField] GameObject threatTracker;

    //cached references
    Slider mySlider;
    int sumValue;
    public int currentThreatValue;
    
    // Start is called before the first frame update
    void Start()
    {
        mySlider = GetComponent<Slider>();
        currentThreatValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mySlider.maxValue = allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue;
        maxValueText.text = mySlider.maxValue.ToString();

        UpdateSumTracker();
        UpdateThreatTracker();
    }

    private void UpdateSumTracker()
    {
        sumValue = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;
        sumValueText.text = sumValue.ToString();
        var sliderOffset = (sumValue - (mySlider.maxValue / 2)) / (mySlider.maxValue / 2);
        var sliderLength = mySlider.transform.localScale.x * mySlider.GetComponent<RectTransform>().sizeDelta.x / 2;
        var newSumTrackerPos = new Vector3((sliderOffset * sliderLength) + mySlider.transform.position.x, sumTracker.transform.position.y, sumTracker.transform.position.z);
        sumTracker.transform.position = newSumTrackerPos;
    }
    private void UpdateThreatTracker()
    {
        currentThreatValueText.text = currentThreatValue.ToString();
        var sliderOffset = (currentThreatValue - (mySlider.maxValue / 2)) / (mySlider.maxValue / 2);
        var sliderLength = mySlider.transform.localScale.x * mySlider.GetComponent<RectTransform>().sizeDelta.x / 2;
        var newThreatTrackerPos = new Vector3((sliderOffset * sliderLength) + mySlider.transform.position.x, threatTracker.transform.position.y, threatTracker.transform.position.z);
        threatTracker.transform.position = newThreatTrackerPos;
    }
}
