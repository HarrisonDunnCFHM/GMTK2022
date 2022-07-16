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
    [SerializeField] float moveSpeed = 10f;

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
        valueText.text = currentValue.ToString();
        statusText.text = name + ": " + minValue.ToString() + " - " + maxValue.ToString();
        GrabDice();
    }

    private void GrabDice()
    {
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
            grabbed = true;
            myRigidbody.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            if(transform.position != adjustedMousePos)
            {
                myRigidbody.velocity = (adjustedMousePos - myRigidbody.transform.position) * moveSpeed;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            grabbed = false;
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
