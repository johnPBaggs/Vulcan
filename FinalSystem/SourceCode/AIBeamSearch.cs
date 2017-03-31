/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIBeamSearch class
 * This class will be responsible with conducting and completing the Beam search algorithm on the navigation mesh.
 * 		Essentially the A* search but with a limit imposed on the size of the open list.
 */

using UnityEngine;
using System.Collections;

public class AIBeamSearch : AISearchInterface
{

	AIBeamSearchList openList; //this will be a list that contain all the non explored nodes in the navigation mesh
	AIPolygon[] polygonArray; //This is the polygon array that makes up the navigation mesh
	AIAgentAStarSearchNode  finalSolutionStart; //the last node in the solution
	AIPolygon[] finalSolutionArray; //the array that will hold the polygons that make up the final solution in order
	Vector3 goalPosition; //the position of the goal object
	Vector3 agentStartPosition; //the position of the agent that is starting
	public GameObject agent; //the agent GameObject
	public Rigidbody AgentRigidbody;
	int polygonFinalCount;
	float finalPathCost;
	int maxQueueSize; //limit imposed on the size of the open list
	int nodesVisited;
	bool[] closedList2;




	/*
	 * AIBeamSearch methos is the constructor for this class. It will set all the variables to their initial values
	 * 		and start the search
	 * Parameters:	(Vector3)goalToAdd is the position that the goal object is at
	 * 				(AIPolygon[])polygonArrayToAdd is the polygonArray made from the navigation mesh
	 * Return:	none
	 */
	public AIBeamSearch(Vector3 goalToAdd, AIPolygon[] polygonArrayToAdd)
	{
		polygonArray = polygonArrayToAdd;
		goalPosition = goalToAdd;
		polygonFinalCount = 0;
		maxQueueSize = 0;
		nodesVisited = 0;
		finalPathCost = 0f;
		closedList2 = new bool[polygonArray.Length];
		for (int count = 0; count < polygonArray.Length; count++)
			closedList2 [count] = false;
	}



	/*
	 * startSearch method will begin the search by first setting the first node in the open list, which holds the
	 * 		polygon that has the agent GameObject in it, then it will call the functions to do the search, build the
	 * 		finalSolution array, and set the wayPoints
	 *	Parameters:	none
	 *	Return:	none
	*/
	public void startSearch()
	{
		openList = new AIBeamSearchList (goalPosition, polygonArray.Length, this);
		for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
		if (polygonArray [count].getHasAgent () == true) {
			openList.addNode (polygonArray [count], null, 0); //adds the first polygon to the openList
			count = polygonArray.Length;
		}
		AStarSearch(); //does the AStar search
		printFinalSolution (); //prints the finalSolution for debugging
		addFinalSolutionPolygons (); //adds the finalsolution to the finalsolution array

	}



	/*
	 * AStarSearch method will preform an A* algorithm for searching a space to find a goal. It does this by taking the best
	 * 		polygon for the search off the openList, adding its neighbors to the openlist, placing the node on the closed list
	 * 		then repeating until it found a polygon that has the goal gameObject inside it
	 * Parameters:	none
	 * Return:	none
	 */
	void AStarSearch()
	{
		maxQueueSize = 1;
		AIAgentAStarSearchNode  currentNode;
		float gCost = 0f;

		while(openList.isEmpty() == false)//goes until nothing is left on the open list meaning a path could not be found
		{
			currentNode = openList.popNode(); //take the first(Best) polygon off the openList
			nodesVisited++;
			if(currentNode == null)
				return;
			closedList2[currentNode.getPolygon().getID()] = true;
			if(currentNode.getPolygon().getHasGoal() == true) //checks to see if the currentNode has the goal inside its polygon
			{
				finalSolutionStart = currentNode;
				return;
			}
			for(int count = 0; count < currentNode.getPolygon().getNeighborsHeld(); count++) //adds all the neighbors that are not on the closed list to the open list
			{
				if(closedList2[currentNode.getPolygon().getNeighborAt(count)] == false)
				{
					gCost = (currentNode.getPolygon().getCenterVector() - polygonArray[currentNode.getPolygon().getNeighborAt(count)].getCenterVector()).magnitude + currentNode.getGFromStartingNode();
					if(openList.isNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]) == false)
					{
						openList.addNode(polygonArray[currentNode.getPolygon().getNeighborAt(count)], currentNode, gCost);
					}
					else if(openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]).compareToG(gCost) > 0f) //updates the a Nodes information if the new GCost (cost from start to node) is less then what was previously in it
					{
						//Debug.Log(Time.realtimeSinceStartup + " updating polygon id = " + openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]).getPolygon().getID());
						openList.updateNode(openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]), currentNode, gCost);
					}
				}
			}
			if(openList.getSize() > maxQueueSize)
				maxQueueSize = openList.getSize();
		}
	}

	public void deletingNode(int id)
	{
		closedList2 [id] = true;
	}

	//used for debugging
	void printFinalSolution()
	{
		printSolutionRecusively (finalSolutionStart);
	}

	//used for debugging
	void printSolutionRecusively(AIAgentAStarSearchNode  currentNode)
	{
		if (currentNode != null) {
			printSolutionRecusively (currentNode.getParentNode ());
			polygonFinalCount++;
		}
	}


	/*
	 * addFinalSolutionPolygons method will call a funtion that recusivly look at the polygons in the Finalsolution
	 * 		and adds them from the starting node to the ending node to the finalSolution array
	 * Parameters:	none
	 * Return:	none
	 */
	void addFinalSolutionPolygons ()
	{
		finalSolutionArray = new AIPolygon[polygonFinalCount];
		int counter = 0;
		addFinalSolutionPolygons (finalSolutionStart, ref counter); //calls a recusive method to get the FinalSolution

	}



	/*
	 * addFinalsolutionPolygons method is a recusive method that will look at the parent of each node passed in until
	 * 		the node passed in is null, then it will add each Node's polygon to the finalSolutionArray in order
	 * 		from start until end
	 * Parameter:	(AIAgentAStarSearchNode)currentNode is the node that needs to be checked and then have its parent passed
	 * 				(ref int)counter is the current count of polygons in the FinalSolution array used to access the next index
	 * Return:	none
	 */
	void addFinalSolutionPolygons (AIAgentAStarSearchNode  currentNode, ref int counter)
	{
		if (currentNode != null) {
			addFinalSolutionPolygons (currentNode.getParentNode (), ref counter);
			finalSolutionArray [counter] = currentNode.getPolygon ();
			if(counter != 0)
				finalPathCost += (finalSolutionArray[counter].getCenterVector() - finalSolutionArray[counter - 1].getCenterVector()).magnitude; 
			counter++;
		}
	}

	/* GetFinalPathLength returns the final length of the path in terms of number
	 * of nodes.
	 * Return: int
	 *							number of nodes in final path
	 */
	public int getFinalPathLength()
	{
		return finalSolutionArray.Length;
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
	 *							number of nodes visited by search
	 */
	public int getNodesVisited()
	{
		return nodesVisited;
	}

  /* getMaxQueueSize returns the maximum number of nodes the open list holds during the search
	 * Return: int
	 *						  maximum number of nodes the open list holds during the search
	 */
	public int getMaxQueueSize()
	{
		return maxQueueSize;
	}

  /* The method getFinalSolution returns an array holding the nodes that comprise the final solution
	 * Return: AIPolygon[]
	 * 											array containing the final solution from the search										
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
 *
 *
 *
 *
 */
