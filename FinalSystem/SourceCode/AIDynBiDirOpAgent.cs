/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIDynBiDirOpAgen class
 * This class will act as one search of the dynamically Bi-directional search on the Navigation Mesh
 *		It will be responsible with keeping a openList and a closedList, getting the shortest path, and gathering
 * 		the finalSolution that the movement will use. This search will alternate between a start to goal search and
 *   	 a goal and to start search and when the frontiers meet the search is complete
 *   	 It will use the finalSolution to set waypoints that is the
 * 		positions the movement Agent will travel to.
 */
using UnityEngine;
using System.Collections;

public class AIDynBiDirOpAgent {
	
	AIDynBiDirOpList  openList; //this will be a list that contain all the non explored nodes in the navigation mesh
	AIPolygon[] polygonArray; //This is the polygon array that makes up the navigation mesh
	AIDynBiDirOpNode  finalSolutionStart; //the last node in the solution
	AIPolygon[] finalSolutionArray; //the array that will hold the polygons that make up the final solution in order
	Vector3[] wayPoints; //the positions that will be sent to the movement Agent 
	Vector3 goalPosition; //the position of the goal object
	Vector3 agentStartPosition; //the position of the agent that is starting 
	int polygonFinalCount;
	int maxQueueSize;
	int nodesVisited;
	int queueSize;
	AIDynBiDirOpNode currentNode;
	float gCost;
	bool isBackwards; //bool to know if its going from the goal or from the agent
	AIDynBiDirOpNode[] closedList;
	public static float StartingH;
	
	/*
	 * AIDynBiDirOpAgent's constructor will set up the initial values for instance variables
	 * Parameters:	(Vector3)goalPositionToAdd is the position that the goal this search is looking for is at
	 * 				(AIPolygon[])polygonsToAdd is the array of polygons that it will search through
	 * 				(bool)isBackwardsToAdd is the bool to tell if this search is going from the goal or from the agent
	 */
	public AIDynBiDirOpAgent(Vector3 goalPositionToAdd, AIPolygon[] polygonsToAdd, bool isBackwardsToAdd)
	{
		polygonArray = polygonsToAdd;
		goalPosition = goalPositionToAdd;
		isBackwards = isBackwardsToAdd;
		polygonFinalCount = 0;
		maxQueueSize = 0;
		nodesVisited = 0;
		openList = new AIDynBiDirOpList (goalPosition, polygonArray.Length, this);
		closedList = new AIDynBiDirOpNode[polygonArray.Length];
		for (int count = 0; count < polygonArray.Length; count++)
			closedList [count] = null;
		if (isBackwards == true) {//going from the goal
			for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
			if (polygonArray [count].getHasGoal () == true) {
				openList.addNode (polygonArray [count], null, 0); //adds the first polygon to the openList
				count = polygonArray.Length;
			}
		}
		else{ //going from the agent
			for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
			if (polygonArray [count].getHasAgent () == true) {
				openList.addNode (polygonArray [count], null, 0); //adds the first polygon to the openList
				count = polygonArray.Length;
			}
		}
		gCost = 0f;
	}
	


