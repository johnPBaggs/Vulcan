/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIPolygonHolder class
 * This class will act as a holder for Polygons. These polygons will be on a certian plain.
 * 		The polygons will also be from one type of GameObject. These GameObjects could be surface GameObjects
 * 		or obstacle GameObjects. This class will be able to create a polygon with a Vector3[] of vertices, it
 * 		will be able to delete a polygon that is no longer needed, it will be able to ask polygons to check
 * 		for their neighbors, and it will be able to tell polygons to merge.
 */
using UnityEngine;
using System.Collections;


public class AIPolygonHolder {
	AIPolygon[] polygonArray; //this array will hold the polygons for this plain
	int polygonsHeld; //this is the number of polygonsHeld by this object
	AIPolygonQueue mergeQueue; //this is the reference to the Queue that will hold the Merged Polygons
	float polygonLength;

	/*
	 *	AIPolygonHolder is the constructor for this class. It will set up the AIPolygon array, number
	 *			of polygons being held, and the mergedQueue to the initial state
	 *	Parameter:	(int)numberOfPolygons is the number of polygons that this object will initially hold
	 *	Return:	None
	 */
	public AIPolygonHolder( int numberOfPolygons )
	{
		polygonArray = new AIPolygon[numberOfPolygons];
		polygonsHeld = 0;
		mergeQueue = new AIPolygonQueue ();
	}


	/*
	 * AIPolygonHolder is the constructor for this class. It has nothing to set up
	 */
	public AIPolygonHolder()
	{

	}


	/*
	 * setAll method will set up all the variables needed for this class, is used when the data is read from a file
	 * Parameters:	(AIPolygon[]) polygonsToAdd is the polygons this holder will hold
	 * 				(int)polygonCount is the number of polygons being held
	 * Return:	none
	 */
	public void setAll(AIPolygon[] polygonsToAdd, int polygonCount)
	{
		polygonArray = polygonsToAdd;
		polygonsHeld = polygonCount;
	}


	/*
	 * getPolygonsHeld will return the number of polygons that is being held by this object
	 * Parameter:	None
	 * Return:	(int)
	 * 				the number of polygons Held in the polygonArray array
	 */
	public int getPolygonsHeld()
	{
		return polygonArray.Length;
	}


	/*
	 * getPolygonvectorsAtindex will return a Vector3[] of a polygon at a certian index inside the polygonArray
	 * Parameter:	(int)index will be the index inside the polygonArray in which the caller wants
	 * Return:		(Vector3[])
	 * 					The vertices that make up the polygon held at the index which the caller passed inside
	 * 						polygonArray
	 * 					null if the element at index of the polygonArray is set to null
	 */
	public Vector3[] getPolygonVectorsAtindex(int index)
	{
		if (polygonArray [index] != null)
			return polygonArray [index].getVertices ();
		else
			return null;
	}


	/*
	 * addPolygon will make a new polygon using a Vector3[] called vertices. It will add it to the array polygonArray
	 * 		and then it will check to see if any new neighbors can be made with this new polygon
	 * Parameter:	(Vector3[])vertices is the vertices array that will be given to a new polygon object to be used
	 * 					as its vertices
	 * Return:	None
	 */
	public void addPolygon(Vector3[] vertices)
	{
		Vector3[] temp = new Vector3[vertices.Length];
		for (int count = 0; count < vertices.Length; count++) {
			temp [count] = new Vector3 (vertices [count].x, vertices [count].y, vertices [count].z); //copy the vertices points into a new Vector3
		}
		polygonArray [polygonsHeld] = new AIPolygon (temp, polygonsHeld); //make the new AIPolygon
		checkNeighbors (); //checks the Neighbors to see if a polygon needs to add this polygon as a neighbor
		polygonsHeld++;
	}


	/*
	 * checkNeighbors will check every polygon in polygonArray to see if they have a neighbor that is also in the array
	 * 		The neighbor will be checked using isNeighbor from the polygon object
	 * Parameters:	None
	 * Return:	None
	 */
	void checkNeighbors()
	{
		int position = polygonsHeld;
		for (int count = 0; count < polygonsHeld; count ++) {
			if (count != position) {
				if (polygonArray [position].isNeighbor (polygonArray [count])) {
					polygonArray [position].addNeighbor (count); //add new neighbor to the polygon
					polygonArray [count].addNeighbor (position); //add new neighbor to the polygon
				}
			}
		}
	}


