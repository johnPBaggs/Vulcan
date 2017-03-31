/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIBinarySpaceTree class
 * This class will act as a Binary Space Partition tree that will assest the calling classes with subdividing a polygon
 * 		with a gameObject. This class can set up a binary tree, subdivid a polygon making two new polygons, decide when to stop
 * 		subdividing, and return a Queue of only free polygons
 */
using UnityEngine;
using System.Collections;

public class AIBinarySpaceTree {

	AIBinaryTreeNode rootNode; //the starting point for the Binary Space Partition Tree
	Vector3[,] arrayOfEdges; //array of edges with no Y coordinate changes
    Vector3[] edgeVertices;
	int indexForEdges; //idk why ujs
	bool[] usedEdgesForObject;
	Vector3[] verticeToMakePolygonFrom;
	int verticesTomakePolygonFromCount;
	Vector3[] verticesLeft;
	Vector3[] verticesRight;
	int indexCount;


	/*
	 * AIBinarySpaceTree method is the constructor for this class. It will set up all the variables to their initial state.
	 * Parameters:	(GameObject)objectToAdd will be the object that the polygons are going to be split with
	 * 				(Vector3[,])arrayOfEdgesToAdd  will be the array of edges with no Y coordinate change for objectToAdd
	 * 				(AIPolygon)polygonToAdd is the starting polygon that will be the in the rootNode of the tree
	 * 				(AIPolygonQueue)polygonQueueToAdd is the reference to the Queue that the free polygons will be added to
	 * Return:	none
	 */
	public AIBinarySpaceTree(GameObject objectToAdd, Vector3[,] arrayOfEdgesToAdd, Vector3[] arrayOfEdgeVerticesAdd, AIPolygon polygonToAdd, AIPolygonQueue polygonQueueToAdd)
	{
		rootNode = new AIBinaryTreeNode (polygonToAdd);
		arrayOfEdges = arrayOfEdgesToAdd;
		edgeVertices = getArrayOfEdgeVertices ();
		indexForEdges = 0;
		indexCount = 0;
		usedEdgesForObject = new bool[arrayOfEdges.Length / 2];
		for (int count = 0; count < usedEdgesForObject.Length; count++)
			usedEdgesForObject [count] = false;
	}

	/* The method getArrayOfEdgeVertices constructs an array of edge vertices from the polygons
	 * Return: Vector3[]
	 *									 array of edge vertices
	 */
	Vector3[] getArrayOfEdgeVertices()
	{
		Vector3[] tempArray = new Vector3[arrayOfEdges.Length / 2];
		tempArray[0] = new Vector3( arrayOfEdges[0,0].x, arrayOfEdges[0,0].y, arrayOfEdges[0,0].z);
		tempArray[1] = new Vector3( arrayOfEdges[0,1].x, arrayOfEdges[0,1].y, arrayOfEdges[0,1].z);
		bool flag = true;
		for (int count = 1, position = 2; count < tempArray.Length && position < tempArray.Length; count++) {
			for (int count2 = 0; count2 < position && position < tempArray.Length; count2++) {
				if (AIPolygon.checkVertices (tempArray [count2], arrayOfEdges [count, 0]) == true) {
					flag = false;
				}
			}
			if(flag == true)
			{
				tempArray [position] = new Vector3 (arrayOfEdges [count, 0].x, arrayOfEdges [count, 0].y, arrayOfEdges [count, 0].z);
				position++;
			}
			flag = true;
			for (int count2 = 0; count2 < position && position < tempArray.Length; count2++) {
				if (AIPolygon.checkVertices (tempArray [count2], arrayOfEdges [count, 1]) == true) {
					flag = false;
				}
			}
			if(flag == true)
			{
				tempArray [position] = new Vector3 (arrayOfEdges [count, 1].x, arrayOfEdges [count, 1].y, arrayOfEdges [count, 1].z);
				position++;
			}
			flag = false;
		}
		return seperateEdgesIntoVectors(tempArray);
	}

