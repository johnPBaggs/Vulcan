/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIFringeSearch class
 * This class will implement a fringe A* search. The fringe search is a combination of IDA* and A*.
 * 		It goes though array holding the polygons from the navigation mesh certian levels at a time, 
 * 		but it starts off at the beginning every time.
 */

using UnityEngine;
using System.Collections;

public class AIFringeSearch : AISearchInterface
{

	AIFringeSearchList fringeList; //the list used for this search
	AIFringeSearchNode[] cache; //array used to see if a polygon has already been looked at
	float fLimit; //the depth limit for the search
	float fMin; //what the next depth limit will be set to
	bool goalFound; //if the goal is found
	AIPolygon[] polygonArray; //the array of polygons from the navigation mesh
	AIFringeSearchNode finalSolutionStart; //the starting node for the final solution
	Vector3 goalPosition; //the position of the goal
	Vector3 startPosition; //the starting position
	int maxQueueSize;
	float finalPathCost;
	int nodesVisited;
	int polygonFinalCount;
	AIPolygon[] finalSolutionArray; //the array holding the final solution
	AIPolygon startingPolygon; //the starting polygon
	bool firstMin; 



	/*
	 * AIFringeSearch's constructor will set up the initial values for the search to start
	 * Parameter:	(Vector3)goalToAdd is the goal's position
	 * 				(AIPolygon[]) polygonArrayToAdd is the array of polygons that the navigation mesh added
	 */
	public AIFringeSearch(Vector3 goalToAdd, AIPolygon[] polygonArrayToAdd)
	{
		polygonArray = polygonArrayToAdd;
		goalPosition = goalToAdd;
		cache = new AIFringeSearchNode[polygonArray.Length];
		polygonFinalCount = 0;
		maxQueueSize = 0;
		nodesVisited = 0;
		finalPathCost = 0f;
		goalFound = false;
	}



	/*
	 * StartSearch method will start the fringe search. It will get the first polygon that the search starts in and then
	 * 		it will start the search
	 * Parameter:	none
	 * Return:	none
	 */
	public void startSearch()
	{
		fringeList = new AIFringeSearchList (goalPosition);
		for (int count = 0; count < polygonArray.Length; count++) // looks for the polygon with the agent GameObject inside it
		if (polygonArray [count].getHasAgent () == true && polygonArray[count].getAgentID() == 1) {
			fringeList.enqueue (polygonArray [count], null, 0); //adds the first polygon to the openList
			cache[count] = fringeList.getNodeOnList(polygonArray[count]);
			startingPolygon = polygonArray[count];
			count = polygonArray.Length;
		}
		fringeSearch(); //does the AStar search
		printFinalSolution (); //prints the finalSolution for debugging
		addFinalSolutionPolygons (); //adds the finalsolution to the finalsolution array
	}



	/*
	 * fringeSearch method will preform the fringe search
	 * Parameter:	none
	 * Return:	none
	 */
	void fringeSearch()
	{
		AIFringeSearchNode tempNode = fringeList.getNodeOnList (startingPolygon); // the first polygon in the list
		fLimit = tempNode.getHCost (); //the first limit
		AIFringeSearchList curList = fringeList; //the current list that will be looked at 
		AIFringeSearchList nextList = new AIFringeSearchList (goalPosition); //the list that the not good polygons will be put into
		while (goalFound == false && (curList.isEmpty() == false || nextList.isEmpty() == false)) { 
			fMin = float.MaxValue; //reset the fMin
			firstMin = false; //reset the firstMin
			while (curList.isEmpty() == false ) { //goes until the current list is empty
				tempNode = curList.popNode (); 
				nodesVisited++;
				if (tempNode.getTotalCost () > fLimit) { // the node is no good and the limit must be incresed
					fMin = getMin (tempNode.getTotalCost (), fMin);
					nextList.pushAtTheEnd(tempNode); //adds the node at the end of the nextList
					continue;
				}
				if(tempNode.getPolygon().getHasGoal() == true) //goal node was found
				{
					finalSolutionStart = tempNode;
					return;
				}
				for(int count = tempNode.getPolygon().getNeighborsHeld() - 1; count >= 0; count--)
				{
					AIFringeSearchNode childNode = new AIFringeSearchNode(polygonArray[tempNode.getPolygon().getNeighborAt(count)], tempNode,(tempNode.getGFromStartNode() + (tempNode.getPolygon().getCenterVector() - polygonArray[tempNode.getPolygon().getNeighborAt(count)].getCenterVector()).magnitude), goalPosition);
					if(tempNode.getParentNode() == null || tempNode.getParentNode().getPolygon().getID() != childNode.getPolygon().getID()) //checks to see if the parentNode is the child node trying to be added in
					{
						if(cache[tempNode.getPolygon().getNeighborAt(count)] != null)//checks to see if the polygon has already been seen
						{
							if(childNode.getGFromStartNode() >= cache[tempNode.getPolygon().getNeighborAt(count)].getGFromStartNode())
								continue;
						}
						if(curList.isNodeOnList(childNode.getPolygon()) == true) //checks to see if the polygon is already on the list
						{
							curList.deleteNodeOfId(childNode.getPolygon().getID()); //deletes the node
						}
						curList.enqueue(childNode.getPolygon(), childNode.getParentNode(), childNode.getGFromStartNode()); //adds the node at the front of the current list
						cache[tempNode.getPolygon().getNeighborAt(count)] = childNode;
					}
				}
			if(curList.getSize() > maxQueueSize)
					maxQueueSize = curList.getSize();

			}
			fLimit = fMin; //sets the new Min
			
			AIFringeSearchList tempList = curList;
			curList = nextList;
			nextList = tempList;
		}
	}



