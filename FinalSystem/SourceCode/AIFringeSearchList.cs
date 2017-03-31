/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIFringeSearchList class
 * This class is used as a list for the Fringe search. It will be apple to get a node from the list, put a node in the front of the
 * 		list, delete a node from the list, check if a node is on the list.
 */

using UnityEngine;
using System.Collections;

public class AIFringeSearchList 
{
	AIFringeSearchNode frontOfList; //the reference for the front of the list 
	int numberOfNodesHeld;
	Vector3 goalPosition;



	/*
	 * AIFringeSearchList's constructor will set up the initial values for the instance variables
	 * Parameter:	(Vector3)goalPositionToAdd is the position of the goal
	 */
	public AIFringeSearchList(Vector3 goalPositionToAdd)
	{
		frontOfList = null;
		numberOfNodesHeld = 0;
		goalPosition = goalPositionToAdd;
	}



	/*
	 * enqueue method will place a node in the front of the list
	 * Parameter:	(AIPolygon)polygonToAdd is the polygon for the new node
	 * 				(AIFringeSearchNode)parentNode is the node that will be the parent of this node
	 * 				(float)gCostToAdd is the cost to get to this node
	 * Return:	none
	 */
	public void enqueue(AIPolygon polygonToAdd, AIFringeSearchNode parentNode, float gCostToAdd)
	{
		AIFringeSearchNode tempNode = new AIFringeSearchNode (polygonToAdd, parentNode, gCostToAdd, goalPosition);
		tempNode.setNextNode (frontOfList);
		frontOfList = tempNode;
		numberOfNodesHeld++;
	}



	/*
	 * addAfter will make a new polygon and add it right after the node being passed in
	 * Parameter:	(AIPolygon)polygonToAdd is the polygon for the new node
	 * 				(AIFringeSearchNode)parentNode is the node that will be the parent of this node
	 * 				(float)gCostToAdd is the cost to get to this node
	 * Return:	none
	 */
	public void addAfter(AIPolygon polygonToAdd, AIFringeSearchNode nodeToAddAfter, float gCostToAdd)
	{
		AIFringeSearchNode tempNode = new AIFringeSearchNode (polygonToAdd, nodeToAddAfter, gCostToAdd, goalPosition);
		tempNode.setNextNode (nodeToAddAfter.getNextNode ());
		nodeToAddAfter.setNextNode (tempNode);
		numberOfNodesHeld++;
	}

	

	/* doesIDMatch checks if two specfied ids match
	 * Parameters: (int) polygonId, (int) idToCheck
	 * Return: (bool)
	 *						true if polygonId and idToCheck are the same
	 *						false if polygonId and idToCheck are not the same
	 */
	public bool doesIDMatch(int polygonID, int idToCheck)
	{
		if (polygonID == idToCheck)
			return true;
		return false;
	}



	/* deleteNodeOfId deletes the node from the list that is holding a polygon with a certain id
	 * Parameter: (int) idToDelete is the id of the polygon being held by the node to be deleted
	 * Return: none
	 */
	public void deleteNodeOfId(int idToDelete)
	{
		AIFringeSearchNode tempFront = frontOfList;
		if(frontOfList != null)
		{
			if(doesIDMatch(frontOfList.getPolygon().getID(), idToDelete) == true)
			{
				frontOfList = frontOfList.getNextNode();
				numberOfNodesHeld--;
				return;
			}
			AIFringeSearchNode tempback = tempFront.getNextNode();
			while((tempback.getNextNode() != null) && (doesIDMatch(tempback.getPolygon().getID(), idToDelete) == false))
			{
				tempFront = tempback;
				tempback = tempFront.getNextNode();
			}
			if(doesIDMatch(tempback.getPolygon().getID(), idToDelete) == true)
			{
				tempFront.setNextNode(tempback.getNextNode());
			}
			numberOfNodesHeld--;
		}

	}



