/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIAgentAStarSearchList class
 * This class is used for the open and closed lists used by the AIAgentAStarSearch class. It has the ability to add a node,
 * 		delete a node, see if a node is already on the list, and pop a node off. 
 */

using UnityEngine;
using System.Collections;

public class AIAgentAStarSearchList
{
	AIAgentAStarSearchNode frontOfList; // references the front of the lists
	int numberOfNodesHeld; // keeps track of number of nodes in list
	Vector3 goalPosition; // stores the goal position



	/* AIAgentAStarSearchList method is a constructor for this class and will set up the variables to the initial values
	 * Parameter: (Vector3) goalPositionToAdd
	 */
	public AIAgentAStarSearchList(Vector3 goalPositionToAdd)
	{
		frontOfList = null;
		numberOfNodesHeld = 0;
		goalPosition = goalPositionToAdd;
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
		if (frontOfList == null) {
			newSearchNode.setNextNode (frontOfList);
			frontOfList = newSearchNode;
			numberOfNodesHeld++;
			return;
		}
		AIAgentAStarSearchNode tempFront = frontOfList;
		if (tempFront.compareTo (newSearchNode.getTotalCost ()) > 0f) {
			newSearchNode.setNextNode (tempFront);
			frontOfList = newSearchNode;
			numberOfNodesHeld++;
			return;
		}
		
		AIAgentAStarSearchNode tempBack = tempFront.getNextNode ();
		while(tempBack != null && tempBack.compareTo(newSearchNode.getTotalCost()) < 0f)
		{
			tempFront = tempBack;
			tempBack = tempFront.getNextNode();
		}
		newSearchNode.setNextNode (tempFront.getNextNode());
		tempFront.setNextNode (newSearchNode);
		numberOfNodesHeld++;
	}




	/* printList prints out the ID's of the polygons in the list
	 * Parameter: none
	 * Return: none
	 */
	public void printList()
	{
		AIAgentAStarSearchNode temp;
		temp = frontOfList;
		while (temp != null) {
			temp = temp.getNextNode ();
		}
	}




	/* popNode removes and returns the node at the front of the list (node with lowest fToatlCost value)
	 * Parameter: none
	 * Return: (AIAgentAStarSearchNode)
	 *				node that was removed from the front of this list
	 */
	public AIAgentAStarSearchNode popNode()
	{
		if (frontOfList == null)
			return null;
		AIAgentAStarSearchNode tempNode = frontOfList;
		frontOfList = tempNode.getNextNode ();
		numberOfNodesHeld--;
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
		AIAgentAStarSearchNode tempFront = frontOfList;
		if(frontOfList != null)
		{
			if(doesIDMatch(frontOfList.getPolygon().getID(), idToDelete) == true)
			{
				frontOfList = frontOfList.getNextNode();
				numberOfNodesHeld--;
				return;
			}
			AIAgentAStarSearchNode tempback = tempFront.getNextNode();
			while((tempback.getNextNode() != null) && (doesIDMatch(tempback.getPolygon().getID(), idToDelete) == false))
			{
				tempFront = tempback;
				tempback = tempFront.getNextNode();
			}
			if(doesIDMatch(tempback.getPolygon().getID(), idToDelete) == true)
			{
				tempFront.setNextNode(tempback.getNextNode());
			}
		}
		numberOfNodesHeld--;
		
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
		AIAgentAStarSearchNode tempFront = frontOfList;
		if (tempFront == null)
			return false;
		if (doesIDMatch (polygonToCheck.getID (), tempFront.getPolygon ().getID ()) == true)
			return true;
		AIAgentAStarSearchNode tempBack = tempFront.getNextNode ();
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
	 * Parameter: (AIPolygon) polygonToCheck is the polygon being held by the node to be returned
	 * Return: (AIAgentAStarSearchNode)
	 *														node on list that is holding polygonToCheck
	 */
	public AIAgentAStarSearchNode getNodeOnList(AIPolygon polygonToCheck)
	{
		AIAgentAStarSearchNode tempFront = frontOfList;
		if (tempFront == null)
			return null;
		if (doesIDMatch (polygonToCheck.getID (), tempFront.getPolygon ().getID ()) == true)
			return tempFront;
		AIAgentAStarSearchNode tempBack = tempFront.getNextNode ();
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



	/* isEmpty tells us if this list is empty
	 * Parameter: none
	 * Return: (bool)
	 *						true if list is empty
	 *						false if list is not empty
	 */
	public bool isEmpty()
	{
		return frontOfList == null;
	}



	/* enqueue adds a node to the front of this list
	 * Parameter: (AIAgentAStarSearchNode) nodeToAdd is the node to be added to the front of the list
	 * Return: none
	 */
	public void enqueue(AIAgentAStarSearchNode nodeToAdd)
	{
		AIAgentAStarSearchNode newNode = new AIAgentAStarSearchNode (nodeToAdd.getPolygon (), nodeToAdd.getParentNode (), nodeToAdd.getGFromStartingNode (), goalPosition);
		newNode.setNextNode (frontOfList);
		frontOfList = newNode;
	}



	/*
	 * getSize method will return the number of nodes in the list currently
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes in the list currently
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
