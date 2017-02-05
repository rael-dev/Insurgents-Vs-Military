using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: 
 * Caveats: None
 */ 
public class MovementForces : MonoBehaviour
{
	/* Attributes */
	public Vector3 position; // position of the object
	public Vector3 direction; // direction the object is facing
	public Vector3 velocity; // velocity of the object
	public Vector3 acceleration; // sum of forces acting on the object

	protected float mass; // mass of the object
	protected float maxSpeed; // maximum speed of vehicle
	protected float maxForce; // maximum steering force applied to the vehicle

	protected BehaviourManager behaviourMngr; // behaviour manager to calculate forces
	public Vector3 safeZone;
	public GameObject target = null; // closest object to interact with
	public List<GameObject> targetList = null; // list of objects to interact with
	public List<GameObject> obstacleList = null; // list of objects to avoid

	protected bool debugging; // flag used to toggle debugging lines on and off
	public GameObject mainObstacle = null;

	/// <summary>
	/// Toggles the debug lines.
	/// </summary>
	/// <param name="toggle">If set to <c>true</c> toggle.</param>
	public void SetDebug (bool toggle)
	{
		debugging = toggle;
	}


	/// <summary>
	/// Sets the target list.
	/// </summary>
	/// <param name="targets">Targets.</param>
	public void SetTargetList(List<GameObject> targets)
	{
		targetList = targets;
	}


	/// <summary>
	/// Sets the obstacle list.
	/// </summary>
	/// <param name="obstacles">Obstacles.</param>
	public void SetObstacleList (List<GameObject> obstacles)
	{
		obstacleList = obstacles;
	}


	/// <summary>
	/// Sets the target.
	/// </summary>
	public virtual void SetTarget()
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

