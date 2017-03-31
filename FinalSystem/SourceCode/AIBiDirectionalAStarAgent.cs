/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIBiDirectionalAStarAgent class
 * This class will be responsible with conducting and completing the BiDirectional AStar search algorithm on the navigation mesh.
 * 		It will be responsible with keeping a openList and a closedList, getting the shortest path, and gathering
 * 		the finalSolution that the movement will use. This search will alternate between a start to goal search and
 *    a goal and to start search and when the frontiers meet the search is complete
 *    It will use the finalSolution to set waypoints that is the
 * 		positions the movement Agent will travel to.
 */

﻿using UnityEngine;
using System.Collections;

public class AIBiDirectionalAStarAgent
{
	AIAgentAStarSearchList openList; //this will be a list that contain all the non explored nodes in the navigation mesh
	AIAgentAStarSearchList closedList; //this will be a list that contain all the explored nodes in the navigation mesh
	AIPolygon[] polygonArray; //This is the polygon array that makes up the navigation mesh
	AIAgentAStarSearchNode  finalSolutionStart; //the last node in the solution
	AIPolygon[] finalSolutionArray; //the array that will hold the polygons that make up the final solution in order
	Vector3[] wayPoints; //the positions that will be sent to the movement Agent
	Vector3 goalPosition; //the position of the goal object
	Vector3 agentStartPosition; //the position of the agent that is starting
	int polygonFinalCount;
	int maxQueueSize;
	int nodesVisited;
	int queueSize;
	AIAgentAStarSearchNode  currentNode;
	float gCost;
	bool isBackwards;

  /* The AIBiDirectionalAStarAgent
	 *
	 * Parameters: Vector3 goalPositionToAdd - location of goal in the search
	 * AIPolygon[] polygonsToAdd - array of walkable polygons to be search for a path
	 * bool isBackwardsToAdd - determines if polygon was added as a result of backtracking
	 */
	public AIBiDirectionalAStarAgent(Vector3 goalPositionToAdd, AIPolygon[] polygonsToAdd, bool isBackwardsToAdd)
	{
		polygonArray = polygonsToAdd;
		goalPosition = goalPositionToAdd;
		isBackwards = isBackwardsToAdd;
		polygonFinalCount = 0;
		maxQueueSize = 0;
		nodesVisited = 0;
		openList = new AIAgentAStarSearchList (goalPosition);
		closedList = new AIAgentAStarSearchList (goalPosition);
		if (isBackwards == true) {
			for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
			if (polygonArray [count].getHasGoal () == true) {
				openList.addNode (polygonArray [count], null, 0); //adds the first polygon to the openList
				count = polygonArray.Length;
			}
		}
		else{
			for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
			if (polygonArray [count].getHasAgent () == true) {
				openList.addNode (polygonArray [count], null, 0); //adds the first polygon to the openList
				count = polygonArray.Length;
			}
		}
		queueSize = 1;
		gCost = 0f;
	}

