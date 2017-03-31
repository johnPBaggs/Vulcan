/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIDynBiDirOpSearch class
 * This class will implement a dynamic weighted BDBOP (Bi-Directiona Beam optimized) A* search. It will implement an A* search
 * 		that puts more emphisus on the start of the search. This is done by having a weight added
 * 		to the total cost of going to a new node. The weight will decrease while the search expands more
 * 		nodes. 
 */
using UnityEngine;
using System.Collections;

public class AIDynBiDirOpSearch  : AISearchInterface
{
	
	
	AIDynBiDirOpAgent agentToGoal; //the search going from the movement agent to the goal
	AIDynBiDirOpAgent goalToAgent; //the search going from the goal to the agent
	AIPolygon[] polygonArray; //This is the polygon array that makes up the navigation mesh
	AIDynBiDirOpNode  finalSolutionStart; //the last node in the solution
	AIPolygon[] finalSolutionArray; //the array that will hold the polygons that make up the final solution in order
	Vector3 agentStartPosition; //the position of the agent that is starting
	public GameObject agent; //the agent GameObject
	public Rigidbody AgentRigidbody; 
	int polygonFinalCount;
	float finalPathCost;
	int maxQueueSize;
	int nodesVisited;
	public static float startingH;
	
	
	
	/*
	 * AIDynBiDirOpSearch's constructor will set up the initial values for the searchs
	 * Parameter:	(Vector3)goalToAdd is the position of the goal in the scene
	 * 				(AIPolygon[])polygonArrayToAdd is the array that holds the polygons from the navigation mesh
	 */
	public AIDynBiDirOpSearch (Vector3 goalToAdd, AIPolygon[] polygonArrayToAdd)
	{	
		polygonArray = polygonArrayToAdd;
		polygonFinalCount = 0;
		maxQueueSize = 0;
		nodesVisited = 0;
		finalPathCost = 0f;
		for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
		if (polygonArray [count].getHasAgent () == true) {
			AIDynBiDirOpSearch.startingH = (goalToAdd - polygonArray[count].getCenterVector()).magnitude; 
		}
		agentToGoal = new AIDynBiDirOpAgent (goalToAdd, polygonArray, false);
		goalToAgent = new AIDynBiDirOpAgent (AINavigationMeshAgent.agentStart, polygonArray, true);
	}
	
	
	
	/*
	 * startSearch method will start running the search giving both agents time to run depending on how big the
	 * 		polygonArray size is and then depending on what case for finding a path was returned call the appropriate
	 * 		methods for building the path
	 * Parameters:	none
	 * Return:	none
	 */
	public void startSearch()
	{
		bool pathFound = false;
		int returnValueFromSearch;
		while (pathFound == false) { //having found a path yet
			if(polygonArray.Length >= 8) //checks to see if the polygonArray is long enough for splitting the search into two
			{
				returnValueFromSearch = agentToGoal.doSearch(polygonArray.Length/8, goalToAgent.getCloseList()); //tells the first search to search for a small amount of time
				if(returnValueFromSearch == 1) //the goal was found
				{
					pathFound = true;
					getPathFromBiDirectionAgent(agentToGoal);
				}
				else if(returnValueFromSearch == 2) // a connection between the two searchs was found
				{
					pathFound = true;
					getPathFromBothBiDirectionAgents(false);
				}
				else
				{
					returnValueFromSearch = goalToAgent.doSearch(polygonArray.Length/8, agentToGoal.getCloseList()); //tells the second search to search for a small amount of time
					if(returnValueFromSearch == 1)//agent was found
					{
						pathFound = true;
						getPathFromBiDirectionAgent(goalToAgent);
					}
					else if(returnValueFromSearch == 2) //a connection between the two searchs was found
					{
						pathFound = true;
						getPathFromBothBiDirectionAgents(true);
					}
				}
			}
			else
			{
				returnValueFromSearch = agentToGoal.doSearch(polygonArray.Length * 2, goalToAgent.getCloseList());
				if(returnValueFromSearch == 1)
				{
					pathFound = true;
					getPathFromBiDirectionAgent(agentToGoal);
				}
			}
			
		}
		
	}
	
	
	
	
	
	/*
	 * getPathFromBiDirectionAgent method will get the final path solution array from one of the searchs that was passed in
	 * Parameter:	(AIDynBiDirOpAgent) agentThatFoundPath is the agent that has found the path to its goal
	 * Return:	none
	 */
	void getPathFromBiDirectionAgent(AIDynBiDirOpAgent agentThatFoundPath)
	{
		finalSolutionArray = agentThatFoundPath.getFinalPath ();
		polygonFinalCount = agentThatFoundPath.getFinalPathLength ();
		for (int count = 1; count < finalSolutionArray.Length; count++)
			finalPathCost += (finalSolutionArray [count].getCenterVector () - finalSolutionArray [count - 1].getCenterVector ()).magnitude;
		nodesVisited = agentThatFoundPath.getNodesVisited ();
		maxQueueSize = agentThatFoundPath.getMaxQueueSize ();
	}
	
	
	