	/*
	 * checkAllNeighbors method will look at every AIPolygon being held in this object to see
	 *		what neighbors will each AIPolygon have. This will call a function isNeighbor to
	 *		found is two AIPolygons are neighbors
	 *	Parameters:	None
	 *	Return:	None
	 */
	void checkAllNeighbors(int functionToChoose)
	{
		if (functionToChoose == 1)
			checkAllNeighbors1 ();
		else
			checkALlNeighbors2 ();
	}



	/*
	 * checkAllNeighbors1 method will look at all the polygons and see if they are neighbors by sharing two vertices
	 * Parameter:	none
	 * Return:	none
	 */
	void checkAllNeighbors1()
	{
		for (int firstPosition = 0; firstPosition < polygonArray.Length; firstPosition++) {
			for (int secondPosition = firstPosition + 1; secondPosition < polygonArray.Length; secondPosition++) {
				if (polygonArray [firstPosition].isNeighbor (polygonArray [secondPosition]) == true) { //checks to see if two AIPolygons are neighbors
					polygonArray [firstPosition].addNeighbor (secondPosition); // adds the second AIPolygon's index number to the first
					polygonArray [secondPosition].addNeighbor (firstPosition); // add the first AIPolygon's index number to the second
				}
			}
		}
	}



	/*
	 * checkAllNeighbors2 method will loook at all the polygons and see if they are neighbors by all the neighbor checks
	 * Parameter:	none
	 * Return:	none
	 */
	void checkALlNeighbors2()
	{
		for (int firstPosition = 0; firstPosition < polygonArray.Length; firstPosition++) {
			for (int secondPosition = firstPosition + 1; secondPosition < polygonArray.Length; secondPosition++) {
				if (polygonArray [firstPosition].isNeighbor2 (polygonArray [secondPosition]) == true) { //checks to see if two AIPolygons are neighbors
					polygonArray [firstPosition].addNeighbor (secondPosition); // adds the second AIPolygon's index number to the first
					polygonArray [secondPosition].addNeighbor (firstPosition); // add the first AIPolygon's index number to the second
				}
			}
		}
	}

		/*
	 * drawPolygons will tell each polygon that is in the polygonArray to draw itself
	 * Parameter:	None
	 * Return:	None
	 */
	public void drawPolygons()
	{

		for (int count = 0; count < polygonArray.Length; count++) {
			if(polygonArray[count] != null)
			{
				polygonArray[count].drawSelf(Color.magenta);
			}
		}
	}




	/*
	 * merge will attemp to merge all the polygons that are able to merge inside the array...for information to be added
	 * Parameter:	None
	 * Return:	None
	 */
	public void merge()
	{
		int count = 0;
		setUpQueue ();
		AIPolygon tempNode;
		while (mergeQueue.isEmpty() == false) {
			tempNode = mergeQueue.dequeue ();
			mergeWithNeighbors (tempNode); // merge a polygon with its Neighbors
			count++;
		}
		copyAndSrink (polygonArray);
	}


	/*
	 *	deleteNeighborAtIndex method will delete the neighbor information from all polygos being held that
	 *		are neighbors with the AIPolygon at the index being passed in
	 *	Parameter:	(int)neighborIndex is the index at which the AIPolygon is that needs to be removed
	 *										from the other AIPolygons neighbor list
	 *	Return:		None
	 */
	void deleteNeighborAtIndex(int neighborIndex)
	{
		int[] neighborArray = polygonArray [neighborIndex].getNeighbors ();
		for (int count = 0; count < neighborArray.Length; count++)
			polygonArray [neighborArray [count]].deleteNeighbor (neighborIndex, neighborArray[count]); // ask a polygon to Remove the polygon Wanting to be removed from its neighbor's list
	}


	/*
	 *	checkAndAddNeighborAtIndex method will check and add neighbors to the index polygon wanting to
	 *			have itself added to the neighbor list for all the AIPolygons that are neighbors with this
	 *			AIPolygons
	 *	Parameter:	(int)neighborToBeIndex is the index at which the AIPolygon is being held
	 *	Return:	None
	 */
	void checkAndAddNeighborAtIndex(int neighborToBeIndex)
	{
		for(int count = 0; count < polygonArray.Length; count++)
		{
			if(polygonArray[count] != null && count != neighborToBeIndex)
			{
				if(polygonArray[neighborToBeIndex].isNeighbor(polygonArray[count]))// checks to see if This polygon is a neighbor to another
				{
					polygonArray[neighborToBeIndex].addNeighbor(count); //adds a polygon to the neighbor list of the polygon being sent int
					polygonArray[count].addNeighbor(neighborToBeIndex); //adds the polygon index being sent in to the polygon just checked
				}
			}
		}
	}


