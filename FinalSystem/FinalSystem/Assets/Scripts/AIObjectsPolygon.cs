/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIObjectsPolygon class
 * This class will be a a container class for a polygon that is part of static objects of the game scene. It can
 * 		get the vertices count, get all the edges that have no change in the Y, and check if a edge's Y doesnt change
 */
using UnityEngine;
using System.Collections;

public class AIObjectsPolygon
{
	Vector3[] vertices; // array of vertices that make up the polygon
	float minYCordinate;


	/*
	 * AIObjectsPolygon method is the constructor for this class. It will place the vertices inside the array
	 * Parameter:	(Vector3[])verticesForThis is the vertices that this object will hold
	 * Return:	none
	 */
	public AIObjectsPolygon(Vector3[] verticesForThis, float minCord)
	{
		vertices = verticesForThis;
		minYCordinate = minCord;

	}


	/*
	 * getVerticesCount method will return the the number of vertices in this polygon
	 * Parameter:	none
	 * Return:	(int)
	 * 				the number of vertices making up this polygon
	 */
	public int getVerticesCount()
	{
		return vertices.Length;
	}


	/*
	 * getAllEdgesWithNoYChange method will look at all the edges in the polygon and build an array of connecting
	 * 		vertices to use later
	 * Parameter:	(Vector3[,])temp is an array that will hold the connecting vertices
	 * 				(ref int)tempCount will hold the number of connecting nodes found so far
	 * Return:	none
	 */
	public void getAllEdgesWithNoYChange(Vector3[,] temp, ref int tempCount)
	{
		int numberOfEdges = 0;
		int[,] connectingIndices = new int[vertices.Length, 2];
		for (int count1 = 0, count2 = 1; count1 < vertices.Length; count1++, count2++) {
			if(count2 >= vertices.Length)
				count2 = 0;
			if (checkYDoesntChange (vertices [count1], vertices [count2]) == true && onlyOneChange(vertices [count1], vertices [count2]) == true) {
				connectingIndices [numberOfEdges, 0] = count1;
				connectingIndices [numberOfEdges, 1] = count2;
				numberOfEdges++;
			}
		}
		for (int count = 0; count < numberOfEdges; count++) {
			temp [tempCount, 0] = new Vector3(vertices [connectingIndices [count, 0]].x , vertices [connectingIndices [count, 0]].y, vertices [connectingIndices [count, 0]].z );
			temp [tempCount, 1] = new Vector3(vertices [connectingIndices [count, 1]].x , vertices [connectingIndices [count, 1]].y, vertices [connectingIndices [count, 1]].z );
			tempCount++;
		}
	}



	//Point.y > .00001f || Point.y < -.00001
	/*
	 * checkYDoesntChange method will look at a edge of two points and see if the y does not change going from one to the other
	 * Parameters:	(Vector3)startPos is the starting point of the edge
	 * 				(Vector3)endPos is the ending point of the edge
	 * Return:		(bool)
	 * 					true if the Y coordinate does not change between the two points
	 * 					false if the Y coordinate does change between the two points
	 */
	bool checkYDoesntChange(Vector3 startPos, Vector3 endPos)
	{
		float newY = startPos.y - endPos.y;
		if (newY < .0001f && newY > -.0001f)
			return true;
		return false;
	}

  /* The method onlyOneChange makes sure that only one change in vertices has been made
	 * Parameter: Vector3 startPos - position to compare
	 * Vector3 endPos - position to compare
	 * Return: bool
	 *							true if only one change has been made false otherwise
	 */
	bool onlyOneChange(Vector3 startPos, Vector3 endPos)
	{
		float newZ = startPos.z - endPos.z;
		float newX = startPos.x - endPos.x;
		if (newZ < .0001f && newZ > -.0001f)
			return true;
		if (newX < .0001f && newX > -.0001f)
			return true;
		return false;
	}

	// used for debugging
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

  /* The method isFlat makes sure that the polygon is flat
	 * Return: bool
	 *							true if flat, false otherwise
	 */
	public bool isFlat()
	{
		for (int firstPos = 0, secondPos = 1; firstPos < vertices.Length; firstPos++, secondPos++) {
			secondPos = secondPos % vertices.Length;
			if(checkYDoesntChange(vertices[firstPos], vertices[secondPos]) == false)
				return false;
		}
		return true;
	}

  /* The method getVertices returns the vertices from the current node
	 * Return: Vector3[]
	 * 										array containing vertices of polygon held by node
	 */
	public Vector3[] getVertices()
	{
		return vertices;
	}

  /* The method getMinYCord returns the minYCordinate from the current node
	 * Return: float
	 *							minYCordinate
	 */
	public float getMinYCord ()
	{
		return minYCordinate;
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
