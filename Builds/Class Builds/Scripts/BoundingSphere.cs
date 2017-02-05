using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: Creates a bounding sphere for an object that is used for collision detection.
 * Caveats: None
 */ 
public class BoundingSphere : MonoBehaviour
{
	private Vector3 position;
	public float radius = 2.0f;
	public bool colliding = false;

	// Use this for initialization
	void Start ()
	{
		if (radius <= 0.0f)
		{
			radius = 1.0f;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		position = gameObject.transform.position;

		if (gameObject.tag == "Soldier" || gameObject.tag == "Terrorist") 
		{
			position.y += 1.5f;
		}
	}

	// Draws the bounding sphere around the object in the scene
	void OnDrawGizmos()
	{
		if (colliding)
		{
			Gizmos.color = new Color (1.0f, 0.0f, 0.0f, 0.33f);
		}
		else
		{
			Gizmos.color = new Color (1.0f, 1.0f, 0.0f, 0.50f);
		}
		Gizmos.DrawSphere(position, radius);
	}

	/// <summary>
	/// Determines whether this instance is colliding the specified other.
	/// </summary>
	/// <returns><c>true</c> if this instance is colliding the specified other; otherwise, <c>false</c>.</returns>
	/// <param name="other">Other.</param>
	public bool IsColliding(BoundingSphere other)
	{
		bool output = false;
		if(radius + other.radius > Vector3.Distance(position, other.transform.position))
		{
			output = true;
		}
		return output;
	}
}