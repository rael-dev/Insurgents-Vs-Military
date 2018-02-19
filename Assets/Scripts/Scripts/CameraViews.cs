using UnityEngine;
using System.Collections;

/*
 * Author: Israel Anthony (for the OnGUI() method)
 * This class was provided by the professor except for the OnGui() method
 * Purpose: Cycles through the various camera veiws and displays the current view (and how to cycle views) on screen
 * Restrictions: None
 */

public class CameraViews : MonoBehaviour 
{
	// Camera array that holds a reference to every camera in the scene
	public Camera[] cameras;

	// Current camera
	private int currentCameraIndex;

	// Use this for initialization
	void Start () 
	{
		currentCameraIndex = 0;

		// Turn all cameras off, except the first default one
		for (int i = 1; i < cameras.Length; i++) 
		{
			cameras [i].gameObject.SetActive (false);
		}

		// If any cameras were added to the controller, enable the first one
		if (cameras.Length > 0) 
		{
			cameras [0].gameObject.SetActive (true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Press the 'C' key to cycle through cameras in the array
		if (Input.GetKeyDown (KeyCode.C)) 
		{
			// Cycle to the next camera
			currentCameraIndex++; 

			// If cameraIndex is in bounds, set this camera active and last one inactive
			if (currentCameraIndex < cameras.Length) {
				cameras [currentCameraIndex - 1].gameObject.SetActive (false);
				cameras [currentCameraIndex].gameObject.SetActive (true);
			}
			// If last camera, cycle back to first camera
			else 
			{
				cameras [currentCameraIndex - 1].gameObject.SetActive (false);
				currentCameraIndex = 0;
				cameras [currentCameraIndex].gameObject.SetActive (true);
			}
		}
	}

	/// <summary>
	/// Raises the GUI event. Displays the controls for cycling through camera views. Also displays the current view's name.
	/// </summary>
	void OnGUI()
	{
		if (currentCameraIndex == 4) 
		{
			for (int i = 0; i < cameras.Length; i++) 
			{
				if (i == 4) 
				{
					continue;
				}

				cameras [i].enabled = false;
			}
		} 
		else 
		{
			for (int i = 0; i < cameras.Length; i++) 
			{
				cameras [i].enabled = true;
			}
		}
	}
}