	/* isNodeOnList checks to see if node containing a certain polygon is on the list
	 * Parameter: (AIPolygon) polygon whose id will be used to check if a certain node is on list
	 * Return: (bool)
	 *						true if a node is on the list that contains polygonToCheck
	 *						false if no node on the list contains polygonToCheck
	 */
	public bool isNodeOnList(AIPolygon polygonToCheck)
	{
		AIFringeSearchNode tempFront = frontOfList;
		if (tempFront == null)
			return false;
		if (doesIDMatch (polygonToCheck.getID (), tempFront.getPolygon ().getID ()) == true)
			return true;
		AIFringeSearchNode tempBack = tempFront.getNextNode ();
		while ((tempBack != null) && (tempBack.getNextNode() != null) && (doesIDMatch(polygonToCheck.getID(), tempBack.getPolygon().getID()) == false)) {
			tempFront = tempBack;
			tempBack = tempFront.getNextNode();
		}
		if (tempBack == null)
			return false;
		if (doesIDMatch (polygonToCheck.getID (), tempBack.getPolygon ().getID ()) == true)
			return true;
		return false;
	}



	/* getNodeOnList returns node from list that contains a specified polygon
	 * Parameter:	(AIPolygon) polygonToCheck is the polygon being held by the node to be returned
	 * Return:	(AIFringeSearchNode)
	 *				node on list that is holding polygonToCheck
	 */
	public AIFringeSearchNode getNodeOnList(AIPolygon polygonToCheck)
	{
		AIFringeSearchNode tempFront = frontOfList;
		if (tempFront == null)
			return null;
		if (doesIDMatch (polygonToCheck.getID (), tempFront.getPolygon ().getID ()) == true)
			return tempFront;
		AIFringeSearchNode tempBack = tempFront.getNextNode ();
		while ((tempBack != null) && (tempBack.getNextNode() != null) && (doesIDMatch(polygonToCheck.getID(), tempBack.getPolygon().getID()) == false)) {
			tempFront = tempBack;
			tempBack = tempFront.getNextNode();
		}
		if (tempBack == null)
			return null;
		if (doesIDMatch (polygonToCheck.getID (), tempBack.getPolygon ().getID ()) == true)
			return tempBack;
		return null;
	}



	/* 
	 * isEmpty tells us if this list is empty
	 * Parameter: none
	 * Return: (bool)
	 *						true if list is empty
	 *						false if list is not empty
	 */
	public bool isEmpty()
	{
		return frontOfList == null;
	}



	/*
	 * getSize will return the current size of the list
	 * Parameter:	none
	 * Return:	(int)
	 * 				the current number of nodes in the list
	 */
	public int getSize()
	{
		return numberOfNodesHeld;
	}



	/*
	 * getNodeAtIndex method will get a node at a certian index in the list
	 * Parameter:	(int)index is the index in the list that the caller wants the node of
	 * Return:		(AIFringeSearchNode)
	 * 					the node that was at the index being passed int
	 */
	public AIFringeSearchNode getNodeAtIndex(int index)
	{
		if (index == 0)
			return frontOfList;
		AIFringeSearchNode temp = frontOfList;
		for (int count = 0; count < index; count++)
			temp = temp.getNextNode ();
		return temp;
	}


	//used for debug
	public void printList()
	{
		AIFringeSearchNode temp;
		temp = frontOfList;
		while (temp != null) {
			temp = temp.getNextNode ();
		}
	}




	/*
	 * popNode will remove a node from the front of the the list
	 * Parameter:	none
	 * Return:	(AIFringeSearchNode)
	 * 				the node that was removed from the front of the list
	 */
	public AIFringeSearchNode popNode()
	{
		AIFringeSearchNode temp = frontOfList;
		frontOfList = temp.getNextNode ();
		numberOfNodesHeld--;
		return temp;
	}



	/*
	 * pushAtTheEnd method will take a node and push it at the back of the lsit
	 * parameter:	(AIFringeSearchNode)nodeToPush is the node the caller wants at the back of the list
	 * Return:	none
	 */
	public void pushAtTheEnd(AIFringeSearchNode nodeToPush)
	{

		if (frontOfList == null)
		{
			nodeToPush.setNextNode (frontOfList);
			frontOfList = nodeToPush;
			numberOfNodesHeld++;
			return;
		}
		AIFringeSearchNode tempNode = getNodeAtIndex (getSize () - 1);
		nodeToPush.setNextNode (tempNode.getNextNode());
		tempNode.setNextNode (nodeToPush);
		numberOfNodesHeld++;
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
