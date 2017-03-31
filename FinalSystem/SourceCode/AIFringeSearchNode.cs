/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIFringeSearchNode class
 * This class will define a node to be used for the fringe search
 */

using UnityEngine;
using System.Collections;

public class AIFringeSearchNode
{

	AIPolygon polygonBeingHeld;
	AIFringeSearchNode nextNode;
	AIFringeSearchNode parentNode;
	bool childrenPlaced;
	float gFromStartingNode;
	float hCostToGoal;
	float fTotalCost;
	Vector3 goalPosition;

  /* The method AIFringeSearchNode is a contructor for the class
	 * Parameter: AIPolygon polygonToAdd
	 * AIFringeSearchNode parentToAdd
	 * float gCostToAdd
	 * Vector3 goalPositionToAdd
	 */
	public AIFringeSearchNode(AIPolygon polygonToAdd, AIFringeSearchNode parentToAdd, float gCostToAdd, Vector3 goalPositionToAdd)
	{
		polygonBeingHeld = polygonToAdd;
		parentNode = parentToAdd;
		gFromStartingNode = gCostToAdd;
		nextNode = null;
		childrenPlaced = false;
		goalPosition = new Vector3 (goalPositionToAdd.x, goalPositionToAdd.y, goalPositionToAdd.z);
		calculateCost (gFromStartingNode, goalPosition);

	}

  /* The method calculateCost calcluates the new cost from the passed parameters
	 * Paremeter: float gNewFromStartingNode - g cost from start node
	 * Vector3 goalPosition - position of the goal
	 */
	public void calculateCost(float gNewFromStartingNode, Vector3 goalPosition)
	{
		gFromStartingNode = gNewFromStartingNode;
		hCostToGoal = (goalPosition - polygonBeingHeld.getCenterVector()).magnitude;
		fTotalCost = gFromStartingNode + hCostToGoal;
	}

  /* The method setNextNode sets the next node of the current node
	 * AIFringeSearchNode newNextNode - node to be set as next
	 */
	public void setNextNode(AIFringeSearchNode newNextNode)
	{
		nextNode = newNextNode;
	}

  /* The method setParentNode sets the parent node of the curretn node
   * Parameter: AIFringeSearchNode newParentNode - node to be set as parent	 *
	 */
	public void setParentNode(AIFringeSearchNode newParentNode)
	{
		parentNode = newParentNode;
	}

  /* The method getTotalCost returns the f cost of the node
	 * Return: float
   * 								f cost of current node
	 */
	public float getTotalCost()
	{
		return fTotalCost;
	}

  /* The method getHCost returns the H cost of the current node
	 * Return: float
	 *							the h cost of the curretn node
	 */
	public float getHCost()
	{
		return hCostToGoal;
	}

  /* The method getNextNode returns the next node of the current node
	 * Return: AIFringeSearchNode
	 * 														next node of the current node
	 */
	public AIFringeSearchNode getNextNode()
	{
		return nextNode;
	}

  /* The method getParentNode returns the parent node of the current node
   * Return: AIFringeSearchNode
	 *															parent node of current node
	 */
	public AIFringeSearchNode getParentNode()
	{
		return parentNode;
	}

  /* The method compareTo compares this node cost to the fCostToCompare
	 * Parameter: float fCostToCompare - cost to compare nodes cost to
	 * Return: float
	 * 								negative if fCostToCompare is greater, positive if fTotalCost is greater, or 0 if equal
	  */
	public float compareTo(float fCostToCompare)
	{
		return fTotalCost - fCostToCompare;
	}

  /* The method compareToG compares the current nodes g cost to gCostToCompare
	 * Parameter: float gCostToCompare - g cost to compare to the current node's g cost
	 * Return: float
	 * 								negative if gCostToCompare is greater, positive if gFromStartingNode is greater, or 0 if equal
	 */
	public float compareToG(float gCostToCompare)
	{
		return gFromStartingNode - gCostToCompare;
	}

  /* The method getPolygon returns the polygon being held by the current node
	 * Return: AIPolygon
	 * 										polygonBeingHeld by current node
	 */
	public AIPolygon getPolygon()
	{
		return polygonBeingHeld;
	}

  /* The method getGFromStartNode returns gFromStartingNode from current node
	 * Return: float
	 *							gFromStartingNode of current node
	 */
	public float getGFromStartNode()
	{
		return gFromStartingNode;
	}

  /* The method setChildrenPlaced sets value of childrenPlaced
	 * Parameter: bool changeValue - value to set childrenPlaced to
	 *
	 */
	public void setChildrenPlaced(bool changeValue)
	{
		childrenPlaced = changeValue;
	}

  /* The method hasChildrenBeenPlaced returns value of childrenPlaced
	 * Return: bool
	 *							childrenPlaced
	 */
	public bool hasChildrenBeenPlaced()
	{
		return childrenPlaced;
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