	/*
	 *	setUpQueue method will set up the mergeQueue with all the AIPolygons currently being held
	 *	Parameter:	None
	 *	Return:	None
	 */
	void setUpQueue()
	{
		for (int count = polygonArray.Length-1; count >= 0; count--)
			mergeQueue.enqueue(polygonArray[count]);
	}


	/*
	 *	mergeWithNeighbors will attempt to to merge all the AIPolygons it can from its neighbor list
	 *	Parameter:	(AIPolygon)polygonToMerge is the polygon that is going to attempt to merge with
	 *											neighbors
	 *	return: None
	 */
	void mergeWithNeighbors(AIPolygon polygonToMerge)
	{
		if (polygonToMerge != null) {
			int[] neighborHolder = polygonToMerge.getNeighbors (); //array hold the indices for the neighbors of polygonToMerge
			for (int count = 0; count < neighborHolder.Length; count++) {
				if (polygonArray [neighborHolder [count]] != null) {
					AIPolygon mergedPolygon = polygonArray [polygonToMerge.getID ()].mergeRegular (polygonArray [neighborHolder [count]], polygonToMerge.getID ());// this will attempt to merge two polygons and create a new AIPolygon
					if (mergedPolygon != null) { //mergedPolygon will be null if the merge was not accepted
						if (mergedPolygon.isPolygonConvex () == true) { // this will check to see if the new AIPolygon is convex or not
							mergeQueue.deleteNodeOfID (neighborHolder [count]); // delete the old polygon from the neighbor list
							deleteNeighborAtIndex (polygonToMerge.getID ());
							deleteNeighborAtIndex (neighborHolder [count]);
							polygonArray [polygonToMerge.getID ()] = mergedPolygon; // add new Polygon to the polygonArray

							polygonArray [neighborHolder [count]] = null; // remove the last polygon that was merged
							checkAndAddNeighborAtIndex (polygonToMerge.getID ()); //finds the neighbors of the new Polygon
							mergeQueue.enqueue (mergedPolygon); // adds the new Polygon to the mergedQueue
							return;
						}
					}
				}
			}
		}
	}


	/*
	 * getPolygonAtIndex will return the polygon as the index being asked for
	 * Parameter:	(int)index is the position the caller wants the polygon at
	 * Return:		(AIPolygon)
	 *								the polygon being held at the index being passed in
	 */
	public AIPolygon getPolygonAtIndex(int index)
	{
		return polygonArray [index];
	}


	/*
	 *	mergePolygonHolder will merge two AIPolygonHolder objects. This will create a new AIPolygonHolder
	 *			place the polygons from each AIPolygonHolder to the new AIPolygonHolder then merge the Polygons
	 *			in the new AIPolygonHolder
	 *	Parameter:	(AIPolygonHolder)holderToMerge is the AIPolygonHolder that will be merged with this object
	 *	Return:			(AIPolygonHolder)
	 *									the new AIPolygonHolder that is the two merged AIPolygonHolders
	 */
	public AIPolygonHolder mergePolygonHolder(AIPolygonHolder holderToMerge)
	{
		copyAndSrink (polygonArray);
		holderToMerge.copyAndSrink (holderToMerge.getPolygons ());
		AIPolygon[] tempArray = mergeTwoArrays (polygonArray, holderToMerge.getPolygons ()); //makes a new AIPolygonHolder with the capacity of both AIPolygonHolders being Merged
		AIPolygonHolder newHolder = new AIPolygonHolder(tempArray.Length); //makes a new AIPolygonHolder with the capacity of both AIPolygonHolders being Merged
		for (int count = 0; count < tempArray.Length; count++)
			newHolder.addPolygon (tempArray [count].getVertices ());
		newHolder.merge (); // merged the polygons in the new AIPolygonHolders
		return newHolder;
	}



	/*
	 * mergeTwoArray method will take two arrays and return a mergedArray of the polygons in them
	 * Parameter:	(AIPolygon[])firstArray is the array that holds the polygons
	 * 				(AIPolygon[])SecondArray is the array that holds the second polygons
	 * Return:		(AIPolygon[])
	 * 					an array that contains all the polygons from both arrays
	 */
	AIPolygon[] mergeTwoArrays(AIPolygon[] firstArray, AIPolygon[] secondArray)
	{
		AIPolygon[] tempArray = new AIPolygon[firstArray.Length + secondArray.Length];
		int counter = 0;
		for (int count = 0; count < firstArray.Length; count++) {
			tempArray [counter] = new AIPolygon (firstArray [count].getVertices (), counter);
			counter++;
		}
		for (int count = 0; count < secondArray.Length; count++) {
			tempArray [counter] = new AIPolygon (secondArray [count].getVertices (), counter);
			counter++;
		}
		return tempArray;
	}


