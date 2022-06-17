using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: Calculate steering forces for tanks.
 * Caveats: None
 */ 
public class Tank : MovementForces 
{
	public List<GameObject> waypointList;
	//private float pathRadius = 10.0f;
	private int closestIndex;
	private GameObject segmentEnd;

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
		maxSpeed = 25.0f;
		maxForce = 30.0f;
		safeZone = new Vector3(206.0f, 0.0f, 486.0f);

		position = gameObject.transform.position;

		if (targetList.Count > 0) 
		{
			float d = Vector3.Distance (gameObject.transform.position, targetList [0].transform.position);
			closestIndex = 0;

			// Find the closest target and set that as the target
			for (int i = 0; i < targetList.Count; i++) 
			{
				if (Vector3.Magnitude (position - targetList [i].transform.position) < d) 
				{
					closestIndex = i;
					d = Vector3.Distance (gameObject.transform.position, targetList [i].transform.position);
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
		//UpdateTarget();
		//ComplexPathFollow ();

		//Step 1: Add Acceleration to Velocity * Time
		velocity += acceleration * Time.deltaTime;
		//Step 2: Add vel to position * Time
		position += velocity * Time.deltaTime;
		//Step 3: Reset Acceleration vector
		acceleration = Vector3.zero;
		//Step 4: Calculate direction (to know where we are facing)
		direction = velocity.normalized;
	}

	/* Tank specific methods */
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


	void UpdateTarget()
	{
		if (Vector3.Distance (target.transform.position, gameObject.transform.position) < 10.0f) 
		{
			if (closestIndex == targetList.Count - 1) 
			{
				target = targetList [0];
				closestIndex = 0;
			}
			else 
			{
				target = targetList [closestIndex + 1];
				closestIndex++;
			}
		}
	}

	void ComplexPathFollow()
	{
		bool inPath= false;
		Vector3 futurePos = gameObject.transform.position + (velocity - gameObject.transform.position);

		Vector3 nearestSegment = Vector3.zero;

		if (closestIndex == targetList.Count - 1) 
		{
			nearestSegment = target.transform.position + (targetList[0].transform.position - target.transform.position);
			segmentEnd = targetList [0];
		}
		else 
		{
			nearestSegment = target.transform.position + (targetList[closestIndex + 1].transform.position - target.transform.position);
			segmentEnd = targetList [closestIndex + 1];
		}
		Debug.Log ("Segment: " + nearestSegment);

		Vector3 closestPoint = Vector3.zero;
		Vector3 begginingToFuture = target.transform.position + (futurePos - target.transform.position);
		float scalarProjection = Vector3.Dot (begginingToFuture, nearestSegment);

		if (scalarProjection > nearestSegment.magnitude) 
		{
			scalarProjection = nearestSegment.magnitude;
		}
		closestPoint = target.transform.position + nearestSegment.normalized * scalarProjection;
		closestPoint.y = 0.0f;
		Debug.Log ("Closest Point: " + closestPoint);
		Debug.Log ("Scalar projection: " + scalarProjection);

		/*
		if (closestIndex == targetList.Count - 1) 
		{
			target = targetList [0];
		}
		else 
		{
			target = targetList [closestIndex + 1];
		}
		*/

		if (Vector3.Distance (futurePos, closestPoint) >= 10.0f) 
		{
			ApplyForce (Seek (closestPoint));
			inPath = false;
		} 
		else 
		{
			inPath = true;
			if (inPath) 
			{
				if (closestIndex == targetList.Count - 1) 
				{
					ApplyForce (Seek (targetList[0].transform.position));
				}
				else 
				{
					ApplyForce (Seek (targetList[closestIndex + 1].transform.position));
				}	
				inPath = false;
			}
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
