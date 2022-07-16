using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispTracker : MonoBehaviour
{
    //config parameters
    [SerializeField] int startingThreshold;
    [SerializeField] ParticleSystem myParticles;

    //cached references
    public int currentThreshold;
    List<DieStats> allDice;
    int distanceFromDiceMax;
    public bool upgradeable;
    List<WispTracker> wispTrackers;
    
    // Start is called before the first frame update
    void Start()
    {
        allDice = new List<DieStats>(FindObjectsOfType<DieStats>());
        wispTrackers = new List<WispTracker>(FindObjectsOfType<WispTracker>());
        distanceFromDiceMax = (allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue) - startingThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        currentThreshold = (allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue) - distanceFromDiceMax;
        ManageParticles();
    }

    private void OnMouseDown()
    {
        if(upgradeable)
        {
            Debug.Log("clicked " + name);
            distanceFromDiceMax++;
            foreach(WispTracker wispTracker in wispTrackers)
            {
                wispTracker.upgradeable = false;
            }
        }
    }

    private void ManageParticles()
    {
        if(upgradeable)
        {
            myParticles.gameObject.SetActive(true);
        }
        else
        {
            myParticles.gameObject.SetActive(false);
        }
    }
}