	/*
	 *	seperateEdgesIntoVectors will call each polygon being held and tell them to seperate their edges into vectors
	 *	Parameter:	None
	 *	Return:	None
	 */
	public void seperateEdgesIntoVectors ()
	{
		for (int count = 0; count < polygonArray.Length; count++)
			if (polygonArray [count] != null)
				polygonArray [count].seperateEdgesIntoVectors ();
	}


	/*
	 *	ObjectsToSplitPolygon method will split the polygons being held with the static(non-movable) objects in the game
	 *				level
	 *	Parameter:	(AIObjects[])objectsArray is array of static objects in the game level
	 *	Return:		none
	 */
	public void ObjectsToSplitPolygon (AIObjects[] objectsArray, float agentRadius)
	{
		seperateEdgesIntoVectors ();
		int polygonToSplitCount = 0;
		for (int count = 0; count < objectsArray.Length; count++) {
			doMethodsForSplitting(objectsArray[count], ref polygonToSplitCount);
		}
		copyAndSrink (polygonArray);
		checkAllNeighbors (2); // get the neighbor for each polygon in the new array
		checkHasGoal (); // checks the polygons to see if they have the goal inside them
		checkHasAgent ();

	}



	/*
	 * markPolygonsThatSplitWithObject method make an array that consist of the indices for all the polygons that need to be
	 * 		split by an object
	 * Parameter:	(ref int)polygonToSplitCount is the current list of the polygons that need to be split
	 * 				(AIObject) objectToSplitWith is the object that needs to be split
	 * Return:		(int[])
	 * 					an array of indices that are the polygons that need to be split
	 */
	int[] markPolygonsThatSplitWithObject(ref int polygonToSplitCount, AIObjects objectToSplitWith)
	{
		int[] indicesWithObject = new int[polygonArray.Length];
		polygonToSplitCount = 0;
		for (int count2 = 0; count2 < polygonArray.Length; count2++) {
			indicesWithObject [count2] = 0;
			if (polygonArray [count2].getObjctsThatWillSplit (objectToSplitWith) == true) {
				indicesWithObject [count2] = 1;
				polygonToSplitCount++;
			}
		}
		return indicesWithObject;
	}



	/*
	 * getPolygonsThatWillBeSplit method will gather all the polygons that are going to be split
	 * Parameter:	(int[]) indicesWithObject is the array of indices that hold the polygons that need to be split
	 * 				(int) polygonToSplitCount is the number of polygons that are going to be split
	 * Return:		(AIPolygon[])
	 * 					an array of polygons that will be split
	 */
	AIPolygon[] getPolygonsThatWillBeSplit(int[] indicesWithObject, int polygonToSplitCount)
	{
		AIPolygon[] splitingPolygons = new AIPolygon[polygonToSplitCount];
		for(int count = 0, count2 = 0; count < polygonArray.Length; count++)
		{
			if(indicesWithObject[count] == 1)
			{
				splitingPolygons[count2] = polygonArray[count];
				count2++;
			}
		}
		return splitingPolygons;
	}


	/*
	 * splitPolygons method will split the polygons that was collected to be split
	 * Parameter:	(AIPolygon[]) splitingPolygons is an array containing the polygons that need to be split
	 * 				(AIObject) objectsToSplitWith is the object that the polygons will be split by
	 */
	AIPolygon[] splitPolygons(AIPolygon[] splitingPolygons, AIObjects objectToSplitWith)
	{
		AIPolygon[] tempArray = null;
		for (int count = 0; count < splitingPolygons.Length; count++) {
			AIPolygonQueue tempQueue = splitingPolygons [count].splitThisPolygon (objectToSplitWith);
			if (tempQueue != null) {
				tempArray = makeNewArray (count, tempQueue, AINavigationMeshAgent.agentDiameter, tempArray);
			}
		}
		int counter = 0;
		if (tempArray == null)
			return tempArray;
		for(int count = 0; count < tempArray.Length; count++)
		    if(tempArray[count] != null)
		    	counter++;
		AIPolygon[] temp2Array = new AIPolygon[counter];
		for(int count = 0, tempCount = 0; count < tempArray.Length; count++){
			if(tempArray[count] != null){
				temp2Array[tempCount] = new AIPolygon(tempArray[count].getVertices(), tempCount);
				tempCount++;
			}
		}

		return temp2Array;
	}



