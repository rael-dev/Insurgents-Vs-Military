using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/*
 * Author: Israel Anthony
 * Purpose: Spawns all objects and checks and resolves collisions. Also contains the GUI content.
 * Caveats: None
 */ 
public class BehaviourManager : MonoBehaviour
{
	private int generalCount = 1; // only one general in the army
	private int obstacleCount;
	public int soldierCount;
	public int terroristCount;
	public int tankCount;
	public int helicopterCount;

	public List<GameObject> generalList;
	public List<GameObject> soldierList;
	public List<GameObject> terroristList;
	public List<GameObject> tankList;
	public List<GameObject> helicopterList;
	public List<GameObject> tankWaypointList;
	public List<GameObject> helicopterWaypointList;

	public List<GameObject> tankPrototype;
	public List<GameObject> generalPrototype;
	public List<GameObject> soldierPrototype; 
	public List<GameObject> terroristPrototype;
	public GameObject helicopterPrototype;

	private BoundingSphere generalBS;
	private BoundingSphere soldierBS;
	private BoundingSphere terroristBS;
	private BoundingSphere tankBS;
	private BoundingSphere helicopterBS;

	public List<GameObject> obstacleList;
	private BoundingSphere obstacleBS;

	public GameObject player;

	// Materials
	public Material matRed;
	public Material matGreen;
	public Material matBlue;
	public Material matBlack;
	public Material matPurple;

	private bool debugging = false; // flag for debugging lines

	// Auto-spawn variables
	private float oldTimeT; 
	private float oldTimeS;
	public bool autoSpawnTerrorist = false;
	public bool autoSpawnSoldier = false;


	public GameObject generalPosCube;
	public GameObject helicopterFlockCube;
	public GameObject tankPathCube;

	public AudioClip deathScream;
	public AudioClip deathCrunch;

	// Use this for initialization
	void Start ()
	{
		// Error check for prototypes
		if(null == soldierPrototype)
		{
			Debug.Log("Error in " + gameObject.name + 
			          ": SoldierPrototype is not assigned.");
			Debug.Break();
		}

		if(null == terroristPrototype)
		{
			Debug.Log("Error in " + gameObject.name + 
			          ": TerroristPrototype is not assigned.");
			Debug.Break();
		}

		if(null == generalPrototype)
		{
			Debug.Log("Error in " + gameObject.name + 
				": GeneralPrototype is not assigned.");
			Debug.Break();
		}

		GameObject[] obstacles = GameObject.FindGameObjectsWithTag ("Environment");
		obstacleList = new List<GameObject> (obstacles.Length);
		obstacleList = obstacles.ToList ();
		obstacleCount = obstacleList.Count;

		soldierList = new List<GameObject> ();
		terroristList = new List<GameObject> ();
		generalList = new List<GameObject> ();

		// Instantiate generals
		for(int i = 0; i < generalCount; i++)
		{
			int index = Random.Range (0, generalPrototype.Count);
			GameObject general = Instantiate (generalPrototype [index]);
			generalList.Add (general);

			// Error check for components
			generalBS = generalList[i].GetComponent<BoundingSphere>();
			if(null == generalBS)
			{
				Debug.Log("Error in " + gameObject.name + 
					": GeneralPrototype is required to have a BoundingSphere script");
				Debug.Break();
			}

			MilitaryGeneral generalMF = generalList[i].GetComponent<MilitaryGeneral>();
			if(null == generalMF)
			{
				Debug.Log("Error in " + gameObject.name + 
					": GeneralPrototype is required to have a MovementForces script");
				Debug.Break();
			}

			generalMF.SetObstacleList (obstacleList);

			SpawnSoldier(generalList[i]);
		}

		// Instantiate soldiers
		for(int i = 0; i < soldierCount; i++)
		{
			int index = Random.Range (0, soldierPrototype.Count);
			GameObject soldier = Instantiate (soldierPrototype [index]);
			soldierList.Add (soldier);

			// Error check for components
			soldierBS = soldierList[i].GetComponent<BoundingSphere>();
			if(null == soldierBS)
			{
				Debug.Log("Error in " + gameObject.name + 
					": SoldierPrototype is required to have a BoundingSphere script");
				Debug.Break();
			}
				
			Soldier soldierMF = soldierList[i].GetComponent<Soldier>();
			if(null == soldierMF)
			{
				Debug.Log("Error in " + gameObject.name + 
					": SoldierPrototype is required to have a Soldier script");
				Debug.Break();
			}

			soldierMF.SetObstacleList (obstacleList);

			SpawnSoldier(soldierList[i]);
		}
	 	
		// Instantiate terrorists
		for(int i = 0; i < terroristCount; i++)
		{
			int index = Random.Range (0, terroristPrototype.Count);
			GameObject terrorist = Instantiate (terroristPrototype [index]);
			terroristList.Add (terrorist);

			// Error check for components
			terroristBS = terroristList[i].GetComponent<BoundingSphere>();
			if(null == terroristBS)
			{
				Debug.Log("Error in " + gameObject.name + 
					": TerroristPrototype is required to have a BoundingSphere script");
				Debug.Break();
			}

			Terrorist terroristMF = terroristList[i].GetComponent<Terrorist>();
			if(null == terroristMF)
			{
				Debug.Log("Error in " + gameObject.name + 
					": TerroristPrototype is required to have a Terrorist script");
				Debug.Break();
			}

			terroristMF.SetObstacleList (obstacleList);

			SpawnTerrorist(terroristList[i]);
		}

		// Instantiate tanks
		for(int i = 0; i < tankCount; i++)
		{
			int index = Random.Range (0, tankPrototype.Count);
			GameObject tank = Instantiate (tankPrototype [index]);
			tankList.Add (tank);

			// Error check for components
			tankBS = tankList[i].GetComponent<BoundingSphere>();
			if(null == tankBS)
			{
				Debug.Log("Error in " + gameObject.name + 
					": TankPrototype is required to have a BoundingSphere script");
				Debug.Break();
			}

			Tank tankMF = tankList[i].GetComponent<Tank>();
			if(null == tankMF)
			{
				Debug.Log("Error in " + gameObject.name + 
					": TankPrototype is required to have a Tank script");
				Debug.Break();
			}

			tankMF.SetTargetList(tankWaypointList);
			tankMF.SetObstacleList(obstacleList);

			SpawnTank(tankList[i]);
		}

		// Instantiate helicopters
		for(int i = 0; i < helicopterCount; i++)
		{
			helicopterList.Add (Instantiate(helicopterPrototype));

			// Error check for components
			helicopterBS = helicopterList[i].GetComponent<BoundingSphere>();
			if(null == helicopterBS)
			{
				Debug.Log("Error in " + gameObject.name + 
					": HelicopterPrototype is required to have a BoundingSphere script");
				Debug.Break();
			}

			Helicopter helicopterMF = helicopterList[i].GetComponent<Helicopter>();
			if(null == helicopterMF)
			{
				Debug.Log("Error in " + gameObject.name + 
					": HelicopterPrototype is required to have a Helicopter script");
				Debug.Break();
			}

			helicopterMF.SetTargetList(helicopterWaypointList);
			SpawnHeli(helicopterList[i]);
		}
	}