			target = targetList [closestIndex];
		}
		else 
		{
			target = null;
		}
	}


	/// <summary>
	/// Sets the transform.
	/// </summary>
	protected void SetTransform()
	{
		if (gameObject.tag != "Helicopter") 
		{
			position.y = 0.0f;
		}

		transform.position = position;

		if (gameObject.tag == "Vehicle" || gameObject.tag == "Helicopter") 
		{
			transform.forward = -direction;
		} 
		else 
		{
			transform.forward = direction;
		}
	}


	/// <summary>
	/// Applies the force.
	/// </summary>
	/// <param name="force">Force.</param>
	protected void ApplyForce(Vector3 force)
	{
		acceleration += force / mass;
		acceleration.Normalize();
		acceleration *= maxForce;
	}


	/// <summary>
	/// Seek the specified targetPosition.
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	protected Vector3 Seek(Vector3 targetPosition)
	{
		//Step 1: Calculate the desired unclamped velocity
		//which is from this vehicle to target's position
		Vector3 desiredVelocity = targetPosition - position;

		//Step 2: Calculate maximum speed so the vehicle does not move faster than it should
		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed;

		//Step 3: Calculate steering force
		Vector3 steeringForce = desiredVelocity - velocity;

		//Step 4: return the force so it can be applied to this vehicle
		return steeringForce;
	}


	/// <summary>
	/// Pursue the specified targetPosition.
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	protected Vector3 Pursue(Vector3 targetPosition)
	{
		// Calculate future position of the zombie that is pursuing
		Vector3 targetFuturePos = targetPosition + target.GetComponent<Terrorist> ().velocity;

		// Seek the future position of the zombie
		Vector3 steeringForce = Seek(targetFuturePos);

		return steeringForce;
	}


	/// <summary>
	/// Flee the specified targetPosition.
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	protected Vector3 Flee(Vector3 targetPosition)
	{
		//Step 1: Calculate the desired unclamped velocity
		//which is from this vehicle to target's position
		Vector3 desiredVelocity = position - targetPosition;

		//Step 2: Calculate maximum speed so the vehicle does not move faster than it should
		desiredVelocity.Normalize ();
		desiredVelocity *= maxSpeed;

		//Step 3: Calculate steering force
		Vector3 steeringForce = desiredVelocity - velocity;


		//Step 4: return the force so it can be applied to this vehicle
		return steeringForce;
	}


	/// <summary>
	/// Evade the specified targetPosition.
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	protected Vector3 Evade(Vector3 targetPosition)
	{
		// Calculate future position of the soldier that is pursuing
		Vector3 targetFuturePos = targetPosition + target.GetComponent<MovementForces>().velocity;
		targetFuturePos /= 2.0f;

		// Flee the future position of the zombie
		Vector3 steeringForce = Flee(targetFuturePos);

		return steeringForce;
	}


	/// <summary>
	/// Makes the gameObject Wander in a random direction.
	/// </summary>
	protected Vector3 Wander()
	{
		// Set up the circle in front of the game object
		Vector3 circleCenter = position + velocity;
		//circleCenter.Normalize ();
		//circleCenter *= 4.0f;

		float radius = 5.0f;
		float angle = Random.Range (0, (2 * Mathf.PI));

		// Calculate and seek a random point on the circumfrence of the circle
		float targetX = circleCenter.x + (radius * Mathf.Cos (angle));
		float targetZ = circleCenter.z + (radius * Mathf.Sin (angle));
		Vector3 targetPosition = new Vector3 (targetX, position.y, targetZ);

		return Seek (targetPosition);
	}


	/// <summary>
	/// Avoids the obstacle.
	/// </summary>
	/// <param name="obstacles">Obstacles.</param>
	public void AvoidObstacle ()
	{
		if (obstacleList.Count > 0) 
		{

			int mainIndex = 0;
			float d = Vector3.Distance (position, obstacleList [mainIndex].transform.position);
			Vector3 desiredVelocity = Vector3.zero;
	
			for (int i = 0; i < obstacleList.Count; i++) 
			{
				if (Vector3.Distance (position, obstacleList [i].transform.position) < d) 
				{	
					d = Vector3.Distance (position, obstacleList [i].transform.position);
					mainIndex = i;
				}
			}

			mainObstacle = obstacleList [mainIndex];
			Vector3 posToCenter = gameObject.transform.position - mainObstacle.transform.position;

			if (d <= (gameObject.GetComponent<BoundingSphere> ().radius + obstacleList[mainIndex].GetComponent<BoundingSphere> ().radius)) 
			{
				float dotProductF = Vector3.Dot (gameObject.transform.forward, posToCenter);
				float dotProductR = Vector3.Dot (gameObject.transform.right, posToCenter);

				if (dotProductF < 0) 
				{
					return;
				}

				if (gameObject.GetComponent<BoundingSphere> ().radius + mainObstacle.GetComponent<BoundingSphere> ().radius < Mathf.Abs (dotProductR)) 
				{
					return;
				}

				// if the obstacle is on the right, go left
				if (dotProductR > 0) 
				{
					desiredVelocity = gameObject.transform.right;
				}

				// if the obstacle is on the left, go right
				if (dotProductR < 0) 
				{
					desiredVelocity = -gameObject.transform.right;
				}

				desiredVelocity.Normalize ();
				desiredVelocity *= maxSpeed;

				Vector3 steeringForce = desiredVelocity - velocity;
				steeringForce = steeringForce;
				ApplyForce (steeringForce * 0.50f);
			}
		} 
		else 
		{
			return;
		}
	}


	/// <summary>
	/// Separate the specified vehicles.
	/// </summary>
	/// <param name="vehicles">Vehicles.</param>
	/// <param name="separation">Desired separation.</param>
	public void Separate (List<GameObject> vehicles, float separation)
	{
		float desiredSeparation = separation;
		Vector3 sum = new Vector3();
		int count = 0;

		foreach (GameObject vehicle in vehicles) 
		{
			if (gameObject == vehicle) 
			{
				continue;
			}

			float d = Vector3.Distance (position, vehicle.transform.position);

			if ((d > 0) && (d < desiredSeparation)) 
			{
				Vector3 difference = position - vehicle.transform.position;
				difference.y = 0;
				difference.Normalize ();

				difference /= d;
				sum += difference;
				count++;
			}
		}

		if (count > 0) 
		{
			sum /= count;
			sum.Normalize ();
			sum *= maxSpeed;

			Vector3 steeringForce = sum - velocity;
			//steeringForce = Vector3.ClampMagnitude (steeringForce, maxForce);
			ApplyForce (steeringForce * 2.0f);
		}
	}


	/// <summary>
	/// Returns toward the center when approaching the edge of the map.
	/// </summary>
	protected void ReturnCenter()
	{
		//Check within X
		if(position.x > safeZone.x - 3.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, safeZone.y, safeZone.z / 2.0f)) * 10.0f);

		}
		else if(position.x < 3.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, safeZone.y, safeZone.z / 2.0f)) * 10.0f);
		}

		//check within Z
		if(position.z > safeZone.z - 3.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, safeZone.y, safeZone.z / 2.0f)) * 10.0f);

		}
		else if(position.z < 3.0f)
		{
			ApplyForce(Seek (new Vector3 (safeZone.x / 2.0f, safeZone.y, safeZone.z / 2.0f)) * 10.0f);
		}
	}
}
