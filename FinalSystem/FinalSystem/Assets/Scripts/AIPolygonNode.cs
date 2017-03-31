/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIPolygonNode class
 * This class will act as a cotainer for the AIPolygon. It will also act as a Node inside
 *		the AIPolygonQueue class. It will be able to store a AIPolygon, hold a reference to
 *		to the next Node in the queue, set the nextNode reference to another Node, return
 *		the reference to the Next Node, return the AIPolygon being held by this class, and
 *		check if an ID being passed in is the same as the ID of the AIPolygon being held
 */
﻿using UnityEngine;
using System.Collections;

public class AIPolygonNode {

	AIPolygonNode nextNode; // reference to the nextNode in the queue
	AIPolygon polygon; // the polygon being held


	/*
	 *	AIPolygonNode is the constructor for this class. It will set the AIPolygon being held
	 *		, and the initial reference to the next Node in the queue
	 *	Parameter:	(AIPolygon)thisPolygon is the AIPolygon needing to be held by this Node
	 *							(AIPolygonNode)nextPolygonNode is the initial reference to the next Node
	 *									in the queue
	 *	Return:	None
	 */
	public AIPolygonNode(AIPolygon thisPolygon, AIPolygonNode nextPolygonNode)
	{
		polygon = thisPolygon;
		nextNode = nextPolygonNode;
	}


	/*
	 *	setNextNode method will take in a new NextNode reference from the caller and
	 *			set the nextNode reference to the new Node reference
	 *	Parameter:	(AIPolygonNode) newNextNode is the new reference for NextNode
	 *	Return:	None
	 */
	public void setNextNode(AIPolygonNode newNextNode)
	{
		nextNode = newNextNode;
	}

	/*
	 *	AIPolygonNode method will return the NextNode reference to the caller
	 *	Parameter:	None
	 *	Return:	(AIPolygonNode)
	 *							the reference in NextNode
	 */
	public AIPolygonNode getNextNode()
	{
		return nextNode;
	}


	/*
	 *	getPolygon will return the AIPolygon being held by this Node
	 *	Parameter:	None
	 *	Return:		(AIPolygon)
	 *								the AIPolygon being held by this object
	 */
	public AIPolygon getPolygon()
	{
		return polygon;
	}


	/*
	 * doesIDMatch will check to see if an ID being passed in matches the ID of the AIPolygon
	 *		being held by this object
	 *	Parameter:	(int)idToCheck is the ID that the caller wants to check against the AIPolygon
	 *											being held
	 *	Return:			(bool)
	 *								false if the idToCheck does not match the ID of the AIPolygon being held
	 *								true if the idToCheck matches the ID of the AIPolygon being held
	 */
	public bool doesIDMatch(int idToCheck)
	{
		if (idToCheck == polygon.getID ())
			return true;
		return false;
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