	/*
	 * doMethodsForSplitting method will call all the methods for splitting the the polygons with an object\
	 * Parameter:	(AIObjects) objectToSplitWith is the object that the polygons need to be split by
	 * 				(ref int) polygonToSplitCount  is the number of polygons that need to be split
	 */
	void doMethodsForSplitting(AIObjects objectToSplitWith ,ref int polygonToSplitCount)
	{
		int[] indicesWithObject = markPolygonsThatSplitWithObject(ref polygonToSplitCount, objectToSplitWith);
		AIPolygon[] splitingPolygons = getPolygonsThatWillBeSplit(indicesWithObject, polygonToSplitCount);
		AIPolygon[] tempArray = splitPolygons (splitingPolygons, objectToSplitWith);
		if (tempArray == null)
			return;
		AIPolygon[] newArray = new AIPolygon[tempArray.Length + polygonArray.Length - polygonToSplitCount];
		int newArrayCount = 0;
		for(int count = 0; count < polygonArray.Length; count++)
		{
			if(indicesWithObject[count] != 1)
			{
				newArray[newArrayCount] = new AIPolygon(polygonArray[count].getVertices(), newArrayCount);
				newArrayCount++;
			}
		}
		for(int count = 0; count < tempArray.Length; count++)
		{
			newArray[newArrayCount] = new AIPolygon(tempArray[count].getVertices(), newArrayCount);
			newArrayCount++;
		}
		setNewArray(newArray);
	}



	/*
	 *	makeNewArray method will maek a new array of Polygons from the old array of Polygon and the new
	 *			AIPolygonQueue holding the polygons that were resently split
	 *	Parameter:	(int)placeToAddNewPolygons is the place that the old polygon was at before it was split
	 *							(AIPolygonQueue)tempQueue is the AIPolygonQueue holding the new polygons
	 *	Return:	none
	 */
	AIPolygon[] makeNewArray(int placeToAddNewPolygons, AIPolygonQueue tempQueue, float agentRadius, AIPolygon[] arraySomething)
	{
		AIPolygon[] newArray;
		int placeToAdd;
		if(arraySomething != null)
		{
			 newArray = new AIPolygon[arraySomething.Length + tempQueue.getSize ()]; //new arry of polygons
			placeToAdd = 0;
			for (int count = 0; count < arraySomething.Length; count++) {//add polygons from the old array until the position of the polygon that has been split
				if(polygonArray[count] != null)
				{
					newArray[placeToAdd] = new AIPolygon(arraySomething [count].getVertices (), placeToAdd);
					placeToAdd++;
				}
			}
			for (int count = tempQueue.getSize() - 1; count >= 0; count--) {//add polygons that is in tempQueue
				newArray [placeToAdd] = new AIPolygon (tempQueue.getPolygonAtIndex (count).getVertices (), placeToAdd);
				placeToAdd++;
			}
			return newArray;
		}
		newArray = new AIPolygon[tempQueue.getSize ()];
		placeToAdd = 0;
		for (int count = tempQueue.getSize() - 1; count >= 0 ; count--) {//add polygons that is in tempQueue
			newArray [placeToAdd] = new AIPolygon (getVerties(tempQueue.getPolygonAtIndex (count).getVertices ()), placeToAdd);
			placeToAdd++;
		}
		return newArray;
	}



	/*
	 * checkHasGoal method will have each polygon in the array to look and see if they have the goal inside them
	 * Parameter: none
	 * return:	none
	 */
	void checkHasGoal()
	{
		for (int count = 0; count < polygonArray.Length; count++)
			if (polygonArray [count].shootLinesAndFindGoal () == true) { //check to see if a polygon is holding the goal
			return;
			}
	}



	/*
	 * checkHasAgent method will have each polygon in the array to look and see if they have the agent inside them
	 * Parameter:	none
	 * return:	none
	 */
	void checkHasAgent()
	{
		for (int count = 0; count < polygonArray.Length; count++)
			if (polygonArray [count].shootLinesAndFindAgent () == true) { //check to see if a polygon is holding the agent
			//return;
			}
	}