	/*
	 * doSearch method will run the A* search till a goal is found, or a node is in another agent closed list, or the 
	 * 		nodes expanded is equal to the nodesToExpand variable
	 * Parameter:	(int)nodesToExpand is the number of nodes that the search can expand
	 * 				(AIAgentAStarSearchList) secondSearchClosedList is the closed list from the other agent
	 * Return:		(int)
	 * 					0 if nothing was found
	 * 					1 if the goal for this search agent was found
	 * 					2 if a node on the second agent's closed list was found
	 */
	public int doSearch(int nodesToExpand, AIAgentAStarSearchList secondSearchClosedList)
	{
		int nodesExpandedCount = 0;
		while(openList.isEmpty() == false && nodesExpandedCount < nodesToExpand)//goes until nothing is left on the open list meaning a path could not be found
		{
			currentNode = openList.popNode(); //take the first(Best) polygon off the openList
			queueSize--;
			nodesVisited++;
			if(currentNode == null)
				return 0;
			closedList.enqueue(currentNode); //add currentNode to the closedList
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
			if(secondSearchClosedList.isNodeOnList(currentNode.getPolygon()) == true)
			{
				finalSolutionStart = currentNode;
				return 2;
			}
			for(int count = 0; count < currentNode.getPolygon().getNeighborsHeld(); count++) //adds all the neighbors that are not on the closed list to the open list
			{
				if(closedList.isNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]) == false)
				{
					gCost = (currentNode.getPolygon().getCenterVector() - polygonArray[currentNode.getPolygon().getNeighborAt(count)].getCenterVector()).magnitude + currentNode.getGFromStartingNode();
					if(openList.isNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]) == false)
					{
						openList.addNode(polygonArray[currentNode.getPolygon().getNeighborAt(count)], currentNode, gCost);
						queueSize++;
					}
					else if(openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]).compareToG(gCost) > 0f) //updates the a Nodes information if the new GCost (cost from start to node) is less then what was previously in it
					{
						openList.updateNode(openList.getNodeOnList(polygonArray[currentNode.getPolygon().getNeighborAt(count)]), currentNode, gCost);
						queueSize++;
					}
				}
			}
			if(openList.getSize() > maxQueueSize)
				maxQueueSize = openList.getSize();
			nodesExpandedCount++;
		}
		return 0;
	}

  /* The method getCloseList returns the closedList
	 * Return: AIAgentAStarSearchList - list of nodes on the closed list
	 */
	public AIAgentAStarSearchList getCloseList()
	{
		return closedList;
	}



	/*
	 * addFinalsolutionPolygons method is a recursive method that will look at the parent of each node passed in until
	 * 		the node passed in is null, then it will add each Node's polygon to the finalSolutionArray in order
	 * 		from start until end
	 * Parameter:	(AIAgentAStarSearchNode)currentNode is the node that needs to be checked and then have its parent passed
	 * 				(ref int)counter is the current count of polygons in the FinalSolution array used to access the next index
	 * Return:	none
	 */
	void addFinalSolutionPolygonsPostOrder (AIAgentAStarSearchNode  currentNode, ref int counter)
	{
		if (currentNode != null) {
			addFinalSolutionPolygonsPostOrder (currentNode.getParentNode (), ref counter);
			finalSolutionArray [counter] = currentNode.getPolygon ();
			counter++;
		}
	}

  /* addFinalSolutionPolygonsPreOrder adds the final solution polygons selected from the search in preorder
	 * Parameters: AIAgentAStarSearchNode currentNode - currentNode being looked at in recursive descent
	 * ref int counter - keeps count of how many nodes have been looked at
	 */
	void addFinalSolutionPolygonsPreOrder(AIAgentAStarSearchNode currentNode, ref int counter)
	{
		if (currentNode != null) {
			finalSolutionArray [counter] = currentNode.getPolygon ();
			counter++;
			addFinalSolutionPolygonsPreOrder (currentNode.getParentNode (), ref counter);
		}

	}

  /* The method getFinalPath constructs and returns the final solution array
	 * Return: AIPolygon[]
	 * 										 polygon array containing the polygons that comprise the final path solution
	 */
	public AIPolygon[] getFinalPath()
	{
		printFinalSolution ();
		finalSolutionArray = new AIPolygon[polygonFinalCount];
		int counter = 0;
		if (isBackwards == true) {
			addFinalSolutionPolygonsPreOrder(finalSolutionStart, ref counter);
		} else {
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
	void printSolutionRecusively(AIAgentAStarSearchNode  currentNode)
	{
		if (currentNode != null) {
			printSolutionRecusively (currentNode.getParentNode ());
			polygonFinalCount++;
		}
	}

  /* The method getFinalPathLength  returns the length of the final path in terms of polygons
	 * Return: int
	 *						  number of nodes in the final path
	 */
	public int getFinalPathLength ()
	{
		return polygonFinalCount;
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

  /* The getFinalSolution method returns the finalSolutionArray
	 * Return: AIPolygon[]
	 *										array containing the final solution
	 */
	public AIPolygon[] getFinalSolution()
	{
		return finalSolutionArray;
	}

  /* The method getFinalPathStart returns the starting node of the search
	 * Return: AIPolygon
	 *										first polygon in the search path
	 */
	public AIPolygon getFinalPathStart()
	{
		return finalSolutionStart.getPolygon();
	}

  /* The method getFinalPathStartingWithNode
	 * Return: AIPolygon[]
	 *										 Sets the starting polygon of the final path and returns the new finalPath
	 */
	public AIPolygon[] getFinalPathStartingWithNode (AIPolygon polygonToStartWith)
	{
		AIAgentAStarSearchNode tempNode = closedList.getNodeOnList (polygonToStartWith);
		if (tempNode == null)
			return null;
		finalSolutionStart = tempNode;
		return getFinalPath ();
	}
}
