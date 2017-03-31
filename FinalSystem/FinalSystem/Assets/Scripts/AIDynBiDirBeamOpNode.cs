/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIDynBiDirBeamOpNode class
 * This class defines a node to be used by the AIDynBiDirBeamOpSearch. It will be able to calculate the total
 * 		cost for this node, it will use a dynamic weight to get the total cost
 */
using UnityEngine;
using System.Collections;

public class AIDynBiDirBeamOpNode
{

	AIPolygon polygonBeingHeld; //polygon this node holds
	AIDynBiDirBeamOpNode nextNode; //nextNode 
	AIDynBiDirBeamOpNode parentNode; //parent of this node
	float gFromStartingNode; // Path cost from start node to current node
	float hCostToGoal; // Heuristic: straight line distance from current node's centroid to goal
	float fTotalCost; // f = g + h
	int nodesExpandedForThis; //number of nodes expanded to get to this node
	float N; //Hcost from the starting node divided by the nodes expanded
	Vector3 goalPosition;
	int epsion = 4; // used for the weight
	
	
	/* AIDynBiDirBeamOpNode method is a constructor for this class.
	 * Parmeters:	(AIPolygon) polygonToAdd is the polygon held by this node
	 * 				(AIDynBiDirBeamOpNode) parentToAdd is the parent of this node
	 * 				(float) gCostToAdd is the gFromStartingNode value for this node
	 * 				(Vector3) goalPositionToAdd is the goalPosition for this instance
	 * 				(flaot)HToAdd is the H value from the starting node
	 * 				(int) nodesExpandedToAdd is how many nodes were expanded to get to this nod
	 */
	public AIDynBiDirBeamOpNode(AIPolygon polygonToAdd, AIDynBiDirBeamOpNode parentToAdd, float gCostToAdd, Vector3 goalPositionToAdd, float HToAdd, int nodesExpandedToAdd)
	{
		polygonBeingHeld = polygonToAdd;
		parentNode = parentToAdd;
		gFromStartingNode = gCostToAdd;
		nextNode = null;
		goalPosition = new Vector3 (goalPositionToAdd.x, goalPositionToAdd.y, goalPositionToAdd.z);
		N = HToAdd / AINavigationMeshAgent.polygonLengthMin;
		nodesExpandedForThis = nodesExpandedToAdd;
		calculateCost (gFromStartingNode, goalPosition);
		
	}
	
	
	
	
	/* calculateCost calculates a new fTotalCost value for this node using the dynamic weighting
	 * Parameters:	(float) gNewFromStartingNode is the new gNewFromStartingNode cost to be used in calculating the new f value
	 * 				(Vector3) goalPosition is the goal position to be used in calculating new f value
	 * Return: none
	 */
	public void calculateCost(float gNewFromStartingNode, Vector3 goalPosition)
	{
		float W; // is the weight to be added to the huristic
		if (nodesExpandedForThis <= N)
			W = 1 - (nodesExpandedForThis / N);
		else
			W = 0f;
		gFromStartingNode = gNewFromStartingNode;
		hCostToGoal = (goalPosition - polygonBeingHeld.getCenterVector()).magnitude;
		fTotalCost = gFromStartingNode + (1 + (epsion * W)) * hCostToGoal;
	}
	
	/* setNextNode sets a new nextNode value for the current node
	 * Parameter: (AAIDynBiDirBeamOpNode) newNextNode is the node to be set as this instances nextNode
	 * Return: none
	 */
	public void setNextNode(AIDynBiDirBeamOpNode newNextNode)
	{
		nextNode = newNextNode;
	}
	
	/* setParentNode sets the parent node for this instance
	 * Parameter: (AIDynBiDirBeamOpNode) newParentNode is the node to be set as this instance's parent node
	 * Return: none
	 */
	public void setParentNode(AIDynBiDirBeamOpNode newParentNode)
	{
		parentNode = newParentNode;
	}
	
	/* getTotalCost returns the fTotalCost of this node
	 * Parameter: none
	 * Return: fTotalCost
	 */
	public float getTotalCost()
	{
		return fTotalCost;
	}
	
	/* getNextNode returns the node pointed to by nextNode of this instance
	 * Parameter: none
	 * Return: (AIDynBiDirBeamOpNode) nextNode
	 */
	public AIDynBiDirBeamOpNode getNextNode()
	{
		return nextNode;
	}
	
	/* getParentNode returns the parent node of this node
	 * Parameter: none
	 * Return: (AIDynBiDirBeamOpNode) parentNode is the parent of this instance
	 */
	public AIDynBiDirBeamOpNode getParentNode()
	{
		return parentNode;
	}
	
	/* compareTo compares this nodes fTotalCost to that of another node
	 * Parameter: (float) fCostToCompare is the value to compare against this nodes fTotalCost
	 * Return: (float)
	 *			greater than zero if this nodes fTotalCost is higher than cost being compared to
	 * 			less than zero if this nodes fTotalCost is lower than cost being compared to
	 */
	public float compareTo(float fCostToCompare)
	{
		return fTotalCost - fCostToCompare;
	}
	
	/* compareToG compares this nodes gFromStartingNode to that of another node
	 * Parameter: (float) gCostToCompare is the value to compare against this nodes gFromStartingNode
	 * Return: (float)
	 *						greater than zero if this nodes gFromStartingNode is higher than cost being compared to
	 * 						less than zero if this nodes gFromStartingNode is lower than cost being compared to
	 */
	public float compareToG(float gCostToCompare)
	{
		return gFromStartingNode - gCostToCompare;
	}
	
	/* getPolygon returns AIPolygon that is held by this node
	 * Parameter: none
	 * Return: (AIPolygon) polygonBeingHeld is the polygon being held by this node
	 */
	public AIPolygon getPolygon()
	{
		return polygonBeingHeld;
	}
	
	/* getGFromStartingNode returns the getGFromStartingNode value of this node
	 * Parameter: none
	 * Return: (float) getGFromStartingNode
	 */
	public float getGFromStartingNode()
	{
		return gFromStartingNode;
	}
	
	
	
	/*
	 * getDoFN will get the number of nodes that have been expanded to get to this node
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes that have been expanded to get to this node
	 */
	public int getDoFN()
	{
		return nodesExpandedForThis;
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
 * 
 */