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
    [SerializeField] List<WispTracker> wispTrackers;
    [SerializeField] List<TextMeshProUGUI> wispTrackersValueTexts;
    [SerializeField] float moveSpeed = 5f;

    //cached references
    Slider mySlider;
    int sumValue;
    public int currentThreatValue;
    DiceRoller diceRoller;
    
    // Start is called before the first frame update
    void Start()
    {
        mySlider = GetComponent<Slider>();
        currentThreatValue = 0;
        diceRoller = FindObjectOfType<DiceRoller>();
    }

    // Update is called once per frame
    void Update()
    {
        mySlider.maxValue = allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue;
        maxValueText.text = mySlider.maxValue.ToString();

        UpdateSumTracker();
        UpdateThreatTracker();
        UpdateWispTrackers();
    }

    private void UpdateSumTracker()
    {
        sumValue = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue + diceRoller.addedWisps;
        sumValueText.text = sumValue.ToString();
        var sliderOffset = (sumValue - (mySlider.maxValue / 2)) / (mySlider.maxValue / 2);
        var sliderLength = mySlider.transform.localScale.x * mySlider.GetComponent<RectTransform>().sizeDelta.x / 2;
        var newSumTrackerPos = new Vector3((sliderOffset * sliderLength) + mySlider.transform.position.x, sumTracker.transform.position.y, sumTracker.transform.position.z);
        
        sumTracker.transform.position = Vector2.MoveTowards(sumTracker.transform.position, newSumTrackerPos, moveSpeed * Time.deltaTime);
    }
    private void UpdateThreatTracker()
    {
        currentThreatValueText.text = currentThreatValue.ToString();
        var sliderOffset = (currentThreatValue - (mySlider.maxValue / 2)) / (mySlider.maxValue / 2);
        var sliderLength = mySlider.transform.localScale.x * mySlider.GetComponent<RectTransform>().sizeDelta.x / 2;
        var newThreatTrackerPos = new Vector3((sliderOffset * sliderLength) + mySlider.transform.position.x, threatTracker.transform.position.y, threatTracker.transform.position.z);
        threatTracker.transform.position = Vector2.MoveTowards(threatTracker.transform.position, newThreatTrackerPos, moveSpeed * Time.deltaTime);
        mySlider.value = currentThreatValue;

    }
    private void UpdateWispTrackers()
    {
        for (int i=0; i < wispTrackers.Count; i++)
        {
            wispTrackersValueTexts[i].text = wispTrackers[i].currentThreshold.ToString();
            var sliderOffset = (wispTrackers[i].currentThreshold - (mySlider.maxValue / 2)) / (mySlider.maxValue / 2);
            var sliderLength = mySlider.transform.localScale.x * mySlider.GetComponent<RectTransform>().sizeDelta.x / 2;
            var newWispTrackerPos = new Vector3((sliderOffset * sliderLength) + mySlider.transform.position.x, wispTrackers[i].transform.position.y, wispTrackers[i].transform.position.z);
            //wispTrackers[i].transform.position = newWispTrackerPos;
            wispTrackers[i].transform.position = Vector2.MoveTowards(wispTrackers[i].transform.position, newWispTrackerPos, moveSpeed * Time.deltaTime);
        }
    }
}