	/*
	 * doSearch method will run the A* search till a goal is found, or a node is in another agent closed list, or the 
	 * 		nodes expanded is equal to the nodesToExpand variable
	 * Parameter:	(int)nodesToExpand is the number of nodes that the search can expand
	 * 				(AIDynBiDirOpNode[]) secondSearchClosedList is the closed list from the other agent
	 * Return:		(int)
	 * 					0 if nothing was found
	 * 					1 if the goal for this search agent was found
	 * 					2 if a node on the second agent's closed list was found
	 */
	public int doSearch(int nodesToExpand, AIDynBiDirOpNode[] secondSearchClosedList)
	{
		int nodesExpandedCount = 0;
		while(openList.isEmpty() == false && nodesExpandedCount < nodesToExpand)//goes until nothing is left on the open list meaning a path could not be found
		{
			currentNode = openList.popNode(); //take the first(Best) polygon off the openList
			nodesVisited++;
			if(currentNode == null)
				return 0;
			closedList[currentNode.getPolygon().getID()] = currentNode;
			if(isBackwards == true)
			{
				if(currentNode.getPolygon().getHasAgent() == true)
				{
					finalSolutionStart = currentNode;
					return 1;
				}
			}
			else
			{
				if(currentNode.getPolygon().getHasGoal() == true) //checks to see if the currentNode has the goal inside its polygon
				{
					finalSolutionStart = currentNode;
					return 1;
				}
			}
			if(secondSearchClosedList[currentNode.getPolygon().getID()] != null)
			{
				finalSolutionStart = currentNode;
				return 2;
			}
			for(int count = 0; count < currentNode.getPolygon().getNeighborsHeld(); count++) //adds all the neighbors that are not on the closed list to the open list
			{
				if(closedList[currentNode.getPolygon().getNeighborAt(count)] == null)
				{
					gCost = (currentNode.getPolygon().getCenterVector() - polygonArray[currentNode.getPolygon().getNeighborAt(count)].getCenterVector()).magnitude + currentNode.getGFromStartingNode();
					if(openList.isNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]) == false)
					{
						openList.addNode(polygonArray[currentNode.getPolygon().getNeighborAt(count)], currentNode, gCost);
					}
					else if(openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]).compareToG(gCost) > 0f) //updates the a Nodes information if the new GCost (cost from start to node) is less then what was previously in it
					{
						openList.updateNode(openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]), currentNode, gCost);
					}
					
				}
			}
			if(openList.getSize() > maxQueueSize)
				maxQueueSize = openList.getSize();
			nodesExpandedCount++;
		}
		return 0;
	}
	


	/*
	 * deletingNode method will place a node on the closed list essectially deleting it
	 * Parameter:	(AIDynBiDirOpNode)tempNode is the node that needs to be placed on the closed list
	 * Return:	none
	 */
	public void deletingNode(AIDynBiDirOpNode tempNode)
	{
		closedList[tempNode.getPolygon().getID()] = tempNode;
	}
	


	/*
	 * getCloseList method will return the closedList used by this agent
	 * Parameter:	none
	 * Return:	(AIDynBiDirOpNode[])
	 * 				the closed list from this agent
	 */
	public AIDynBiDirOpNode[] getCloseList()
	{
		return closedList;
	}
	
	
	
	/*
	 * addFinalsolutionPolygonsPostOrder method is a recusive method that will look at the parent of each node passed in until
	 * 		the node passed in is null, then it will add each Node's polygon to the finalSolutionArray in post order
	 * 		from start until end
	 * Parameter:	(AIDynBiDirOpNode)currentNode is the node that needs to be checked and then have its parent passed
	 * 				(ref int)counter is the current count of polygons in the FinalSolution array used to access the next index
	 * Return:	none
	 */
	void addFinalSolutionPolygonsPostOrder (AIDynBiDirOpNode  currentNode, ref int counter)
	{
		if (currentNode != null) {
			addFinalSolutionPolygonsPostOrder (currentNode.getParentNode (), ref counter);
			finalSolutionArray [counter] = currentNode.getPolygon ();
			counter++;
		}
	}
	
	
	
	/*
	 * addFinalsolutionPolygonsPreOrder method is a recusive method that will look at the parent of each node passed in until
	 * 		the node passed in is null, then it will add each Node's polygon to the finalSolutionArray in pre order
	 * 		from start until end
	 * Parameter:	(AIDynBiDirOpNode)currentNode is the node that needs to be checked and then have its parent passed
	 * 				(ref int)counter is the current count of polygons in the FinalSolution array used to access the next index
	 * Return:	none
	 */
	void addFinalSolutionPolygonsPreOrder(AIDynBiDirOpNode currentNode, ref int counter)
	{
		if (currentNode != null) {
			finalSolutionArray [counter] = currentNode.getPolygon ();
			counter++;
			addFinalSolutionPolygonsPreOrder (currentNode.getParentNode (), ref counter);
		}
		
	}
	
	
	
	/*
	 * getFinalPath method will get the final path for this agent. It will either be preOrder or PostOrd
	 * Parameter:	none
	 * Return:	(AIPolygon[])
	 * 				the final path for this agent
	 */
	public AIPolygon[] getFinalPath()
	{
		printFinalSolution ();
		finalSolutionArray = new AIPolygon[polygonFinalCount];
		int counter = 0;
		if (isBackwards == true) { //going from the goal
			addFinalSolutionPolygonsPreOrder(finalSolutionStart, ref counter);
		} else { //going from the agent
			addFinalSolutionPolygonsPostOrder(finalSolutionStart, ref counter);
		}
		return finalSolutionArray;
		
	}
	
	
	//used for debugging
	void printFinalSolution()
	{
		printSolutionRecusively (finalSolutionStart);
	}
	
	//used for debugging
	void printSolutionRecusively(AIDynBiDirOpNode  currentNode)
	{
		if (currentNode != null) {
			printSolutionRecusively (currentNode.getParentNode ());
			polygonFinalCount++;
		}
	}
	
	
	
	/*
	 * getFinalPathLength method will return the number of nodes in the final path array
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes in the final path array
	 */
	public int getFinalPathLength ()
	{
		return polygonFinalCount;
	}
	
	
	
	/*
	 * getNodesVisited method will return the number of nodes visited by this search
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes visited by this search
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
	 * getFinalSolution method will return the polygons that are in the final solution array
	 * Parameter:	none
	 * Return:	(AIPolygon[])
	 * 				an array that holds the polygons that are in the final solution array
	 */
	public AIPolygon[] getFinalSolution()
	{
		return finalSolutionArray;
	}
	
	
	
	/*
	 * getFinalPathStart method will return the starting polygon of the final solution
	 * Parameter:	none
	 * Return:	(AIPolygon)
	 * 				the starting polygon of the final solution
	 */
	public AIPolygon getFinalPathStart()
	{
		return finalSolutionStart.getPolygon();
	}
	
	
	
	/*
	 * getFinalPathStartingWithNodes method will return a final path array that starts the the node
	 * 		containing the polygon being sent in
	 * Parameter:	(AIPolygon) polygonToStartWith is the polygon that is in the node the caller wants the
	 * 					final path array to start with
	 * Return:		(AIPolygon[])
	 * 					the final path array starting with the node containing the polygon being passed in
	 */
	public AIPolygon[] getFinalPathStartingWithNode (AIPolygon polygonToStartWith)
	{
		AIDynBiDirOpNode tempNode = closedList[polygonToStartWith.getID()];
		if (tempNode == null)
			return null;
		finalSolutionStart = tempNode;
		return getFinalPath ();
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
 */