  /* The method seperateEdgesIntoVectors seperates edges into vectors to be used later for a check
	 * Parameter: Vector3[] tempEdges - array of edges
	 * Return: Vector3[] array of new vectors from edge array
	 */
	public Vector3[] seperateEdgesIntoVectors (Vector3[] tempEdges)
	{

		Vector3[] tempArray;
		tempArray = new Vector3[(tempEdges.Length * 30) + tempEdges.Length];
		int tempCount = 0;
		for (int count = 0, position = 1; count < tempEdges.Length; count++, position++) {
			if (position >= tempEdges.Length)
				position = 0;
			tempArray[tempCount] = new Vector3(tempEdges[count].x, tempEdges[count].y, tempEdges[count].z);
			tempCount++;
			getPointsOnAnEdge(tempEdges[count], tempEdges[position], tempArray, ref tempCount);
		}
		return tempArray;
	}


	/*
	 * getPointsOnAnEdge will get all the points needed to be seperated between two vertices
	 *	Parameter:	(vector3)startPosition is the start position of the edge
	 *							(vector3)endPosition si the end position of the edge
	 */
	void getPointsOnAnEdge(Vector3 startPosition, Vector3 endPosition, Vector3[] tempArray, ref int tempCount)
	{
		float xDifferenceToAdd = getDistanceToAdd (startPosition.x, endPosition.x);
		float yDifferenceToAdd = getDistanceToAdd (startPosition.y, endPosition.y);
		float zDifferenceToAdd = getDistanceToAdd (startPosition.z, endPosition.z);
		for (int count = 0; count < 30; count++) {
			tempArray[tempCount] = new Vector3 (startPosition.x + (count * xDifferenceToAdd), startPosition.y + (count * yDifferenceToAdd), startPosition.z + (count * zDifferenceToAdd));
			tempCount++;
		}
	}

  /* The method of getDistanceToAdd returns the difference between startCoordinate
	 * and endCoordinate
	 * Parameter: float startCoordinate - coordinate to find difference with
	 * float endCoordinate - coordinate to find difference with
	 * Return: float
	 *								difference of startCoordinate and endCoordinate divided by (30 - 1)
	 */
	float getDistanceToAdd(float startCordinate, float endCordinate)
	{
		float difference;
		if (startCordinate > endCordinate)
			difference = startCordinate - endCordinate;
		else
			difference = endCordinate - startCordinate;
		if (difference == 0f)
			return difference;
		if (difference < 0f)
			difference *= -1;
		if (startCordinate > endCordinate)
			difference *= -1;
		return (difference/(30-1));
	}


	/*
	 * AIBinaryTreeNode method will return the rootNode to the caller
	 * Parameter:	none
	 * Return:	(AIBinaryTreeNode)
	 * 				the rootNode for this binary Space Partition Tree
	 */
	public AIBinaryTreeNode getRootNode()
	{
		return rootNode;
	}


	//public void trySubdivide(

	public AIBinaryTreeNode subDivide2(AIBinaryTreeNode polygonNode, ref int idCounter)
	{
		if (polygonNode == null)
			return null;
		if (polygonNode.checkIfTerminated (arrayOfEdges) == true)
			return null;
		if (polygonNode.checkIfFree (edgeVertices) == true)
			return polygonNode;
		bool polygonSplitSuccessful = tryGetTwoPolygons (polygonNode, ref idCounter);
		if (polygonSplitSuccessful == true) {
			indexForEdges++;
			if (subDivide2 (polygonNode.leftNode, ref idCounter) != null) { //recusively calls on the leftNode child
				polygonNode.leftNode.setToFree ();
			}
			if (subDivide2 (polygonNode.rightNode, ref idCounter) != null) { //recusively calls on the RightNode child
				polygonNode.rightNode.setToFree ();
			}
		}

		return null;
	}

  /* The method getIndexToUseForEdges returns the index to use in the tryGetTwoPolygons
	 * Parameters: bool[] indexArrayOfBools
	 * Return: int
	 *							index to use or -1 if no valid index is found
	 */
	int getIndexToUseForEdges(bool[] indexArrayOfBools)
	{
		for (int count = 0; count < indexArrayOfBools.Length; count++)
			if (indexArrayOfBools [count] == false) {
				return count;
			}
		return -1;
	}

