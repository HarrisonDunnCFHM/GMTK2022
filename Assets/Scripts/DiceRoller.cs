using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    //config parameters
    [SerializeField] List<DieStats> allDice;

    //cached references
    ActionLog actionLog;
    ThreatMeter threatMeter;
    ResourceManager resourceManager;
    List<WispTracker> wispTrackers;
    int rolledSum;
    int earnedWisps;



    // Start is called before the first frame update
    void Start()
    {
        actionLog = FindObjectOfType<ActionLog>();
        threatMeter = FindObjectOfType<ThreatMeter>();
        resourceManager = FindObjectOfType<ResourceManager>();
        wispTrackers = new List<WispTracker>(FindObjectsOfType<WispTracker>());
        foreach (DieStats die in allDice)
        {
            int randomValue = Random.Range(die.minValue, die.maxValue + 1);
            die.currentValue = randomValue;        
        }
        rolledSum = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;
        EarnWisps(rolledSum);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollDice()
    {
        //increase threat
        if (threatMeter.currentThreatValue < allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue)
        {
            threatMeter.currentThreatValue++;
        }
        actionLog.myText = "\n" + actionLog.myText;
        foreach (DieStats die in allDice)
        {
            int randomValue = Random.Range(die.minValue, die.maxValue + 1);
            die.currentValue = randomValue;
            actionLog.myText = die.name + " die rolled a " + randomValue.ToString() + "!\n" + actionLog.myText;
            die.transform.position = die.startingPos;
            die.locked = false;
        }
        rolledSum = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;
        EarnWisps(rolledSum);
        CheckThreat(rolledSum);
    }


    private void EarnWisps(int rolledSum)
    {
        foreach (WispTracker wispTracker in wispTrackers)
        {
            if (rolledSum >= wispTracker.currentThreshold)
            {
                earnedWisps++;
            }
        }
        if (earnedWisps > 0)
        {
            actionLog.myText = "Earned " + earnedWisps.ToString() + " Celestial Wisps from passed thresholds!\n" + actionLog.myText;
            resourceManager.currentCelestial += earnedWisps;
        }
        earnedWisps = 0;
    }
    private void CheckThreat(int rolledSum)
    {
        if(rolledSum < threatMeter.currentThreatValue)
        {
            int threatDiff = threatMeter.currentThreatValue - rolledSum;
            resourceManager.currentCelestial -= threatDiff;
            if (resourceManager.currentCelestial < 0)
            {
                actionLog.myText = "Uh oh. You rolled lower than the current threat level AND didn't have enough Celestial Wisps. " +
                    "You've lost the game...\n" + actionLog.myText;
            }
            else
            {
                actionLog.myText = "Oh no! You rolled lower than the current threat level. You paid " + threatDiff.ToString() +
                    " to hold off the darkness!\n" + actionLog.myText;
            }
        }
    }
}
