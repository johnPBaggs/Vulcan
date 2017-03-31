/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIObjects class
 * This class will act as a container class for a static object in the game scene. It will hold a gameObject this class represents,
 * 		an array of Polygons that make up an object, and an array of edges whos Y does not change
 */
using UnityEngine;
using System.Collections;

public class AIObjects
{
	AIObjectsPolygon[] objectPolygons; //an array of polygons that make up this object
	int numberOfPolygonsHeld;
	GameObject objectGameObject; //the game object this instance is representing
	Vector3[,] edgesWithNoYChange; //an array of edges of the polygon whos Y does not change
    Vector3[] edgeVertices;



	/*
	 * AIObjects method is the constructor for this class. It will set everything to its initial value
	 * Parameters:	(int)numberOfPolygons is the number of polygons that will make up this object
	 * 				(GameObject)thisObject is the gameobject that this instance of the class will hold
	 * Return:	none
	 */
	public AIObjects(int numberOfPolygons, GameObject thisObject)
	{
		objectPolygons = new AIObjectsPolygon[numberOfPolygons];
		objectGameObject = thisObject;
		numberOfPolygonsHeld = 0;
	}


	/*
	 * addPolygon method will add a new polygon to the array that makes up the object
	 * Parameter:	(Vector3[])vertices is an array that holds the vertices that will make up the new polygon
	 * Return:	none
	 */
	public void addPolygon(Vector3[] vertices)
	{
		Vector3[] temp = new Vector3[vertices.Length];
		float minYCord = 10000f;
		for (int count = 0; count < vertices.Length; count++) {
			temp [count] = new Vector3 (vertices [count].x , vertices [count].y , vertices [count].z ); //copy the vertices points into a new Vector3
			if(temp[count].y < minYCord)
				minYCord = temp[count].y;
		}
		objectPolygons [numberOfPolygonsHeld] = new AIObjectsPolygon (temp, minYCord); //make the new AIPolygon
		numberOfPolygonsHeld++;
	}


	/*
	 * isGameObjectEqual method will look to see if the gameObject being held by this instance is equal to 
	 * 		the one the caller wants to check
	 * Parameter:	(GameObject)gameObjectToTest is the gameObject the caller wants checked against the gameObjcet being held
	 * 					by this instance
	 * Retunr:		(bool)
	 * 					true if the two GameObjects are equal
	 * 					false if the two GameObjects are not equal
	 */
	public bool isGameObjectEqual(GameObject gameObjectToTest)
	{
		return objectGameObject == gameObjectToTest;
	}


	/*
	 * getGameObject method will return the GameObject being held by the instance of this class
	 * Parameters:	none
	 * Return:	(GameObject)
	 * 				the GameObject being held by this instance of this class
	 */
	public GameObject getGameObject()
	{
		return objectGameObject;
	}


	/*
	 * getAllEdges will return an array of all the edges that have no Y coordinate change that make up the 
	 * 		object this instance is for
	 * Parameters:	none
	 * Return:	(Vector3[,])
	 * 				all the edges that have no Y coordinate change that make up the object being held
	 */
	public Vector3[,] getAllEdges()
	{
		return edgesWithNoYChange;
	}



	/*
	 * getMinYCord method will return the smallest Y cordinate value for the object's polygons
	 * Parameter:	none
	 * Return:	(float)
	 * 				the minimal y cordidant for the polygons that make up the object
	 */
	public float getMinYCord()
	{
		float minYCord = 10000f;
		for (int count = 0; count < objectPolygons.Length; count++)
			if (objectPolygons [count].getMinYCord () < minYCord)
				minYCord = objectPolygons [count].getMinYCord ();
		return minYCord;
	}


