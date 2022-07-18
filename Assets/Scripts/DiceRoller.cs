using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    //config parameters
    [SerializeField] List<DieStats> allDice;
    [SerializeField] float dieSpin = 0.5f;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] List<AudioClip> shakeSounds;

    //cached references
    ActionLog actionLog;
    SceneChanger sceneChanger;
    ThreatMeter threatMeter;
    AudioManager audioManager;
    ResourceManager resourceManager;
    List<WispTracker> wispTrackers;
    List<ActionCard> actionCards;
    int rolledSum;
    int earnedWisps;
    bool rolling = false;

    // Start is called before the first frame update
    void Start()
    {
        actionLog = FindObjectOfType<ActionLog>();
        sceneChanger = FindObjectOfType<SceneChanger>();
        audioManager = FindObjectOfType<AudioManager>();
        threatMeter = FindObjectOfType<ThreatMeter>();
        resourceManager = FindObjectOfType<ResourceManager>();
        wispTrackers = new List<WispTracker>(FindObjectsOfType<WispTracker>());
        actionCards = new List<ActionCard>(FindObjectsOfType<ActionCard>());
        StartCoroutine(RollDice());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonRollDice()
    {
        if (rolling || sceneChanger.gameOver) { return; }
        if (threatMeter.currentThreatValue < allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue)
        {
            threatMeter.currentThreatValue++;
            foreach(ActionCard card in actionCards)
            {
                card.hasDie = false;
            }
        }
        StartCoroutine(RollDice());
    }

    public IEnumerator RollDice()
    {
        rolling = true;
        int randomShake = Random.Range(0, shakeSounds.Count);
        AudioSource.PlayClipAtPoint(shakeSounds[randomShake], Camera.main.transform.position, audioManager.masterVolume);
        //increase threat
        yield return new WaitForSeconds(dieSpin);
        actionLog.myText = "\n" + actionLog.myText;
        foreach(DieStats die in allDice)
        {
            die.GetComponent<Animator>().enabled = true;
            die.currentValue = 0;
        }
        yield return new WaitForSeconds(dieSpin);
        for (int i = 0; i < allDice.Count; i++)
        {
            yield return new WaitForSeconds(dieSpin);
            //allDice[i].transform.position = allDice[i].startingPos;
            int randomValue = Random.Range(allDice[i].minValue, allDice[i].maxValue + 1);
            allDice[i].currentValue = randomValue;
            actionLog.myText = allDice[i].name + " die rolled a " + randomValue.ToString() + "!\n" + actionLog.myText;
            //allDice[i].transform.position = allDice[i].startingPos;
            allDice[i].GetComponent<Animator>().enabled = false;
            allDice[i].GetComponent<SpriteRenderer>().sprite = defaultSprite;
            allDice[i].locked = false;
            allDice[i].moving = true;
            
        }
        yield return new WaitForSeconds(dieSpin);
        rolledSum = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;
        EarnWisps();
        CheckThreat(rolledSum);
        rolling = false;
    }


    private void EarnWisps()
    {
        rolledSum = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;
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
                sceneChanger.gameOver = true;
            }
            else
            {
                actionLog.myText = "Oh no! You rolled lower than the current threat level. You paid " + threatDiff.ToString() +
                    " to hold off the darkness!\n" + actionLog.myText;
            }
        }
    }
}
