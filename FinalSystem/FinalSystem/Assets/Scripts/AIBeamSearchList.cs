﻿/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIBeamSearchList	class
 * This class will implement a list that will be use by the BeamSearch. It will be able to add a node to the list,
 * 		check if the list has to many nodes in it and then reduce the nodes, delete a node, check if a node is in the list
 * 		get a node from the list
 */

using UnityEngine;
using System.Collections;

public class AIBeamSearchList
{
	AIAgentAStarSearchNode[] heap; // references the front of the lists
	int[] indicesArray;
	int numberOfNodesHeld; // keeps track of number of nodes in list
	Vector3 goalPosition; // stores the goal position
	bool[] inList; //keeps track of what nodes are in the list
	AIBeamSearch search;

	/* AIBeamSearchlist method is a constructor for this class. It will set up the initial values for the search
	 * Parameter:	(Vector3) goalPositionToAdd
	 * 				(int) polygonArrayLength is the length of the polygonArray from the navigation mesh
	 */
	public AIBeamSearchList(Vector3 goalPositionToAdd, int polygonArrayLength, AIBeamSearch searchToAdd)
	{
		search = searchToAdd;
		numberOfNodesHeld = 0;
		goalPosition = goalPositionToAdd;
		inList = new bool[polygonArrayLength];
		indicesArray = new int[polygonArrayLength];
		heap = new AIAgentAStarSearchNode[polygonArrayLength];
		for (int count = 0; count < polygonArrayLength; count++) {
			inList [count] = false;
			heap [count] = null;
		}
	}

	/*
	 * shiftUp method will act as perculate up for the min-heap in a recusive fashion
	 * Parameters:	(int)index is the index that needs to perculate up
	 * Return:	none
	 */
	void shiftUp(int index)
	{
		if (index > 0) {
			int parent = (index - 1) / 2;
			if (heap [parent].getTotalCost () > heap [index].getTotalCost ()) {
				swap (parent, index);
				shiftUp (parent);
			}
		}
	}
	

	/*
	 * swap method will swap two nodes in the heap according to the indices passed in
	 * Parameter:	(int)first is the index of the first node to swap
	 * 				(int)second is the index of the second node to swap
	 * Return:	none
	 */
	void swap(int first, int second)
	{
		AIAgentAStarSearchNode tempNode = heap [first];
		heap [first] = heap [second];
		indicesArray [heap [first].getPolygon ().getID ()] = first;
		heap [second] = tempNode;
		indicesArray [heap [second].getPolygon ().getID ()] = second;
	}



	/* addNode adds a node to this list in order according to its fTotalCost value
	 * Parameter: (AIPolygon) polygonToAdd is the polygon that will be held by the node
	 * (AIAgentAStarSearchNode) parentNodeToAdd is the parent of the node to be added
	 * (float) gCostToAdd is the getGFromStartingNode value to be stored in the node to be added
	 * Return: none
	 */
	public void addNode(AIPolygon polygonToAdd, AIAgentAStarSearchNode parentNodeToAdd, float gCostToAdd)
	{

		AIAgentAStarSearchNode newSearchNode = new AIAgentAStarSearchNode (polygonToAdd, parentNodeToAdd, gCostToAdd, goalPosition);
		if (numberOfNodesHeld > 229)
			deleteMax ();
		inList [polygonToAdd.getID ()] = true;
		indicesArray [polygonToAdd.getID ()] = numberOfNodesHeld;
		heap [numberOfNodesHeld] = newSearchNode;
		shiftUp (numberOfNodesHeld);
		numberOfNodesHeld++;
	
	}



	/* printList prints out the ID's of the polygons in the list
	 * Parameter: none
	 * Return: none
	 */
	public void printList()
	{
		Debug.Log (Time.realtimeSinceStartup + " printList");
		for (int count = 0; count < numberOfNodesHeld; count++)
			Debug.Log (Time.realtimeSinceStartup + " count = " + count + " node = " + heap [count].getPolygon ().getID () + " cost = " + heap [count].getTotalCost ());
	}
	
	
	/*
	 * shiftDown method will perculate down an item of min-heap in a recusive fashion
	 * Parameter:	(int)index is the index that needs to be perculated down
	 * Return:	none
	 */
	void shiftDown(int index)
	{
		int leftChild = 2 * index + 1;
		int rightChild = 2 * index + 2;
		
		if (rightChild >= numberOfNodesHeld && leftChild >= numberOfNodesHeld)
			return;
		int smallestChild = heap [rightChild].getTotalCost () > heap [leftChild].getTotalCost () ? leftChild : rightChild;
		
		if (heap [index].getTotalCost () > heap [smallestChild].getTotalCost ()) {
			swap (smallestChild, index);
			shiftDown (smallestChild);
		}
		
		
	}



