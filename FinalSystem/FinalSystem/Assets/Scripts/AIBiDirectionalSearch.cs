/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIBiDirectionalSearch class
 * This class will be responsible with conducting and completing the AIBiDirectionalAStar search algorithm on the navigation mesh.
 * 		It will be responsible with keeping a openList and a closedList, getting the shortest path, and gathering
 * 		the finalSolution that the movement will use. It will use the finalSolution to set waypoints that is the
 * 		positions the movement Agent will travel to. Two searches are carried out alternating one form the start to the goal and
 *    one from the goal to the start
 */

using UnityEngine;
using System.Collections;

public class AIBiDirectionalSearch : AISearchInterface{


	AIBiDirectionalAStarAgent agentToGoal;
	AIBiDirectionalAStarAgent goalToAgent;
	AIPolygon[] polygonArray; //This is the polygon array that makes up the navigation mesh
	AIAgentAStarSearchNode  finalSolutionStart; //the last node in the solution
	AIPolygon[] finalSolutionArray; //the array that will hold the polygons that make up the final solution in order
	Vector3[] wayPoints; //the positions that will be sent to the movement Agent
	Vector3 agentStartPosition; //the position of the agent that is starting
	public GameObject agent; //the agent GameObject
	public Rigidbody AgentRigidbody;
	int polygonFinalCount;
	float finalPathCost;
	int maxQueueSize;
	int nodesVisited;

  /* AIBiDirectionalSearch is a constructor for this class
	 * Parameter: Vector3 goalToAdd - location of the goal
	 * AIPolygon[] polygonArrayToAdd - array holding the polygons that will be used to search
	 */
	public AIBiDirectionalSearch(Vector3 goalToAdd, AIPolygon[] polygonArrayToAdd)
	{
		polygonArray = polygonArrayToAdd;
		polygonFinalCount = 0;
		maxQueueSize = 0;
		nodesVisited = 0;
		finalPathCost = 0f;
		agentToGoal = new AIBiDirectionalAStarAgent (goalToAdd, polygonArray, false);
		goalToAgent = new AIBiDirectionalAStarAgent (AINavigationMeshAgent.agentStart, polygonArray, true);
	}

  /* The startSearch method starts and coordinates the alternating searches
	 */
	public void startSearch()
	{
		bool pathFound = false;
		int returnValueFromSearch;
		while (pathFound == false) {
			if(polygonArray.Length >= 8)
			{
				returnValueFromSearch = agentToGoal.doSearch(polygonArray.Length/8, goalToAgent.getCloseList());
				if(returnValueFromSearch == 1)
				{
					pathFound = true;
					getPathFromBiDirectionAgent(agentToGoal);
				}
				else if(returnValueFromSearch == 2)
				{
					pathFound = true;
					getPathFromBothBiDirectionAgents(false);
				}
				else
				{
					returnValueFromSearch = goalToAgent.doSearch(polygonArray.Length/8, agentToGoal.getCloseList());
					if(returnValueFromSearch == 1)
					{
						pathFound = true;
						getPathFromBiDirectionAgent(goalToAgent);
					}
					else if(returnValueFromSearch == 2)
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

  /* The getPathFromBiDirectionAgent gets the path from the AIBiDirectionalAStarAgent and sets
	 * the stats needed for analysis( final polygon count, nodes visited, and maz queue size)
	 * Parameter: AIBiDirectionalAStarAgent agentThatFoundPath - Agent that carried out the search
	 */
	void getPathFromBiDirectionAgent(AIBiDirectionalAStarAgent agentThatFoundPath)
	{
		finalSolutionArray = agentThatFoundPath.getFinalPath ();
		polygonFinalCount = agentThatFoundPath.getFinalPathLength ();
		for (int count = 1; count < finalSolutionArray.Length; count++)
			finalPathCost += (finalSolutionArray [count].getCenterVector () - finalSolutionArray [count - 1].getCenterVector()).magnitude;
		nodesVisited = agentToGoal.getNodesVisited() +  goalToAgent.getNodesVisited();
		maxQueueSize = agentToGoal.getMaxQueueSize() +  goalToAgent.getMaxQueueSize();
	}

  /* The method getPathFromBothBiDirectionAgents sets the path depending on whether is starts from the beginning
	 * or the end and sets the stats needed for analysis( final polygon count, nodes visited, and maz queue size)
	 * Parameter: bool isStartBackwards - true if path starts from goal false is path starts from starting point
	 */
	void getPathFromBothBiDirectionAgents(bool isStartBackwards)
	{
		if (isStartBackwards == false)
			getPathStartingWithForward ();
		else
			getPathStartingWithbackwards ();

		polygonFinalCount = agentToGoal.getFinalPathLength () + goalToAgent.getFinalPathLength () - 1;
		for(int count = 1; count < finalSolutionArray.Length; count++)
			finalPathCost += (finalSolutionArray [count].getCenterVector () - finalSolutionArray [count - 1].getCenterVector()).magnitude;
		nodesVisited = agentToGoal.getNodesVisited () + goalToAgent.getNodesVisited ();
		maxQueueSize = agentToGoal.getMaxQueueSize () + goalToAgent.getMaxQueueSize ();
	}

  /* The method getPathStartingWithbackwards constructs the path starting from the goal
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

  /* The getPathStartingWithForward method constructs the path starting from the starting point
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

	/* The method getFinalPathLength  returns the length of the final path in terms of polygons
	 * Return: int
	 *						  number of nodes in the final path
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


	/* The method getNodesVisited returns the number of nodes visited by the search
	 * Return: int
	 *						number of nodes visited by the search
	 */
	public int getNodesVisited ()
	{
		return nodesVisited;
	}

	/* The method getMaxQueueSize returns the maximum number of nodes held by the open list
	 * Return: int
	 *							maximum number of nodes held by the open list
	 */
	public int getMaxQueueSize ()
	{
		return maxQueueSize;
	}

  /* The method getFinalSolution returns an array containing the final solution
	 * Return: AIPolygon[]
	 *										an array containing the final solution
	 */
	public AIPolygon[] getFinalSolution()
	{
		return finalSolutionArray;
	}
}