	/*
	 * getAllEdgesWithNoYChange method will ask each polygon that makes up the object being held by this 
	 * 		instance to get all their edges that have no Y coordinate change
	 * Parameter:	none
	 * Return:	none
	 */
	public void getAllEdgesWithNoYChange()
	{
		int numberForVector3Array = 0;
		float yCord = getMinYCord ();
		for (int count = 0; count < objectPolygons.Length; count++)
			numberForVector3Array += objectPolygons [count].getVerticesCount ();
		Vector3[,] temp = new Vector3[numberForVector3Array, 2]; //array of possible edges that have no Y coordinate change
		int tempCountHeld = 0;
		for (int count = 0; count < objectPolygons.Length; count++)
			objectPolygons [count].getAllEdgesWithNoYChange (temp, ref tempCountHeld); //ask an polygon to get all its edges that have no Y coordniate change
		edgesWithNoYChange = removeReduntantEdges(temp, tempCountHeld, yCord);
		findEdgeVertices();
	}



	/*
	 * findEdgeVertices method will gather together all the vertices that make up the just the other edge of the object
	 * Parameter:	none
	 * Return:	none
	 */
    void findEdgeVertices()
    {
        edgeVertices = new Vector3[edgesWithNoYChange.Length / 2];
		int edgeCount = 0;
		Vector3 tempVector;
		edgeVertices [edgeCount] = new Vector3 (edgesWithNoYChange [0, 0].x, edgesWithNoYChange [0, 0].y, edgesWithNoYChange [0, 0].z);
		edgeCount++;
		edgeVertices [edgeCount] = new Vector3 (edgesWithNoYChange [0, 1].x, edgesWithNoYChange [0, 1].y, edgesWithNoYChange [0, 1].z);
		edgeCount++;
		for (; edgeCount < edgeVertices.Length;) {
			tempVector = findNextVerticeFromPassedIn(edgeVertices[edgeCount-1], edgeCount);
			edgeVertices[edgeCount] = new Vector3(tempVector.x, 0f, tempVector.z);
			edgeCount++;
		}
    }



	/*
	 * findNextVerticeFromPassedIn method will take a vector3 passed in and return a edge verice to add into the edgeVertices array
	 * Parameter:	(Vector3)verticeToFindAdjacent is the last vertice that was put into the array
	 * 				(int)canGoTo is what count the search can go to
	 * Return:		(vector3)
	 * 					the newest found edge
	 */
	Vector3 findNextVerticeFromPassedIn(Vector3 verticeToFindAdjacent, int canGoTo)
	{
		for (int count = 0; count < edgesWithNoYChange.Length/2; count++) {
			if(checkVertices(verticeToFindAdjacent, edgesWithNoYChange[count, 0]) == true)
			{
				if(isVertexInList(edgesWithNoYChange[count, 1], canGoTo) == false)
					return edgesWithNoYChange[count, 1];
			}
			else if(checkVertices(verticeToFindAdjacent, edgesWithNoYChange[count, 1]) == true)
			{
				if(isVertexInList(edgesWithNoYChange[count, 0], canGoTo) == false)
					return edgesWithNoYChange[count, 0];
			}
		}
		return new Vector3 (1000f, 1000f, 1000f);
	}



	/*
	 * isVertexInList method will look to see if the vertex is already in the list
	 * Parameter:	(Vector3)vertexToCheck is the vertex the caller wants to know is in the list
	 * 				(int)canGoTo is the count the search can go to
	 * Return:		(bool)
	 * 					true is if the vertex is already in the list
	 * 					false if the vertex is not in the list
	 */
	bool isVertexInList(Vector3 vertextToCheck, int canGoTo)
	{
		for(int count = 0; count < canGoTo; count++)
		{
			if(checkVertices(vertextToCheck, edgeVertices[count]) == true)
				return true;
		}
		return false;
	}


