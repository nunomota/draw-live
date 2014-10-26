using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	private draw drawScript;
	private int posListCounter = 0;

	private bool inMenu = false;

	//menu related stuff
	private Texture menuButtonTexture;
	private Texture menuBackgroundTexture;
	private Texture hostServerTexture;
	private Texture joinServerTexture;
	private GUIStyle buttonStyle;
	private float curMenuPosX = 0.0f;
	private float curMenuPosY = (Screen.height/2.0f) - (Screen.height/12.0f);
	private float menuOffsetX = Screen.width/8.0f;
	private float menuButtonsWidth = Screen.width/8.0f;
	private float menuButtonsHeight = Screen.height/6.0f;
	private float menuSpeed = 180.0f;


	//Network related variables
	public string IP = "192.168.0.1";
	public int PORT = 25000;
	private int maxConnections = 10;

	public bool knowsServerMeasures = false;
	public float serverScreenWidth;
	public float serverScreenHeight;

	//options realted code
	private bool isErasing = false;
	private Texture rubberButtonTexture;

	// Use this for initialization
	void Start () {
		drawScript = (draw)GameObject.Find("Main Camera").GetComponent(typeof(draw));

		menuButtonTexture = (Texture)Resources.Load ("MenuButton");
		menuBackgroundTexture = (Texture)Resources.Load ("MenuBG");
		hostServerTexture = (Texture)Resources.Load ("CreateServerUnpressed");
		joinServerTexture = (Texture)Resources.Load("JoinServerUnpressed");
		rubberButtonTexture = (Texture)Resources.Load ("RubberButton");
		buttonStyle = new GUIStyle();
		buttonStyle.fontSize = 20;

	}
	
	// Update is called once per frame
	void Update () {
		if (Network.peerType == NetworkPeerType.Disconnected) {
			//not connected to a client/server, don't do anything
		} else {
			if (Network.peerType == NetworkPeerType.Server) {
				if (Input.touchCount > 0) {
					UnityEngine.Touch touch = Input.GetTouch(0);
					
					//after toogle between "in" and "not in" menu do something...
					if (inMenu) {
						
					} else {
						
						switch (touch.phase) {
							
						case TouchPhase.Began:
							
							break;
							
						case TouchPhase.Moved:

								//	we want to add a new position each time we move our finger, so that
								//a line is drawn going through all the positions previously, and currently,
								//occupied by our finger;
								networkView.RPC("addPosition", RPCMode.OthersBuffered, new Vector3(touch.position.x, touch.position.y, 0));
								addPosition(touch.position);
								break;
							
						case TouchPhase.Ended:

								//	every time the touch ends, we want the cycle to skip the vector
								//that would be created from the position where we lifted our finger from,
								//to the position where our next touch begins;
								networkView.RPC("addPosition", RPCMode.OthersBuffered, new Vector3(-1, 0, 0));
								addPosition(new Vector3(-1, 0, 0));
								break;
						}
					}
				}
			} else if (Network.peerType == NetworkPeerType.Client) {
				if (!knowsServerMeasures) {
					networkView.RPC("getServerScreen", RPCMode.Server);
					knowsServerMeasures = true;
				}
			}
		}
	}

	void OnGUI () {

		if (Network.peerType == NetworkPeerType.Disconnected) {
			float buttonHeight = Screen.height/3.0f;
			float buttonWidth = buttonHeight;
			float textFieldHeight = Screen.height/20.0f;

			GUI.Label(new Rect(Screen.width/3.0f - buttonWidth/2.0f, Screen.height/2.0f - buttonHeight/2.0f - Screen.height/35.0f, buttonWidth, Screen.height/10.0f), "Host Session", buttonStyle);
			if (GUI.Button(new Rect(Screen.width/3.0f - buttonWidth/2.0f, Screen.height/2.0f - buttonHeight/2.0f, buttonWidth, buttonHeight), hostServerTexture, buttonStyle)) {
				//just became the server
				Network.InitializeServer(maxConnections, PORT, false);
			}

			GUI.Label(new Rect((2*Screen.width)/3.0f - buttonWidth/2.0f, Screen.height/2.0f - buttonHeight/2.0f - Screen.height/35.0f, buttonWidth, Screen.height/10.0f), "Join Session", buttonStyle);
			IP = GUI.TextField(new Rect((2*Screen.width)/3.0f - buttonWidth/2.0f, Screen.height/2.0f + buttonHeight/2.0f + textFieldHeight/2.0f, buttonWidth, textFieldHeight), IP);
			if (GUI.Button(new Rect((2*Screen.width)/3.0f - buttonWidth/2.0f, Screen.height/2.0f - buttonHeight/2.0f, buttonWidth, buttonHeight), joinServerTexture, buttonStyle)) {
				//just became the client
				Network.Connect(IP, PORT);
			}
		} else {

			if (Network.peerType == NetworkPeerType.Server) {

				//draw the menu background
				if (Event.current.type == EventType.Repaint)
				{
					Graphics.DrawTexture(new Rect(0, 0, curMenuPosX, Screen.height), menuBackgroundTexture);
				}
				
				//draw the menu expansion button
				if (GUI.Button(new Rect(curMenuPosX, curMenuPosY, Screen.width/18.0f, Screen.height/6.0f), menuButtonTexture, buttonStyle)) {
					inMenu = !inMenu;
				}
				
				if (inMenu) {
					
					//expand the menu...
					if (curMenuPosX < menuOffsetX) {
						curMenuPosX += Time.deltaTime * menuSpeed;
					} else {
						curMenuPosX = menuOffsetX;
						if(GUI.Button(new Rect(0, 0, menuButtonsWidth, menuButtonsHeight), "Clear")) {
							Network.RemoveRPCs(networkView.viewID);
							networkView.RPC("clearLists", RPCMode.Others);
							clearLists();
						}
					}
					
				} else {
					//shrink the menu...
					if (curMenuPosX > 0.0f) {
						curMenuPosX -= Time.deltaTime * menuSpeed;
					} else {
						curMenuPosX = 0.0f;

						//TODO finish this "rubberButton" behaviour
						/*
						if (GUI.Button(new Rect(0, 0, menuButtonsWidth, menuButtonsHeight), rubberButtonTexture, buttonStyle)) {
							isErasing = !isErasing;
						}
						*/
					}
				}
			} else if (Network.peerType == NetworkPeerType.Client) {
				if (GUI.Button (new Rect(0, 0, Screen.width/10.0f, Screen.height/15.0f), "Disconnect")) {
					Network.Disconnect(250);
				}
			}
		}
	}

	[RPC]
	void addPosition(Vector3 newPos) {
		if (Network.peerType == NetworkPeerType.Client) {
			float newXCoord = (newPos.x * Screen.width) / serverScreenWidth;
			float newYCoord = (newPos.y * Screen.height) / serverScreenHeight;
			drawScript.posList.Add(new Vector2(newXCoord, newYCoord));
			posListCounter++;
		} else {
			drawScript.posList.Add(new Vector2(newPos.x, newPos.y));
			posListCounter++;
		}
	}

	[RPC]
	void clearLists() {
		drawScript.posList.Clear();
		posListCounter = 0;
	}

	[RPC]
	void getServerScreen() {
		networkView.RPC("setServerScreen", RPCMode.Others, new Vector3(Screen.width, Screen.height, 0));
	}

	[RPC]
	void setServerScreen(Vector3 screenVector) {
		serverScreenWidth = screenVector.x;
		serverScreenHeight = screenVector.y;
	}
}
