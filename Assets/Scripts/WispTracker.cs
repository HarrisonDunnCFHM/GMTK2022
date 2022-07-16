using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispTracker : MonoBehaviour
{
    //config parameters
    [SerializeField] int startingThreshold;

    //cached references
    public int currentThreshold;
    List<DieStats> allDice;
    int distanceFromDiceMax;
    
    // Start is called before the first frame update
    void Start()
    {
        allDice = new List<DieStats>(FindObjectsOfType<DieStats>());
        distanceFromDiceMax = (allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue) - startingThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        currentThreshold = (allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue) - distanceFromDiceMax;
    }
}
