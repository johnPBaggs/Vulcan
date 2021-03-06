﻿/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIDynamicWeightedSearchList class
 * This class is used for the open and closed lists used by the AIDynamicWeightedSearch. It will be able to 
 * 		add a node into the list either at the front or in order. Delete a node from the list, check if a node
 * 		is already on the list. 
 */

using UnityEngine;
using System.Collections;

public class AIDynamicWeightedSearchList
{

	AIDynamicWeightedSearchNode[] heap;
	int[] indicesArray;
	int numberOfNodesHeld; // keeps track of number of nodes in list
	Vector3 goalPosition; // stores the goal position
	bool[] inList; //keeps track of what nodes are in the list



	/* AIDynamicWeightedSearchList method is a constructor for this class that will set up the initial values for the
	 * 		instance variables
	 * Parameter:	(Vector3) goalPositionToAdd is the position of the goal
	 * 				(int) polygonArrayLength is the number of polygons that are in the naviagation mesh
	 */
	public AIDynamicWeightedSearchList(Vector3 goalPositionToAdd, int polygonArrayLength)
	{
		numberOfNodesHeld = 0;
		goalPosition = goalPositionToAdd;
		inList = new bool[polygonArrayLength];
		indicesArray = new int[polygonArrayLength];
		heap = new AIDynamicWeightedSearchNode[polygonArrayLength];
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
		AIDynamicWeightedSearchNode tempNode = heap [first];
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
	public void addNode(AIPolygon polygonToAdd, AIDynamicWeightedSearchNode parentNodeToAdd, float gCostToAdd)
	{
		AIDynamicWeightedSearchNode newSearchNode;
		if(numberOfNodesHeld == 0)
			newSearchNode = new AIDynamicWeightedSearchNode (polygonToAdd, parentNodeToAdd, gCostToAdd, goalPosition, AIDynamicWeightedSearch.startingH, 1);
		else
			newSearchNode = new AIDynamicWeightedSearchNode (polygonToAdd, parentNodeToAdd, gCostToAdd, goalPosition, AIDynamicWeightedSearch.startingH, (parentNodeToAdd.getDoFN() + 1));
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
	 * Return: (AIDynamicWeightedSearchNode)
	 *				node that was removed from the front of this list
	 */
	public AIDynamicWeightedSearchNode popNode()
	{
		if (numberOfNodesHeld == 0)
			return null;
		AIDynamicWeightedSearchNode tempNode = heap [0];
		indicesArray [tempNode.getPolygon ().getID ()] = -1;
		numberOfNodesHeld--;
		heap [0] = heap [numberOfNodesHeld];
		indicesArray [heap [0].getPolygon ().getID ()] = 0;
		if (numberOfNodesHeld > 0)
			shiftDown (0);
		return tempNode;
	}




	/* updateNode updates the parent of a node and the gFromStartingNodeCost of a node
	 * Parameter:	(AIDynamicWeightedSearchNode) nodeToUpdate is the node to be updated
	 * 				(AIDynamicWeightedSearchNode) newParentNode is the new parent of the node to be updated
	 * 				(float) newCost is the new gFromStartingNodeCost of the node to be updated
	 * Return: none
	 */
	public void updateNode(AIDynamicWeightedSearchNode nodeToUpdate, AIDynamicWeightedSearchNode newParentNode, float newCost)
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
	 * Parameter:	(AIPolygon) polygonToCheck is the polygon being held by the node to be returned
	 * Return:	(AIDynamicWeightedSearchNode)
	 *				node on list that is holding polygonToCheck
	 */
	public AIDynamicWeightedSearchNode getNodeOnList(AIPolygon polygonToCheck)
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
