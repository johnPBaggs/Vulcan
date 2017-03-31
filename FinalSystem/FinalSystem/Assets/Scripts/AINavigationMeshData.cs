/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AINavigationMeshData class
 * This class will be used add the navigation mesh data to the data canvas. 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AINavigationMeshData : MonoBehaviour {

	public static float timeTaken; //the time the navigation mesh took
	public static int initialPolygonCount; //the initial number of polygons for the level
	public static int finalPolygonCount; //the final count of the navigation mesh
	public static int obstacleCount; //the number of obstacles that is in the scene
	Text dataToWrite;


	// Use this for initialization
	void Start () {
		dataToWrite = GetComponent<Text> ();

	}
	
	/*
	 * Updata will write the data to the canvas every frame
	 * parameter:	none
	 * Return:	none
	 */
	void Update () {

		string tempString = "";
		tempString += string.Format("Time: {0:F6} sec\n", AINavigationMeshData.timeTaken);
		tempString += "Initial # Polygons: " + AINavigationMeshData.initialPolygonCount + "\n";
		tempString += "Obstacle Count: " + AINavigationMeshData.obstacleCount + "\n";
		tempString += "Final # Polygons: " + AINavigationMeshData.finalPolygonCount + "\n";
	
		dataToWrite.text = tempString;
	}
}
