﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetBullet : MonoBehaviour {

    //Components
    private Rigidbody2D rb2D;
    new private SpriteRenderer renderer;

    //Information Variables
    private CMoveCombatable caster;
    public int damage;
    private float stunTime;
    private bool hitTarget;

	//Target Variables
	private CMoveCombatable target;

	//Lifetime Variables
    private float lifespan = 3.1f;
    private float timeShot = 0f;
    private float durability = 3;

    //Rebound Variables
    private float rebounded = 0;
    private float reboundTime = 0.1f;

    //Speed Variables
    private float velocity = .25f;
    private Vector3 dir;

	// Use this for initialization
	public void Setup(CMoveCombatable caster, int damage, float stunTime, Vector3 dir)
    {
        rb2D = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        hitTarget = false;

        this.caster = caster;
        this.damage = damage;
        this.stunTime = stunTime;
        this.dir = dir;

		target = null;
        timeShot = Time.time;

        Vector3 spawnPos = new Vector3(caster.transform.position.x, caster.transform.position.y + caster.objectHeight / 2, caster.transform.position.z);
        transform.position = spawnPos + (dir * 0.4f) ;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //If bullet is rebounding then don't detect collisions
        if(rebounded + reboundTime > Time.time)
            return;
        //If an object has been hit first, destroy the bullet
        if (collider.transform.gameObject.tag == "Object")
        {
            if (!collider.isTrigger)
                this.gameObject.SetActive(false);
            //Add destoryed particle effect here
        }

        CHitable targetHit = collider.GetComponentInParent<CHitable>();

        //If object hit is hitable, and this bullet hasn't hit anything else this life
        if (targetHit != null && !hitTarget)
        {

            if (targetHit.isInvuln() || targetHit.isKnockedback())
                return;

            CMoveCombatable enemy = collider.GetComponentInParent<CMoveCombatable>();
            if(enemy != null && enemy.parrying){
                dir *= -1;
                rebounded = Time.time;
                return;
            }
            hitTarget = true;

            //Apply damage and knockback
            targetHit.setAttacker(caster);
            //objectHit.knockback(pos, abilityKnockback, objectHit.objectHeight); //Need to use original pos for knockback so the position of where you attacked from is the knockback
            targetHit.loseHealth(damage);

            //Apply stun to the target
            targetHit.applyStun(stunTime);

            //TODO: Play audio sound
            caster.attackHit();
			durability--;

			if(durability == 0)
            	this.gameObject.SetActive(false);
			else
			{
				findNewTarget();
				if(target == null)
            		this.gameObject.SetActive(false);
			}
        }
    }

	private void findNewTarget()
	{
		target = null;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);

		int i = 0;
        while (i < hitColliders.Length && target == null)
        {
            CMoveCombatable character = hitColliders[i].GetComponentInParent<CMoveCombatable>();
			if(character != null && FactionManager.instance.isHostile(caster.faction, character.faction))
				target = character;
            i++;
        }
		Debug.Log(hitColliders.Length);
	}

    private void Update()
    {
        if(Time.time - lifespan > timeShot)
            this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
		if(target != null)
			dir = target.getDirection(target.transform.position, target.objectHeight);

        rb2D.MovePosition(transform.position + (dir * velocity));
    }
}
