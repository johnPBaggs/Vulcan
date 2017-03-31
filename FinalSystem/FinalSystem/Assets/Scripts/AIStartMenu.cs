/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIStartMenu class
 * This class will be the script for the start menu., It will take the selection from the
 * 		user and give it to the main agent to start running the navigation mesh and then
 * 		the search the user wants to run.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIStartMenu : MonoBehaviour {

	public Canvas startMenu; // the canvas object that is the main menu
	public Canvas dataCanvas; //the canvis object that is were the data is shown
	public Button startButton; // the button that once clicked will allow the agents to start
	public Dropdown searchType; // the drop down menu holding the search algorithm choices
	public Toggle loadNavigationMesh; //toggle for loading the navigation mesh from a file
	public Toggle runSearch1000Times; //toggle for running the search 1000 times
	public Toggle runAllSearchs; //toggle for running all the searchs
	public GameObject navigationMeshAgent; // the game object that is the used as the starting agent


	/*
	 * Start method is called once ther level is loaded and isused to initialize the variables
	 * Parameters:	none
	 * Return:	none
	 */
	void Start () {
		navigationMeshAgent = GameObject.FindGameObjectWithTag ("Agent0");
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	/*
	 * buttonPressed method is called once the start button is pressed in the scene. It will
	 * 		tell the main agent what search to do and to start
	 * Parameters:	none
	 * Return:	none
	 */
	public void buttonPressed()
	{
		Text searchChoice = searchType.captionText;
		string tempSearchChoice = searchChoice.text;
		AINavigationMeshAgent.loadNavigationMesh = loadNavigationMesh.isOn;
		AINavigationMeshAgent.runSearch1000Times = runSearch1000Times.isOn;
		AINavigationMeshAgent.runAllSearchs = runAllSearchs.isOn;
		navigationMeshAgent.SendMessage ("doNavigationMethods", tempSearchChoice);
		dataCanvas.enabled = true;
		AIDataCanvas.alreadySet = 1;
		startMenu.enabled = false;
	}
	
}
/*
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */