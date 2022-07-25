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
    public Vector3 targetPos;
    Rigidbody2D myRigidbody;
    bool originalVelocitySaved = false;
    Vector2 originalVelocity;
    Vector2 targetVelocity;
    float lerpTimer = 0;


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
            targetVelocity = targetPos - myRigidbody.transform.position;
            originalVelocitySaved = true;
        }
        if (lerpTimer < timeOfLerp)
        {
            myRigidbody.velocity = Vector2.Lerp(originalVelocity, targetPos - myRigidbody.transform.position, lerpTimer / timeOfLerp);
            lerpTimer += Time.deltaTime;
        }
        else
        {
            myRigidbody.velocity = (targetPos - myRigidbody.transform.position).normalized * moveSpeed;
        }
        //myRigidbody.velocity = Vector2.MoveTowards(myRigidbody.velocity, (targetPos - myRigidbody.transform.position), moveSpeed * Time.deltaTime);

        var distToTarget = Vector2.Distance(transform.position, targetPos);
        if (distToTarget < snapDist)
        {
            transform.position = targetPos;
            myRigidbody.velocity = Vector2.zero;
            GameObject myParticles = Instantiate(myParticleSystem, transform.position, Quaternion.identity);
            myParticles.GetComponent<ParticleSystem>().Play();
            Destroy(myParticles, 1f);
            Destroy(gameObject);
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
