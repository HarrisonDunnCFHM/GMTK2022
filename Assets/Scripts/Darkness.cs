using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{
    //config parameters
    [SerializeField] float turnSpeed;
    

    //cached references
    public float targetZ;
    public bool rotatingUp;
    Vector3 targetRot;

    // Start is called before the first frame update
    void Start()
    {
        turnSpeed = Random.Range(-10f, 10f);
        float startingTarget = Random.Range(-45f, 45f);
        targetZ = startingTarget;
        rotatingUp = (targetZ >= 0); 
        targetRot = new Vector3(0, 0, targetZ);
    }

    // Update is called once per frame
    void Update()
    {
        RotateDarkness();
    }

    private void RotateDarkness()
    {
        if(rotatingUp)
        {
            transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, targetRot, turnSpeed * Time.deltaTime);
            if(targetZ - transform.eulerAngles.z <= 1)
            {
                transform.eulerAngles = targetRot;
                targetZ = Random.Range(-45, transform.eulerAngles.z);
                targetRot = new Vector3(0, 0, targetZ);
                rotatingUp = false;
            }
        }
        if(!rotatingUp)
        {
            transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, targetRot, turnSpeed * Time.deltaTime);
            if (transform.rotation.z - targetZ  <= 1)
            {
                transform.eulerAngles = targetRot;
                targetZ = Random.Range(transform.eulerAngles.z, 45f);
                targetRot = new Vector3(0, 0, targetZ);
                rotatingUp = true;
                
            }
        }
    }
}