	/*
	 * getPathFromBothBiDirectionAgents method will get the final path from the two searchs depending on what agent
	 * 		found the connecting path
	 * Parameter:	(bool)isStartBackwards tells which search found the path
	 * Return:	none
	 */
	void getPathFromBothBiDirectionAgents(bool isStartBackwards)
	{
		if (isStartBackwards == false) //starting from search from the agent to the goal
			getPathStartingWithForward ();
		else //starting from the search fromt the goal to the agent
			getPathStartingWithbackwards ();
		
		polygonFinalCount = agentToGoal.getFinalPathLength () + goalToAgent.getFinalPathLength () - 1;
		for (int count = 1; count < finalSolutionArray.Length; count++)
			finalPathCost += (finalSolutionArray [count].getCenterVector () - finalSolutionArray [count - 1].getCenterVector ()).magnitude;
		nodesVisited = agentToGoal.getNodesVisited () + goalToAgent.getNodesVisited ();
		maxQueueSize = agentToGoal.getMaxQueueSize () + goalToAgent.getMaxQueueSize ();
	}
	
	
	
	/*
	 * getPathStartingWithbackwards method gets the final path from the agent to the connecting node found by the 
	 * 		search from the goal to the agent then gets the final path for the goal
	 * Parameter:	none
	 * Return:	none
	 */
	void getPathStartingWithbackwards ()
	{
		AIPolygon[] frontPath = agentToGoal.getFinalPathStartingWithNode  (goalToAgent.getFinalPathStart ());
		if (frontPath == null)
			return;
		AIPolygon[] backPath = goalToAgent.getFinalPath ();
		finalSolutionArray = new AIPolygon[frontPath.Length + backPath.Length - 1];
		int finalSolutionCounter = 0;
		for (int count = 0; count < frontPath.Length; count++, finalSolutionCounter++)
			finalSolutionArray [finalSolutionCounter] = frontPath [count];
		for (int count = 1; count < backPath.Length; count++, finalSolutionCounter++)
			finalSolutionArray [finalSolutionCounter] = backPath [count];
	}
	
	
	
	/*
	 * getPathStartingWithForward method gets the final path from the agent to the connecting node found by the 
	 * 		search from the agent to the goal then gets the final path for the goal
	 * Parameter:	none
	 * Return:	none
	 */
	void getPathStartingWithForward ()
	{
		AIPolygon[] frontPath = agentToGoal.getFinalPath ();
		if (frontPath == null)
			return;
		AIPolygon[] backPath = goalToAgent.getFinalPathStartingWithNode (agentToGoal.getFinalPathStart ());
		finalSolutionArray = new AIPolygon[frontPath.Length + backPath.Length - 1];
		int finalSolutionCounter = 0;
		for (int count = 0; count < frontPath.Length; count++, finalSolutionCounter++)
			if(frontPath[count] != null)
				finalSolutionArray [finalSolutionCounter] = frontPath [count];
		for (int count = 1; count < backPath.Length; count++, finalSolutionCounter++)
			if(backPath[count] != null)
				finalSolutionArray [finalSolutionCounter] = backPath [count];
	}
	
	
	
	/*
	 * getFinalPathLength method will return the number of nodes that are in the final path array
	 * Parameter:	none
	 * Return:	(int)
	 * 				sthe number of nodes that are in the final path array
	 */
	public int getFinalPathLength ()
	{
		return polygonFinalCount;
	}
	

	/*
	 * getFinalPathost will return the cost of the final path in units
	 * Parameters:	none
	 * Return:	(float)
	 * 				the cost of the final path
	 */
	public float getFinalPathCost()
	{
		return finalPathCost;
	}
	
	
	/*
	 * getNodesVisited method will return the number of nodes that have been visited by the search
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes visited by the search
	 */
	public int getNodesVisited ()
	{
		return nodesVisited;
	}
	
	
	
	/*
	 * getMaxQueueSize method will return the max size the queue was during the search
	 * Parameter:	none
	 * Return:	(int)
	 * 				the max size the queue was during the search
	 */
	public int getMaxQueueSize ()
	{
		return maxQueueSize;
	}
	
	
	
	/*
	 * getFinalSolution method will return the final path solutiona polygons
	 * Parameter:	none
	 * Return:	(AIPolygon[])
	 * 				the final solution path polygons
	 */
	public AIPolygon[] getFinalSolution()
	{
		return finalSolutionArray;
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