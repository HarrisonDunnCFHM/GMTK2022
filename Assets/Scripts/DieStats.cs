using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DieStats : MonoBehaviour
{

    //config variables
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] public ParticleSystem myParticles;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float snapDist = 0.3f;

    //cached references
    Rigidbody2D myRigidbody;
    List<ActionCard> allActions;
    public List<DieStats> otherDice;
    public Vector3 startingPos;

    public int maxValue = 6;
    public int minValue = 1;
    public int currentValue;
    bool hovered = false;
    public bool grabbed = false;
    public bool locked = false;
    public bool moving = false;
    public bool wrongMove = false;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        allActions = new List<ActionCard>(FindObjectsOfType<ActionCard>());
        //currentValue = maxValue;
        otherDice = new List<DieStats>(FindObjectsOfType<DieStats>());
        otherDice.Remove(this);
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentValue == 0)
        {
            valueText.text = "";
        }
        else
        {
            valueText.text = currentValue.ToString();
        }
        statusText.text = name + ": " + minValue.ToString() + " - " + maxValue.ToString();
        GrabDice();
        ReturnHome();
    }

    private void ReturnHome()
    {
        if (moving)
        {
            GetComponent<Collider2D>().enabled = false;
            myRigidbody.velocity = (startingPos - myRigidbody.transform.position) * moveSpeed;
            if (!wrongMove)
            {
                myParticles.Play();
            }
        }
        var distToPos = Vector2.Distance(startingPos, myRigidbody.transform.position);
        if(distToPos <= snapDist && moving)
        {
            transform.position = startingPos;
            GetComponent<Collider2D>().enabled = true;
            moving = false;
            wrongMove = false;
        }
    }

    private void GrabDice()
    {
        if (moving)
        {
            myRigidbody.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            return; 
        }
        if (!grabbed)
        {
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            foreach (DieStats otherDie in otherDice)
            {
                if(otherDie.grabbed)
                {
                    return;
                }
            }
        }
        if (locked) { return; }
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var adjustedMousePos = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        if((Input.GetMouseButton(0) && hovered) || grabbed)
        {
            //moving = true;
            grabbed = true;
            myRigidbody.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            foreach(ActionCard action in allActions)
            {
                action.GetComponent<Collider2D>().enabled = false;
            }
            if(transform.position != adjustedMousePos)
            {
                myRigidbody.velocity = (adjustedMousePos - myRigidbody.transform.position) * moveSpeed;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            grabbed = false;
            foreach (ActionCard action in allActions)
            {
                action.GetComponent<Collider2D>().enabled = true;
            }
        }
    }

 

    private void OnMouseEnter()
    {
        hovered = true;
    }

    private void OnMouseExit()
    {
        hovered = false;
    }
}
