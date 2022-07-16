using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCard : MonoBehaviour
{
    public enum Action { AddMax, AddMin, GainXSun, GainXMoon, GainXStar, GainOneCelestial};
    
    //config paramters
    [SerializeField] Action myAction;
    [SerializeField] GameObject myActionSlot;

    //cached refs
    ActionLog actionLog;
    ResourceManager resourceManager;
    bool invalidAction = false;
    
    // Start is called before the first frame update
    void Start()
    {
        actionLog = FindObjectOfType<ActionLog>();
        resourceManager = FindObjectOfType<ResourceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<DieStats>())
        {
            TriggerAbility(myAction, collision.GetComponent<DieStats>());
            if (invalidAction)
            {
                collision.transform.position = collision.GetComponent<DieStats>().startingPos;
                collision.GetComponent<DieStats>().grabbed = false;
                invalidAction = false;
            }
            else
            {
                collision.transform.position = myActionSlot.transform.position;
                collision.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                collision.GetComponent<DieStats>().locked = true;
                collision.GetComponent<DieStats>().grabbed = false;
            }
        }
    }

    private void TriggerAbility(Action myAction, DieStats die)
    {
        switch (myAction)
        {
            case Action.AddMax: 
                IncreaseMaxValue(die);
                break;
            case Action.AddMin:
                IncreaseMinValue(die);
                break;
            case Action.GainXSun:
            case Action.GainXMoon:
            case Action.GainXStar:
                GainXResource(die);
                break;
                //GainXResource(die);
                //break;
                //GainXResource(die);
                //break;
            case Action.GainOneCelestial:
                GainXResource(die);
                break;
            default:
                actionLog.myText = "Error! No action defined!\n" + actionLog.myText;
                Debug.Log("no action defined");
                break;
        }
    }

    ///ABILITIES///

    private void IncreaseMaxValue(DieStats die)
    {
        die.maxValue++;
        actionLog.myText = die.name + " die max value increased to " + die.maxValue.ToString() + "! (+1)\n" + actionLog.myText;
    }

    private void IncreaseMinValue(DieStats die)
    {
        if (die.minValue + 1 >= die.maxValue)
        {
            actionLog.myText = die.name + " die min value can't be equal to or greater than its highest value!\n" + actionLog.myText;
            invalidAction = true;
        }
        else
        {
            die.minValue++;
            actionLog.myText = die.name + " die min value increased to " + die.maxValue.ToString() + "! (+1)\n" + actionLog.myText;
        }
    }

    private void GainXResource(DieStats die)
    {
        switch(myAction)
        {
            case Action.GainXSun:
                resourceManager.currentSun += die.currentValue;
                actionLog.myText = "Gained " + die.currentValue.ToString() + " Sunbeams\n" + actionLog.myText;
                break;
            case Action.GainXMoon:
                resourceManager.currentMoon += die.currentValue;
                actionLog.myText = "Gained " + die.currentValue.ToString() + " Moondrops\n" + actionLog.myText;
                break;
            case Action.GainXStar:
                resourceManager.currentStar += die.currentValue;
                actionLog.myText = "Gained " + die.currentValue.ToString() + " Stardust\n" + actionLog.myText;
                break;
            case Action.GainOneCelestial:
                resourceManager.currentCelestial++;
                actionLog.myText = "Gained 1 Celestial Wisp!\n" + actionLog.myText;
                break;
            default:
                actionLog.myText = "Error! No action defined!\n" + actionLog.myText;
                Debug.Log("no action defined");
                break;
        }
    }
}