	/*
	 * getMin function will return the minimal value if this is the first time getMin has been called
	 * 		if not then it return the maximum value between the two numbers being passed in
	 * Parameter:	(float)firstNumber is the first number being checked by this function
	 * 				(float)SecondNumber is the second number being checked by this function
	 * Return:		(float)
	 * 					the minimual value if this  is the first Time this method has been called this time through the search
	 * 					the maximum value if this is not the first time this method has been called this time through the search
	 */
	float getMin(float firstNumber, float secondNumber)
	{
		if (firstMin == false) {
			firstMin = true;
			if (firstNumber < secondNumber)
				return firstNumber;
			return secondNumber;
		} else {
			if (firstNumber > secondNumber)
				return firstNumber;
			return secondNumber;
		}
	}



	//used for debugging
	void printFinalSolution()
	{
		printFinalSolutionRecursively (finalSolutionStart);
	}

	//used for debugging
	void printFinalSolutionRecursively(AIFringeSearchNode currentNode)
	{
		if (currentNode != null) {
			printFinalSolutionRecursively (currentNode.getParentNode ());
			polygonFinalCount++;
		}
	}



	/*
	 * addFinalSolutionPolygons method will call a funtion that recusivly look at the polygons in the Finalsolution 
	 * 		and adds them from the starting node to the ending node to the finalSolution array
	 * Parameters:	none
	 * Return:	none
	 */
	void addFinalSolutionPolygons()
	{
		finalSolutionArray = new AIPolygon[polygonFinalCount];
		int counter = 0;
		addFinalSolutionPolygons (finalSolutionStart, ref counter); //calls a recusive method to get the FinalSolution
	}



	/*
	 * addFinalsolutionPolygons method is a recusive method that will look at the parent of each node passed in until
	 * 		the node passed in is null, then it will add each Node's polygon to the finalSolutionArray in order
	 * 		from start until end
	 * Parameter:	(AIFringeSearchNode)currentNode is the node that needs to be checked and then have its parent passed
	 * 				(ref int)counter is the current count of polygons in the FinalSolution array used to access the next index
	 * Return:	none
	 */
	void addFinalSolutionPolygons (AIFringeSearchNode  currentNode, ref int counter)
	{
		if (currentNode != null) {
			addFinalSolutionPolygons (currentNode.getParentNode (), ref counter);
			finalSolutionArray [counter] = currentNode.getPolygon ();
			if(counter != 0)
				finalPathCost += (finalSolutionArray[counter].getCenterVector() - finalSolutionArray[counter - 1].getCenterVector()).magnitude;
			counter++;
		}
	}


	
	/*
	 * getFinalPathLength will return the length of the final Path array
	 * Parameters:	none
	 * Return:	(int)
	 * 				the length of the final path array
	 */
	public int getFinalPathLength()
	{
		return finalSolutionArray.Length;
	}
	

	/*
	 * getFinalPathost will return the cost of the final path in units
	 * Parameters:	none
	 * Return:	(float)
	 * 				the cost of the final path
	 */
	public float getFinalPathCost()
	{
		return finalPathCost;
	}


	/*
	 * getNodesVisited method will return the number of nodes visited by the search
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of nodes visited by the search
	 */
	public int getNodesVisited()
	{
		return nodesVisited;
	}
	
	
	
	/*
	 * getMaxQueueSize method will return the max size the queue was during the search
	 * Parameter:	none
	 * Return:	(int)
	 * 				the max size the queue was during the search
	 */
	public int getMaxQueueSize()
	{
		return maxQueueSize;
	}
	
	
	
	/*
	 * getFinalSolution method will return the final path solutiona polygons
	 * Parameter:	none
	 * Return:	(AIPolygon[])
	 * 				the final solution path polygons
	 */
	public AIPolygon[] getFinalSolution()
	{
		return finalSolutionArray;
	}

}
