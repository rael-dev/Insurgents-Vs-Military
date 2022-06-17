using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: Places weapons in the position of the soldiers'/insurgents' hands.
 * Caveats: None
 */ 
public class WeaponPlacer : MonoBehaviour 
{
	public List<GameObject> insurgentWeapons;
	public List<GameObject> militaryWeapons;
	private GameObject weapon;

	// Use this for initialization
	void Start () 
	{
		if (gameObject.tag == "Soldier") 
		{
			int index = Random.Range (0, militaryWeapons.Count);
			weapon = Instantiate(militaryWeapons[index]);
		} 
		else 
		{
			weapon = Instantiate(insurgentWeapons[0]);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetWeapon ();
	}

	void SetWeapon()
	{
		Vector3 position = gameObject.transform.position + gameObject.transform.forward;
		position.y += 1.5f;

		weapon.transform.position = position;

		weapon.transform.right = gameObject.transform.forward;
	}
}
