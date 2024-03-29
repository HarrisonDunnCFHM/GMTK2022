using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionCard : MonoBehaviour
{
    public enum Action { AddMax, AddMin, GainXSun, GainXMoon, GainXStar, GainOneCelestial, UpgradeTracker };
    public enum ResourceType { Sunbeam, Moondrop, Stardust, Wisp, All};

    //config paramters
    [SerializeField] Action myAction;
    [SerializeField] GameObject myActionSlot;
    [SerializeField] TextMeshProUGUI myCostText;
    [SerializeField] GameObject greyOutBox;
    [SerializeField] ShootingResource shootingResource;
    [SerializeField] List<ShootingResource> eachResource;
    [SerializeField] List<GameObject> eachBankIcon;
    [SerializeField] GameObject shootingResourceDestination;
    [SerializeField] GameObject shootingEachDestination;
    [SerializeField] float shootDelay = 0.1f;
    [SerializeField] float spawnVectorMultiplier = 3f;
    

    //cached refs
    ActionLog actionLog;
    ResourceManager resourceManager;
    List<WispTracker> wispTrackers;
    List<DieStats> allDice;
    DieStats grabbedDie;
    bool invalidAction = false;
    public bool hasDie = false;
    public int actionCost;
    public bool canAfford = true;
    public ResourceType resourceCost;
    int upgradeTrackerCost = 1;


    // Start is called before the first frame update
    void Start()
    {
        actionLog = FindObjectOfType<ActionLog>();
        resourceManager = FindObjectOfType<ResourceManager>();
        wispTrackers = new List<WispTracker>(FindObjectsOfType<WispTracker>());
        allDice = new List<DieStats>(FindObjectsOfType<DieStats>());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCostText();
        UpdateCanAfford();
    }

    private void UpdateCanAfford()
    {
        if(!canAfford || hasDie)
        {
            greyOutBox.SetActive(true);
        }
        else
        {
            greyOutBox.SetActive(false);
        }
    }

    private void UpdateCostText()
    {
        if (myCostText == null) { return; }
        bool dieIsGrabbed = false;
        foreach (DieStats die in allDice)
        {
            if (die.grabbed == true)
            {
                dieIsGrabbed = true;
                grabbedDie = die;
                break;
            }
        }
        if (dieIsGrabbed)
        {
            UpdateCosts(grabbedDie);
        }
        else
        {
            canAfford = true;
            switch(myAction)
            {
                case Action.UpgradeTracker:
                    actionCost = upgradeTrackerCost;
                    resourceCost = ResourceType.All;
                    if (actionCost > resourceManager.currentSun || actionCost > resourceManager.currentMoon || actionCost > resourceManager.currentStar)
                    {
                        canAfford = false;
                    }
                    else { canAfford = true; }
                    myCostText.text = "Cost: " + actionCost.ToString() + " each - Sunbeam, Moondrop, Stardust";
                    break;
                default:
                    myCostText.text = "Grab a die to see cost";
                    break;
            }
        }
    }

    private void UpdateCosts(DieStats grabbedDie)
    {
        int currentAvailable = 0;
        switch (myAction)
        {
            case Action.AddMax:
                string costTypeAsString;
                switch (grabbedDie.name)
                {
                    case "Sun":
                        costTypeAsString = "Sunbeam";
                        resourceCost = ResourceType.Sunbeam;
                        currentAvailable = resourceManager.currentSun;
                        break;
                    case "Moon":
                        costTypeAsString = "Moondrop";
                        resourceCost = ResourceType.Moondrop;
                        currentAvailable = resourceManager.currentMoon;
                        break;
                    case "Star":
                        costTypeAsString = "Stardust";
                        resourceCost = ResourceType.Stardust;
                        currentAvailable = resourceManager.currentStar;
                        break;
                    default:
                        costTypeAsString = "";
                        break;
                }
                actionCost = grabbedDie.maxValue;
                myCostText.text = "Cost: " + actionCost.ToString() + " " + costTypeAsString;
                if(actionCost > currentAvailable)
                {
                    canAfford = false;
                }
                else { canAfford = true; }
                break;
            case Action.AddMin:
                actionCost = grabbedDie.minValue;
                resourceCost = ResourceType.Wisp;
                currentAvailable = resourceManager.currentCelestial;
                if(actionCost > currentAvailable)
                { 
                    canAfford = false;
                }
                else { canAfford = true; }
                myCostText.text = "Cost: " + actionCost.ToString() + " Wisps";
                break;
            case Action.UpgradeTracker:
                actionCost = upgradeTrackerCost;
                    resourceCost = ResourceType.All;
                    if (actionCost > resourceManager.currentSun || actionCost > resourceManager.currentMoon || actionCost > resourceManager.currentStar)
                    {
                        canAfford = false;
                    }
                    else { canAfford = true; }
                    myCostText.text = "Cost: " + actionCost.ToString() + " each - Sunbeam, Moondrop, Stardust";
                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDie) { return; }
        if (collision.GetComponent<DieStats>())
        {
            TriggerAbility(myAction, collision.GetComponent<DieStats>());
            if (invalidAction)
            {
                collision.GetComponent<DieStats>().grabbed = false;
                collision.GetComponent<DieStats>().moving = true;
                collision.GetComponent<DieStats>().wrongMove = true;
                invalidAction = false;
            }
            else
            {
                collision.transform.position = myActionSlot.transform.position;
                collision.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                collision.GetComponent<DieStats>().locked = true;
                collision.GetComponent<DieStats>().grabbed = false;
                collision.GetComponent<DieStats>().myParticles.Play();
                hasDie = true;
            }
        }
    }

    private void TriggerAbility(Action myAction, DieStats die)
    {
        switch (myAction)
        {
            case Action.AddMax:
                if (PayCost(resourceCost,actionCost, die)) { IncreaseMaxValue(die); }
                break;
            case Action.AddMin:
                if (PayCost(resourceCost, actionCost, die)) { IncreaseMinValue(die); }
                break;
            case Action.GainXSun:
            case Action.GainXMoon:
            case Action.GainXStar:
                GainXResource(die);
                break;
            case Action.UpgradeTracker:
                if (PayCost(resourceCost, actionCost, die)) { UpgradeTracker(); }
                break;
            default:
                actionLog.myText = "Error! No action defined!\n" + actionLog.myText;
                Debug.Log("no action defined");
                break;
        }
    }

    private bool PayCost(ResourceType resourceType, int cost, DieStats die)
    {
        switch(resourceType)
        {
            case ResourceType.Sunbeam:
                if(cost <= resourceManager.currentSun)
                {
                    resourceManager.currentSun -= cost;
                    actionLog.myText = "Paid " + cost.ToString() + " Sunbeams...\n" + actionLog.myText;

                    return true;
                }
                /*else if(cost <= (resourceManager.currentSun + resourceManager.currentCelestial))
                {
                    cost -= resourceManager.currentSun;
                    var sunPaid = resourceManager.currentSun;
                    resourceManager.currentSun = 0;
                    resourceManager.currentCelestial -= cost;
                    actionLog.myText = "Paid " + sunPaid.ToString() + " Sunbeams and " + cost.ToString() + " Wisps...\n" + actionLog.myText;
                    return true;
                }*/
                else
                {
                    actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                    invalidAction = true;
                    return false ;
                }
            case ResourceType.Moondrop:
                if (cost <= resourceManager.currentMoon)
                {
                    resourceManager.currentMoon -= cost;
                    actionLog.myText = "Paid " + cost.ToString() + " Moondrops...\n" + actionLog.myText;

                    return true;
                }
                /*else if (cost <= (resourceManager.currentMoon + resourceManager.currentCelestial))
                {
                    cost -= resourceManager.currentMoon;
                    var moonPaid = resourceManager.currentMoon;
                    resourceManager.currentMoon = 0;
                    resourceManager.currentCelestial -= cost;
                    actionLog.myText = "Paid " + moonPaid.ToString() + " Moondrops and " + cost.ToString() + " Wisps...\n" + actionLog.myText;
                    return true;
                }*/
                else
                {
                    actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                    invalidAction = true;
                    return false;
                }
            case ResourceType.Stardust:
                if (cost <= resourceManager.currentStar)
                {
                    resourceManager.currentStar -= cost;
                    actionLog.myText = "Paid " + cost.ToString() + " Stardust...\n" + actionLog.myText;

                    return true;
                }
                /*else if (cost <= (resourceManager.currentStar + resourceManager.currentCelestial))
                {
                    cost -= resourceManager.currentStar;
                    var starPaid = resourceManager.currentStar;
                    resourceManager.currentStar = 0;
                    resourceManager.currentCelestial -= cost;
                    actionLog.myText = "Paid " + starPaid.ToString() + " Stardust and " + cost.ToString() + " Wisps...\n" + actionLog.myText;
                    return true;
                }*/
                else
                {
                    actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                    invalidAction = true;
                    return false;
                }
            case ResourceType.Wisp:
                if(cost <= resourceManager.currentCelestial)
                {
                    resourceManager.currentCelestial -= cost;
                    actionLog.myText = "Paid " + cost.ToString() + " Celestial Wisps...\n" + actionLog.myText;
                    return true;
                }
                else
                {
                    actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                    invalidAction = true;
                    return false;
                }
            case ResourceType.All:
                if(cost <= resourceManager.currentSun 
                    && cost <= resourceManager.currentMoon
                    && cost <= resourceManager.currentStar)
                {
                    resourceManager.currentSun -= cost;
                    resourceManager.currentMoon -= cost;
                    resourceManager.currentStar -= cost;
                    actionLog.myText = "Paid " + cost.ToString() + " each of Sunbeams, Moondrops, Stardust...\n" + actionLog.myText;
                    upgradeTrackerCost++;
                    return true;
                }
                /*
                else if (cost <= resourceManager.currentSun + resourceManager.currentMoon + resourceManager.currentStar + resourceManager.currentCelestial)
                {
                    int tempSun = resourceManager.currentSun;
                    int paidSun = 0;
                    int tempMoon = resourceManager.currentMoon;
                    int paidMoon = 0;
                    int tempStar = resourceManager.currentStar;
                    int paidStar = 0;
                    int tempWisp = resourceManager.currentCelestial;
                    int paidWisp = 0;
                    int tempCost = cost;
                    //check Sun
                    if(cost <= tempSun)
                    {
                        tempSun -= cost;
                        paidSun = cost;
                    }
                    else if(cost <= tempSun + tempWisp)
                    {
                        tempWisp += tempSun;
                        paidSun = tempSun;
                        tempSun = 0;
                        tempWisp -= cost;
                        paidWisp += (cost - paidSun);
                    }
                    else
                    {
                        actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                        invalidAction = true;
                        return false;
                    }
                    //check moon
                    if (cost <= tempMoon)
                    {
                        tempMoon -= cost;
                        paidMoon = cost;
                    }
                    else if (cost <= tempMoon + tempWisp)
                    {
                        tempWisp += tempMoon;
                        paidMoon = tempMoon;
                        tempMoon = 0;
                        tempWisp -= cost;
                        paidWisp += (cost - paidMoon);
                    }
                    else
                    {
                        actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                        invalidAction = true;
                        return false;
                    }
                    //check stars
                    if (cost <= tempStar)
                    {
                        tempStar -= cost;
                        paidStar = cost;
                    }
                    else if (cost <= tempStar + tempWisp)
                    {
                        tempWisp += tempStar;
                        paidStar = tempStar;
                        tempStar = 0;
                        tempWisp -= cost;
                        paidWisp += (cost - paidStar);
                    }
                    else
                    {
                        actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                        invalidAction = true;
                        return false;
                    }
                    resourceManager.currentSun = tempSun;
                    resourceManager.currentMoon = tempMoon;
                    resourceManager.currentStar = tempStar;
                    resourceManager.currentCelestial = tempWisp;
                    actionLog.myText = "Paid " + paidSun.ToString() + " Sunbeams, " +
                        paidMoon.ToString() + " Moondrops, " +
                        paidStar.ToString() + " Stardust, and " +
                        paidWisp.ToString() + " Wisps...\n" + actionLog.myText;
                    upgradeTrackerCost++;
                    return true;
                }*/
                else
                {
                    actionLog.myText = "Insufficient resources!\n" + actionLog.myText;
                    invalidAction = true;
                    return false;
                }
            default:
                return false;
        }
    }

    ///ABILITIES///

    private void IncreaseMaxValue(DieStats die)
    {
        die.maxValue++;
        actionLog.myText = die.name + " die max value increased to " + die.maxValue.ToString() + "!\n" + actionLog.myText;
        StartCoroutine(die.ShootResourceSpend(actionCost,shootDelay, false));
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
            actionLog.myText = die.name + " die min value increased to " + die.minValue.ToString() + "!\n" + actionLog.myText;
            StartCoroutine(die.ShootResourceSpend(actionCost, shootDelay, true));
        }
    }

    private void GainXResource(DieStats die)
    {
        ResourceType gainThisType = ResourceType.Sunbeam;
        switch(myAction)
        {
            case Action.GainXSun:
                //resourceManager.currentSun += die.currentValue;
                gainThisType = ResourceType.Sunbeam;
                actionLog.myText = "Gained " + die.currentValue.ToString() + " Sunbeams\n" + actionLog.myText;
                break;
            case Action.GainXMoon:
                //resourceManager.currentMoon += die.currentValue;
                gainThisType = ResourceType.Moondrop;
                actionLog.myText = "Gained " + die.currentValue.ToString() + " Moondrops\n" + actionLog.myText;
                break;
            case Action.GainXStar:
                //resourceManager.currentStar += die.currentValue;
                gainThisType = ResourceType.Stardust;
                actionLog.myText = "Gained " + die.currentValue.ToString() + " Stardust\n" + actionLog.myText;
                break;
            /*case Action.GainOneCelestial:
                resourceManager.currentCelestial++;
                actionLog.myText = "Gained 1 Celestial Wisp!\n" + actionLog.myText;
                break;*/
            default:
                actionLog.myText = "Error! No action defined!\n" + actionLog.myText;
                Debug.Log("no action defined");
                break;
        }
        StartCoroutine(ShootResourceGain(die.currentValue, gainThisType));
    }

    private IEnumerator ShootResourceGain(int resourceGain, ResourceType resourceToGain)
    {
        float randomF = Random.Range(1.4f, 2.2f);
        for (int i = 0; i < resourceGain; i++)
        {
            ShootingResource shotResource = Instantiate(shootingResource, myActionSlot.transform.position, Quaternion.identity);
            shotResource.targetObj = shootingResourceDestination;
            shotResource.resourceToIncrease = resourceToGain;
            shotResource.increaseResource = true;
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(0f, 1f);
            Vector2 tempVelocity = new Vector2(randomX * spawnVectorMultiplier, randomY * spawnVectorMultiplier);
            shotResource.gameObject.GetComponent<Rigidbody2D>().velocity = tempVelocity;
            shotResource.GetComponent<AudioSource>().pitch = randomF + (i * 0.1f);
            yield return new WaitForSeconds(shootDelay);
        }
    }

    private IEnumerator ShootResourceEach(int resourceSpent)
    {
        for (int i = 0; i < resourceSpent; i++)
        {
            float randomF = Random.Range(1.4f, 2.2f);
            for(int i2 = 0; i2 < eachResource.Count; i2++)
            {
                ShootingResource shotResource = Instantiate(eachResource[i2], eachBankIcon[i2].transform.position, Quaternion.identity);
                shotResource.targetObj = shootingEachDestination;
                float randomX = Random.Range(-1f, 1f);
                float randomY = Random.Range(-1f, 0f);
                Vector2 tempVelocity = new Vector2(randomX * spawnVectorMultiplier, randomY * spawnVectorMultiplier);
                shotResource.gameObject.GetComponent<Rigidbody2D>().velocity = tempVelocity;
                shotResource.GetComponent<AudioSource>().pitch = randomF + (i * 0.1f);
                float timeToDelay = shootDelay * 2 / (float)i;
                yield return new WaitForSeconds(shootDelay * 2);
            }
        }
    }

    private void UpgradeTracker()
    {
        actionLog.myText = "Click a shining tracker to upgrade it!\n" + actionLog.myText;
        foreach (WispTracker wispTracker in wispTrackers)
        {
            var otherTrackers = new List<WispTracker>(FindObjectsOfType<WispTracker>());
            otherTrackers.Remove(wispTracker);
            bool oneBelow = false;
            foreach (WispTracker otherTracker in otherTrackers)
            {
                if(otherTracker.currentThreshold == wispTracker.currentThreshold - 1)
                {
                    oneBelow = true;
                }
            }
            if (!oneBelow)
            {
                wispTracker.upgradeable = true;
            }
        }
        StartCoroutine(ShootResourceEach(actionCost));
    }
}