  /* The method of tryGetTwoPolygons attempts to merge two polygons
	 * Parameter: AIBinaryTreeNode polygonNode, ref int idCounter
	 * Return: bool
	 *							true if merge was successful, false otherwise
	 */
	bool tryGetTwoPolygons(AIBinaryTreeNode polygonNode, ref int idCounter)
	{
		int[] indexArrayToUse = getAllFalseIndices ();
		bool[] indexArrayOfBools = getABoolArrayOfFalses (indexArrayToUse.Length);
		int indexToUse = -1;

		indexArrayToUse = getAllFalseIndices ();
		indexArrayOfBools = getABoolArrayOfFalses (indexArrayToUse.Length);
		indexToUse = -1;
		while (allIndicesAreTrue(indexArrayOfBools) == false) {
			indexToUse = -1;
			indexToUse = getIndexToUseForEdges (indexArrayOfBools);
			if (indexToUse == -1)
				return false;
			for (int count = 0; count < polygonNode.getPolygon().getVerticesCount(); count++) {
				int secondPolygonStartingIndex = getTwoPolygons (polygonNode.getPolygon (), indexArrayToUse [indexToUse], count); // seperates the one polygon into two and returns the stating index of the second polygon
				if (secondPolygonStartingIndex == -1)
				{
					indexArrayOfBools [indexToUse] = true;
				}
				else {
					if(makeNewPolygons (polygonNode, secondPolygonStartingIndex, polygonNode.getPolygon ().getVertices (), ref idCounter) == true)// makes two new polygons from the seperated one
					{
						usedEdgesForObject [indexArrayToUse [indexToUse]] = true;
							indexCount++;
						return true;
					}
				}
			}
		indexArrayOfBools [indexToUse] = true;
		}
		return false;
	}

  /* The method getTwoPolygons a.k.a. "The Beast" contructs two polygons from one.
	 * Parameter: AIpolygon polygonToSplit - polygon that will be attempted to be split
	 * int indexToUse - index of vetex to start split from
	 * int startPosition -
	 * Return: int
	 *							returns -1 if not possible, index to start next iteration from
	 */
	int getTwoPolygons(AIPolygon polygonToSplit, int indexToUse, int startPosition)
	{
		verticeToMakePolygonFrom = new Vector3[polygonToSplit.getVertices ().Length * 3];
		Vector3[] polygonVertices = polygonToSplit.getVertices ();
		Vector3 connecting;
		Vector3 notReal = new Vector3 (1000f, 1000f, 1000f);
		int cutTimes = 0;
		int secondPositionStart = -1;
		verticesTomakePolygonFromCount = 0;
		int position = startPosition;
		int count = 0;
		verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (polygonVertices [position].x, polygonVertices [position].y, polygonVertices [position].z);
		verticesTomakePolygonFromCount++;
		position++;
		position = position % polygonVertices.Length;
		count++;
		if (indexToUse < 0 || indexToUse >= arrayOfEdges.Length / 2)
			return -1;
		connecting = polygonToSplit.getIntersectingPoint (arrayOfEdges [indexToUse, 0], arrayOfEdges [indexToUse, 1], verticeToMakePolygonFrom [verticesTomakePolygonFromCount - 1], polygonVertices [position]);
		while ((count < polygonVertices.Length) && (AIPolygon.checkVertices(connecting, notReal) == true)) { //chekcs to see if an intersecting point was found yet if not continue to add old vertices to first new polygon
			verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (polygonVertices [position].x, polygonVertices [position].y, polygonVertices [position].z);
			verticesTomakePolygonFromCount++;
			position++;
			count++;
			if (position >= polygonVertices.Length)
				position = 0;
			connecting = polygonToSplit.getIntersectingPoint (arrayOfEdges [indexToUse, 0], arrayOfEdges [indexToUse, 1], verticeToMakePolygonFrom [verticesTomakePolygonFromCount - 1], polygonVertices [position]);
		}
		if (AIPolygon.checkVertices(connecting, notReal) == false) { //checks to see if the inersecting point was a valid connection and if so add it to both the new polygons
			cutTimes++;
			verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (connecting.x, connecting.y, connecting.z);
			verticesTomakePolygonFromCount++;
			secondPositionStart = verticesTomakePolygonFromCount;
			verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (connecting.x, connecting.y, connecting.z);

			verticesTomakePolygonFromCount++;
			verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (polygonVertices [position].x, polygonVertices [position].y, polygonVertices [position].z);
			verticesTomakePolygonFromCount++;
		}
		position++;
		count++;
		if (position >= polygonVertices.Length)
			position = 0;
		connecting = polygonToSplit.getIntersectingPoint (arrayOfEdges [indexToUse, 0], arrayOfEdges [indexToUse, 1], verticeToMakePolygonFrom [verticesTomakePolygonFromCount - 1], polygonVertices [position]);
		while((count < polygonVertices.Length ) && (AIPolygon.checkVertices(connecting, notReal) == true)) { //checks to see if an intersecting pont was found yet if not continue to add old vertices to the second new polygon
			verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (polygonVertices [position].x, polygonVertices [position].y, polygonVertices [position].z);
			verticesTomakePolygonFromCount++;
			position++;
			count++;
			if (position >= polygonVertices.Length)
				position = 0;
			connecting = polygonToSplit.getIntersectingPoint (arrayOfEdges [indexToUse, 0], arrayOfEdges [indexToUse, 1], verticeToMakePolygonFrom [verticesTomakePolygonFromCount - 1], polygonVertices [position]);
		}
		if (AIPolygon.checkVertices (connecting, notReal) == false) { //checks to see if the intersecting point was a valid connection and if so add it to both the new polygons
			cutTimes++;
			verticeToMakePolygonFrom [verticesTomakePolygonFromCount] = new Vector3 (connecting.x, connecting.y, connecting.z);
			verticesTomakePolygonFromCount++;
		}
		if (count != polygonVertices.Length || cutTimes != 2)
			return -1;
		return secondPositionStart;
	}