	// Update is called once per frame
	void Update ()
	{
		CollisionCheck ();
		debugging = false;

		// Update necessary information for each of the terrorists and soldiers
		for (int i = 0; i < terroristList.Count; i++) 
		{
			Terrorist terroristMF = terroristList[i].GetComponent<Terrorist>();

			terroristMF.SetTargetList(soldierList);
			terroristMF.SetTarget();
			terroristMF.SetDebug (debugging);
			terroristMF.Separate (terroristList, 4.0f);
		}

		for (int i = 0; i < generalList.Count; i++) 
		{
			MilitaryGeneral generalMF = generalList[i].GetComponent<MilitaryGeneral>();

			generalMF.SetTargetList(terroristList);
			generalMF.SetTarget();
			generalMF.SetDebug (debugging);
			generalMF.Separate (tankList, 8.0f);
			generalMF.Separate (soldierList, 4.0f);
			generalMF.Separate (obstacleList, 10.0f);
		}

		for (int i = 0; i < soldierList.Count; i++) 
		{
			Soldier soldierMF = soldierList[i].GetComponent<Soldier>();

			soldierMF.SetTargetList(terroristList);
			soldierMF.SetTarget();
			soldierMF.SetLeader (generalList [0]);
			soldierMF.SetDebug (debugging);
			soldierMF.Separate (soldierList, 4.0f);
			soldierMF.Separate (tankList, 8.0f);
		}

		for (int i = 0; i < tankList.Count; i++) 
		{
			Tank tankMF = tankList[i].GetComponent<Tank>();

			tankMF.SetDebug (debugging);
			tankMF.Separate (tankList, 8.0f);
			tankMF.Separate (soldierList, 8.0f);
			tankMF.Separate (generalList, 8.0f);
		}

		for (int i = 0; i < helicopterList.Count; i++) 
		{
			Helicopter helicopterMF = helicopterList[i].GetComponent<Helicopter>();

			helicopterMF.SetDebug (debugging);
			helicopterMF.SetFlockerList (helicopterList);
			helicopterMF.Separate (helicopterList, 10.0f);
		}

		// Update leaders for soldiers
		for (int i = 0; i < generalList.Count; i++) 
		{
			for (int j = 0; j < soldierList.Count; j++) 
			{
				Soldier soldierMF = soldierList[j].GetComponent<Soldier>();

				soldierMF.SetLeader (generalList [i]);
			}
		}

		/*
		// Draw lines between waypoints
		for (int i = 0; i < tankWaypointList.Count - 1; i++) 
		{
			Debug.DrawLine (tankWaypointList [i].transform.position, tankWaypointList [i + 1].transform.position, Color.black);
		}
		Debug.DrawLine (tankWaypointList [0].transform.position, tankWaypointList [tankWaypointList.Count - 1].transform.position, Color.black);
		*/

		// Handle auto-spawning
		if (autoSpawnTerrorist) 
		{
			if (Time.time >= oldTimeT + 3.0f) 
			{
				for (int i = 0; i < 2; i++) 
				{
					int index = Random.Range (0, terroristPrototype.Count);
					GameObject terrorist = Instantiate (terroristPrototype [index]);
					terroristList.Add (terrorist);
					terroristCount++;
					SpawnTerrorist (terrorist);
					oldTimeT = Time.time;
				}
			}
		}

		if (autoSpawnSoldier) 
		{
			if (Time.time >= oldTimeS + 5.0f) 
			{
				int index = Random.Range (0, soldierPrototype.Count);
				GameObject soldier = Instantiate (soldierPrototype [index]);
				soldierList.Add (soldier);
				soldierCount++;
				SpawnSoldier (soldier);
				oldTimeS = Time.time;
			}
		}

		// Place a cube at general's transform for the camera view
		generalPosCube.transform.position = generalList [0].transform.position;
		generalPosCube.transform.forward = generalList [0].transform.forward;

		// Place a cube at the center of the flock
		Vector3 pSum = Vector3.zero;

		for (int i = 0; i < helicopterList.Count; i++) 
		{
			pSum += helicopterList [i].transform.position;
		}

		Vector3 flockCenter = pSum / helicopterList.Count;
		helicopterFlockCube.transform.position = flockCenter;
		helicopterFlockCube.transform.forward = -helicopterList [0].transform.forward;
		pSum = Vector3.zero;

		// Place a cube at the center of the tanks
		Vector3 tSum = Vector3.zero;

		for (int i = 0; i < tankList.Count; i++) 
		{
			tSum += tankList [i].transform.position;
		}

		Vector3 groupCenter = tSum / tankList.Count;
		tankPathCube.transform.position = groupCenter;
		int closestIndex = 0;
		float d = Vector3.Distance (tankPathCube.transform.position, tankList [0].transform.position);
		for (int i = 0; i < tankList.Count; i++) 
		{
			if (Vector3.Distance (tankPathCube.transform.position, tankList [i].transform.position) < d) 
			{
				closestIndex = i;
				d = Vector3.Distance (tankPathCube.transform.position, tankList [i].transform.position);
			}
		}
		tankPathCube.transform.forward = -tankList [closestIndex].transform.forward;
		tSum = Vector3.zero;
	}


