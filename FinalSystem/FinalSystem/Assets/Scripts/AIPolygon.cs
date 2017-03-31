/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIPolygon class
 * This class conceptually will make a polygon. It will be able to merge two polygons,
 * 		find if a polygon is a neighbor (has two shared vertices), and so far check is it is
 * 		a convex polygon or not
 */

using UnityEngine;
using System.Collections;

public class AIPolygon {
	
	Vector3[] vertices; //this holds the vertices for this polygon
	Vector3[] scaledVertices;
	int[] neighborIndices; // this holds the neighbors of this polygon
	int neighborsHeld; // this indicates how many neighbors this polygon has
	public int id; // the index in the AIPolygonHolder this polygon is placed
	Vector3[] edgeVectors; // an array of Vector3s that are the vectors on the edges of this polygon
	int edgeVectorsCount;  //number of edge vectors being held
	int MAXEDGECOUNT = 31; // max number to split each edge
	AIObjects[] objectsToSplitThis; // an array that olds objects that are in this polygon
	Vector3[,] objectToSplitVertices; //an array of edges that make up an object that is in the polygon
	Vector3 pointToDraw;
	AIPolygonQueue freePolygonQueue; // a queue that will hold only the free polygons after a split
	bool hasGoal; // variable to see if this polygon has the goal in it
	bool hasAgent; // variable to see if this polygon has the agent in it
	int AgentID;
	public static int agentCount = 0;
	string objectString;



	/*
	 * AIPolygon is the constructor for this class that will set the initial vertices for the polygon,
	 *		neighbors that this polygon has, and the freePolygonQueue
	 *	Parameters:		(Vector3[])vertices is the vertiecs that make up this polygon initially
	 *					(int)ID is the place in the AIPolygonHolder that this polygon is placed
	 *	Return:	none
	 */
	public AIPolygon(Vector3[] vertices, int ID)
	{
		this.vertices = vertices;
		scaledVertices = this.scaleWholePolygon (.95f);
		this.neighborIndices = new int[1];
		this.neighborsHeld = 0;
		freePolygonQueue = null;
		id = ID;
	}




	/*
	 * AIPolygon is the constructor that only has to set the AIPolygonQueue to null
	 */
	public AIPolygon()
	{
		freePolygonQueue = null;
	}



	/*
	 * setData method will set all the data that the polygon needs to be effective. All the data is read from the file
	 * Parameters:	(Vector3[[])verticesToAdd is the vertices that will make up this polygon
	 * 				(int)idToAdd is the id for this polygon
	 * 				(int)neighborsHeldToAdd the number of neighbors that will be held
	 * 				(int[])neighborsToAdd is the neighbor array to be added
	 * 				(bool)hasGoalToAdd is if this polygon has the goal
	 * 				(bool)hasAgentToAdd is if this polygon has the agent
	 * Return:	none
	 */
	public void setData(Vector3[] verticesToAdd, int idToAdd, int neighborsHeldToAdd, int[] neighborsToAdd, bool hasGoalToAdd, bool hasAgentToAdd, int agentIdToAdd)
	{
		vertices = verticesToAdd;
		id = idToAdd;
		neighborsHeld = neighborsHeldToAdd;
		neighborIndices = neighborsToAdd;
		hasGoal = hasGoalToAdd;
		hasAgent = hasAgentToAdd;
		AgentID = agentIdToAdd;

	}
	
	/* getVertices returns the Vector3[] that holds the vertices for this polygon
	 * Parameters: None
	 * Return:	(Vector3[]) vertices that make up this polygon
	 */
	public Vector3[] getVertices()
	{
		return vertices;
	}
	
	/* drawSelf will draw the edges between vertices. It will draw by using debug to
	 * 	draw a line.
	 * Parameter:	(Color)color is the color that the caller wants the polygon to be
	 * 					drawn as
	 * Return:	none
	 */
	public void drawSelf(Color color)
	{
		for (int count = 0; count < vertices.Length; count++) {
			if (count == (vertices.Length - 1)) { //makes the wrap around happen
				Debug.DrawLine (vertices [count], vertices [0], color); //draws a line that can be seen in the game mode and in scene mode
			} else {
				Debug.DrawLine (vertices [count], vertices [count + 1], color);
			}
		}
	}
	
	
	/*
	 * getVerticesCount will return the number of vertices this polygons has
	 * Parameter:	None
	 * Return:	(int)vertices.Length is the size of the Vector3[] vertices
	 * 				the size of the array will always be the number of vertices
	 * 				for the polygon
	 */
	public int getVerticesCount()
	{
		return vertices.Length;
	}
	
	
	/*
	 * getVectorAtIndex will return a vertex that is at a certain index inside the array vertices
	 * Parameter:	(int)index is the place in the array that the caller wants the vertex
	 * Return:		(Vector3)vertices[index] is the vertex(Vector3) at the position of index in the array vertices
	 */
	public Vector3 getVectorAtIndex(int index)
	{
		return vertices[index];
	}



	//checkVertices(Vector3 thisPolygonVertex, Vector3 polygonToMergeVertex)
	/*
	 * isNeighbor is a function that will check to see two polygons are neighbors by comparing
	 * 		two vertices. If the polygon it wants to check has two vertices that have the same
	 * 		position as two vertices on this polygon it will consider the polygon to check a
	 * 		neighbor
	 * Parameter:	(AIPolygon)polygonToCheck is the polygon that this polygon will check to
	 * 					see if the two are neighbors
	 * Return:		(bool)
	 * 					true if the two polygons are neighbors by the standards outlined
	 * 					false if the two polygons are not neighbors by the standards outlined
	 */
	public bool isNeighbor(AIPolygon polygonToCheck)
	{
		if (checkTwoSharedEdges (polygonToCheck) == true)
			return true;
		return false;
	}



	/* The method isNeighbor2 determines if the parameter is a neighbor to the current node
	 * Parameter: AIPolygon polygonToCheck - polygon to check if neighbor
  	 * Return: bool
	 *							true if neighbors, false if not neighbors
	 */
	public bool isNeighbor2(AIPolygon polygonToCheck)
	{
		if (checkTwoSharedEdges (polygonToCheck) == true)
		{
			return true;
		}
		if (checkOneSharedEdgeAndPointInbetweenLine (polygonToCheck) == true)
		{
			return true;
		}
		if (checkTwoPointsInbetweenALine (polygonToCheck) == true)
		{
			return true;
		}
		if (checkOneVertexInbetweenALineAndTheSecondIsAlso (polygonToCheck) == true) {
			return true;
		}
		return false;
	}
	


	/*
	 *	checkTwoSharedEdges method will look at this polygon and another polygon to see if they both shar two edges
	 *			meaning they are neighbors
	 *	Parameters:	(AIPolygon)polygonToCheck is the polygon that the caller wants to check to see if its a neighbor
	 *										of this polygon
	 *	Return:			(bool)
	 *									true if this polygon and polygonToCheck is neighbors
	 *									false if this polygon and polygonToCheck are not neighbors
	 */
	public bool checkTwoSharedEdges(AIPolygon polygonToCheck)
	{
		int vertexMatchCount = 0;
		for (int thisCount = 0; thisCount < vertices.Length; thisCount++) { //goes for the amout of vertices this polygon has
			for (int toCheckCount = 0; toCheckCount < polygonToCheck.getVerticesCount(); toCheckCount++) { //goes for the amout of vertices polygonToCheck has
				if (checkVertices (vertices[thisCount], polygonToCheck.getVectorAtIndex(toCheckCount)) == true)
					vertexMatchCount++; //increments if all three coordiant points are the same
			}
		}
		if (vertexMatchCount >= 2)
			return true;
		return false;
	}
	


	/*
	 *	checkTwoSharedEdgesMidpoint method will look to see if this polygon and polygonToCheck have two
	 *			shared edges and if so will return the midpoint of the edge
	 *	Parameters:	(AIPolygon)polygonToCheck is the polygon that the caller wants to check against this polygon
	 *	Return:			(Vector3)
	 *								the midpoint of the edge that was found to be shared
	 *								(1000f, 1000f, 1000f) if no edge is shared
	 */
	public Vector3 checkTwoSharedEdgesMidpoint(AIPolygon polygonToCheck)
	{
		int vertexMatchCount = 0;
		int vertexMatch = -1;
		for (int thisCount = 0; thisCount < vertices.Length; thisCount++) { //goes for the amout of vertices this polygon has
			for (int toCheckCount = 0; toCheckCount < polygonToCheck.getVerticesCount(); toCheckCount++) { //goes for the amout of vertices polygonToCheck has
				if (checkVertices (vertices[thisCount], polygonToCheck.getVectorAtIndex(toCheckCount)) == true)
				{
					vertexMatchCount++; //increments if all three coordiant points are the same
					if(vertexMatchCount >= 2)
					{
						Vector3 temp = (vertices[thisCount] + vertices[vertexMatch]);
						return new Vector3(temp.x/2, 0f, temp.z/2); // returns the midpint of the edge
					}
					else
						vertexMatch = thisCount;
				}
			}
		}
		return new Vector3(1000f, 1000f, 1000f);
	}
	