	/*
	 * findbottomPolygon will get an array of all edges that make up the bottom polygon of the object
	 * Parameter:	none
	 * Return:	none
	 */
	void findbottomPolygon()
	{
		Vector3[] tempArray;
		for (int count = 0; count < objectPolygons.Length; count++) {
			if (objectPolygons [count].isFlat () == true) {
				tempArray = objectPolygons[count].getVertices();
				edgeVertices = new Vector3[tempArray.Length];
				for(int count2  = 0; count2 < tempArray.Length; count2++)
				{
					edgeVertices[count2] = new Vector3(tempArray[count2].x, 0f, tempArray[count2].z);
				}
				return;
			}
		}

	}



	/*
	 * getEdgeVertices will return the array of edges for the object
	 * Parameter:	none
	 * Return:	(Vector3[])
	 * 				the array that make up the edges for the object
	 */
    public Vector3[] getEdgeVertices()
    {
        return edgeVertices;
    }



	/*
	 * checkVertices method will look at two vertices and see if they are the same
	 * Parameter:	(Vector3)thisPolygonVertex is the first vertex to check
	 * 				(Vector3)polygonToMergeVertex is the second vertex to check
	 * Return:		(bool)
	 * 					true if the vertices match
	 * 					false if the vertices dont match
	 */
    public bool checkVertices(Vector3 thisPolygonVertex, Vector3 polygonToMergeVertex)
    {
        Vector3 thisP = new Vector3(thisPolygonVertex.x, 0f, thisPolygonVertex.z);
        Vector3 thatP = new Vector3(polygonToMergeVertex.x, 0f, polygonToMergeVertex.z);
        Vector3 Point = thisP - thatP;

        if (Point.x > .00001f || Point.x < -.00001f)
            return false;
        if (Point.y > .00001f || Point.y < -.00001f)
            return false;
        if (Point.z > .00001f || Point.z < -.00001f)
            return false;
        return true;
    }


    /*
	 * removeReduntantEdges will look at the array that is holding the edges with no Y coordinate changes and remove any edge that
	 * 		is a duplicate of another
	 * Parameter:	(Vector3[,])temp is the array of edges that need the redundant edges removed
	 * 				(int)tempCount is the number of edges that is currently being held
	 * Return:		(Vector3[,])
	 * 					a new array of edges that have no redundant vertices
	 */
    Vector3[,] removeReduntantEdges(Vector3[,] temp, int tempCount, float yCord)
	{
		bool[] redunantEdges = new bool[tempCount];
		for (int count = 0; count < tempCount; count++)
			redunantEdges [count] = true;
		int counter = tempCount;
		for(int count1 = 0; count1 < tempCount; count1++){
			for(int count2 = count1 + 1; count2 < tempCount; count2++){
				if(redunantEdges[count1] == true){
					if(((temp[count1,0].x == temp[count2,0].x) && (temp[count1, 1].x == temp[count2, 1].x)) || ((temp[count1,0].x == temp[count2,1].x) && (temp[count1, 1].x == temp[count2, 0].x))){
						if(((temp[count1,0].z == temp[count2,0].z) && (temp[count1, 1].z == temp[count2, 1].z)) || ((temp[count1,0].z == temp[count2,1].z) && (temp[count1, 1].z == temp[count2, 0].z))){
							redunantEdges[count2] = false;
							counter--;
						}
					}
				}
			}
		}

		Vector3[,] newTemp = new Vector3 [counter, 2];
		for (int count = 0, newTempCount = 0; count < tempCount; count++) {
			if(redunantEdges[count] == true)
			{
				newTemp [newTempCount, 0] = new Vector3 (temp [count, 0].x , yCord, temp [count, 0].z );
				newTemp [newTempCount, 1] = new Vector3 (temp [count, 1].x , yCord, temp [count, 1].z );
				newTempCount++;
			}
		}
		return newTemp;
	}


	//used for debug purposes
	public void drawPolygons()
	{
		for (int count = 0; count < objectPolygons.Length; count++) {
			//if(objectPolygons[count] != null)
				objectPolygons [3].drawSelf (Color.red);
		}
	}


	//used for debug purposes
	public override string ToString()
	{
		return objectGameObject.ToString ();
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