	/// <summary>
	/// Randomizes the position of a GameObject.
	/// </summary>
	/// <param name="theObject">The object.</param>
	void RandomizePosition(GameObject theObject)
	{
		Vector3 position = new Vector3 (Random.Range(0.0f,100.0f), 0.0f, Random.Range(0.0f,100.0f));
		theObject.transform.position = position;
	}


	/// <summary>
	/// Spawns tanks into the military base to begin path following.
	/// </summary>
	/// <param name="tank">The tank.</param>
	void SpawnTank(GameObject tank)
	{
		Vector3 position = new Vector3 (Random.Range(7.0f,45.0f), 0.0f, Random.Range(7.0f,45.0f));
		tank.transform.position = position;
	}


	/// <summary>
	/// Spawns helicopters into the military base to begin path following.
	/// </summary>
	/// <param name="helicopter">The helicopter.</param>
	void SpawnHeli(GameObject helicopter)
	{
		Vector3 position = new Vector3 (Random.Range(0.0f,50.0f), Random.Range(50.0f, 75.0f), Random.Range(0.0f,50.0f));
		helicopter.transform.position = position;
	}


	/// <summary>
	/// Spawns soldiers onto the battlefield.
	/// </summary>
	/// <param name="soldier">The soldier.</param>
	void SpawnSoldier(GameObject soldier)
	{
		Vector3 position = new Vector3 (Random.Range(130.0f,150.0f), 0.0f, Random.Range(155.0f,170.0f));
		soldier.transform.position = position;
	}


	/// <summary>
	/// Spawns terrorists onto the battlefield.
	/// </summary>
	/// <param name="terrorist">The terrorist.</param>
	void SpawnTerrorist(GameObject terrorist)
	{
		Vector3 position = new Vector3 (Random.Range(100.0f,180.0f), 0.0f, Random.Range(410.0f,420.0f));
		terrorist.transform.position = position;
	}


