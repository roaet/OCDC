using UnityEngine;
using System.Collections;

public class MasterGameController : MonoBehaviour {
	/*
	 * This class is the worst example of a singleton
	 * ever made
	 */
	
	public GameObject gameArea;
	public GameObject lineLevelArrow;
	public tk2dTextMesh timeLabel;
	public tk2dTextMesh lifeLabel;
	public tk2dTextMesh lineLabel;
	public tk2dTextMesh ptsLevel;
	public tk2dTextMesh primaryText;
	public tk2dTextMesh levelText;
	public GameObject jumpSound;
	public GameObject dashSound;
	public GameObject menuMusic;
	public GameObject title;
	public GameObject splash;
	public GameObject helpSprite;
	public tk2dTextMesh titleText;
	
	private static float WTF_PADDING = -15.9819f;	
	private static MasterGameController instance;
	private int obstacles = 2;
	private int obstaclesFinished = 0;
	private bool gameRunning;
	private LevelLoader levelLoader;
	private int lineGoal;
	private ArrayList blocks;
	private bool waitingForSpace;
	private bool playerHitSpike;
	private bool eggSpiked;
	private bool gameover;
	private string messageText;
	private string levelMessageText;
	private bool startingRun = true;
	private string gamestate;
	private float startTime;
	private int completed = 1;
	
	void Awake() {
		Application.targetFrameRate = 30;	
	}
	
	// Use this for initialization
	
	
	void Start () {
		gamestate = "splash";
		blocks = new ArrayList();
		instance = this;	
		playerHitSpike = gameRunning = false;
		levelLoader = new LevelLoader(gameArea);
		ClearBlockRegistry();
		lineGoal = 0;
		levelMessageText = "";
		TextAsset text = Resources.Load("levels") as TextAsset;
		levelLoader.loadLevels(text.text);
		gameover = false;
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(gamestate == "splash") {
			titleText.text = "";
			titleText.Commit();
			if(Input.anyKey || Time.time > startTime + 5.0f) {
				splash.transform.position = (new Vector3(
					splash.transform.position.x,
					splash.transform.position.y,
					splash.transform.position.z -100.0f)
				);
				gamestate = "title";
			}
		} else if(gamestate == "title") {
			if(startingRun) {
				titleText.text = "Press Space to Start";
				titleText.Commit();
				menuMusic.audio.enabled = true;
				menuMusic.audio.loop = true;
				menuMusic.audio.Play();
				startingRun = false;
			}
			if(Input.GetKeyDown(KeyCode.Space)) {
				helpSprite.transform.position = (new Vector3(
					helpSprite.transform.position.x,
					helpSprite.transform.position.y,
					helpSprite.transform.position.z -1.1f)
				);
				gamestate = "help";
			}
			
		} else if(gamestate == "help"){
			helpSprite.renderer.enabled = true;
			titleText.text = "Press Space to Continue";
			titleText.Commit();
			if(Input.GetKeyDown(KeyCode.Space)) {
				title.transform.position = (new Vector3(
					title.transform.position.x,
					title.transform.position.y,
					title.transform.position.z -100)
				);
				gamestate = "game";
				//menuMusic.audio.Stop();
			}
			titleText.renderer.enabled = false;
		} else if(gamestate == "game") {
			if(gameRunning) {
				bool autowin = false;
				if(Input.GetKeyDown(KeyCode.N)) {
					autowin = true;
				}
				if(Input.GetKeyDown(KeyCode.Escape)) {
					levelLoader.unloadLevel();
					levelLoader.loadCurrentLevel();
				}
				if(playerHitSpike) {
					gameRunning = false;
					levelLoader.unloadLevel();
					primaryText.text = "Ouch! Sharp and pointies";
					waitingForSpace = true;
					primaryText.Commit();
					levelText.text = "";
					levelText.Commit();
				} else if(eggSpiked) {
					gameRunning = false;
					levelLoader.unloadLevel();
					primaryText.text = "Protect the eggs!";
					waitingForSpace = true;
					primaryText.Commit();
					levelText.text = "";
					levelText.Commit();
				} else if(autowin || CheckCompletion()) {
					gameRunning = false;
					if(levelLoader.hasNextLevel()) {
						levelLoader.unloadLevel();
						if(messageText != null && messageText.Length > 0) {
							primaryText.text = messageText;
						} else {
							primaryText.text = "Good job!";	
						}
						waitingForSpace = true;
						primaryText.Commit();
						levelText.text = "";
						levelText.Commit();
						completed++;
						ptsLevel.text = completed.ToString();
						ptsLevel.Commit();
					} else {
							
						levelLoader.unloadLevel();
						primaryText.text = "Game Over! You Win!";
						waitingForSpace = false;
						gameover = true;
						primaryText.Commit();
						levelText.text = "";
						levelText.Commit();
					}
				}
			} else {
				// Lets test this level loader shit
				if(waitingForSpace) {
					if(Input.GetKeyDown(KeyCode.Space)) {
						waitingForSpace = false;
						if(playerHitSpike || eggSpiked) {
							levelLoader.unloadLevel();
							levelLoader.loadCurrentLevel();
							eggSpiked = playerHitSpike = false;
						} else {
							messageText = "";
							levelMessageText = "";
							levelLoader.LoadNextLevel();
							levelText.text = levelMessageText;
							levelText.Commit();
						}
						gameRunning = true;
						primaryText.text = "";
						primaryText.Commit();
					}
				} else if(!gameover) {
					levelLoader.unloadLevel();
					levelLoader.loadCurrentLevel();
					levelText.text = levelMessageText;
					levelText.Commit();
					ptsLevel.text = completed.ToString();
					ptsLevel.Commit();
					gameRunning = true;
				}
			}
		}
	}
	
	public void ShowLevelText(string text) {
		levelMessageText = text;	
	}
	
	public void ShowMessageText(string text) {
		messageText = text;
	}
	
	public void PlayerTouchedSpike() {
		playerHitSpike = true;
	}
	
	public void SpikeTouchedEgg() {
		eggSpiked = true;	
	}
	
	public void PlayerJumped() {
		jumpSound.GetComponent<AudioSource>().audio.Play();
	}
	
	public void PlayerDashed() {
		dashSound.GetComponent<AudioSource>().audio.Play();
	}
	
	public float GetPositionForColumn(float col) {
		return col * 32.0f;	
	}
	
	public float GetPositionForLine(float col) {
		return col * 32.0f;	
	}
	
	private bool CheckCompletion() {
		foreach(GameObject block in blocks) {
			float totalHeight = block.transform.position.y + 32.0f + WTF_PADDING;
			//Debug.Log("Comparing " + totalHeight + " against " + GetPositionForLine(lineGoal));
			if(totalHeight > GetPositionForLine(lineGoal))
				return false;
		}
		return true;
	}
	
	public void RegisterBlock(GameObject block) {
		if(!blocks.Contains(block)) 
			blocks.Add(block);
	}
	
	public void ClearBlockRegistry() {
		blocks.RemoveRange(0, blocks.Count);
	}
	
	public void SetLevelLineGoal(int lineGoal) {
		this.lineGoal = lineGoal;
		lineLevelArrow.transform.position = new Vector3(lineLevelArrow.transform.position.x,
			lineGoal * 32.0f, lineLevelArrow.transform.position.z);
		lineLabel.text = lineGoal.ToString();
		lineLabel.Commit();
	}
	
	public static MasterGameController Instance {
		get {
			return instance;	
		}
	}
	
}
