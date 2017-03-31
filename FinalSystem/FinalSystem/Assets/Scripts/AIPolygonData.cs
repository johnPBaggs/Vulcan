/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIPolygonData class
 * This class will hold all the data for a polygon and is created to be serialized into a binary
 * 		file
 */

using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class AIPolygonData 
{


	public float[,] vertices; //vertices that make up a polygon
	public int[] neighbors; //neighbors of the polygon
	public int neighborCount; //number of neighbors of the polygon
	public bool gotGoal; //if the polygon holds the goal
	public bool gotAgent; //if the polygon holds the agent
	public int agentID;
	public int id; //the id of the polygon
	
}
