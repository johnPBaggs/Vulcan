/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIPSearch class
 * This class will call a specific search type to start running. then it will take the final
 * 		path and create the waypoints from it. Finally it will send the way points to the movement agent.
 */
using UnityEngine;
using System.Collections;

public class AISearch {


	AISearchInterface searchAgent; // the search reference
	GameObject agent; //the agent game object
	Vector3 goalPosition; //the goals position
	Vector3 agentStartPosition; // the agents start position
	float startTime; //the start of the search time
	string searchType; // the search type the user wants to see
	Vector3[] wayPoints; // the waypoints from the final path
	public static bool canWalk;


	/*
	 * AISearch constructor will set up the search and the initial values for the instantce variables
	 * Parameters:	(string)searchTpeToAdd is a string containing the search type the user wants to run
	 * 				(Vector3)goalPositionToAdd is the position of the goal
	 * 				(AIPolygons[])polygonsToAdd is the polygons that make up the navigation mesh
	 */
	public AISearch(string searchTypeToAdd, Vector3 goalPositionToAdd, AIPolygon[] polygonsToAdd)
	{
		agent = GameObject.FindGameObjectWithTag ("Agent");
		goalPosition = goalPositionToAdd;
		agentStartPosition = agent.transform.position;
		searchType = searchTypeToAdd;
		searchAgent = getSearchAgent (searchType, goalPositionToAdd, polygonsToAdd);
		doSearchMethods ();
	}



	/*
	 * getSearchAgentr will make a new reference for the specific search type the user inputed
	 * Parameters:	(string)searchTpeToAdd is a string containing the search type the user wants to run
	 * 				(Vector3)goalPositionToAdd is the position of the goal
	 * 				(AIPolygons[])polygonsToAdd is the polygons that make up the navigation mesh
	 * Return:		(AISearchInterfeace) a reference to a new search object that is secific to the type
	 * 					the user wants
	 */
	AISearchInterface getSearchAgent(string searchType, Vector3 goalPositionToAdd, AIPolygon[] polygonsToAdd)
	{
		if(searchType.Equals("A*") == true)
			return new AIAgentAStarSearch(goalPositionToAdd, polygonsToAdd);
		if(searchType.Equals("Bi-Directional") == true)
			return new AIBiDirectionalSearch(goalPositionToAdd, polygonsToAdd);
		if (searchType.Equals ("FringeSearch") == true)
			return new AIFringeSearch (goalPositionToAdd, polygonsToAdd);
		if (searchType.Equals ("A* Optimal") == true)
			return new AIAgentAStarSearchOp (goalPosition, polygonsToAdd);
		if (searchType.Equals ("Bi-Directional Optimal") == true)
			return new AIBiDirectionalSearchOp (goalPosition, polygonsToAdd);
		if (searchType.Equals ("Beam Search") == true)
			return new AIBeamSearch (goalPosition, polygonsToAdd);
		if (searchType.Equals ("Dynamic Weighted A*") == true)
			return new AIDynamicWeightedSearch (goalPosition, polygonsToAdd);
		if (searchType.Equals ("Dynamic Weighted BDBOP") == true)
			return new AIDynBiDirBeamOpSearch (goalPosition, polygonsToAdd);
		if (searchType.Equals ("Dynamic Weighted BDOP") == true)
			return new AIDynBiDirOpSearch (goalPosition, polygonsToAdd);
		return new AIAgentAStarSearch(goalPositionToAdd, polygonsToAdd);
	}



	/*
	 * doSearchMethods will call the search to find a path then gather the data and make the waypoints
	 * Parameters:	none
	 * Return:		none
	 */
	void doSearchMethods()
	{
		startTime = Time.realtimeSinceStartup;
		searchAgent.startSearch ();
		sendData ();
		if (AISearch.canWalk == true) {
			AIPolygon[] finalSolution = searchAgent.getFinalSolution ();
			makeWayPoints (finalSolution);
		}

	}



	/*
	 * sendData will send the data to the search agent data class to print to the screen
	 * Parameters:	none
	 * Return:		none
	 */
	void sendData()
	{
		AISearchAgentData.timeTaken = Time.realtimeSinceStartup - startTime;
		AISearchAgentData.searchType = searchType;
		AISearchAgentData.finalPathLength = searchAgent.getFinalPathLength ();
		AISearchAgentData.finalPathCost = searchAgent.getFinalPathCost ();
		AISearchAgentData.nodesVisited = searchAgent.getNodesVisited ();
		AISearchAgentData.maxQueueSize = searchAgent.getMaxQueueSize ();
	}


	/*
	 * makeWayPoints method will take the Finalsolution array and use the midpoints of edges and the center point 
	 * 		of polygons to make the wayPoints that will be given to the movementAgent
	 * Parameters:	(AIPolygon[])finalSolutionArray holds the polygons that make up the path to the goal
	 * Return:	none
	 */
	void makeWayPoints (AIPolygon[] finalSolutionArray)
	{
		if (finalSolutionArray == null)
			return;
		if (finalSolutionArray.Length == 0) {
			return;
		}
		if (((finalSolutionArray.Length * 2) - 1) == 1) {
			wayPoints = new Vector3[2];
			wayPoints [0] = new Vector3 (agentStartPosition.x, agentStartPosition.y, agentStartPosition.z); //add the agent's current position to the front of the array
			wayPoints [1] = new Vector3 (goalPosition.x, goalPosition.y, goalPosition.z); //add the goals current position to the last position in the array
		}
		else {
			wayPoints = new Vector3[(finalSolutionArray.Length * 2)-1];// an array that will hold the wayPoints
			Vector3 tempVector;
			int totalCount = finalSolutionArray.Length - 1;
			int wayPointCount = 0;
			int arrayCount = 0;
			wayPoints [wayPointCount] = new Vector3 (agentStartPosition.x, agentStartPosition.y, agentStartPosition.z); //add the agent's current position to the front of the array
			wayPointCount++;
			for (; arrayCount < finalSolutionArray.Length - 1;) {//looks at each polygon and adds the midpoint of the edge to the next node and the center of the next Node's polygon
				if ((totalCount - arrayCount) > 1) { //if the next Node is not the node containing the goal
					tempVector = finalSolutionArray [arrayCount].getMidpointOfEdge (finalSolutionArray [arrayCount + 1]); //get the midPoint of the edge
					wayPoints [wayPointCount] = new Vector3 (tempVector.x, tempVector.y, tempVector.z);
					wayPointCount++;
					tempVector = finalSolutionArray [arrayCount + 1].getCenterVector (); //get the center of the next Polygon
					wayPoints [wayPointCount] = new Vector3 (tempVector.x, tempVector.y, tempVector.z);
					wayPointCount++;
					arrayCount++;
				} else { //the nextnode is the node containing the goal
					tempVector = finalSolutionArray [arrayCount].getMidpointOfEdge (finalSolutionArray [arrayCount + 1]); //get the midPoint of the edge
					wayPoints [wayPointCount] = new Vector3 (tempVector.x, tempVector.y, tempVector.z);
					wayPointCount++;
					arrayCount = finalSolutionArray.Length - 1;
				}
			}
			wayPoints [wayPointCount] = new Vector3 (goalPosition.x, goalPosition.y, goalPosition.z); //add the goals current position to the last position in the array
		}
		agent.SendMessage ("startMovement", wayPoints);
		AgentMovement.canWalk = true;
	}

	//used for debugging
	public Vector3[] getWayPoints()
	{
		return wayPoints;
	}

}
