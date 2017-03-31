/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIPolygonQueue class
 * This class will act as a Queue DataStructe. It will be implemented as a LIFO Queue This means it will hold AIPolygonNode
 *		and be able to manipulate them accordingly. This class can add AIPolygonNodes
 *		, delete AIPolygonNodes, check if the queue is empty, get the size of the queue,
 *		get a polygon at certian index in the queue, and delete a specific node with a
 *		certian ID. This is a container class for AIPolygonNode
 */
﻿using UnityEngine;
using System.Collections;

public class AIPolygonQueue {
	AIPolygonNode front; //this is the front reference to the queue
	int size; //this is the size of the queue


	/*
	 * AIPolygonQueue is the constructor for this class. It will set up the queue as
	 * 		empty
	 */
	public AIPolygonQueue()
	{
		front = null;
		size = 0;
	}


	/*
	 * enqueue method will place a new node inside the Queue at the front
	 * Parameters:	(AIPolygon)polygonToAdd is the polygon that needs to be added
	 											to a node to add into the queue
	 * Return:	None
	 */
	public void enqueue(AIPolygon polygonToAdd)
	{
		AIPolygonNode newNode = new AIPolygonNode (polygonToAdd, front);
		front = newNode;
		size++;
	}


	/*
	 * isEmpty will see if the queue is empty or if the queue is not empty
	 * Parameters:	None
	 * Return:	(bool)
	 *						true if the queue is empty
	 *						false if the queue is Not empty
	 */
	public bool isEmpty()
	{
		if (front == null)
			return true;
		return false;
	}


	/*
	 * dequeue method will remove the top Node from the Queue and return the AIPolygon being
	 * 		held inside the node being removed
	 * Parameters:	None
	 * Return:	(AIPolygon)
	 *							null if the queue is empty
	 *							the AIPolygon being held if the queue is not empty
	 */
	public AIPolygon dequeue()
	{
		AIPolygonNode temp = front;
		front = temp.getNextNode ();
		size--;
		return temp.getPolygon();
	}


	/*
	 * getSize method will return the size of the queue
	 * Parameters:	None
	 * Return:	(int)
	 *						the size of the Queue
	 */
	public int getSize()
	{
		return size;
	}



	//used for debugging
	public override string ToString()
	{
		string temp = "";
		return temp;
	}

	/*
	 *	getPolygonAtIndex will find a AIPolygonNode at a certian point inside the Queue
	 *		and return the AIPolygon being held inside the node. It will not remove the
	 *		Node from the Queue
	 *	Parameters:	(int)index is the index in the Queue that the caller wants sent back
	 *	Return:			(AIPolygon)
	 *									the AIPolygon at the index in the Queue
	 */
	public AIPolygon getPolygonAtIndex(int index)
	{
		if (index == 0)
			return front.getPolygon ();
		AIPolygonNode temp = front;
		for (int count = 0; count < index; count++)
			temp = temp.getNextNode ();
		return temp.getPolygon ();
	}


	/*
	 *	deleteNodeOfID method will delete the node at a certian index in the Queue. It will Not
	 *			return the AIPolygon being held or the AIPolygonNode
	 *	Parameters:	(int)idToDelete is the position at which the caller want the node to delete
	 *	Return:	None
	 */
	public void deleteNodeOfID(int idToDelete)
	{

		AIPolygonNode tempFront = front;
		if (front != null) {
			if (front.doesIDMatch (idToDelete) == true) {
				front = front.getNextNode ();
				return;
			}
			AIPolygonNode tempBack = front.getNextNode ();
			while (tempBack.getNextNode() != null && tempBack.doesIDMatch(idToDelete) == false) {
				tempFront = tempBack;
				tempBack = tempFront.getNextNode ();
			}
			if (tempBack.doesIDMatch (idToDelete) == true) {
				tempFront.setNextNode (tempBack.getNextNode ());
				return;
			}
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