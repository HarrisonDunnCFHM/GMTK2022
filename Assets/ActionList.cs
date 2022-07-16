using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ActionList 
{
    public static void IncreaseMaxValue(DieStats die)
    {
        die.maxValue++;
    }
    public static bool IncreaseMinValue(DieStats die)
    {
        if(die.minValue + 1 >= die.maxValue)
        {
            return false;
        }
        else 
        { 
            die.minValue++;
            return true;
        }
    }
}
