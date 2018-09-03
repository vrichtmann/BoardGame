using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class minigamePose : MonoBehaviour {

	public bool startGame = false;
	public bool pauseGame = false;
	private float contTime = 0f;
	private minigamePose_npcTarget npc;


	void Start () {
		StartCoroutine (startContGame (3)); 
		npc = GameObject.FindGameObjectWithTag ("Npc").GetComponent<minigamePose_npcTarget> ();
	}
		
	void Update () {
	}

	public IEnumerator startContGame(float contDownValue = 0){
		contTime = contDownValue;
		while (contTime > 0) {
			yield return new WaitForSeconds (1.0f);
			contTime--;
		}
		if (contTime == 0) {
			startGame = true;
			npc.stopPlayersPose ();
		}
		//Debug.Log ("Start");
	}

	public IEnumerator checkResponse(float contDownValue = 0){
		yield return new WaitForSeconds (contDownValue);
		npc.checkPlayersPose ();
	}

	public void stopLevel(){
		pauseGame = true;
		StartCoroutine (checkResponse (0.5f)); 
		Debug.Log ("STOP");
	}
		
}