	/// <summary>
	/// Checks for collisions between the military and the terrorists and kills terrorists upon collision.
	/// </summary>
	void CollisionCheck()
	{
		// Check collisions
		for (int i = 0; i < soldierList.Count; i++) 
		{
			if (i < generalList.Count) {
				generalBS = generalList [i].GetComponent<BoundingSphere> ();
			}

			if (i < tankList.Count) {
				tankBS = tankList [i].GetComponent<BoundingSphere> ();
			}

			soldierBS = soldierList[i].GetComponent<BoundingSphere>();

			for (int j = 0; j < terroristList.Count; j++) 
			{
				terroristBS = terroristList [j].GetComponent<BoundingSphere> ();

				if(soldierBS.IsColliding(terroristBS) || tankBS.IsColliding(terroristBS) || generalBS.IsColliding(terroristBS) ||  player.GetComponent<BoundingSphere>().IsColliding(terroristBS))
				{
					gameObject.GetComponent<AudioSource>().PlayOneShot (deathCrunch);
					gameObject.GetComponent<AudioSource>().PlayOneShot (deathScream);
					Destroy(terroristList[j]);
					terroristList.RemoveAt (j);
					terroristCount--;
				}
			}
		}
	}


	/// <summary>
	/// "Nukes" (Destroys) a number of humans and zombies from the scene. Spam to delete all.
	/// </summary>
	void Nuke()
	{
		for (int i = 0; i < terroristList.Count; i++) 
		{
			Destroy(terroristList[i]);
			terroristList.RemoveAt (i);
			terroristCount--;
		}

		for (int i = 0; i < soldierList.Count; i++) 
		{
			Destroy(soldierList[i]);
			soldierList.RemoveAt (i);
			soldierCount--;
		}

		gameObject.GetComponent<AudioSource>().PlayOneShot (deathCrunch);
		gameObject.GetComponent<AudioSource>().PlayOneShot (deathScream);
	}
		

	// Draws the buttons and lables that hold information about the scene onto the screen.
	void OnGUI()
	{
		if (Event.current.Equals(Event.KeyboardEvent("d")))
		{
			debugging = !debugging;
		}

		/*
		// Buttons for interactivity
		if (GUI.Button (new Rect (10.0f, 5.0f, 75.0f, 25.0f), "Nuke")) 
		{
			if (terroristCount > 0) 
			{
				Nuke ();
			}

			if (soldierCount > 0)
			{
				Nuke ();
			}
		}
		if (GUI.Button (new Rect (10.0f, 35.0f, 120.0f, 25.0f), "Spawn Terrorist")) 
		{
			int index = Random.Range (0, terroristPrototype.Count);
			GameObject terrorist = Instantiate (terroristPrototype [index]);
			terroristList.Add (terrorist);
			terroristCount++;
			SpawnTerrorist (terrorist);
		}
		if (GUI.Button (new Rect (10.0f, 65.0f, 120.0f, 25.0f), "Spawn Soldier")) 
		{
			int index = Random.Range (0, soldierPrototype.Count);
			GameObject soldier = Instantiate (soldierPrototype [index]);
			soldierList.Add (soldier);
			soldierCount++;
			SpawnSoldier (soldier);
		}


		if (GUI.Button (new Rect (10.0f, 95.0f, 235.0f, 25.0f), "Toggle Auto Terrorist Spawner " + "(" + autoSpawnTerrorist + ")")) 
		{
			autoSpawnTerrorist = !autoSpawnTerrorist;

			if (autoSpawnTerrorist) 
			{
				oldTimeT = Time.time;
			}
		}
		if (GUI.Button (new Rect (10.0f, 125.0f, 235.0f, 25.0f), "Toggle Auto Soldier Spawner " + "(" + autoSpawnSoldier + ")")) 
		{
			autoSpawnSoldier = !autoSpawnSoldier;

			if (autoSpawnSoldier) 
			{
				oldTimeS = Time.time;
			}
		}
		*/

		// Labels for information about the scene
		GUI.Label (new Rect (10.0f, 5.0f, 150.0f, 25.0f), "Terrorist Count: " + terroristCount);
		GUI.Label (new Rect (10.0f, 35.0f, 150.0f, 25.0f), "Soldier Count: " + soldierCount);
		//GUI.Label (new Rect (10.0f, 65.0f, 150.0f, 25.0f), "Obstacle Count: " + obstacleCount);
		//GUI.Label (new Rect (10.0f, 305.0f, 250.0f, 25.0f), "Press 'D' to toggle debug lines " + "(" + debugging + ")");
		GUI.Label (new Rect (10.0f, 65.0f, 250.0f, 25.0f), "Press 'C' to cycle through cameras");
	}
}
