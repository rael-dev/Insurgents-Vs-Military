using UnityEngine;
using System.Collections;


/*
 * Author: Israel Anthony
 * Purpose: Calculate steering forces for generals.
 * Caveats: None
 */ 
public class MilitaryGeneral : MovementForces 
{
	// Use this for initialization
	void Start () 
	{
		// Error checking
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
		maxForce = 25.0f;
		safeZone = new Vector3(206.0f, 0.0f, 486.0f);

		position = gameObject.transform.position;
	}


	// Update is called once per frame
	void Update () 
	{
		UpdatePosition ();
		ReturnCenter ();
		SetTransform ();
	}


	void UpdatePosition()
	{
		if (target == null) 
		{
			//Step 0: update position to current tranform
			position = gameObject.transform.position;

			Vector3 wanderingForce = Wander () * 0.75f;
			ApplyForce (wanderingForce);

			//AvoidObstacle();

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
			Vector3 pursuingForce = Pursue(target.transform.position) * 0.75f;
			ApplyForce (pursuingForce);

			//AvoidObstacle();

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

			// Point Behind vector
			behaviourMngr.matPurple.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex (position - direction * 3.0f);
			GL.End ();

			// Black line to target
			if (null != targetList) 
			{
				foreach (GameObject v in targetList) 
				{
					if (v == target) 
					{
						behaviourMngr.matBlack.SetPass (0);
						GL.Begin (GL.LINES);
						GL.Vertex (position);
						GL.Vertex (v.transform.position);
						GL.End ();
					}
				}
			}

			GL.PopMatrix ();
		}
	}
}