	/* popNode removes and returns the node at the front of the list (node with lowest fToatlCost value)
	 * Parameter: none
	 * Return: (AIAgentAStarSearchNode)
	 *				node that was removed from the front of this list
	 */
	public AIAgentAStarSearchNode popNode()
	{
		if (numberOfNodesHeld == 0)
			return null;
		AIAgentAStarSearchNode tempNode = heap [0];
		indicesArray [tempNode.getPolygon ().getID ()] = -1;
		numberOfNodesHeld--;
		heap [0] = heap [numberOfNodesHeld];
		indicesArray [heap [0].getPolygon ().getID ()] = 0;
		if (numberOfNodesHeld > 0)
			shiftDown (0);
		return tempNode;
	}





	/* updateNode updates the parent of a node and the gFromStartingNodeCost of a node
	 * Parameter: (AIAgentAStarSearchNode) nodeToUpdate is the node to be updated
	 * (AIAgentAStarSearchNode) newParentNode is the new parent of the node to be updated
	 * (float) newCost is the new gFromStartingNodeCost of the node to be updated
	 * Return: none
	 */
	public void updateNode(AIAgentAStarSearchNode nodeToUpdate, AIAgentAStarSearchNode newParentNode, float newCost)
	{
		AIPolygon temp = nodeToUpdate.getPolygon ();
		deleteNodeOfId (nodeToUpdate.getPolygon ().getID ());
		addNode (temp, newParentNode, newCost);
	}



	/* deleteNodeOfId deletes the node from the list that is holding a polygon with a certain id
	 * Parameter: (int) idToDelete is the id of the polygon being held by the node to be deleted
	 * Return: none
	 */
	public void deleteNodeOfId(int idToDelete)
	{
		int index = indicesArray [idToDelete];
		if (index == 0)
			return;
		if (index >= numberOfNodesHeld)
			return;
		if (index == numberOfNodesHeld - 1) {
			numberOfNodesHeld--;
			return;
		}
		
		numberOfNodesHeld--;
		heap [index] = heap [numberOfNodesHeld];
		indicesArray [heap [index].getPolygon ().getID ()] = index;
		if (index > 0 && heap [index].getTotalCost () < heap [(index - 1) / 2].getTotalCost ())
			shiftUp (index);
		else
			shiftDown (index);
		
	}




	/* doesIDMatch checks if two specfied ids match
	 * Parameters: (int) polygonId, (int) idToCheck
	 * Return: (bool)
	 *						true if polygonId and idToCheck are the same
	 *						false if polygonId and idToCheck are not the same
	 */
	public bool doesIDMatch(int polygonId, int idToCheck)
	{
		if (idToCheck == polygonId)
			return true;
		return false;
	}





	/* isNodeOnList checks to see if node containing a certain polygon is on the list
	 * Parameter: (AIPolygon) polygon whose id will be used to check if a certain node is on list
	 * Return: (bool)
	 *						true if a node is on the list that contains polygonToCheck
	 *						false if no node on the list contains polygonToCheck
	 */
	public bool isNodeOnList(AIPolygon polygonToCheck)
	{
		return inList [polygonToCheck.getID ()];
	}






	/* getNodeOnList returns node from list that contains a specified polygon
	 * Parameter: (AIPolygon) polygonToCheck is the polygon being held by the node to be returned
	 * Return: (AIAgentAStarSearchNode)
	 *														node on list that is holding polygonToCheck
	 */
	public AIAgentAStarSearchNode getNodeOnList(AIPolygon polygonToCheck)
	{
		return heap [indicesArray [polygonToCheck.getID ()]];
	}




	/* isEmpty tells us if this list is empty
	 * Parameter: none
	 * Return: (bool)
	 *						true if list is empty
	 *						false if list is not empty
	 */
	public bool isEmpty()
	{
		return numberOfNodesHeld == 0;
	}


	/*
	 * deleteMax method will delete the node with the biggest totalCost of the whole heap
	 * Paramtere:	none
	 * Return:	none
	 */
	public void deleteMax ()
	{
		int index = 0;
		float max = heap [index].getTotalCost ();
		int maxIndex = index;
		for (int count = 1; count < numberOfNodesHeld; count++) {
			if (heap [count].getTotalCost () > max) {
				max = heap [count].getTotalCost ();
				maxIndex = count;
			}
		}
		//Debug.Log (Time.realtimeSinceStartup + " deleteMAx id = " + heap [maxIndex].getPolygon ().getID ());
		search.deletingNode (heap [maxIndex].getPolygon ().getID ());
		deleteNodeOfId (heap [maxIndex].getPolygon ().getID ());

	}




	/*
	 * getSize method will return the number of nodes held by this list at the moment
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes being held by the list at this moment
	 */
	public int getSize()
	{
		return numberOfNodesHeld;
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
 *
 */