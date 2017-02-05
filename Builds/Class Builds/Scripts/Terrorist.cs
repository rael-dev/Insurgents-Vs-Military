using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: Calculate steering forces for terrorists.
 * Caveats: None
 */ 
public class Terrorist : MovementForces 
{
	// Use this for initialization
	void Start () 
	{
		// Error check
		GameObject sceneMngr = GameObject.Find("SceneManager");
		if(null == sceneMngr)
		{
			Debug.Log("Error in " + gameObject.name + 
				": Requires a SceneManager object in the scene.");
			Debug.Break();
		}
		behaviourMngr = sceneMngr.GetComponent<BehaviourManager>();

		mass = 1.0f;
		maxSpeed = 15.0f;
		maxForce = 20.0f;
		safeZone = new Vector3(206.0f, 0.0f, 486.0f);

		position = gameObject.transform.position;
	}


	// Update is called once per frame
	void Update () 
	{
		UpdatePosition ();
		ReturnToSafety ();
		SetTransform ();
	}


	public override void SetTarget()
	{
		if (targetList.Count > 0) 
		{
			float d = Vector3.Magnitude (position - targetList [0].transform.position);
			int closestIndex = 0;

			// Find the closest target and set that as the target
			for (int i = 0; i < targetList.Count; i++) 
			{
				if (Vector3.Magnitude (position - targetList [i].transform.position) < d) 
				{
					closestIndex = i;
					d = Vector3.Magnitude (position - targetList [i].transform.position);
				}
			}

			if (d < 25.0f) 
			{
				target = targetList [closestIndex];
			} 
			else 
			{
				target = null;
			}
		}
		else 
		{
			target = null;
		}
	}


	void UpdatePosition()
	{
		if (target == null) 
		{
			//Step 0: update position to current tranform
			position = gameObject.transform.position;

			Vector3 wanderingForce = Wander ();
			ApplyForce (wanderingForce);

			AvoidObstacle();

			//Step 1: Add Acceleration to Velocity * Time
			velocity += acceleration * Time.deltaTime;
			//Step 2: Add vel to position * Time
			position += velocity * Time.deltaTime;
			//Step 3: Reset Acceleration vector
			acceleration = Vector3.zero;
			//Step 4: Calculate direction (to know where we are facing)
			direction = velocity.normalized;
		} 
		else 
		{
			//Step 0: update position to current tranform
			position = gameObject.transform.position;

			//Step 0.5: Evade the target
			Vector3 evadingForce = Evade(target.transform.position);
			ApplyForce (evadingForce);

			AvoidObstacle();

			//Step 1: Add Acceleration to Velocity * Time
			velocity += acceleration * Time.deltaTime;
			//Step 2: Add vel to position * Time
			position += velocity * Time.deltaTime;
			//Step 3: Reset Acceleration vector
			acceleration = Vector3.zero;
			//Step 4: Calculate direction (to know where we are facing)
			direction = velocity.normalized;
		}
	}


	/// <summary>
	/// Returns to base.
	/// </summary>
	void ReturnToSafety()
	{
		//Check within X
		if(position.x > 195.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, 0.0f, safeZone.z / 2.0f)) * 20.0f);

		}
		else if(position.x < 5.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, 0.0f, safeZone.z / 2.0f)) * 20.0f);

		}

		//check within Z
		if(position.z > 465.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, 0.0f, safeZone.z / 2.0f)) * 20.0f);

		}
		else if(position.z < 190.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, 0.0f, safeZone.z / 2.0f)) * 20.0f);
		}
	}

	/// <summary>
	/// Raises the render object event. Draws debugging lines.
	/// </summary>
	void OnRenderObject()
	{
		if (debugging) 
		{
			// Forward vector
			GL.PushMatrix ();
			behaviourMngr.matBlue.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex (position + gameObject.transform.forward * 3);
			GL.End ();

			// Right vector
			behaviourMngr.matRed.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex (position + gameObject.transform.right * 3);
			GL.End ();

			// Future Position vector
			behaviourMngr.matGreen.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex (position + velocity);
			GL.End ();
			GL.PopMatrix ();
		}
	}
}
