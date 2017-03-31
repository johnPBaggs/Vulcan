/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AISearchAgentData class
 * This class will set the data recieved from the search agent to the screen for the user to see
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AISearchAgentData : MonoBehaviour {

	public static float timeTaken; // the time the search took
	public static string searchType; //the type of search done
	public static int nodesVisited; //the number of nodes visited in the search
	public static int maxQueueSize; //the max number of nodes that were in the queue at once
	public static int finalPathLength; //the final length of the path
	public static float finalPathCost;
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
		tempString += "Search Type: " + AISearchAgentData.searchType + "\n";
		tempString += string.Format("Time: {0:F6} sec\n", AISearchAgentData.timeTaken);
		tempString += "Nodes Visited: " + AISearchAgentData.nodesVisited + "\n";
		tempString += "Max Queue Size: " + AISearchAgentData.maxQueueSize + "\n";
		tempString += "Path Cost: " + AISearchAgentData.finalPathCost + "\n";
		tempString += "Path Length: " + AISearchAgentData.finalPathLength;
		
		dataToWrite.text = tempString;
	}
}