  /* The method allIndicesAreTrue determines if all elements in the bool array are true
	 * Parameter: bool[] indexArrayOfBools - array to check
	 * Return: bool
	 *							true if all elements in array are true, false otherwise
	 */
	bool allIndicesAreTrue(bool[] indexArrayOfBools)
	{
		for (int count = 0; count < indexArrayOfBools.Length; count++)
			if (indexArrayOfBools [count] == false)
				return false;
		return true;
	}

  /* The method getABoolArrayOfFalses creates and initializes a bool array of falses
	 * Parameter: int lengthOfArray - length of boolean array to return
	 * Return: bool[] array of falses to return
	 */
	bool[] getABoolArrayOfFalses(int lengthOfArray)
	{
		bool[] tempArray = new bool[lengthOfArray];
		for (int count = 0; count < tempArray.Length; count++)
			tempArray [count] = false;
		return tempArray;
	}

  /* The method getAllFalseIndices returns an integer array containing the indices not used for edges
	 * on an object
	 * Return: int[]
	 *								array indicating the false indices
	 */
	int[] getAllFalseIndices()
	{
		int counter = 0;
		for (int count = 0; count < usedEdgesForObject.Length; count++)
			if (usedEdgesForObject [count] == false)
				counter++;
		int[] tempArray = new int[counter];
		for (int count = 0, tempCount = 0; count < usedEdgesForObject.Length; count++) {
			if (usedEdgesForObject [count] == false) {
				tempArray [tempCount] = count;
				tempCount++;
			}
		}
		return tempArray;
	}








