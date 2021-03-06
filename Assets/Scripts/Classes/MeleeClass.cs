﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeClass : CClass {

	// Use this for initialization
	public MeleeClass () 
	{
		name = "Duelist";
    	setupClass();
		selectRandomAbilities();
	}
	
	public override void setupClass()
	{
		//Set up offensive Abilities
		offensiveAbilityPool = new Ability[1];

        offensiveAbilityPool[0] = new DashStrike();

		//Set up defensive Abilities
		defensiveAbilityPool = new Ability[2];

		defensiveAbilityPool[0] = new DodgeRoll();
		defensiveAbilityPool[1] = new Parry();

		//Set up basic and heavy attacks
		basicAttack = new BasicAttack();
		heavyAttack = new HeavyAttack();
	}
}
