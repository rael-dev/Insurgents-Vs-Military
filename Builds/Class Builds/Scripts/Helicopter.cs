using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: Calculate steering forces for helicopters.
 * Caveats: None
 */ 
public class Helicopter : MovementForces 
{
	public List<GameObject> waypointList;
	public List<GameObject> flockerList;

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

		mass = 5.0f;
		maxSpeed = 30.0f;
		maxForce = 40.0f;
		safeZone = new Vector3(250.0f, 0.0f, 500.0f);

		position = gameObject.transform.position;

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

			target = targetList [closestIndex];
		}
		else 
		{
			target = null;
		}
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
		//Step 0: update position to current tranform
		position = gameObject.transform.position;
	
		Vector3 seekingForce = Seek (target.transform.position);
		ApplyForce (seekingForce);
		SimplePathFollow ();

		Vector3 alignForce = Align (flockerList) * 4.0f;
		ApplyForce (alignForce);

		Vector3 cohesionForce = Cohesion (flockerList) * 3.0f;
		ApplyForce (cohesionForce);

		//Step 1: Add Acceleration to Velocity * Time
		velocity += acceleration * Time.deltaTime;
		//Step 2: Add vel to position * Time
		position += velocity * Time.deltaTime;
		//Step 3: Reset Acceleration vector
		acceleration = Vector3.zero;
		//Step 4: Calculate direction (to know where we are facing)
		direction = velocity.normalized;
	}


	public void SetFlockerList(List<GameObject> flockers)
	{
		flockerList = flockers;
	}

	/// <summary>
	/// Switches the target to the next waypoint in the list.
	/// </summary>
	void SimplePathFollow()
	{
		BoundingSphere waypointBS = target.GetComponent<BoundingSphere> ();

		int index = 0;

		for (int i = 0; i < targetList.Count; i++) 
		{
			if (targetList [i] == target) 
			{
				index = i;
			}
		}

		if(gameObject.GetComponent<BoundingSphere>().IsColliding(waypointBS))
		{
			index++;
		}

		if(index >= targetList.Count)
		{
			index = 0;
		}

		target = targetList [index];
	}


	public Vector3 Align (List<GameObject> vehicles)
	{
		float neighborDist = 100.0f;
		Vector3 sum = Vector3.zero;
		int count = 0;

		for(int i = 0; i < vehicles.Count; i++)
		{
			float d = Vector3.Distance (gameObject.transform.position, vehicles[i].transform.position);
			if (d > 0.0f && d < neighborDist) 
			{
				sum += vehicles[i].GetComponent<Helicopter> ().velocity;
				count++;
			}
		}

		if (count > 0) 
		{
			sum /= count;
			sum.Normalize ();
			sum *= maxSpeed;

			Vector3 steeringForce = sum - velocity;
			return steeringForce;
		}
		else 
		{
			return Vector3.zero;
		}
	}


	public Vector3 Cohesion (List<GameObject> vehicles)
	{
		float neighborDist = 100.0f;
		Vector3 sum = Vector3.zero;
		int count = 0;

		for(int i = 0; i < vehicles.Count; i++)
		{
			float d = Vector3.Distance (gameObject.transform.position, vehicles[i].transform.position);
			if (d > 0 && d < neighborDist) 
			{
				sum += vehicles[i].transform.position;
				count++;
			}
		}

		if (count > 0) 
		{
			sum /= count;
			return Seek (sum);
		}
		else 
		{
			return Vector3.zero;
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
			GL.Vertex (position + gameObject.transform.forward * -3);
			GL.End ();

			// Right vector
			behaviourMngr.matGreen.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex (position + gameObject.transform.right * -3);
			GL.End ();

			// Future Position vector
			behaviourMngr.matGreen.SetPass (0);
			GL.Begin (GL.LINES);
			GL.Vertex (position);
			GL.Vertex (position + velocity);
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