	/*
	 * printPolygon method will print out the vetices of the polygons being held
	 */
	void printPolygon()
	{
		Debug.Log(Time.realtimeSinceStartup + " printPolygon polygonArray.length = " + polygonArray.Length);
		for(int count = 0; count < polygonArray.Length; count++)
		{
			if(polygonArray[count] != null)
			{
				for(int count2 = 0; count2 < polygonArray[count].getVerticesCount(); count2++)
				{
					Debug.Log("count = " + count + " count2 = " + count2 + " vertex = " + polygonArray[count].getVectorAtIndex(count2));
				}
			}
		}
	}



	/*
	 *	getVerties will make a new array of of vertices out of an array of vertices to be used by the caller
	 *	Parameter:	(Vector3[])vertices is the array of Vector3 that needs to be copied and sent back
	 *	Return:			(Vector3[])
	 *								the new array that is a copy of the array sent into it
	 */
	Vector3[] getVerties( Vector3[] vertices)
	{
		Vector3[] newVertices = new Vector3[vertices.Length];
		for (int count = 0; count < newVertices.Length; count++)
			newVertices [count] = new Vector3 (vertices [count].x, vertices [count].y, vertices [count].z);
		return newVertices;
	}




	/*
	 *	setNewArray will take in an array of AIPolygon and the number of polygons added to that array and
	 *			make a new array then set polygonArray to the new array
	 *	Parameter:	(AIPolygon[])newArray is the array of polygons that needs to be added to a new array
	 *							(int)numberOfPolygons is the number of polygons that is being held by the array that
	 *									has been passed in
	 */
	void setNewArray(AIPolygon[] newArrays)
	{
		AIPolygon[] tempArray = new AIPolygon[newArrays.Length];
		for (int count = 0; count < tempArray.Length; count++) {
			tempArray [count] = newArrays [count];
		}
		polygonArray = tempArray;
	}




	/*
	 * getPolygons will return to the caller the polygons that are being held
	 *	Parameter:	none
	 *	Return:	(AIPolygon[])
	 *							the array of polygons being held by this object
	 */
	public AIPolygon[] getPolygons()
	{
		return polygonArray;
	}



	/*
	 * copyAndSrink method will take a a array and take all the polygons that are not null and place them into a new array
	 * Parameter:	(AIPolygon[[]) polygonArrayToCopy is the array that needs to be shrunk
	 * Return:	none
	 */
	public void copyAndSrink(AIPolygon[] polygonArrayToCopy)
	{
		int counter = 0;
		for(int count = 0; count < polygonArrayToCopy.Length; count++)
			if(polygonArray[count] != null)
				counter++;
		AIPolygon[] tempArray = new AIPolygon[counter];

		for (int count = 0, tempCounter = 0; count < polygonArrayToCopy.Length; count++) {
			if (polygonArrayToCopy [count] != null) {
				tempArray [tempCounter] = new AIPolygon (polygonArrayToCopy [count].getVertices (), tempCounter);
				tempCounter++;
			}
		}
		polygonArray = tempArray;
		checkHasAgent ();
		checkHasGoal ();
	}



	/*
	 * getAveragePolygonLength method will get the average polygon length and then return the value
	 * Parameter:	none
	 * Return:	(float)
	 * 				the average length of the polygons
	 */
	public float getAveragePolygonLength()
	{
		polygonLength = 0f;
		for (int count = 0; count < polygonArray.Length; count++)
			polygonLength += polygonArray [count].maxLength ();
		polygonLength /= polygonArray.Length;
		return polygonLength;
	}



	/*
	 * getMinPolygonLength will get the smallest length of a polygon in the navigations mesh
	 * Parameter:	none
	 * Return:	(float)
	 * 				the smallest length of the polygon in the navigation mesh
	 */
	public float getMinPolygonLength()
	{
		polygonLength = float.MaxValue;
		float tempFloat;
		for (int count = 0; count < polygonArray.Length; count++) {
			tempFloat = polygonArray [count].maxLength ();
			if (tempFloat < polygonLength)
				polygonLength = tempFloat;
		}
		return polygonLength;
	}



	/*
	 * getMaxPolygonLength method will return the max length of a polygon in the navigation mesh
	 * Parameter:	none
	 * Return:	(float)
	 * 				the max length of the polygon in the navigation mesh
	 */
	public float getMaxPolygonLength()
	{
		polygonLength = 0f;
		float tempFloat;
		for (int count = 0; count < polygonArray.Length; count++) {
			tempFloat = polygonArray [count].maxLength ();
			if (tempFloat > polygonLength)
				polygonLength = tempFloat;
		}
		return polygonLength;

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