	/*
	 * checkOneSharedEdgeAndPointInbetweenLineMidpoint method will see if the two polygons have a shared vertex
	 *			and the second vertex is inbetween a line on the edge and return the midpint of the edge
	 *	Parameters:	(AIPolygon)polygonToCheck is the polygon that the caller wants to check as a neighbor
	 *	Return:			(Vector3)
	 *									the midpoint of the edge if the two polygons are neighbors
	 *									(1000f, 1000f, 1000f) if the two polygons are not neighbors
	 */
	public Vector3 checkOneSharedEdgeAndPointInbetweenLineMidpoint(AIPolygon polygonToCheck)
	{
		for (int count = 0; count < vertices.Length; count++) {
			for (int count2 = 0; count2 < polygonToCheck.getVerticesCount(); count2++) {
				if (checkVertices (vertices [count], polygonToCheck.getVectorAtIndex (count2)) == true) {
					Vector3 tempVector = doesPointsAgacentShareAnEdgeV (count, count2, polygonToCheck.getVertices ()); // gets the edge that makes the two polygons neighbors
					return new Vector3(tempVector.x/2, 0f, tempVector.z/2); // returns the midpoint of the edge that makes the two polygons neighbors
				}
			}
		}
		return new Vector3(1000f, 1000f, 1000f);
	}


	
	/*
	 * doesPointsAgacentShareAnEdgeV method will check to see if a shared point coorisponds to another Point
	 *		on the edge of one of the two polygon and if so return the edge points added
	 *	Parameters:	(int)count1 is the point in the first polygon that matches a point in the second
	 *							(int)count2 is the point in the second polygon that matches a point in the first
	 *							(Vector3[]) verticesToCheck is all the vertices that make up the second polygon
	 *	Return:			(Vector3)
	 *								the points that make the two polygons neighbors added
	 *								(1000f, 1000f, 1000f) if no connection making the two polygons neighbors was found
	 */
	Vector3 doesPointsAgacentShareAnEdgeV(int count1, int count2, Vector3[] verticesToCheck)
	{
		int polygonToCheckFirst = getOneBehind (count2, verticesToCheck);
		int polygonToCheckThird = getOneAhead (count2, verticesToCheck);
		int thisFirst = getOneBehind (count1, vertices);
		int thisThird = getOneAhead (count1, vertices);
		
		if (isOnALineBetweenTwoPoints (verticesToCheck [polygonToCheckFirst], verticesToCheck [count2], vertices [thisFirst]) == true)
			return (vertices[count1] + vertices[thisFirst]);
		if (isOnALineBetweenTwoPoints (verticesToCheck [polygonToCheckFirst], verticesToCheck [count2], vertices[thisThird]) == true)
			return (vertices[count1] + vertices[thisThird]);
		if (isOnALineBetweenTwoPoints (verticesToCheck [count2], verticesToCheck [polygonToCheckThird], vertices [thisFirst]) == true)
			return (vertices[count1] + vertices[thisFirst]);
		if (isOnALineBetweenTwoPoints (verticesToCheck [count2], verticesToCheck [polygonToCheckThird], vertices [thisThird]) == true)
			return (vertices[count1] + vertices[thisThird]);
		if (isOnALineBetweenTwoPoints (vertices [thisFirst], vertices [count1], verticesToCheck [polygonToCheckFirst]) == true)
			return (verticesToCheck[count2] + verticesToCheck[polygonToCheckFirst]);
		if (isOnALineBetweenTwoPoints (vertices [thisFirst], vertices [count1], verticesToCheck [polygonToCheckThird]) == true)
			return (verticesToCheck[count2] + verticesToCheck[polygonToCheckThird]);
		if (isOnALineBetweenTwoPoints (vertices [count1], vertices [thisThird], verticesToCheck [polygonToCheckFirst]) == true)
			return (verticesToCheck[count2] + verticesToCheck[polygonToCheckFirst]);
		if (isOnALineBetweenTwoPoints (vertices [count1], vertices [thisThird], verticesToCheck [polygonToCheckThird]) == true)
			return (verticesToCheck[count2] + verticesToCheck[polygonToCheckThird]);
		return new Vector3(1000f, 1000f, 1000f);
	}
	


	/*
	 *	checkTwoPointsInbetweenALineMidpoint will look at the edges of both polygons to see if one edge is inbetween
	 *		an edge on the other polygon and if so return the midpoint of the edge
	 *	Parameter:	(AIPolygon)polygonToCheck is the polygon the caller wishes to find out if is a neighbor of this polygon
	 *	Return:			(Vector3)
	 *									the midpoint of the edge that make the two polygons neighbors
	 *									(1000f, 1000f, 1000f) if no edge that makes the two polygons neighbors was found
	 */
	public Vector3 checkTwoPointsInbetweenALineMidpoint(AIPolygon polygonToCheck)
	{
		Vector3 temp = isTwoPointsOnALinebetweenTwoPointsV(vertices, polygonToCheck.getVertices ());
		Vector3 notReal = new Vector3 (1000f, 1000f, 1000f);
		if (checkVertices (temp, notReal) == false)
			return new Vector3 (temp.x / 2, 0f, temp.z / 2);
		temp = isTwoPointsOnALinebetweenTwoPointsV(polygonToCheck.getVertices (), vertices);
		if (checkVertices (temp, notReal) == false)
			return new Vector3 (temp.x / 2, 0f, temp.z / 2);
		return notReal;
	}
	


	/*
	 * isTwoPointsOnALinebetweenTwoPointsV method will look at two polygons and see if one edge of one polygon is inbetween an each on the other
	 * 		and then return the two points added together
	 * Parameter:	(Vector3[])firstPolygonVertices is the array of edges of the first polygon
	 * 				(Vector3[])secondPolygonVertices is the array of edges of the second polygon
	 * Return:		(Vector3)
	 * 					the two points added together if the polygons are neighbors according to this method of checking
	 * 					(1000f, 1000f, 1000f) is the two polygons are not neighbors according to this method of checking
	 */
	Vector3 isTwoPointsOnALinebetweenTwoPointsV(Vector3[] firstPolygonVertices, Vector3[] secondPolygonVertices)
	{
		for(int firstPos1 = 0, firstPos2 = 1; firstPos1 < firstPolygonVertices.Length; firstPos1++, firstPos2++)
		{
			if(firstPos2 >= firstPolygonVertices.Length)
				firstPos2 = 0;
			for(int secondPos1 = 0, secondPos2 = 1; secondPos1 < secondPolygonVertices.Length; secondPos1++, secondPos2++)
			{
				if(secondPos2 >= secondPolygonVertices.Length)
					secondPos2 = 0;
				if((isOnALineBetweenTwoPoints(firstPolygonVertices[firstPos1], firstPolygonVertices[firstPos2], secondPolygonVertices[secondPos1]) == true) && (isOnALineBetweenTwoPoints(firstPolygonVertices[firstPos1], firstPolygonVertices[firstPos2], secondPolygonVertices[secondPos2]) == true))
					return (secondPolygonVertices[secondPos1] + secondPolygonVertices[secondPos2]);
			}
		}
		return new Vector3 (1000f, 1000f, 1000f);
	}



