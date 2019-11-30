﻿/*
 * Created by:
 * Name: James Sturdgess
 * Sid: 1314371
 * Date Created: 06/10/2019
 * Last Modified 22/10/2019
 * Modified By: Dominik Waldowski
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private ObjectPooling explosionEffect;
    [SerializeField]
    private float blastRadius = 3f;
    public GameObject decal;
    private PlayerBase parent;
    private float bombSpeed = 3;
    private float deathTime = 15;                                   //time until bomb disappears automatically
    private float bombThrowDistance = 5;                           //how far the bomb will travel
    private float bombTopHeight = 2;                                //how high the arc of parabola will be
    private float startTime;                                        //Currently time when bomb activates
    private float journeyLength;                                   //total length of journey travelled by bomb
    private Vector3 endPos;                                        //endPosition of the bomb(where it will land)
    private Vector3 startPos;                                      //start position of the bomb(where it becomes enabled)
    public PlayerBase Parent { get => parent; set => parent = value; }

    RaycastHit hit;
    Ray ray;
    public DrawColor DrawColor { get => drawColor; set => drawColor = value; }
    [SerializeField]
    private DrawColor drawColor;
    private void OnEnable()
    {
        startTime = 0;
        startTime = Time.time;
        startPos = transform.position;                                //used for calculating time
        endPos = this.transform.position + (transform.forward * bombThrowDistance);            //sets destination to which the bomb will move
        startPos = this.transform.position;                              //sets start position which is the current position of the bomb
        journeyLength = Vector3.Distance(startPos, endPos);         //calculates total distance by taking start and end point
        explosionEffect = GameObject.Find("EffectsPool").GetComponent<ObjectPooling>();
        Invoke("DestroyBomb", deathTime);

        drawColor = GameObject.Find("GameManager").GetComponent<DrawColor>();
    }

    //sets bomb to inactive
    private void DestroyBomb()
    {
        this.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Parent = null;
        CancelInvoke("DisableCollision");
    }

    //sets the object that thrown bomb used for calculating whose colour bomb should paint in
    public void SetParent(PlayerBase p)
    {

        Parent = p;
    }

    //Moves the bomb using parabola class. 
    private void Update()
    {
        float distCovered = (Time.time - startTime) * bombSpeed;
        journeyLength = Vector3.Distance(startPos, endPos);
        float fractionOfJourney = distCovered / journeyLength;
        transform.position = MathParabola.Parabola(startPos, endPos, bombTopHeight, fractionOfJourney * bombSpeed);
    }

    //Triggers explosion on collision
    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject != parent.gameObject)
            {
            ExplosionEffect();
            Explode();
            }
    }

    private void ExplosionEffect()
    {
        GameObject newExplosion = explosionEffect.GetPooledObject();
        newExplosion.transform.position = this.transform.position;
        newExplosion.transform.rotation = this.transform.rotation;
        newExplosion.SetActive(true);
        Debug.Log(newExplosion);
    }
    //calculates explosion radius
    private void Explode()
    {
        Vector3 dwn = transform.TransformDirection(-Vector3.up);
        if (Physics.Raycast(transform.position, dwn, out hit, Mathf.Infinity))
        {
            CollideWith(hit.collider.gameObject.tag);
        }
        //TODO: implement bomb laying paint

        DestroyBomb();

    }
    private void CollideWith(string tag)
    {
        switch (tag)
        {
            case "Player":



                break;
            case "PaintableEnvironment":
                //Renderer _wallRenderer = hit.collider.gameObject.GetComponent<Renderer>();

                int _id = parent.Player.skinId;
                float _smult;
                float _tmult;
                if (hit.collider.GetComponent<PaintSizeMultiplier>())
                {
                    _smult = hit.collider.GetComponent<PaintSizeMultiplier>().multiplier;
                    _tmult = hit.collider.GetComponent<PaintSizeMultiplier>().multiplier;
                }
                else
                {
                    _smult = 4.5f;
                    _tmult = 1;
                }
                
                switch (_id)
                {
                    case (0):
                        {
                            DrawColor.DrawOnSplatmap(hit, new Color(1, 0, 0, 0), 200, _smult, _tmult);
                            break;
                        }
                    case (1):
                        {
                            DrawColor.DrawOnSplatmap(hit, new Color(0, 1, 0, 0), 200, _smult, _tmult);
                            break;
                        }
                    case (2):
                        {
                            DrawColor.DrawOnSplatmap(hit, new Color(0, 0, 1, 0), 200, _smult, _tmult);
                            break;
                        }
                    case (3):
                        {
                            DrawColor.DrawOnSplatmap(hit, new Color(0, 0, 0, 1), 200, _smult, _tmult);
                            break;
                        }
                }
                break;

        }
    }
}