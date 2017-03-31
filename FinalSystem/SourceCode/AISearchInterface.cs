/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AISearchInterface interface
 * This is the interface the search classes must implement.
 */
using UnityEngine;
using System.Collections;

interface AISearchInterface{


	/*
	 * StartSearch method must be implemented and will run the search
	 * Parameters:	none
	 * Return:	none
	 */
	void startSearch();



	/*
	 * getFinalPathLength method must be implemented and will return the length of the final
	 * 		path
	 * Parameter:	none
	 * Return:	(int) 
	 * 				the length of the final path
	 */
	int getFinalPathLength ();


	/*
	 * getFinalPathCost method must be implemented and will return the cost of the final path
	 * Paramter:	none
	 * Return:	(float)
	 * 				the cost of the final path
	 */
	float getFinalPathCost();


	/*
	 * getNodesVisited method must be implemented and will return the nodes visited by the search
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes visited
	 */
	int getNodesVisited ();



	/*
	 * getMaxQueueSize method must be implemented and will return the max number of nodes in the queue
	 * 			at one time
	 * Parameter:	none
	 * Return:	(int)
	 * 				the max numnber of nodes that were in the queue at once
	 */
	int getMaxQueueSize ();



	/*
	 * getFinalSolution method must be implemented and will return the final path
	 * Parameter:	none
	 * Return:	(AIPolygon[])
	 * 				an array containing the final path polygons
	 */
	AIPolygon[] getFinalSolution();
}
