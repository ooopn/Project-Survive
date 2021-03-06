﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CHitable : MonoBehaviour {

    //Components
    [HideInInspector]
    public Rigidbody2D rb2D;
    protected HealthBar healthBar;

    [HideInInspector]
    public float objectHeight;

    //Health Variables
    [Header("Health Variables")]
    public int maxHealth = 100;
    public int currentHealth { get; protected set; }

    //Invulnerable Variables
    public bool invulnerable = false;
    public float invulnTime = 0.3f;
    protected bool knockedback = false;

    //Attacker Variables
    public string lastAttackInfo;
    protected CMoveCombatable lastAttacker;

    //Abstract Functions
    protected abstract IEnumerator flash();
    protected abstract void death();
    public abstract void knockUp(Vector2 target, int knockbackForce, int knockupForce, float targetHeight);
    

    public void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        //Only create a health bar if the object doesn't already have one
        if (healthBar == null)
        {
            healthBar = UIManager.instance.newHealthBar();
            healthBar.setTarget(transform);
            healthBar.setActive(false);
        }
    }

    public abstract void applyStun(float stunTime);

    public bool isInvuln()
    {
        return invulnerable;
    }

    public bool isKnockedback()
    {
        return knockedback;
    }

    /// <summary>
    /// Sets the character that landed the last hit on this object
    /// </summary>
    /// <param name="attacker"></param>
    public void setAttacker(CMoveCombatable attacker)
    {
        lastAttacker = attacker;
    }

    public CMoveCombatable getAttacker()
    {
        return lastAttacker;
    }

    public virtual void knockback(Vector2 target, int force, float targetHeight)
    {
        knockedback = true;
        rb2D.AddForce(getDirection(target, targetHeight) * force * -1); //Added object height?
        StartCoroutine("beingKnockedBack");
    }

    public Vector2 getDirection(Vector2 target, float targetHeight)
    {
        //Get position of this
        Vector2 pos = new Vector2(transform.position.x, transform.position.y + objectHeight / 2);

        //Get direction by subtracting player location
        target = (new Vector3(target.x, target.y + targetHeight/2, 0) - (Vector3)pos);
        //Normalize the direction so mouse distance doesn't affect it
        float distance = target.magnitude;

        Vector2 direction = target / (distance + 0.0001f); // This is now the normalized direction. add 0.001f to avoid dividing by 0

        return direction;
    }

    //Set AActive
    public virtual void loseHealth(int damage)
    {
        //UIManager.instance.newTextMessage(this.gameObject, WorldManager.instance.banterGen.getPainYell());
        StartCoroutine("flash");
        currentHealth -= damage;

        //Stop showHealth so it doesn't remove the health bar off an earilier call
        StopCoroutine("showHealth");
        StartCoroutine("showHealth");

        healthBar.loseHealth((float) currentHealth /  (float) maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            death();
        }
        else
        {
            setInvulnerable(invulnTime);
        }
    }

    public void recoverHealth(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthBar.recoverHealth((float)currentHealth / (float)maxHealth);
    }

    public void setInvulnerable(float invulnTime)
    {
        StartCoroutine("invulnerableState", invulnTime);
    }

    IEnumerator invulnerableState(float time)
    {
        invulnerable = true;

        yield return new WaitForSeconds(time);

        invulnerable = false;
    }

    IEnumerator beingKnockedBack()
    {
        knockedback = true;

        yield return new WaitForSeconds(0.001f);

        while (rb2D.velocity.magnitude > 0.6f) //Alter??
        {
            yield return new WaitForSeconds(0.001f);
        }

        knockedback = false;
    }

    IEnumerator showHealth()
    {
        healthBar.setActive(true);

        yield return new WaitForSeconds(1f);

        healthBar.setActive(false);
    }

}
