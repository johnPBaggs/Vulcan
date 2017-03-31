/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIBinaryTreeNode class
 * This class will act as a node for a binary tree. It will hold a polygon, will have a reference to leftNode
 * 		, will have a reference to the right node, will have the ability to be able to tell if a polygon is terminated
 * 		or free, and have the ability to return the polygon
 */
using UnityEngine;
using System.Collections;

public class AIBinaryTreeNode {

	bool free; //variable that tells if the polygon is free
	bool terminated; //variable that tells if the polygon is terminated
	AIPolygon polygonBeingHeld; //the polygon this node is holding
	public AIBinaryTreeNode leftNode; //reference to the leftNode child
	public AIBinaryTreeNode rightNode; //reference to the rightNode child


	/*
	 * AIBinaryTreeNode method is the constructor for this class. It will set up all the variables to their initial
	 * 		state
	 * Parameter:	(AIPolygon)polygonToAdd is the polygon that this node will hold
	 * Return:	none
	 */
	public AIBinaryTreeNode(AIPolygon polygonToAdd)
	{
		polygonBeingHeld = polygonToAdd;
		free = false;
		leftNode = null;
		rightNode = null;
	}


	/*
	 * getPolygon method will return the polygon that is being held by this instance
	 * Parameters:	none
	 * Return:	(AIPolygon)
	 * 				the polygon that is being held by this instance
	 */
	public AIPolygon getPolygon()
	{
		return polygonBeingHeld;
	}


	/*
	 * setToFree method will set the free variable to true
	 * Parameter:	none
	 * Return:	none
	 */
	public void setToFree()
	{
		free = true;
	}


	/*
	 * isFree method will return wether or not this polygon is freee
	 * Parameters:	none
	 * Return:	(bool)
	 * 				true if this polygon is free
	 * 				false if the polygon is free
	 */
	public bool isFree()
	{
		return free;
	}


	/*
	 * checkIfFree method will call the polygon that is being held to ask if it is free of the GameObject to check
	 * Parameter:	(GameObject)objectToCheck is the GameObject that the polygon will check if it is free of
	 * Return:		(bool)
	 * 					true if the polygon is free of objectToCheck
	 * 					false if the polygon is not free of the objectToCheck
	 */
	public bool checkIfFree(Vector3[] objectVertices)
	{
		return polygonBeingHeld.isFree(objectVertices);

	}


	/*
	 * checkIfTerminated method will call the polygon that is being held to ask if it is terminated with the GameObject to check
	 * Paramter: (Vector3[,])arrayOfEdges is the array of edges with no Y coordinate change of the gameObject that will be used to
	 * 				check if the polygon is terminated or not
	 * Return:	 (bool)
	 * 				true if the polygon should be terminated with the GameObject
	 * 				false if the polygon should not be terminated with the GameObejct
	 */
	public bool checkIfTerminated(Vector3[,] arrayOfEdges)
	{
		return polygonBeingHeld.isTerminated(arrayOfEdges);
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