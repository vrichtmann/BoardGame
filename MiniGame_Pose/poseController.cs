using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poseController : MonoBehaviour {
	private Vector3 inposLeftHand;
	private Vector3 inposRigthHand;
	private Vector3 inposLeftleg;
	private Vector3 inposRigthLeg;

	public GameObject head;
	public GameObject leftHand;
	public GameObject rigthHand;
	public GameObject leftleg;
	public GameObject rigthLeg;

	public ArrayList pressButtons = new ArrayList();

	private string[] buttonsController = new string[]{"A", "Y", "X", "B"};

	private Vector2 velocidade;
	private float distance = 1.5f;
	private float tiltAngle = 30f;
	private string currentPlayer = "player1";
	public Animator animator;

	public bool npc = false;
	public string npcLevel = "normal";
	private bool mvhNPC = false;

	public bool isGameOver = false;

	private minigamePose minigame;

	void Start () {
		currentPlayer = this.name;
		inposLeftHand = leftHand.transform.position;
		inposRigthHand = rigthHand.transform.position;
		inposLeftleg = leftleg.transform.position;
		inposRigthLeg = rigthLeg.transform.position;

		minigame = GameObject.FindGameObjectWithTag ("GameController").GetComponent<minigamePose> ();
		animator = this.GetComponent<Animator> ();
	}

	void Update () {
		if(!minigame.pauseGame && npc == false && !isGameOver)setPlayerControler ();
		if(npc) npcUpdate ();
	}

	private void setPlayerControler (){
		velocidade = new Vector2 (Input.GetAxis ("Horizontal_" + currentPlayer) * tiltAngle, Input.GetAxis ("Vertical_" + currentPlayer) * tiltAngle);

		Quaternion target = Quaternion.Euler (velocidade.y, -(velocidade.x), 0);
		head.transform.rotation = Quaternion.Slerp (head.transform.rotation, target, Time.deltaTime * 5);

		if (Input.GetAxis ("xbox_A_" + currentPlayer) != 0) {
			
			moveBodyParts ("A");
			if(pressButtons.IndexOf("A") == -1)pressButtons.Add ("A");
		} else {
			if(pressButtons.IndexOf("A") != -1)pressButtons.RemoveAt(pressButtons.IndexOf("A"));
			leftleg.transform.position = inposLeftleg;
		}

		if (Input.GetAxis ("xbox_Y_" + currentPlayer) != 0) {
			moveBodyParts ("Y");
			if(pressButtons.IndexOf("Y") == -1)pressButtons.Add ("Y");
		} else {
			if(pressButtons.IndexOf("Y") != -1)pressButtons.RemoveAt(pressButtons.IndexOf("Y"));
			rigthHand.transform.position = inposRigthHand;
		}

		if (Input.GetAxis ("xbox_X_" + currentPlayer) != 0) {
			moveBodyParts ("X");
			if(pressButtons.IndexOf("X") == -1)pressButtons.Add ("X");
		} else {
			if(pressButtons.IndexOf("X") != -1)pressButtons.RemoveAt(pressButtons.IndexOf("X"));
			leftHand.transform.position = inposLeftHand;
		}

		if (Input.GetAxis ("xbox_B_" + currentPlayer) != 0) {
			moveBodyParts ("B");
			if(pressButtons.IndexOf("B") == -1)pressButtons.Add ("B");
		} else {
			if(pressButtons.IndexOf("B") != -1)pressButtons.RemoveAt(pressButtons.IndexOf("B"));
			rigthLeg.transform.position = inposRigthLeg;
		}
	}

	// // // // // // // // // // // // // // // // // // // NPC CONTROLLER
	private void npcUpdate(){
		if (mvhNPC) {
			Quaternion target = Quaternion.Euler (velocidade.x, velocidade.y, 0);
			head.transform.rotation = Quaternion.Slerp (head.transform.rotation, target, Time.deltaTime * 5);
		}
	}

	public void setNPCControler(ArrayList _sortBodyButtons, Vector2 _sortHead, int _currentLevel){//Verlho, Amarelo, Verde se mexerem
		pressButtons = new ArrayList();
		bool loose = setDificult (npcLevel, _currentLevel);

		if (loose) {
			Debug.Log ("LOOSE");

			int npcErro = Random.Range (0, 2);
			Vector2 targetHead = new Vector2 (_sortHead.x, _sortHead.y);
			if (npcErro == 0) {////////////////////////////////////////////////////body parts
				Debug.Log ("ERROR BODY");
				int sortButtonsLength = _sortBodyButtons.Count;
				if (sortButtonsLength == 1) {
				} else if (sortButtonsLength > 1 && sortButtonsLength < 4) {
					int k = Random.Range (0, 2);
					if (k == 0) {
						CopyArray (pressButtons, _sortBodyButtons);
						pressButtons.RemoveAt (0);
					} else {
						CopyArray (pressButtons, _sortBodyButtons);
						addNewButton (pressButtons);
					}
				} else {//////////////////////////////////////////////////////////head  parts
					pressButtons = _sortBodyButtons;
					pressButtons.RemoveAt (0);
				}
			} else {
				Debug.Log ("ERROR HEAD");
				CopyArray (pressButtons, _sortBodyButtons);
				targetHead = new Vector2 (Mathf.FloorToInt(Random.Range (-1, 2)) * tiltAngle, Mathf.FloorToInt(Random.Range (-1, 2)) * tiltAngle);
			}

			for (int i = 0; i < pressButtons.Count; i++) {
				moveBodyParts (pressButtons[i] as string);
			}
			moveHead (targetHead);
		
		} else {
			for (int i = 0; i < _sortBodyButtons.Count; i++) {
				pressButtons.Add (_sortBodyButtons[i] as string);
				moveBodyParts (_sortBodyButtons[i] as string);
				moveHead (_sortHead);
			}
		}
	
	}

	private void CopyArray(ArrayList _currentArray, ArrayList _targetArray){
		for (int i = 0; i < _targetArray.Count; i++) {
			_currentArray.Add (_targetArray [i]);
		}
	}

	private void addNewButton(ArrayList _sortBodyButtons){
		for (int i = 0; i < buttonsController.Length; i++) {
			if (_sortBodyButtons.IndexOf (buttonsController [i]) == -1) {
				pressButtons.Add (buttonsController [i]);
				Debug.Log ("ADICIONA BOTAO");
				break;
			}
		}
	}

	private bool setDificult(string dificult, int _level){

		int passLevel = 0;
		if(dificult == "easy"){
			passLevel = Random.Range (2, 6);
			if (_level > passLevel) {
				return true;
			}
		}else if(dificult == "normal"){
			passLevel = Random.Range (3, 8);
			if (_level > passLevel) {
				return true;
			}
		}else if(dificult == "hard"){
			passLevel = Random.Range (5, 9);
			if (_level > passLevel) {
				return true;
			}
		}
		return false;
	}

	private void moveBodyParts(string _button){
		if (_button == "A") {
			leftleg.transform.localPosition = new Vector3 ( -(distance - 0.5f), leftleg.transform.localPosition.y, leftleg.transform.localPosition.z);
		}else if (_button == "Y") {
			rigthHand.transform.localPosition = new Vector3 (distance, rigthHand.transform.localPosition.y, rigthHand.transform.localPosition.z);
		}else if (_button == "X") {
			leftHand.transform.localPosition = new Vector3 ( -distance, leftHand.transform.localPosition.y, leftHand.transform.localPosition.z);
		}else if (_button == "B") {
			rigthLeg.transform.localPosition = new Vector3 ( (distance - 0.5f), rigthLeg.transform.localPosition.y, rigthLeg.transform.localPosition.z);
		}
	}

	public void resetPositionsButtons(){
		velocidade = new Vector2 (0, 0);
		leftHand.transform.position = inposLeftHand;
		rigthHand.transform.position = inposRigthHand;
		leftleg.transform.position = inposLeftleg;
		rigthLeg.transform.position = inposRigthLeg;
	}

	private void moveHead(Vector2 _headPosition){
		mvhNPC = true;
		velocidade = _headPosition;
	}
	// // // // // // // // // // // // // // // // // // // NPC CONTROLLER

	public void stopAnimation(){
		Animator anim = GetComponent<Animator> ();
		anim.enabled = false;
	}

	public void gameOver(){
		Animator anim = GetComponent<Animator> ();
		anim.enabled = true;
		isGameOver = true;
		resetPositionsButtons ();
		velocidade = new Vector2 (0, 0);
	}

	public void win(){
		animator.SetBool ("isWin", true);
	}

	public void lose(){
		animator.SetBool ("isGameOver", true);
	}
}
