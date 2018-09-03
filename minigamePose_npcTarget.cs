using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minigamePose_npcTarget : MonoBehaviour {
	private Vector3 inposLeftHand;
	private Vector3 inposRigthHand;
	private Vector3 inposLeftleg;
	private Vector3 inposRigthLeg;

	public GameObject head;
	public GameObject leftHand;
	public GameObject rigthHand;
	public GameObject leftleg;
	public GameObject rigthLeg;

	private Vector2 velocidade;
	private int currentLevel = 1;
	private int[] gameLevel = new int[]{1, 1, 1, 1, 1, 2, 2, 2, 4, 4, 4, 3, 3};
	private string[] buttonsController = new string[]{"A", "Y", "X", "B"};
	private ArrayList sortButtons = new ArrayList();

	private float distance = 1.5f;
	private float tiltAngle = 30;

	private bool mvHead = false;
	private float targetRotationX = 0;
	private float targetRotationY = 0;

	private int contGameOver = 0;
	private int contDancing = 0;
	private int maxDancing = 3;

	private minigamePose minigame;
	private GameObject[] players;

	private Animator animator;

	void Start () {
		inposLeftHand = leftHand.transform.position;
		inposRigthHand = rigthHand.transform.position;
		inposLeftleg = leftleg.transform.position;
		inposRigthLeg = rigthLeg.transform.position;

		minigame = GameObject.FindGameObjectWithTag ("GameController").GetComponent<minigamePose> ();
		players = GameObject.FindGameObjectsWithTag ("Player");
		animator = this.GetComponent<Animator> ();
	}

	void Update () {
		if(mvHead)moveHead ();
	}

	public void danceAgain(){
		currentLevel++;
		contDancing = 0;

		minigame.pauseGame = false;
		targetRotationX = 0;
		targetRotationY = 0;
		sortButtons = new ArrayList();
		GetComponent<Animator> ().enabled = true;

		leftHand.transform.position = inposLeftHand;
		rigthHand.transform.position = inposRigthHand;
		leftleg.transform.position = inposLeftleg;
		rigthLeg.transform.position = inposRigthLeg;
	}

	private void checkDance(){//Danca 3 vezes de pois faz uma pose
		if (minigame.startGame) {
			contDancing++;
			if (contDancing == maxDancing) {
				dancePose ();
			}
		}
	}

	private void dancePose(){//Movimento para copiar(fez alguma pose)
		mvHead = false;
		GetComponent<Animator> ().enabled = false;
		moveBody ();
		moveHead ();
		StartCoroutine (startPausedTime (3)); //Tempo para acabar o level(chorinho)
		StartCoroutine (delayNPCTime (1));//Tempo Mexer npc
	}

	private void moveBody(){//1mover o corpo
		int numberParts = currentLevel < gameLevel.Length ? gameLevel[currentLevel - 1] : 4;
		int rdnPart = Random.Range (1, numberParts);
		Debug.Log ("rdnPart : " + rdnPart);

		for (int i = 0; i < numberParts; i++) {//Numero de partes sorteadas
			int n = Random.Range (1, 5);//Qualparte vai sortear

			while (sortButtons.IndexOf (buttonsController[n - 1]) != -1) {
				n = Random.Range (1, 5);
			}
			//1 = A , 2 = Y, 3 = X, 4 = B
			if (buttonsController[n - 1] == "A") {
				leftleg.transform.localPosition = new Vector3 ( -(distance - 0.5f), leftleg.transform.localPosition.y, leftleg.transform.localPosition.z);
			}
			if (buttonsController[n - 1] == "Y") {
				rigthHand.transform.localPosition = new Vector3 (distance, rigthHand.transform.localPosition.y, rigthHand.transform.localPosition.z);
			}
			if (buttonsController[n - 1] == "X") {
				leftHand.transform.localPosition = new Vector3 ( -distance, leftHand.transform.localPosition.y, leftHand.transform.localPosition.z);
			}
			if (buttonsController[n - 1] == "B") {
				rigthLeg.transform.localPosition = new Vector3 ( (distance - 0.5f), rigthLeg.transform.localPosition.y, rigthLeg.transform.localPosition.z);
			}
			sortButtons.Add (buttonsController[n - 1]);
		}
	}

	private void moveHead(){//Move a cabeca
		if(mvHead == false){
			mvHead = true;
			targetRotationX = Mathf.FloorToInt(Random.Range (-1, 2)) * tiltAngle;
			targetRotationY = Mathf.FloorToInt(Random.Range (-1, 2)) * tiltAngle;
		}
		Quaternion target = Quaternion.Euler (targetRotationX, targetRotationY, 0);
		head.transform.rotation = Quaternion.Slerp (head.transform.rotation, target, Time.deltaTime * 10);
	}
		
	public void stopPlayersPose(){
		for (int i = 0; i < players.Length; i++) {
			players [i].GetComponent<poseController> ().stopAnimation ();
		}
	}

	public void checkPlayersPose(){
		bool playerPass = false;
		for (int i = 0; i < players.Length; i++) {
			bool playerBodyPass = checkBody (players [i]);
			bool playerHeadPass = checkHead (players [i]);
			if (playerBodyPass && playerHeadPass) {
				poseController currentPlayer = players [i].GetComponent<poseController> ();
				if (currentPlayer.npc) {
					currentPlayer.resetPositionsButtons ();
				}
				playerPass = true;
				players [i].GetComponent<poseController> ().animator.SetBool ("rigthPose", true);
				players [i].GetComponent<poseController> ().animator.enabled = true;
			} else if(!players [i].GetComponent<poseController> ().isGameOver) {
				players [i].GetComponent<poseController> ().animator.SetBool ("wrongPose", true);
				players [i].GetComponent<poseController> ().gameOver ();
				contGameOver++;
			}
		}

		StartCoroutine (timeFeedback (playerPass)); 
	}

	public IEnumerator timeFeedback(bool _pass){
		
		yield return new WaitForSeconds (3);
		for (int i = 0; i < players.Length; i++) {
			poseController pose = players [i].GetComponent<poseController> ();
			pose.animator.SetBool ("rigthPose", false);
			pose.animator.SetBool ("wrongPose", false);
			if (!pose.isGameOver)
				pose.animator.enabled = false;
		}
		if (contGameOver >= 3)gameOver ();
		if(_pass && contGameOver < 3)danceAgain ();
	}

	private bool checkBody(GameObject _player){
		ArrayList pressedButtons = _player.GetComponent<poseController> ().pressButtons;
		if (pressedButtons.Count == sortButtons.Count) {
			if (pressedButtons.Count == 0)return true;
			for (int j = 0; j < sortButtons.Count; j++) {
				if (pressedButtons.IndexOf (sortButtons [j]) != -1) {
				} else {
					return false;
				}
			}
		} else {
			return false;
		}
		return true;
	}

	private bool checkHead(GameObject _player){
		GameObject head = _player.GetComponent<poseController> ().head;
		Vector3 rotacao = head.transform.rotation.eulerAngles;
		float marginError = 12f;
		float headX = head.transform.localRotation.x * 100;
		float headY = head.transform.localRotation.y * 100;
		float totalHeadX = headX + tiltAngle;//player
		float totalHeadY = headY + tiltAngle;
		float totalRotationX = targetRotationX + tiltAngle;//npc
		float totalRotationY = targetRotationY + tiltAngle;

		if (totalHeadX > (totalRotationX - marginError) && totalHeadX < (totalRotationX + marginError)) {
			if (totalHeadY > (totalRotationY - marginError) && totalHeadY < (totalRotationY + marginError)) {
				return true;
			}
		}

		return false;
	}

	public IEnumerator startPausedTime(float contDownValue = 1){
		yield return new WaitForSeconds (contDownValue);
		//Debug.Log ("ACABOU LEVEL");
		minigame.stopLevel ();
	}

	// // // // // // // // // // // // // // NPC
	public IEnumerator delayNPCTime(float contDownValue = 1){
		yield return new WaitForSeconds (contDownValue);
		setNpcPose ();
	}

	private void setNpcPose (){//NPC MEXE O CORPO
		for (int i = 0; i < players.Length; i++) {
			poseController playerPose = players[i].GetComponent<poseController> ();
			if (playerPose.npc && !playerPose.isGameOver) {
				playerPose.setNPCControler (sortButtons, new Vector2(targetRotationX, targetRotationY), currentLevel);
			}
		}
	}
	// // // // // // // // // // // // // // // // /

	private void gameOver(){
		Debug.Log ("GAME OVER");
		GetComponent<Animator> ().enabled = true;
		animator.SetBool ("isGameOver", true);
		targetRotationX = 0;
		targetRotationY = 0;
		leftHand.transform.position = inposLeftHand;
		rigthHand.transform.position = inposRigthHand;
		leftleg.transform.position = inposLeftleg;
		rigthLeg.transform.position = inposRigthLeg;

		for (int i = 0; i < players.Length; i++) {
			poseController playerPose = players[i].GetComponent<poseController> ();

			if (!playerPose.isGameOver) {
				playerPose.gameOver ();
				playerPose.win ();
			} else {
				playerPose.lose ();
			}
		}
	}
}
