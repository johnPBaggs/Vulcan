/*AI programming Project
 * John P Baggs and Mathew S. Renner
 * AIDataCanvas class
 * This class will be used only to turn off the data canvas before it can show up on with the
 * 		start menu
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIDataCanvas : MonoBehaviour {

	public static int alreadySet = 0;
	public Canvas dataCanvas;

	/*
	 * Start method will be the called when the scene is started and will turn off the 
	 * 		data canvas
	 */
	void Start () {
		if(AIDataCanvas.alreadySet == 0)
			dataCanvas.enabled = false;
	}
	
	// not used
	void Update () {
	
	}
}
