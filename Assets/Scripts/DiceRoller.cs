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

    
    // Start is called before the first frame update
    void Start()
    {
        actionLog = FindObjectOfType<ActionLog>();
        threatMeter = FindObjectOfType<ThreatMeter>();
        foreach (DieStats die in allDice)
        {
            int randomValue = Random.Range(die.minValue, die.maxValue + 1);
            die.currentValue = randomValue;        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollDice()
    {
        //increase threat
        threatMeter.currentThreatValue++;
        actionLog.myText = "\n" + actionLog.myText;
        foreach (DieStats die in allDice)
        {
            int randomValue = Random.Range(die.minValue, die.maxValue + 1);
            die.currentValue = randomValue;
            actionLog.myText = die.name + " die rolled a " + randomValue.ToString() + "!\n" + actionLog.myText;
            die.transform.position = die.startingPos;
            die.locked = false;
        }
    }
}