	/*
	 * makeNewPolygons method will take an array of vertices and a position that the second polygon starts and make two new binarySpaceNodes from those verices
	 * Parameters:	(AIBinaryTreeNode)polygonNode is the node that holds the polygon that was logically split. will be used to place the leftNode child and RightNode child
	 * 				(int)secondPolygonStartingIndex is the position inside the polygonToSplit array that the second polygon starts
	 * 				(Vector3[])polygonToSplit is the array that olds the vertices for both the new polygons that are about to be created
	 * 				(ref int)idCounter is the counter of the polygons created so far and is also used to give a polygon an ID at creation
	 * Return:	none
	 */
	bool makeNewPolygons(AIBinaryTreeNode polygonNode, int secondPolygonstartingIndex, Vector3[] polygonToSplit, ref int idCounter)
	{

		Vector3[] firstPolygonVertices = new Vector3[secondPolygonstartingIndex + 1];
		Vector3[] secondPolygonVertices = new Vector3[verticesTomakePolygonFromCount - secondPolygonstartingIndex];
		int count = 0;
		int firstCount = 0;
		for (; firstCount < secondPolygonstartingIndex; firstCount++) { //adds the first polygon's vertices until the start of the second polygon
			firstPolygonVertices [firstCount] = new Vector3 (verticeToMakePolygonFrom [count].x, verticeToMakePolygonFrom [count].y, verticeToMakePolygonFrom [count].z);
			count++;
		}
		for (int secondCount = 0; secondCount < secondPolygonVertices.Length; secondCount++) { //adds the second polygon's vertices until the end of the list
			secondPolygonVertices [secondCount] = new Vector3 (verticeToMakePolygonFrom [count].x, verticeToMakePolygonFrom [count].y, verticeToMakePolygonFrom [count].z);
			count++;
		}
		firstPolygonVertices [firstCount] = new Vector3 (verticeToMakePolygonFrom [count-1].x, verticeToMakePolygonFrom [count-1].y, verticeToMakePolygonFrom [count-1].z); //add the final intersecting point in the array
		if (isAllVerticesDifferent (firstPolygonVertices) == false || isAllVerticesDifferent (secondPolygonVertices) == false) {
			return false;
		}
		polygonNode.leftNode = new AIBinaryTreeNode(new AIPolygon(firstPolygonVertices, idCounter)); //make a new BinarySpaceNode with the new vertices and sets it to polygonNode's leftNode reference
		idCounter++;
		polygonNode.rightNode = new AIBinaryTreeNode(new AIPolygon(secondPolygonVertices, idCounter));//make a new BinarySpaceNode with the new vertices and sets it to polygonNode's rightNode reference
		idCounter++;
		return true;
	}

  /* The method isAllVerticesDifferent checks if all the vertices in the Vector3[] vericesToCheck array are
	 * different
	 * Parameters: Vector3[] verticesToCheck - array of vertices to be checked
	 * Return: bool
	 *							true if the vertices are all different, false otherwise
	 */
	bool isAllVerticesDifferent(Vector3[] verticesToCheck)
	{
		for (int count1 = 0; count1 < verticesToCheck.Length; count1++)
			for (int count2 = count1 + 1; count2 < verticesToCheck.Length; count2++)
				if (AIPolygon.checkVertices (verticesToCheck [count1], verticesToCheck [count2]) == true)
					return false;
		return true;
	}


	/*
	 * getFreeNodes method will call a postOrder traversal of the tree created to get all the Free Polygons of the tree
	 * Parameters: none
	 * Return:	none
	 */
	public AIPolygonQueue getFreeNodes ()
	{
		AIPolygonQueue tempQueue = new AIPolygonQueue ();
		postOrderTraversal (rootNode, tempQueue);
		return tempQueue;
	}



	/*
	 * postOrderTraversal method will recursivly traverse the Binary Space Partition tree and enqueue any Free polygons
	 * 		it finds into the AIPolygonQueue in a postOrder order
	 * Parameter:	(AIBinaryTreeNode)nodeToCheck is the current node that needs to be checked and then added into the Queue
	 * Reurn:	none
	 */
	void postOrderTraversal(AIBinaryTreeNode nodeToCheck, AIPolygonQueue tempQueue)
	{

		if (nodeToCheck != null) {
			postOrderTraversal (nodeToCheck.leftNode, tempQueue); //recursive call on the LeftNode child
			postOrderTraversal (nodeToCheck.rightNode, tempQueue); //recursivecall on the RightNode child
			placeNodeInQueue (nodeToCheck,tempQueue); //check to see if the polygon is free and if so add it to the Queue
		}

	}



	/*
	 * placeNodeInQueue method will look at a node to see if its polygon is free and if so it will add the polygon being held
	 * 		to a AIPolygonQueue
	 * Parameter:	(AIBinaryTreeNode)nodeToPlace is the node that needs its polygon check to see if its free
	 * Return:	none
	 */
	void placeNodeInQueue(AIBinaryTreeNode nodeToPlace, AIPolygonQueue tempQueue)
	{

		if(nodeToPlace.isFree() == true)
		{
			tempQueue.enqueue(nodeToPlace.getPolygon());
		}
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
