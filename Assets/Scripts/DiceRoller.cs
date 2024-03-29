using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    //config parameters
    [SerializeField] List<DieStats> allDice;
    [SerializeField] float dieSpin = 0.5f;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] List<AudioClip> shakeSounds;
    [SerializeField] Button rerollButton;
    [SerializeField] TextMeshProUGUI rerollButtonText;
    [SerializeField] ShootingResource shootingWisp;
    [SerializeField] GameObject wispBankIcon;
    [SerializeField] float shootDelay = 0.3f;
    [SerializeField] float spawnVectorMultiplier = 20;
    [SerializeField] List<WispTracker> wispTrackers;
    [SerializeField] GameObject threatTracker;

    //cached references
    ActionLog actionLog;
    SceneChanger sceneChanger;
    ThreatMeter threatMeter;
    AudioManager audioManager;
    ResourceManager resourceManager;
    List<ActionCard> actionCards;
    int rolledSum;
    int earnedWisps;
    bool rolling = false;
    bool allDiceLocked = false;
    bool trackersUpgradeable = false;
    bool gameLost = false;
    public int addedWisps = 0;

    // Start is called before the first frame update
    void Start()
    {
        actionLog = FindObjectOfType<ActionLog>();
        sceneChanger = FindObjectOfType<SceneChanger>();
        audioManager = FindObjectOfType<AudioManager>();
        threatMeter = FindObjectOfType<ThreatMeter>();
        resourceManager = FindObjectOfType<ResourceManager>();
        //wispTrackers = new List<WispTracker>(FindObjectsOfType<WispTracker>());
        actionCards = new List<ActionCard>(FindObjectsOfType<ActionCard>());
        StartCoroutine(RollDice());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateContextualRollButton();
    }

    private void UpdateContextualRollButton()
    {
        allDiceLocked = true;
        trackersUpgradeable = false;
        foreach(DieStats die in allDice)
        {
            if(!die.locked)
            {
                allDiceLocked = false;
                break;
            }
        }
        foreach(WispTracker wispTracker in wispTrackers)
        {
            if(wispTracker.upgradeable)
            {
                trackersUpgradeable = true;
                break;
            }
        }
        if(!allDiceLocked)
        {
            rerollButtonText.text = "use dice";
            rerollButton.interactable = false;
        }
        else if(trackersUpgradeable)
        {
            rerollButtonText.text = "upgrade a tracker";
            rerollButton.interactable = false;
        }
        else
        {
            rerollButtonText.text = "roll dice";
            rerollButton.interactable = true;
        }
    }

    public void ButtonRollDice()
    {
        if (rolling || sceneChanger.gameOver) { return; }
        if (threatMeter.currentThreatValue < allDice[0].maxValue + allDice[1].maxValue + allDice[2].maxValue)
        {
            threatMeter.currentThreatValue++;
        }
        foreach(ActionCard card in actionCards)
        {
            card.hasDie = false;
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
            addedWisps = 0;
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
        StartCoroutine(EarnWisps());
        rolling = false;
    }


    private IEnumerator EarnWisps()
    {
        rolledSum = allDice[0].currentValue + allDice[1].currentValue + allDice[2].currentValue;
        foreach (WispTracker wispTracker in wispTrackers)
        {
            yield return new WaitForSeconds(shootDelay);
            if (rolledSum >= wispTracker.currentThreshold)
            {
                //earnedWisps++;
                ShootResource(1, wispTracker.gameObject, wispBankIcon);
            }
        }
        if (earnedWisps > 0)
        {
            actionLog.myText = "Earned " + earnedWisps.ToString() + " Celestial Wisps from passed thresholds!\n" + actionLog.myText;
            resourceManager.currentCelestial += earnedWisps;
        }
        earnedWisps = 0;
        StartCoroutine(CheckThreat(rolledSum));
    }

    private void ShootResource(int resourceGain, GameObject shootingResourceOrigin, GameObject shootingResourceDestination)
    {
        float randomF = Random.Range(1.4f, 2.2f);
        for (int i = 0; i < resourceGain; i++)
        {
            ShootingResource shotResource = Instantiate(shootingWisp, shootingResourceOrigin.transform.position, Quaternion.identity);
            shotResource.targetObj = shootingResourceDestination;
            if(shootingResourceDestination == wispBankIcon)
            {
                shotResource.increaseResource = true;
                shotResource.resourceToIncrease = ActionCard.ResourceType.Wisp;
            }
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(0f, 1f);
            Vector2 tempVelocity = new Vector2(randomX * spawnVectorMultiplier, randomY * spawnVectorMultiplier);
            shotResource.gameObject.GetComponent<Rigidbody2D>().velocity = tempVelocity;
            shotResource.GetComponent<AudioSource>().pitch = randomF + (i * 0.1f);

        }
    }

    private IEnumerator CheckThreat(int rolledSum)
    {
        if(rolledSum < threatMeter.currentThreatValue)
        {
            int threatDiff = threatMeter.currentThreatValue - rolledSum;
            int wispsToShoot = Mathf.Min(threatDiff,resourceManager.currentCelestial);
            for (int i = 0; i < wispsToShoot; i++)
            {
                yield return new WaitForSeconds(shootDelay);
                ShootResource(1, wispBankIcon, threatTracker);
                resourceManager.currentCelestial--;
                addedWisps++;
                rolledSum++;
                foreach (WispTracker tracker in wispTrackers)
                {
                    if (rolledSum == tracker.currentThreshold)
                    {
                        ShootResource(1, tracker.gameObject, wispBankIcon);
                        wispsToShoot++;
                        //resourceManager.currentCelestial++;
                    }
                }
                if (rolledSum == threatMeter.currentThreatValue) { break; }
            }
            if(rolledSum < threatMeter.currentThreatValue)
            {
                gameLost = true;
                sceneChanger.gameOver = true;
            }
            /*
            if (threatDiff <= resourceManager.currentCelestial)
            {
                resourceManager.currentCelestial -= threatDiff;
            }
            else
            {
                gameLost = true;
                wispsToShoot = resourceManager.currentCelestial;
                resourceManager.currentCelestial = 0;
            }
            if (gameLost)
            {
                actionLog.myText = "Uh oh. You rolled lower than the current threat level AND didn't have enough Celestial Wisps. " +
                    "You've lost the game...\n" + actionLog.myText;
            }
            else
            {
                actionLog.myText = "Oh no! You rolled lower than the current threat level. You paid " + threatDiff.ToString() +
                    " Wisps to hold off the darkness!\n" + actionLog.myText;
            }
            for (int i = 0; i < wispsToShoot; i++)
            {
                yield return new WaitForSeconds(shootDelay);
                ShootResource(1, wispBankIcon, threatTracker);
                addedWisps++;
                rolledSum++;
                foreach(WispTracker tracker in wispTrackers)
                {
                    if (rolledSum == tracker.currentThreshold)
                    {
                        ShootResource(1, tracker.gameObject, wispBankIcon);
                        resourceManager.currentCelestial++;
                    }
                }
            }*/
        }
    }
}
