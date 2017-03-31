/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AgentMovement class
 * This class will control the movement of the ZomBear
 */

﻿using UnityEngine;


public class AgentMovement : MonoBehaviour
{
	public float duration = 1f;
	Vector3 movement;
	Animator anim;
	public static bool canWalk = false;
	Vector3[] wayPointLocations;
	Vector3 startPoint;
	Vector3 endPoint;
	Vector3 positionToLook;
	float startTime;
	int currentWayPoint;
	Vector3 direction;

  // Start begins the ZomBear on his journey
	void Start()
	{
		currentWayPoint = 0;
		anim = GetComponent<Animator> ();
		startTime = Time.time;
	}

  /* The Update method controls the movement of the zomBear along the path
	 * ensuring clean rotations at pivot points.
	 */
	void Update()
	{
		if (canWalk == true) {
			float journeyLength = Vector3.Distance(startPoint, endPoint);
			float i = 1.5f *(((Time.time - startTime) * duration)/ journeyLength);
			Animating();
			direction = Vector3.RotateTowards(transform.forward, (positionToLook - transform.position), 3f * Time.deltaTime, 0.0f);
			transform.position = Vector3.Lerp(startPoint, endPoint,i);
			transform.rotation = Quaternion.LookRotation(direction);
			if(transform.position.Equals(endPoint) == true && currentWayPoint < wayPointLocations.Length)
			{
				startTime = Time.time;
				currentWayPoint++;
				startPoint = endPoint;
				endPoint = wayPointLocations[currentWayPoint];
				positionToLook = endPoint;
				direction = Vector3.RotateTowards(transform.forward, (positionToLook - transform.position), 3f * Time.deltaTime, 0.0f);
				transform.rotation = Quaternion.LookRotation(direction);
			}
		}
	}

  /* The startMovement method sets the waypoints generated by the search
	 * for the zomBear to follow
	 * Parameters: Vector3[] wayPointToAdd - next waypoint for the zomBear to travel to
	 */
	public void startMovement(Vector3[] wayPointToAdd)
	{
		canWalk = true;
		wayPointLocations = wayPointToAdd;
		startPoint = wayPointLocations [currentWayPoint];
		startTime = Time.time;
		currentWayPoint++;
		endPoint = wayPointToAdd [currentWayPoint];
		positionToLook = endPoint;
		direction = Vector3.RotateTowards(transform.forward, (positionToLook - transform.position), 3f * Time.deltaTime, 0.0f);
		transform.rotation = Quaternion.LookRotation(direction);
	}

  /* The OnCollisionEnter method is used to make him fall over when he gets to
	 * the goal.
	 * Parameters: Collsion collision - gameobject that if going to collide with the goal
	 */
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Goal") {
			anim.SetTrigger ("GotGoal");
			canWalk = false;
		}
	}

  // The method Animating controls the animation of the zomBear walking
	void Animating()
	{
		anim.SetBool ("IsWalking", canWalk);
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