	/* The method checkOneVertexInbetweenALineAndTheSecondIsAlsoMidPoint checks if a vertex is in between two points
	 * and if it is returns the midpoint
	 * Parameter: AIPolygon polygonToCheck - polygon to check against current node
	 * Return: Vector3
	 *								midpoint of the line vertex is in between, (1000f, 1000f, 1000f)
	 */
	Vector3 checkOneVertexInbetweenALineAndTheSecondIsAlsoMidPoint (AIPolygon polygonToCheck)
	{
		Vector3[] polygonToCheckVertices = polygonToCheck.getVertices ();
		int firstVertex = -1;
		int secondVertex = -1;
		Vector3 tempVector;
		int indexFound = findVertexInbetweenALine (polygonToCheckVertices, ref firstVertex, ref secondVertex);
		if (indexFound == -1)
			return new Vector3(1000f, 1000f, 1000f);
		int polygonToCheckFirst = getOneBehind (indexFound, polygonToCheckVertices );
		int polygonToCheckThird = getOneAhead (indexFound, polygonToCheckVertices );
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [polygonToCheckFirst], polygonToCheckVertices [indexFound], vertices [firstVertex]) == true) {
			if (Vector3.Distance (polygonToCheckVertices [indexFound], vertices [firstVertex]) >= AINavigationMeshAgent.agentDiameter) {
				tempVector = polygonToCheckVertices [indexFound] + vertices [firstVertex];
				return new Vector3 (tempVector.x / 2, 0f, tempVector.z / 2);
			}
		}
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [polygonToCheckFirst], polygonToCheckVertices [indexFound], vertices [secondVertex]) == true) {
			if (Vector3.Distance (polygonToCheckVertices [indexFound], vertices [secondVertex]) >= AINavigationMeshAgent.agentDiameter) {
				tempVector = polygonToCheckVertices [indexFound] + vertices [secondVertex];
				return new Vector3 (tempVector.x / 2, 0f, tempVector.z / 2);
			}
		}
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [indexFound], polygonToCheckVertices [polygonToCheckThird], vertices [firstVertex]) == true) {
			if (Vector3.Distance (polygonToCheckVertices [indexFound], vertices [firstVertex]) >= AINavigationMeshAgent.agentDiameter) {
				tempVector = polygonToCheckVertices [indexFound] + vertices [firstVertex];
				return new Vector3 (tempVector.x / 2, 0f, tempVector.z / 2);
			}
		}
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [indexFound], polygonToCheckVertices [polygonToCheckThird], vertices [secondVertex]) == true) {
			if (Vector3.Distance (polygonToCheckVertices [indexFound], vertices [firstVertex]) >= AINavigationMeshAgent.agentDiameter) {
				tempVector = polygonToCheckVertices [indexFound] + vertices [secondVertex];
				return new Vector3 (tempVector.x / 2, 0f, tempVector.z / 2);
			}
		}
		return new Vector3(1000f, 1000f, 1000f);
	}



	//isOnALineBetweenTwoPoints(Vector3 startPoint, Vector3 endPoint, Vector3 pointToCheck)
	/* checkOneSharedEdgeAndPointInbetweenLine method will look to see if that this polygon and the polygon
	 *	passed in are neighbors by examining if they have a shared vertex and the second vertex is inbetween
	 *	an edge on of the polygons
	 *	Parameter:	(AIPolygon)polygonToCheck is the polygon the caller wants to know if is a neighbor with this polygon
	 *	Return:			(bool)
	 *								true if the two polygons are neighbors using this method of checking
	 *								false if the two polygons are not neighbors using this method of checking
	 */
	public bool checkOneSharedEdgeAndPointInbetweenLine(AIPolygon polygonToCheck)
	{
		for (int count = 0; count < vertices.Length; count++) {
			for (int count2 = 0; count2 < polygonToCheck.getVerticesCount(); count2++) {
				if (checkVertices (vertices [count], polygonToCheck.getVectorAtIndex (count2)) == true) {
					if (doesPointsAgacentShareAnEdge (count, count2, polygonToCheck.getVertices ()) == true)
						return true;
				}
			}
		}
		return false;
	}
	



	/*
	 *	checkTwoPointsInbetweenALine method will check to see if that either polygon has an edge that is
	 *			inbetween an edge of the other polygon
	 *	Parameter:	(AIPolygon)polygonToCheck is the polygon the caller wishes to know is a neighbor of this polygon
	 *	Return:			(bool)
	 *									true if the two polygons are neighbors according to this method of checking
	 *									false if the two polygons are not neighbors according to this method of checking
	 */
	public bool checkTwoPointsInbetweenALine(AIPolygon polygonToCheck)
	{
		if (isTwoPointsOnALineBetweenTwoPoints (vertices, polygonToCheck.getVertices ()) == true)
			return true;
		if(isTwoPointsOnALineBetweenTwoPoints (polygonToCheck.getVertices (), vertices) == true)
			return true;
		return false;
	}
	



	/* The method checkOneVertexInbetweenALineAndTheSecondIsAlso checks if two vertices are between two vertices
	 * on the other polygon
	 * Parameter: AIPolygon polygonToCheck - polygon to check against current node's polygon
	 * Return: bool
	 *							returns true if there are two vertices on one polygon on one edge of another one
   */
	public bool checkOneVertexInbetweenALineAndTheSecondIsAlso (AIPolygon polygonToCheck)
	{
		Vector3[] polygonToCheckVertices = polygonToCheck.getVertices ();
		int firstVertex = -1;
		int secondVertex = -1;
		int indexFound = findVertexInbetweenALine (polygonToCheckVertices, ref firstVertex, ref secondVertex);
		if (indexFound == -1)
			return false;
		int polygonToCheckFirst = getOneBehind (indexFound, polygonToCheckVertices );
		int polygonToCheckThird = getOneAhead (indexFound, polygonToCheckVertices );
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [polygonToCheckFirst], polygonToCheckVertices [indexFound], vertices [firstVertex]) == true)
			if(Vector3.Distance(polygonToCheckVertices [indexFound], vertices [firstVertex]) >= AINavigationMeshAgent.agentDiameter)
				return true;
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [polygonToCheckFirst], polygonToCheckVertices [indexFound], vertices [secondVertex]) == true)
			if(Vector3.Distance(polygonToCheckVertices [indexFound], vertices [secondVertex]) >= AINavigationMeshAgent.agentDiameter)
				return true;
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [indexFound], polygonToCheckVertices [polygonToCheckThird], vertices [firstVertex]) == true)
			if(Vector3.Distance(polygonToCheckVertices [indexFound], vertices [firstVertex]) >= AINavigationMeshAgent.agentDiameter)
				return true;
		if (isOnALineBetweenTwoPoints (polygonToCheckVertices [indexFound], polygonToCheckVertices [polygonToCheckThird], vertices [secondVertex]) == true)
			if(Vector3.Distance(polygonToCheckVertices [indexFound], vertices [secondVertex]) >= AINavigationMeshAgent.agentDiameter)
				return true;
		return false;
		
	}




	/* The method findVertexInbetweenALine determines if there is a vertex in between a line
	 * Parameter: Vector3[] polygonToCheckVertices - vertices of the polygon to check against the current polygon
	 * ref int firstVertex - index of the first vertex
	 * ref int secondVertex - index of the second vertex
	 */
	int findVertexInbetweenALine (Vector3[] polygonToCheckVertices, ref int firstVertex, ref int secondVertex)
	{
		for (int count = 0; count < polygonToCheckVertices.Length; count++) {
			for (int firstPos = 0, secondPos = 1; firstPos < vertices.Length; firstPos++, secondPos++) {
				if (secondPos >= vertices.Length)
					secondPos = 0;
				if (isOnALineBetweenTwoPoints (vertices [firstPos], vertices [secondPos], polygonToCheckVertices [count]) == true) {
					firstVertex = firstPos;
					secondVertex = secondPos;
					return count;
				}
			}
		}
		return -1;
	}




	/*
	 * doesPointsAgacentShareAnEdge method will look and see if one polygon has anther point that makes an
	 *		edge with the point passed in and is inbetween an edge of the other polygon
	 *	Parameters:	(int)count1 the position in this polygon that a shared vertex was found
	 *							(int)count2 the position in the polygonToCheck that a shared vertex was found
	 *							(vector3[])verticesToCheck is the array of vertices in the polygonToCheck
	 *	Return:			(bool)
	 *								true if the two polygons are neighbors according to this method of checking
	 *								false if the two polygons are not neighbors according to this method of checking
	 */
	bool doesPointsAgacentShareAnEdge(int count1, int count2, Vector3[] verticesToCheck)
	{
		int polygonToCheckFirst = getOneBehind (count2, verticesToCheck);
		int polygonToCheckThird = getOneAhead (count2, verticesToCheck);
		int thisFirst = getOneBehind (count1, vertices);
		int thisThird = getOneAhead (count1, vertices);
		if (isOnALineBetweenTwoPoints (vertices [thisFirst], vertices [count1], verticesToCheck [polygonToCheckFirst]) == true) {
			if (Vector3.Distance(verticesToCheck [polygonToCheckFirst], verticesToCheck [count2]) >= AINavigationMeshAgent.agentDiameter) {
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (vertices [thisFirst], vertices [count1], verticesToCheck [polygonToCheckThird]) == true){
			if (Vector3.Distance(verticesToCheck [count2], verticesToCheck [polygonToCheckThird]) >= AINavigationMeshAgent.agentDiameter){
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (vertices [count1], vertices [thisThird], verticesToCheck [polygonToCheckFirst]) == true){
			if (Vector3.Distance(verticesToCheck [polygonToCheckFirst] , verticesToCheck [count2]) >= AINavigationMeshAgent.agentDiameter) {
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (vertices [count1], vertices [thisThird], verticesToCheck [polygonToCheckThird]) == true){
			if (Vector3.Distance(verticesToCheck [count2] , verticesToCheck [polygonToCheckThird]) >= AINavigationMeshAgent.agentDiameter) {
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (verticesToCheck [polygonToCheckFirst], verticesToCheck [count2], vertices [thisFirst]) == true) {
			if (Vector3.Distance(vertices [thisFirst] , vertices [count1]) >= AINavigationMeshAgent.agentDiameter){
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (verticesToCheck [polygonToCheckFirst], verticesToCheck [count2], vertices [thisThird]) == true) {
			if (Vector3.Distance(vertices [count1], vertices [thisThird]) >= AINavigationMeshAgent.agentDiameter){
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (verticesToCheck [count2], verticesToCheck [polygonToCheckThird], vertices [thisFirst]) == true) {
			if (Vector3.Distance(vertices [thisFirst] , vertices [count1]) >= AINavigationMeshAgent.agentDiameter){
				return true;
			}
		}
		if (isOnALineBetweenTwoPoints (verticesToCheck [count2], verticesToCheck [polygonToCheckThird], vertices [thisThird]) == true) {
			if (Vector3.Distance(vertices [count1] , vertices [thisThird]) >= AINavigationMeshAgent.agentDiameter){
				return true;
			}
		}
		return false;
	}
	



	/*
	 * getOneBehind method gets the index of the logical behind vertex in the array of vertices
	 *	Parameter:	(int)count is the index of the vertex that needs the connecting vertex behind it
	 *							(vector3[])verticesToCheck is the array of vertices that the connecting vertex must come from
	 *	Return:			(int)
	 *								the logical behind vertex index
	 */
	int getOneBehind(int count, Vector3[] verticesToCheck)
	{
		if (count == 0)
			return (verticesToCheck.Length - 1);
		return (count - 1);
	}



	
	/*
	 * getOneAhead method gets the index of the logical ahead vertex in the array of vertices
	 *	Parameters:	(int)count is the index of the vertex that needs the connecting vertex ahead of it
	 *							(Vector3[])verticesToCheck is the array of vertices that the connecting vertex must come from
	 *	Return:			(int)
	 *								the logical ahead vertex index
	 */
	int getOneAhead(int count, Vector3[] verticesToCheck)
	{
		if (count == (verticesToCheck.Length - 1))
			return 0;
		return count + 1;
	}



	
	/*
	 * isTwoPointsOnALineBetweenTwoPoints method will look to see if and edge in the firstPolygon is inbetween
	 *		and edge in the secondPolygon
	 *	Parameters:	(Vector3[])firstPolygonVertices is the vertices that make up the first polygon
	 *							(vector3)secondPolygonVertices is the vertices that make up the second polygon
	 *	Return:			(bool)
	 *								true if the two polygons are neighbors according to this method of checking
	 *								false if the two polygons are not neighbors according to this method of checking
	 */
	bool isTwoPointsOnALineBetweenTwoPoints (Vector3[] firstPolygonVertices,Vector3[] secondPolygonVertices)
	{
		for(int firstPos1 = 0, firstPos2 = 1; firstPos1 < firstPolygonVertices.Length; firstPos1++, firstPos2++)
		{
			if(firstPos2 >= firstPolygonVertices.Length)
				firstPos2 = 0;
			for(int secondPos1 = 0, secondPos2 = 1; secondPos1 < secondPolygonVertices.Length; secondPos1++, secondPos2++)
			{
				if(secondPos2 >= secondPolygonVertices.Length)
					secondPos2 = 0;
				if((isOnALineBetweenTwoPoints(firstPolygonVertices[firstPos1], firstPolygonVertices[firstPos2], secondPolygonVertices[secondPos1]) == true) && (isOnALineBetweenTwoPoints(firstPolygonVertices[firstPos1], firstPolygonVertices[firstPos2], secondPolygonVertices[secondPos2]) == true))
					if((secondPolygonVertices[secondPos1] - secondPolygonVertices[secondPos2]).magnitude >= AINavigationMeshAgent.agentDiameter)
						return true;
			}
		}
		return false;
		
	}
	



	/*
	 * addNeighbor will add a index of a neighbor. If the neighborIndices array is filled it
	 * 		will call growArray to double the size of the array. It will ad the index of the
	 * 		new neighbor to the array neighborIndices
	 * Parameter:	(int)indexOfNeighbor is the index number in a AIPolygonHolder's array that
	 * 					represents the neighbor to this polygon
	 * Return:	None
	 */
	public void addNeighbor(int indexOfNeighbor)
	{
		if (neighborsHeld >= neighborIndices.Length) {
			growArray ();//doubls the size of the array
		}
		neighborIndices [neighborsHeld] = indexOfNeighbor;
		neighborsHeld++;
	}
	


	
	/*
	 * deleteNeighbor method will delete the index of the neighbor the caller wants deleted from the neighbor list
	 *	Parameters:	(int)indexOfNeighbor is the index of the neighbor to be deleted
	 *	Return:	none
	 */
	public void deleteNeighbor(int indexOfNeighbor, int id)
	{
		
		for (int count = 0; count < neighborIndices.Length; count++) {
			if (neighborIndices [count] != -1) {
				if (neighborIndices [count] == indexOfNeighbor) {
					neighborIndices [count] = -1;
				}
			}
		}
		shrinkNeighborArray ();
	}


	
	/*
	 * shrinkNeighborArray will condence the neighbor list down so there is no empty space in the array
	 *	Parameters:	none
	 *	Return:	none
	 */
	void shrinkNeighborArray()
	{
		int neighborCount = 0;
		for (int count = 0; count < neighborIndices.Length; count++)
			if (neighborIndices[count] != -1)
				neighborCount++;
		int[] newArray = new int[neighborCount];
		for (int newArrayCount = 0, oldArrayCount = 0; oldArrayCount < neighborIndices.Length && newArrayCount < neighborCount; oldArrayCount++) {
			if (neighborIndices [oldArrayCount] > -1) {
				newArray [newArrayCount] = neighborIndices [oldArrayCount];
				newArrayCount++;
			}
		}
		neighborsHeld = neighborCount ;
		neighborIndices = newArray;
	}
	


	/*
	 * getNeighborsHeld will return the number of neighbors being held by this polygon
	 *	Parameters:	none
	 *	Return:	(int)
	 *						the number of neighbors in the neighbor list
	 */
	public int getNeighborsHeld()
	{
		return neighborsHeld;
	}


	
	/*
	 * growArray will double the size of the neighborIndices array by 2 and then copy
	 * 		the elements of neighborIndices into the new array and finally set neighborIndices
	 * 		to the new array
	 * Parameter:	None
	 * Return:	None
	 */
	void growArray()
	{
		int[] newArray = new int[(neighborIndices.Length + 1) * 2]; // creats a new array of double the size
		for (int count = 0; count < newArray.Length; count++)
			newArray [count] = -1;
		for (int count = 0; count < neighborsHeld; count++) {
			newArray [count] = neighborIndices [count]; //add element of old array to new array
		}
		neighborIndices = newArray;
	}
	



	/*
	 * getNeighbors will make an array of the exact number of neighbors this polygons has
	 * 		then it will return that array to the caller
	 * Parameters:	None
	 * Return:	(int[])
	 * 				a new array of exactly the amount of neighbors indexes that neighborsIndices
	 * 					is holding
	 */
	public int[] getNeighbors()
	{
		int [] newArray = new int [neighborsHeld];
		for (int count = 0; count < neighborsHeld; count++)
			newArray [count] = neighborIndices [count];
		
		return newArray;
	}



	
	/*
	 * mergeRegular will attempt to merge two polygons that share two vertices that make up an edge.
	 * 		This will accour by creating a new Vector3[] that will hold all the vertices of both polygons
	 * 		It will call 5 methods to do this merge if the polygons can. First is getClockWiseIndices
	 * 		which will first check to see if the polygons share two clockwise vertices. And then it will
	 * 		find the indexs inside the first polygon's vertices array that holds the first vertices of the pair
	 * 		found and then the last. It will then call setVertices to set this polygons vertices starting with
	 * 		the furthest clockwise most shared vertices or (endIndex). Then it will call setVertices on the polygonTomerge
	 * 		vertices starting from the last vertices added to mergedVertices. Next this method will call deleteRedundantVertices
	 * 		with the new mergedvertices that will delete vertices that are duplicates. Next this method will call
	 * 		deleteNotNeededvertices which will delete vertices that are not necessary to contain the shape of the
	 * 		polygon (i.e. vertex that is on a line inbetween two vertices)
	 * Parameter:	(AIPolygon)polygonToMerge this is the polygon that the caller wants to merge
	 * Return:		(AIPolygon) if getClockWiseIndices was good a new polygon will be return
	 * 				(null) if getClockWiseIndices was not good it will return null
	 */
	public AIPolygon mergeRegular(AIPolygon polygonToMerge, int newID)
	{
		Vector3[] mergedVertices = new Vector3[(vertices.Length + polygonToMerge.getVerticesCount())];
		int startIndex, endIndex;
		startIndex = endIndex = -1;
		if (getClockWiseIndices (polygonToMerge, ref startIndex, ref endIndex) == true) {
			setVertices (mergedVertices, vertices [endIndex], 0);
			polygonToMerge.setVertices (mergedVertices, vertices [startIndex], vertices.Length-1);
			return new AIPolygon (deleteNotNeededVertices(deleteRedundantVertices (mergedVertices)), newID);
			
		} else
			return null;
		
	}
	



	/*
	 * getClockWiseIndeices will look at this polygon and another polygon that wants to be merged
	 * 		and try to what is the furtest clockwise shared vertex and what is the not furthest clockwise
	 * 		shared vertex. If it can not do this it will return false. If it can do this the furthest
	 * 		clockwise shared vertex will be put into endIndex, the first vertex will be put into
	 * 		startIndex.
	 * Parameter:	(AIPolygon)polygonToMerge is the polygon that needs to be merged
	 * 				(ref int)startIndex will be the index into the vertices array for this polygon that was the first
	 * 						in the clockwise pair
	 * 				(ref int)endIndex will be the index into the vertices array for this polygon that is the last
	 * 						in the clockwise pair
	 * Return:		(bool)
	 * 					true if two clockwise vertices were found
	 * 					false if two clockwise vertices were not found
	 */
	bool getClockWiseIndices(AIPolygon polygonToMerge, ref int startIndex, ref int endIndex)
	{
		int place = 0;
		int oldFound = -1;
		int Found = 0;
		for (int count1 = 0; count1 <= vertices.Length; count1++) { //goes the amount of vertices inside this polygon
			if (count1 == vertices.Length) // does the wrap around
				place = 0;
			else
				place = count1;
			oldFound = Found; //sets oldFound to compare to make sure we found another vertice
			for (int count2 = 0; count2 < polygonToMerge.getVerticesCount(); count2++) { //goes the amount of vertices inside polygonToMerge
				if (checkVertices (vertices [place], polygonToMerge.getVectorAtIndex (count2)) == true) {
					Found++;
					if (Found == 2) {
						endIndex = place;
						return true; //two clockwise vertices were found
					}
					startIndex = place;
					count2 = 100000; //gets out of this for loop
				}
			}
			if (oldFound == Found) //if Found didnt change n the last time around
				Found = 0;
		}
		return false; //two clockwise vertices were not found
	}



	
	/*
	 * checkVertices will see if two Vector3 points given to it are the same point
	 * Parameter:	(Vector3)thisPolygonVertex is the point needed to be checked of this polygon
	 * 				(Vector3)polygonToMergeVertex is the point needed to be check of the polygonToMerge
	 * Return:		(bool)
	 * 					true if the x, y, and z of both points match
	 * 					false if any coordinate dont match
	 */
	public static bool checkVertices(Vector3 thisPolygonVertex, Vector3 polygonToMergeVertex)
	{
		Vector3 Point = thisPolygonVertex - polygonToMergeVertex;
		
		if (Point.x > .00001f || Point.x < -.00001f)
			return false;
		if (Point.y > .00001f || Point.y < -.00001f)
			return false;
		if (Point.z > .00001f || Point.z < -.00001f)
			return false;
		return true;
	}
	



	/*
	 * setVertices will take the vertices starting from the index of StartPosition till all the vertices are put into
	 * 		mergedVertices. It will start putting the vertices inside mergedVertices at index mergedVerticeStart
	 * Parameter:	(Vector3[])mergedVertices is the array in which this polygon's vertices will go into
	 * 				(Vector3)startPosition is the point that will corrilate to the index that is the start of what
	 * 					vertices will go inside the array mergedVertices
	 * 				(int)mergedVerticesStart is the starting point of the mergedVertices array that this method will
	 * 					start point the new vertices at
	 * Return:	None
	 */
	public void setVertices(Vector3[] mergedVertices, Vector3 startPosition, int mergedVerticesStart)
	{
		int position = getIndexOf (startPosition);
		int mergedVerticesPosition = mergedVerticesStart;
		if (position < -1)
			return;
		for (int count1 = 0; count1 <= vertices.Length; count1++, position++, mergedVerticesPosition++) {
			if(position >= vertices.Length)
			{
				position = 0;
			}
			mergedVertices[mergedVerticesPosition] = new Vector3(vertices[position].x, vertices[position].y, vertices[position].z);		}
	}



	/*
	 * getIndexOf will get the index at which the point passed into this method is at
	 * Parameter:	(Vector3)position is the position at which the index is needed
	 * Return:		(int)
	 * 					the index number at which the position is found
	 * 					-1 if the position could not be found in the vertices array
	 */
	public int getIndexOf(Vector3 position)
	{
		for (int count = 0; count < vertices.Length; count++) {
			if (checkVertices (vertices [count], position) == true)
			{
				return count;
			}
		}
		return -1;
	}
	



	/*
	 * deleteRedundantVertices will delete all the duplicate vertices. A vertice is duplicate
	 * 		if another vertex has the same coordinates as it. It will return a new array of
	 * 		Vector3 that are the mergedVertices without the duplicate vertices
	 * Parameter:	(Vector3[])mergedVertices is the vertices that need to be check for duplicates
	 * Return:		(vector3[])
	 * 					a new array with all duplicate vertices removed
	 */
	Vector3[] deleteRedundantVertices(Vector3[] mergedVertices)
	{
		
		int mergedVerticesCount = mergedVertices.Length; //number of vertices that will need to be moved to the new array
		bool[] duplicateArray = new bool[mergedVertices.Length]; //an array that will be used to check if a vertex is a duplicate or not
		for (int count1 = 0; count1 < duplicateArray.Length; count1++) //set all elements of duplicateArray to true
			duplicateArray [count1] = true;
		for (int count1 = 0; count1 < mergedVertices.Length; count1++) { //goes the amount of the size of mergedvertices
			for (int count2 = count1 + 1; count2 < mergedVertices.Length; count2++) {//goes the amount of the size of mergedvertices
				if (checkVertices (mergedVertices [count1], mergedVertices [count2]) == true)//if vertices at index count1 and index count2 are the same
				{
					duplicateArray [count2] = false; //logically removing vertice from list of able vertices to merge
					mergedVerticesCount--;
				}
			}
			
		}
		
		//this will create a new Vector3[] with only the none duplicate vertices from mergedVertices
		Vector3[] newArray = new Vector3[mergedVerticesCount];
		for (int count = 0, newArrayCount = 0; count < mergedVertices.Length; count++) {
			if (duplicateArray [count] == true) {
				newArray [newArrayCount] = new Vector3 (mergedVertices [count].x, mergedVertices [count].y, mergedVertices [count].z);
				newArrayCount++;
			}
		}
		return newArray;
	}
	



	/*
	 * deleteNotNeededVertices will take pairs of three vertices and see if the middle vertices
	 * 		in a line inbetween the first and lst vertex. If so it will be removed from the array of vertices
	 * Parameter:	(Vector3[])mergedVertices is the vertices that need to be check for not needed vertices
	 * Return:		(Vector3[])
	 * 					a new array of only vertices that are needed to contain shape of a polygon
	 */
	Vector3[] deleteNotNeededVertices(Vector3[] mergedVertices)
	{
		int mergedVerticesCount = mergedVertices.Length; //number of vertices that will need to be moved to the new array
		bool[] duplicateArray = new bool[mergedVerticesCount]; //an array that will be used to check if a vertex is a duplicate or not
		for (int count = 0; count < mergedVerticesCount; count++)//set all elements of duplicateArray to true
			duplicateArray [count] = true;
		for (int count1 = 0, count2 = 1, count3 = 2; count1 < mergedVertices.Length; count1++, count2++, count3++) {//count1 is the first vector in the list (one end of the line segment)
			if (count2 >= mergedVertices.Length)																	//count2 is the middle vector in the list (vector to check for on the line segment)
				count2 = 0;																							//count3 is the last vector in the list (one end of the line segment)
			if (count3 >= mergedVertices.Length)
				count3 = 0;
			if (isOnALine (mergedVertices [count1], mergedVertices [count3], mergedVertices [count2]) == true) {
				duplicateArray [count2] = false;//logically removing vertice from list of able vertices to merge
				mergedVerticesCount--;
			}
			
		}
		//this will create a new Vector3[] with only the none duplicate vertices from mergedVertices
		Vector3[] newArray = new Vector3[mergedVerticesCount];
		for (int count = 0, newArrayCount = 0; count < mergedVertices.Length; count++) {
			if (duplicateArray [count] == true) {
				newArray [newArrayCount] = new Vector3 (mergedVertices [count].x, mergedVertices [count].y, mergedVertices [count].z);
				newArrayCount++;
			}
		}
		return newArray;
	}
	


	/*
	 * isOnALine will look at three points to see if the point pointToCheck is on the line made between the
	 * 		startPoint and the endPoint
	 * Parameter:	(Vector3)startPoint is one logical end of the line segment
	 * 				(Vector3)endPoint is one logical end of the line segment
	 * 				(Vector3)pointToCheck is point that this method will check if it is on the line
	 * 					between startPoint and endPoint
	 * Return:		(bool)
	 * 					true if pointToCheck is on the line between startPoint and endPoint
	 * 					flase if pointToCheck is not on the linebetween startPoint and endPoint
	 */
	public bool isOnALine(Vector3 startPoint, Vector3 endPoint, Vector3 pointToCheck)
	{
		Vector3 point1 = startPoint - pointToCheck;//create the first Vector to get the angle between
		Vector3 point2 = endPoint - pointToCheck;//creat the second vector to get the angle between
		
		point1.Normalize ();//normalizes the maginatude to be between 1
		point2.Normalize ();//normalizes the maginatude to be between 1
		if ((Vector3.Dot (point1, point2) == -1) || (Vector3.Dot (point1, point2) == 1)) { //uses the dot product between two points -1 means between the two points and 1 means perpendicular
			return true;
		}
		return false;
	}
	
	//returns true is on pointTocheck is on a line between startPoint and end point
	//returns fals is not
	public bool isOnALineBetweenTwoPoints(Vector3 startPoint, Vector3 endPoint, Vector3 pointToCheck)
	{
		//Debug.Log ("startPoint = " + startPoint.ToString () + " endPoint = " + endPoint.ToString () + " pointToCheck = " + pointToCheck.ToString ());
		if((checkVertices(pointToCheck, startPoint) == true) || (checkVertices(pointToCheck, endPoint) == true))
			return true;
		Vector3 point1 = startPoint - pointToCheck;//create the first Vector to get the angle between
		Vector3 point2 = endPoint - pointToCheck;//creat the second vector to get the angle between
		
		point1.Normalize ();//normalizes the maginatude to be between 1
		point2.Normalize ();//normalizes the maginatude to be between 1
		if (Vector3.Dot (point1, point2) == -1) { //uses the dot product between two points -1 means between the two points and 1 means perpendicular
			return true;
		}
		return false;
	}




	/*
	 * isPolygonConvex will check to see if this polygon is convex or not. It does this by checking
	 * 		the angles between three points
	 * Parameter:	None
	 * Return:	(bool)
	 * 				true if the polygon is Convex
	 * 				false if the polygon is not Convex
	 */
	public bool isPolygonConvex()
	{
		int flag = 0;
		if (vertices.Length == 3)
			return true;
		for (int count1 = 0, count2 = 1, count3 = 2; count1 < vertices.Length; count1++, count2++, count3++) {	//count1 is the first vector in the list (one end of the line segment)
			if(count2 >= vertices.Length)																		//count2 is the middle vector in the list (vector to check for on the line segment)
				count2 = 0;																						//count3 is the last vector in the list (one end of the line segment)
			if(count3 >= vertices.Length)
				count3 = 0;
			if(isAngleConvex(vertices[count1], vertices[count3], vertices[count2]) == 0) // checks to see if the angle is allowed to be convex
			{
				return false;
			}
			else if(isAngleConvex(vertices[count1], vertices[count3], vertices[count2]) == 1)
			{
				if(flag == -1)
					return false;
				flag = 1;
			}
			else if(isAngleConvex(vertices[count1], vertices[count3], vertices[count2]) == -1)
			{
				if(flag == 1)
					return false;
				flag = -1;
			}
			
		}
		return true;
	}
	



	/*
	 * isAngleConvex will check to see if the angle the three points make is allowed to be convex
	 * Parameter:	(Vector3)startPoint is one logical end of the line segment
	 * 				(Vector3)endPoint is one logical end of the line segment
	 * 				(Vector3)pointToCheck is point that this method will check if it is on the line
	 * 					between startPoint and endPoint
	 * Return:		(bool)
	 * 					false if the angle is not convex
	 * 					true if the angle is convex
	 */
	int isAngleConvex(Vector3 startPoint, Vector3 endPoint, Vector3 pointToCheck)
	{
		Vector3 point1 = startPoint - pointToCheck;//create the first Vector to get the angle between
		Vector3 point2 = endPoint - pointToCheck;//creat the second vector to get the angle between
		point1.Normalize ();//normalizes the maginatude to be between 1
		point2.Normalize ();//normalizes the maginatude to be between 1
		
		Vector3 finalVertex = Vector3.Cross (point1, point2); //gets a vector that represents the cross product of two points
		
		if ( finalVertex.x > .00001f || finalVertex.x < -.00001f )
			return 0;
		if ( finalVertex.z > .00001f || finalVertex.z < -.00001f )
			return 0;
		if (finalVertex.y > 0f)
			return 1;
		if (finalVertex.y < 0f)
			return -1;
		return 0;
	}
	



	/*
	 * getID method will return the id of this polygon
	 *	Parameters:	none
	 * Return:	(int)
	 *						the id of this polygon
	 */
	public int getID()
	{
		return id;
	}
	




	/*
	 * getNeighborAt method will return the neighbor index at a certian position in the neighbor list
	 * Parameters:	(int)index is the position in the neighbor list that the caller wants the index of
	 * Return:			(int)
	 *								-1 if that position doesnt exist
	 *								the index of the neighbor held at that position in the neighbor list
	 */
	public int getNeighborAt(int index)
	{
		if (neighborIndices [index] != -1)
			return neighborIndices [index];
		else
			return -1;
	}




	/* The method scaleWholePolygon scales the size of the polygon
	 * Parameter: float scale - how much to scale the polygon by
	 * Return: Vector3[]
	 *									new vertices of the scaled polygon
	 */
	public Vector3[] scaleWholePolygon(float scale)
	{
		Vector3[] tempArray = new Vector3[vertices.Length];
		for (int count = 0; count < tempArray.Length; count++)
			tempArray [count] = new Vector3 (vertices [count].x, vertices [count].y, vertices [count].z);
		Vector3 tempCenter = this.getCenterVector ();
		for (int count = 0; count < tempArray.Length; count++) {
			tempArray [count] -= tempCenter;
			tempArray [count] *= scale;
			tempArray[count] += tempCenter;
		}
		return tempArray;
		
	}
	


	/*
	 * seperateEdgesIntoVectors method will seperate all edges of this polygon into vertices to be used later
	 *	Parameters:	none
	 *	Return:	none*/
	public void seperateEdgesIntoVectors ()
	{
		
		edgeVectors = new Vector3[scaledVertices.Length * MAXEDGECOUNT];
		edgeVectorsCount = 0;
		for (int count = 0, position = 1; count < scaledVertices.Length; count++, position++) {
			if (position >= scaledVertices.Length)
				position = 0;
			getPointsOnAnEdge(scaledVertices[count], scaledVertices[position]);
		}
	}
	


	/*
	 * getPointsOnAnEdge will get all the points needed to be seperated between two vertices
	 *	Parameter:	(vector3)startPosition is the start position of the edge
	 *							(vector3)endPosition si the end position of the edge
	 */
	void getPointsOnAnEdge(Vector3 startPosition, Vector3 endPosition)
	{
		float xDifferenceToAdd = getDistanceToAdd (startPosition.x, endPosition.x);
		float yDifferenceToAdd = getDistanceToAdd (startPosition.y, endPosition.y);
		float zDifferenceToAdd = getDistanceToAdd (startPosition.z, endPosition.z);
		for (int count = 0; count < MAXEDGECOUNT; count++) {
			edgeVectors [edgeVectorsCount] = new Vector3 (startPosition.x + (count * xDifferenceToAdd), startPosition.y + (count * yDifferenceToAdd), startPosition.z + (count * zDifferenceToAdd));
			edgeVectorsCount++;
		}
	}
	


	/*
	 * getDistanceToAdd will get the distance to from each other on the edge that each vertice will be seperated by
	 *	Parameters:	(float)startCordinate is one of the coordinates for the starting Point of the edge
	 *							(float)endCordinate is one of the coordinate for the end point of the edge
	 *	Return:			(float)
	 *								the distance that each point will be seperated by
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
		return (difference/(MAXEDGECOUNT-1));
	}
	


	/*
	 *	getEdgeVectors method will return the array of vectors to the caller
	 *	Parameters:	none
	 *	Return:	(vector3[])
	 *						teh edge vectors of this polygon
	 */
	public Vector3[] getEdgeVectors()
	{
		return edgeVectors;
	}
	

	
	/*
	 * printSelf method will print out all the vertices that make up this polygon
  	 * Parameters:	none
	 * Return:	none
	 */
	public void printSelf()
	{
		for (int count = 0; count < vertices.Length; count++)
			Debug.Log ("printSelf count = " + count + " vertice = " + vertices [count].ToString ());
	}
	


	//return true if terminated
	//return false if not terminated
	/*
	 * isTerminated method will check to see if this polygon is completely covered by the object that this polygon
	 *			is being split with
	 *	Parameters:	(vector3[,])arrayOfEdges is the array of edges that make up the object
	 *	Return:			(bool)
	 *								true if this polygon is completely covered by the object
	 *								false if this polygon is not completely covered by this object
	 */
	public bool isTerminated(Vector3[,] arrayOfEdges)
	{
		
		bool verticesMatch = false;
		for (int count = 0; count < vertices.Length; count++) {
			verticesMatch = false;
			for (int edgeCount = 0; edgeCount < arrayOfEdges.Length/2; edgeCount++) {
				if (checkVertices (vertices [count], new Vector3(arrayOfEdges [edgeCount, 0].x, 0f, arrayOfEdges [edgeCount, 0].z)) == true)
					verticesMatch = true;
				if (checkVertices (vertices [count], new Vector3(arrayOfEdges [edgeCount, 1].x, 0f, arrayOfEdges [edgeCount, 1].z)) == true)
					verticesMatch = true;
			}
			if (verticesMatch == false)
			{
				return false;
			}
		}
		return true;
	}
	
	
	
	//return true if objectToCheck is not in the polygon
	//return false if objectToCheck is in the polygon
	/*
	 * isFree method will check to see if this polygon does not contain the object that it was being split by
	 * Parameter:	(gameObject)objectToCheck is the object that the polygon is being split by
	 * Return:		(bool)
	 *							true if the objectToCheck is not in the polygon
	 *							false if the objectToCheck is in the polygon
	 */
	public bool isFree(GameObject objectToCheck)
	{
		seperateEdgesIntoVectors ();
		Vector3 vectorToShootFrom;
		Vector3 vectorToShootTo;
		RaycastHit hit;
		int layerMask = 1 << 8;
		for (int firstPosition = 0, secondPosition = 1; firstPosition < vertices.Length; firstPosition++, secondPosition++) {
			if (secondPosition >= vertices.Length)
				secondPosition = 0;
			
			for (int edgeCount = 1; edgeCount < MAXEDGECOUNT; edgeCount++) {
				vectorToShootFrom = new Vector3 (vertices [firstPosition].x, vertices [firstPosition].y + .05f, vertices [firstPosition].z);
				vectorToShootTo = new Vector3 (edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].x, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].y + .05f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].z);
				if (Physics.Linecast (vectorToShootFrom, vectorToShootTo, out hit, layerMask)) {
					if(checkingHit(hit.point) == false && hit.transform.gameObject == objectToCheck)
					{
						return false;
					}
				}
				
			}
		}
		return true;
	}



	/* The method isFree determines if a polygon has any obstacles in it.
	 * Parameter: Vector3[] objectsVertices - vertices of polygon to check
	 * Return: bool
	 *							true if no obstacles in polygon, false otherwise
	 */
	public bool isFree(Vector3[] objectsVertices)
	{
		Vector3[] tempArray = scaleWholePolygon (.99f);
		for (int count = 0; count < objectsVertices.Length; count++) {
			if(isAPointInThePolygon(tempArray, objectsVertices[count]) == true)
			{
				return false;
			}
			
		}
		return true;
	}



	/*
	 * checkingHit method will check to see if the hit point of the line cast was inbetween an edge of this polygon
	 * Parameter:	(vector3)hitPoint is the point the line cast hit the object
	 * Return:		(bool)
	 *							true if the hitPoint is inbetween an edge of this polygon
	 *							false if the hitPoint is not inbetween an edge of this polygon
	 */
	bool checkingHit(Vector3 hitPoint)
	{
		Vector3 newHitPoint = new Vector3 (hitPoint.x, 0f, hitPoint.z);
		for (int count = 0, position = 1; count < vertices.Length; count++, position++) {
			if (position >= vertices.Length)
				position = 0;
			if (isOnALineBetweenTwoPoints (vertices [count], vertices [position], newHitPoint) == true)
				return true;
		}
		return false;
	}
	


	/*
	 * getObjctsThatWillSplit will make an arrya of objects that are inside this polygons. It will also tell those
	 *			Objects to get their edges that have no change in the y
	 * Parameters:	(AIobject[])objectsArray is an array of all static objects in the game level
	 * Return:	none
	 */
	public bool getObjctsThatWillSplit (AIObjects objectsToSplit)
	{
		if (shootLinesAndFindObjects (objectsToSplit) == true)
		{
			objectsToSplit.getAllEdgesWithNoYChange ();
			return true;
		}
		return false;
	}
	


	//subDivide(Vector3[,] arrayOfEdges, AIPolygon polygonToSubDivide, int indexForEdges)
	/*
	 * splitThisPolygon method will split this polygon with one of the objects that is in it
	 * Parameter:	none
	 *	Return:	(AIPolygonQueue)
	 *							a queue that holds all the free polygons that were made from the split
	 */
	public AIPolygonQueue splitThisPolygon(AIObjects objectToSplit)
	{
		freePolygonQueue = new AIPolygonQueue();
		Vector3[,] arrayOfEdges;
		Vector3[] arrayOfEdgesVertices;
		int idCounter = 0;
		arrayOfEdges = objectToSplit.getAllEdges ();
		arrayOfEdgesVertices = objectToSplit.getEdgeVertices();
		if (checkAllVerticesInsidePolygon (arrayOfEdgesVertices) == false) {
			arrayOfEdges = changeArrayOfEdgesAndVertices(arrayOfEdges, arrayOfEdgesVertices, objectToSplit);
		}
		AIBinarySpaceTree treeNode = new AIBinarySpaceTree (objectToSplit.getGameObject(), arrayOfEdges, arrayOfEdgesVertices, this, freePolygonQueue);
		treeNode.subDivide2 (treeNode.getRootNode(), ref idCounter);
		freePolygonQueue = treeNode.getFreeNodes ();
		return freePolygonQueue;
	}
	
	Vector3 getSharedVertices(Vector3[,] arrayOFEdges)
	{
		for (int EdgeCount = 0; EdgeCount < arrayOFEdges.Length/2; EdgeCount++) {
			for (int count = 0, count2 = 1; count < vertices.Length; count++, count2++) {
				if (count2 >= vertices.Length)
					count2 = 0;
			}
		}
		return new Vector3 (0f, 0f, 0f);
	}
	


	/*
	 * getIntersectingPoint will look at two lines and get the point that they connect at and return it
	 *	Parameters:	(Vector3)line1Start is the start vector of the first line
	 *							(Vector3)line1End is the end vector of the first line
	 *							(Vector3)line2Start is the start vector of the second line
	 *							(Vector3)line2End is the end vector of the second line
	 *	Return:			(vector3)
	 *								the intersection point if there is one
	 *								(1000f, 1000f, 1000f) if no intersection point was found
	 */
	public Vector3 getIntersectingPoint(Vector3 line1Start, Vector3 line1End, Vector3 line2Start, Vector3 line2End)
	{
		
		float A1 = line1End.z - line1Start.z;
		float B1 = line1Start.x - line1End.x;
		float C1 = A1 * line1Start.x + B1 * line1Start.z;
		
		float A2 = line2End.z - line2Start.z;
		float B2 = line2Start.x - line2End.x;
		float C2 = A2 * line2Start.x + B2 * line2Start.z;
		
		float delta = A1 * B2 - A2 * B1;
		if (delta == 0)
			return new Vector3 (1000f, 1000f, 1000f);
		Vector3 tempVector = new Vector3 ((B2 * C1 - B1 * C2) / delta, line2Start.y, (A1 * C2 - A2 * C1) / delta);
		
		if(isOnALineBetweenTwoPoints(line2Start, line2End, tempVector) == false)
			return new Vector3 (1000f, 1000f, 1000f);
		return tempVector;
		
	}



	/*
	 * shootLinesAndFindGoal will shot line cast from each vertex to the edge vertices across from it until it its
	 *			and object and that object is named "Goal"
	 *	Parameters:	none
	 *	Return:	(bool)
	 *						true if the goal was found in this polygon
	 */
	public bool shootLinesAndFindGoal()
	{
		seperateEdgesIntoVectors ();
		Vector3 vectorToShootFrom;
		Vector3 vectorToShootTo;
		RaycastHit hit;
		int layerMask = 1 << 10;
		for (int firstPosition = 0, secondPosition = 1, thirdPosition = 2; firstPosition < vertices.Length; firstPosition++, secondPosition++, thirdPosition++) {
			if (secondPosition >= vertices.Length)
				secondPosition = 0;
			if (thirdPosition >= vertices.Length)
				thirdPosition = 0;
			for (int edgeCount = 0; edgeCount < MAXEDGECOUNT; edgeCount++) {
				vectorToShootFrom = new Vector3 (vertices [firstPosition].x-.05f, vertices [firstPosition].y + .25f, vertices [firstPosition].z-.05f);
				vectorToShootTo = new Vector3 (edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].x-.05f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].y + .25f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].z-.05f);
				if (Physics.Linecast (vectorToShootFrom, vectorToShootTo, out hit, layerMask)) {
					if(hit.transform.name.Equals("Goal") == true)
					{
						hasGoal = true;
						return true;
					}
				}
			}
		}
		return false;
	}
	


	/*
	 * shootLinesAndFindAgent method will shoot line cast from each vertex to the edge vertices across from it until
	 *			it hits an object named "Agent"
	 *	Parameters:	none
	 *	Return:	(bool)
	 *						true if this polygon holds the object named "Agent"
	 *						false if this polygon doesnt hold the object named "Agent"
	 */
	public bool shootLinesAndFindAgent()
	{
		seperateEdgesIntoVectors ();
		Vector3 vectorToShootFrom;
		Vector3 vectorToShootTo;
		RaycastHit hit;
		int layerMask = 1 << 9;
		for (int firstPosition = 0, secondPosition = 1, thirdPosition = 2; firstPosition < vertices.Length; firstPosition++, secondPosition++, thirdPosition++) {
			if (secondPosition >= vertices.Length)
				secondPosition = 0;
			if (thirdPosition >= vertices.Length)
				thirdPosition = 0;
			for (int edgeCount = 0; edgeCount < MAXEDGECOUNT; edgeCount++) {
				vectorToShootFrom = new Vector3 (vertices [firstPosition].x-.05f, vertices [firstPosition].y + .25f, vertices [firstPosition].z-.05f);
				vectorToShootTo = new Vector3 (edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].x-.05f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].y + .25f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].z-.05f);
				if (Physics.Linecast (vectorToShootFrom, vectorToShootTo, out hit, layerMask)) {
					//if(hit.transform.name.Equals("Agent") == true)
					//{
						hasAgent = true;
					string name = hit.transform.gameObject.name;
					string last = name.Remove(0, 5);
						AgentID = int.Parse(last) + 1;
						//AIPolygon.agentCount++;
						Debug.Log(Time.realtimeSinceStartup + " found agent id = " + AgentID + " polygonId = " + id);
						return true;
					//}
				}
				
			}
		}
		return false;
	}

	/*
	 * int layerMask = 1 << 8;
		for (int firstPosition = 0, secondPosition = 1, thirdPosition = 2; firstPosition < vertices.Length; firstPosition++, secondPosition++, thirdPosition++) {
			if (secondPosition >= vertices.Length)
				secondPosition = 0;
			if (thirdPosition >= vertices.Length)
				thirdPosition = 0;
			for (int edgeCount = 0; edgeCount < MAXEDGECOUNT; edgeCount++) {
				vectorToShootFrom = new Vector3 (vertices [firstPosition].x, vertices [firstPosition].y + .25f, vertices [firstPosition].z);
				vectorToShootTo = new Vector3 (edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].x, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].y + .25f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].z);
				if (Physics.Linecast (vectorToShootFrom, vectorToShootTo, out hit, layerMask)) {
					if(objectsArrayToFind.isGameObjectEqual(hit.transform.gameObject) == true)
						return true;
				}
			}*/


	/*
	 *	getHasGoal method will return to the caller if this polygon has the goal or not
	 *	Parameters:	none
	 * 	Return:	(bool)
	 *						true if the goal is contained inside this polygon
	 *						false if the goal is not contained inside this polygon
	 */
	public bool getHasGoal()
	{
		return hasGoal;
	}
	


	/*
	 * getHasAgent method will return to the caller if this polygon contains the agent or not
	 *	Parameters:	none
	 *	Return:	(bool)
	 *						true if the agent is contained inside this polygon
	 *						false if the agent is not contained inside this polygon
	 */
	public bool getHasAgent()
	{
		return hasAgent;
	}
	


	/*
	 * shootLinesAndFindObjects will shoot line cast from each vertex to the edge vertices across from it and
	 *			if an static object is found it will add it to an array of objects
	 *	Parameter:	(AIObjects[])objectsArray is the array of static objects being held inside the game scene
	 *							(int[])objectArrayIndices is the array that the indices of the objects found are
	 *							(ref int)objectArrayCount is the current count of non duplicate objects found
	 *	Return:	none
	 */
	bool shootLinesAndFindObjects(AIObjects objectsArrayToFind)
	{
		seperateEdgesIntoVectors ();
		Vector3 vectorToShootFrom;
		Vector3 vectorToShootTo;
		RaycastHit hit;
		int layerMask = 1 << 8;
		for (int firstPosition = 0, secondPosition = 1, thirdPosition = 2; firstPosition < vertices.Length; firstPosition++, secondPosition++, thirdPosition++) {
			if (secondPosition >= vertices.Length)
				secondPosition = 0;
			if (thirdPosition >= vertices.Length)
				thirdPosition = 0;
			for (int edgeCount = 0; edgeCount < MAXEDGECOUNT; edgeCount++) {
				vectorToShootFrom = new Vector3 (vertices [firstPosition].x, vertices [firstPosition].y + .25f, vertices [firstPosition].z);
				vectorToShootTo = new Vector3 (edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].x, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].y + .25f, edgeVectors [edgeCount + (MAXEDGECOUNT * secondPosition)].z);
				if (Physics.Linecast (vectorToShootFrom, vectorToShootTo, out hit, layerMask)) {
					if(objectsArrayToFind.isGameObjectEqual(hit.transform.gameObject) == true)
						return true;
				}
			}
		}
		return false;
	}
	


	/* checkObjectAndAddToArray method will check the object just found with the line cast and see if it was already added
	 *	logically to the array of objects held in this polygon
	 * Parameter:	(AIObjects[])objectArray is an array of all the static objects in the game scene
	 *						(int[])objectArrayIndices is the array that the indices of the objects found are
	 *						(ref int)objectArrayCount is the current count of non duplicate objects found
	 * Return:	none
	 */
	void checkObjectAndAddToArray(AIObjects[] objectArray, GameObject objectToAdd, int[] objectArrayIndices, ref int objectArrayCount)
	{
		int index = -1;
		int flag = 0;
		for (int count = 0; count < objectArray.Length; count++) {
			if (objectArray [count].isGameObjectEqual (objectToAdd) == true) {
				index = count;
				count = objectArray.Length;
			}
		}
		for (int count = 0; count < objectArrayCount; count++) {
			if (objectArrayIndices [count] == index) {
				flag = 1;
				count = objectArrayCount;
			}
		}
		if (flag != 1) {
			objectArrayIndices [objectArrayCount] = index;
			objectArrayCount++;
		}
	}
	
	
	//not ujsed
	void drawConnections ()
	{
		
	}


	
	/*
	 * getCenterVector method will get the center of this polygon by getting the average of all the verticesLeft
	 *	Parameter:	none
	 *	Return:	(Vector3)
	 *						the center point of this polygon
	 */
	public Vector3 getCenterVector()
	{
		float xCoordinate = 0f;
		float yCoordinate = 0f;
		float zCoordinate = 0f;
		for (int count = 0; count < vertices.Length; count++) {
			xCoordinate += vertices [count].x;
			yCoordinate += vertices [count].y;
			zCoordinate += vertices [count].z;
		}
		xCoordinate /= vertices.Length;
		yCoordinate /= vertices.Length;
		zCoordinate /= vertices.Length;
		
		return new Vector3 (xCoordinate, 0f, zCoordinate);
	}
	
	//used for debuging
	public override string ToString()
	{
		string temp = "";
		for (int count = 0; count < vertices.Length; count++)
			temp += vertices [count].ToString () + ", ";
		return temp;
	}
	
	
	
	/*
	 * getMidpointOfEdge will look at this polygon and the polygonToCheck and find the midpoint of an edge that
	 *		is the edge that connects the two polygons
	 *	Parameter:	(AIPolygon)polygonToCheck is the polygon that the call wants to check for a midpoint of a shared edge
	 *	Return:			(Vector3)
	 *								the midpoint of a edge that connects the two polygons as neighbors
	 *								(1000f, 1000f, 1000f) is no connecting edge was found
	 */
	public Vector3 getMidpointOfEdge(AIPolygon polygonToCheck)
	{
		Vector3 tempVector;
		Vector3 toCheckVector = new Vector3(1000f, 1000f, 1000f);
		tempVector = checkTwoSharedEdgesMidpoint (polygonToCheck);
		if (checkVertices (tempVector, toCheckVector) == false) {
			return tempVector;
		}
		tempVector = checkOneSharedEdgeAndPointInbetweenLineMidpoint (polygonToCheck);
		if (checkVertices (tempVector, toCheckVector) == false){
			return tempVector;
		}
		tempVector = checkTwoPointsInbetweenALineMidpoint (polygonToCheck);
		if (checkVertices (tempVector, toCheckVector) == false){
			return tempVector;
		}
		tempVector = checkOneVertexInbetweenALineAndTheSecondIsAlsoMidPoint (polygonToCheck);
		if (checkVertices (tempVector, toCheckVector) == false){
			return tempVector;
		}
		return toCheckVector;
	}


	
	/* The method isAPointInThePolygon determines if a certain point is contained in the polygon
	 * Parameter: Vector3 pointToCheck - point to see if it is polygon
	 * Return: bool
	 * 							true if point is in the polygon, and false otherwise
	 */
	public bool isAPointInThePolygon(Vector3 pointToCheck)
	{
		int flag = 0;
		for(int firstPos = 0, secondPos = 1; firstPos < vertices.Length; firstPos++, secondPos++)
		{
			secondPos = secondPos % vertices.Length;
			float results = ((pointToCheck.z- vertices[firstPos].z) * (vertices[secondPos].x - vertices[firstPos].x)) - ((pointToCheck.x - vertices[firstPos].x) * (vertices[secondPos].z - vertices[firstPos].z));
			if(results == 0f) //on the line segment
				return true;
			if(results < 0f) //on the right side of the segment
			{
				if(flag == 0)
					flag = -1;
				if(flag == 1)
					return false;
			}
			else if(results > 0f) //on the left side of the segment
			{
				if(flag == 0)
					flag = 1;
				if(flag == -1)
					return false;
			}
		}
		return true;
	}



	/* The method isAPointInThePolygon determines if pointToCheck is inside polygon defined by tempArray
			 * Parameter: Vector3[] tempArray - polygon to tell if point is inside
			 * Vector3 pointToCheck - point to determine if it is in a certain polygon
			 * Return: bool
			 *							true if point is in tempArray, false otherwise
			 */
	public bool isAPointInThePolygon(Vector3[] tempArray, Vector3 pointToCheck)
	{
		int flag = 0;
		for(int firstPos = 0, secondPos = 1; firstPos < tempArray.Length; firstPos++, secondPos++)
		{
			secondPos = secondPos % vertices.Length;
			float results = ((pointToCheck.z- tempArray[firstPos].z) * (tempArray[secondPos].x - tempArray[firstPos].x)) - ((pointToCheck.x - tempArray[firstPos].x) * (tempArray[secondPos].z - tempArray[firstPos].z));
			if(results == 0f) //on the line segment
				return true;
			if(results < 0f) //on the right side of the segment
			{
				if(flag == 0)
					flag = -1;
				if(flag == 1)
					return false;
			}
			else if(results > 0f) //on the left side of the segment
			{
				if(flag == 0)
					flag = 1;
				if(flag == -1)
					return false;
			}
		}
		return true;
	}



	/* The method checkAllVerticesInsidePolygon checks to see if all the points inside
	 * arrayOfEdgesVertces are inside the polygon
	 * Parameter: Vector3[] arrayOfEdgesVertices - vertices to check
	 * Return: bool
	 *							true if all points in arrayOfEdgesVertices are in the polygon, false otherwise
	 */
	bool checkAllVerticesInsidePolygon (Vector3[] arrayOfEdgesVertices)
	{
		for(int count = 0; count < arrayOfEdgesVertices.Length; count++)
			if(isAPointInThePolygon(arrayOfEdgesVertices[count]) == false)
				return false;
		return true;
	}



	/* The method changeArrayOfEdgesAndVertices makes new array of edges when the polygon is no good
	 * Parameter: Vector3[,] arrayOfEdges -
   * Vector3[] arrayOfEdgesVertices -
	 * AIObjects objectToSplit -
	 * Return: Vector[,]
	 *										new array of edges
	 */
	Vector3[,] changeArrayOfEdgesAndVertices(Vector3[,] arrayOfEdges, Vector3[] arrayOfEdgesVertices, AIObjects objectToSplit)
	{
		float yCord = objectToSplit.getMinYCord ();
		objectString = objectToSplit.getGameObject ().ToString ();
		Vector3[] tempArrayOfVertices = getNewArrayOfEdgeVertices (arrayOfEdgesVertices);
		Vector3[,] tempArrayOfEdges = new Vector3[tempArrayOfVertices.Length, 2];
		if(tempArrayOfEdges == null)
			Debug.Log(Time.realtimeSinceStartup + " object = " + objectString);
		for (int firstPos = 0, secondPos = 1, counter = 0; firstPos < tempArrayOfVertices.Length; firstPos++, secondPos++, counter++) {
			secondPos = secondPos % tempArrayOfVertices.Length;
			tempArrayOfEdges[counter, 0] = new Vector3(tempArrayOfVertices[firstPos].x , yCord , tempArrayOfVertices[firstPos].z );
			tempArrayOfEdges[counter, 1] = new Vector3(tempArrayOfVertices[secondPos].x , yCord ,tempArrayOfVertices[secondPos].z );
		}
		return tempArrayOfEdges;
	}



	/* The method getNewArrayOfEdgeVertices gets the new array of edges for the obstacle about to be cut around
	 * Parameter: Vector3[] arrayOfEdgesVertices - vertices of edges to be put into new arrayOfEdges array
	 * Return: Vector3[]
	 *									 new array of edges
	 */
	Vector3[] getNewArrayOfEdgeVertices (Vector3[] arrayOfEdgesVertices)
	{
		int firstPos = 0;
		int secondPos = 1;
		Vector3 notReal = new Vector3 (1000f, 1000f, 1000f);
		Vector3[] tempArray = new Vector3[arrayOfEdgesVertices.Length + 2];
		int tempArrayCount = 0;
		Vector3 tempVector = hasAnIntersectingPoint (arrayOfEdgesVertices [firstPos], arrayOfEdgesVertices [secondPos]);
		while (firstPos < arrayOfEdgesVertices.Length && checkVertices(tempVector, notReal) == true) {
			firstPos++;
			secondPos++;
			secondPos = secondPos % arrayOfEdgesVertices.Length;
			if(firstPos >= arrayOfEdgesVertices.Length)
				Debug.Log(Time.realtimeSinceStartup + " object = " + objectString);
			tempVector = hasAnIntersectingPoint (arrayOfEdgesVertices [firstPos], arrayOfEdgesVertices [secondPos]);
		}
		if (checkVertices (tempVector, notReal) == false) {
			int positionToStartAt = secondPos;
			tempArray[tempArrayCount] = new Vector3(tempVector.x, tempVector.y, tempVector.z);
			tempArrayCount++;
			if (isAPointInThePolygon (arrayOfEdgesVertices [secondPos]) == false) { //outside the polygon
				int count = 0;
				firstPos = positionToStartAt;
				secondPos = positionToStartAt + 1;
				secondPos = secondPos % arrayOfEdgesVertices.Length;
				tempVector = hasAnIntersectingPoint (arrayOfEdgesVertices [firstPos], arrayOfEdgesVertices [secondPos]);
				while(count < arrayOfEdgesVertices.Length && checkVertices(tempVector, notReal) == true) {
					firstPos++;
					secondPos++;
					count++;
					firstPos = firstPos % arrayOfEdgesVertices.Length;
					secondPos = secondPos % arrayOfEdgesVertices.Length;
					tempVector = hasAnIntersectingPoint (arrayOfEdgesVertices [firstPos], arrayOfEdgesVertices [secondPos]);
				}
				if (checkVertices (tempVector, notReal) == false) {
					tempArray[tempArrayCount] = new Vector3(tempVector.x, tempVector.y, tempVector.z);
					tempArrayCount++;
					count++;
					for(; count < arrayOfEdgesVertices.Length; count++)
					{
						secondPos = secondPos % arrayOfEdgesVertices.Length;
						tempArray[tempArrayCount] = new Vector3(arrayOfEdgesVertices[secondPos].x, arrayOfEdgesVertices[secondPos].y, arrayOfEdgesVertices[secondPos].z);
						tempArrayCount++;
						secondPos++;
					}
					return shrinkEdgeArray(tempArray, tempArrayCount);
				}
				else
					return null;
			} else { //inside the polygon
				int count = 0;
				tempArray[tempArrayCount] = new Vector3(arrayOfEdgesVertices [secondPos].x, arrayOfEdgesVertices [secondPos].y, arrayOfEdgesVertices [secondPos].z);
				tempArrayCount++;
				firstPos++;
				secondPos++;
				firstPos = firstPos % arrayOfEdgesVertices.Length;
				secondPos = secondPos % arrayOfEdgesVertices.Length;
				tempVector = hasAnIntersectingPoint (arrayOfEdgesVertices [firstPos], arrayOfEdgesVertices [secondPos]);
				while(count < arrayOfEdgesVertices.Length && checkVertices(tempVector, notReal) == true) {
					tempArray[tempArrayCount] = new Vector3(arrayOfEdgesVertices[secondPos].x, arrayOfEdgesVertices[secondPos].y, arrayOfEdgesVertices[secondPos].z);
					tempArrayCount++;
					firstPos++;
					secondPos++;
					firstPos = firstPos % arrayOfEdgesVertices.Length;
					secondPos = secondPos % arrayOfEdgesVertices.Length;
					tempVector = hasAnIntersectingPoint (arrayOfEdgesVertices [firstPos], arrayOfEdgesVertices [secondPos]);
				}
				if (checkVertices (tempVector, notReal) == false) {
					
					tempArray[tempArrayCount] = new Vector3(tempVector.x, tempVector.y, tempVector.z);
					tempArrayCount++;
					return shrinkEdgeArray(tempArray, tempArrayCount);
				}
				else
					return null;
			}
		} else
			return null;
		
	}



	/* The method shrinkEdgeArray shrinks the size of a certain Vector3[] array
	 * Parameter: Vector3[] arrayToShrink - array that will shrunks
	 * int size - size to shrink array to
	 * Return: Vector3[]
	 *										new shrunken array
	 */
	Vector3[] shrinkEdgeArray(Vector3[] arrayToShrink, int size)
	{
		Vector3[] tempArray = new Vector3[size];
		for (int count = 0; count < tempArray.Length; count++) {
			tempArray [count] = new Vector3 (arrayToShrink [count].x, arrayToShrink [count].y, arrayToShrink [count].z);
		}
		return tempArray;
	}



	/* The method hasAnIntersectingPoint finds if either parameters intersect any of the edges of the polygon
	 * Parameter: Vector3 firstPoint, Vector3 secondPoint
	 * Return: Vector3
	 * 									returns intersecting point if there is one, (1000f, 1000f, 1000f) if there isn't
	 */
	Vector3 hasAnIntersectingPoint(Vector3 firstPoint, Vector3 secondPoint)
	{
		Vector3 notReal = new Vector3 (1000f, 1000f, 1000f);
		Vector3 tempVector;
		for (int thisFirst = 0, thisSecond = 1; thisFirst < vertices.Length; thisFirst++, thisSecond++) {
			thisSecond = thisSecond % vertices.Length;
			tempVector = getIntersectingPoint (vertices [thisFirst], vertices [thisSecond], firstPoint, secondPoint);
			if (checkVertices (tempVector, notReal) == false) {
				return tempVector;
				
			}
		}
		return new Vector3 (1000f, 1000f, 1000f);
	}



	/* The method maxLength returns the maximum edge length of the polygon
	 * Return: float
	 * 								returns the length of the maximum edge
	 */
	public float maxLength()
	{
		float max = 0f;
		float temp;
		for (int firstPos = 0, secondPos = 1; firstPos < vertices.Length; firstPos++, secondPos++) {
			secondPos = secondPos % vertices.Length;
			temp = (vertices [firstPos] - vertices [secondPos]).magnitude;
			if (temp > max)
				max = temp;
		}
		return max;
	}

	public int getAgentID()
	{
		return AgentID;
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
