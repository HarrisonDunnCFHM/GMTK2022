using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingResource : MonoBehaviour
{
    //config parameters
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float snapDist = 0.01f;
    [SerializeField] float spawnCooldown = 0.3f;
    [SerializeField] GameObject myParticleSystem;
    [SerializeField] float timeOfLerp = 0.1f;

    //cached references
    public GameObject targetObj;
    Rigidbody2D myRigidbody;
    bool originalVelocitySaved = false;
    Vector2 originalVelocity;
    Vector2 targetVelocity;
    float lerpTimer = 0;
    bool destroying = false;


    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if(!originalVelocitySaved)
        {
            originalVelocity = myRigidbody.velocity;
            targetVelocity = targetObj.transform.position - myRigidbody.transform.position;
            originalVelocitySaved = true;
        }
        if (lerpTimer < timeOfLerp)
        {
            myRigidbody.velocity = Vector2.Lerp(originalVelocity, targetObj.transform.position - myRigidbody.transform.position, lerpTimer / timeOfLerp);
            lerpTimer += Time.deltaTime;
        }
        else
        {
            myRigidbody.velocity = (targetObj.transform.position - myRigidbody.transform.position).normalized * moveSpeed;
        }
        //myRigidbody.velocity = Vector2.MoveTowards(myRigidbody.velocity, (targetPos - myRigidbody.transform.position), moveSpeed * Time.deltaTime);

        var distToTarget = Vector2.Distance(transform.position, targetObj.transform.position);
        if (distToTarget < snapDist)
        {
            transform.position = targetObj.transform.position;
            myRigidbody.velocity = Vector2.zero;
            if (!destroying)
            {
                destroying = true;
                if (targetObj.GetComponent<DieRangeText>())
                { 
                    targetObj.GetComponent<DieRangeText>().myText.fontSize = targetObj.GetComponent<DieRangeText>().enlargedFontSize;
                    targetObj.GetComponent<DieRangeText>().myText.color = targetObj.GetComponent<DieRangeText>().targetColor;
                }
                GetComponent<SpriteRenderer>().enabled = false;
                GameObject myParticles = Instantiate(myParticleSystem, transform.position, Quaternion.identity);
                myParticles.GetComponent<ParticleSystem>().Play();
                GetComponent<AudioSource>().volume = FindObjectOfType<AudioManager>().masterVolume * .1f;
                GetComponent<AudioSource>().Play();
                //AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, Camera.main.transform.position, FindObjectOfType<AudioManager>().masterVolume);
                Destroy(myParticles, 1f);
                Destroy(gameObject, GetComponent<AudioSource>().clip.length);
            }
        }
        /*if (spawnCooldown < 0f)
        {
            myRigidbody.velocity = (targetPos - myRigidbody.transform.position) * moveSpeed;
            var distToTarget = Vector2.Distance(transform.position, targetPos);
            if (distToTarget < snapDist)
            {
                Destroy(gameObject);
            }
        }
        else { spawnCooldown -= Time.deltaTime; }*/
    }
}
