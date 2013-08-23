using UnityEngine;
using System.Collections;

public class LevelLoader : Object {
	private GameObject gameLayer;
	ArrayList createdObjects;
	private Vector2 glpos;
	private ArrayList levelSet;
	int currentLevel;
	private GameObject characterSprite;
	
	public LevelLoader(GameObject gameLayer) {
		this.gameLayer = gameLayer;
		createdObjects = new ArrayList();
		levelSet = new ArrayList();
		glpos = gameLayer.transform.position;
		currentLevel = 0;
		characterSprite = null;
		
	}
	
	public bool hasNextLevel() {
		if(currentLevel + 1 >= levelSet.Count) return false;
		return true;
	}
	
	public void DestroyCharacter() {
		if(characterSprite) {
			Destroy(characterSprite);	
		}
		characterSprite = null;
	}
	
	public void unloadLevel() {
		MasterGameController.Instance.ClearBlockRegistry();	
		DestroyCharacter();
		foreach(GameObject block in createdObjects) {
			Destroy(block);	
		}
	}
	
	public void loadCurrentLevel() {
		ArrayList curList = levelSet[currentLevel] as ArrayList;
		DestroyCharacter();
		foreach(string line in curList) {
			loadString (line);
		}
	}
	
	public void LoadNextLevel() {
		unloadLevel();
		currentLevel++;
		loadCurrentLevel();
	}
	
	public void loadLevels(string fullLevelText) {
		string[] lines = fullLevelText.Split('\n');
		int levelCounter = 0;
		int currentLevelSlot = levelCounter;
		foreach(string line in lines) {
			if(line.StartsWith("=")) { // we found a new level
				if(currentLevelSlot != levelCounter) currentLevelSlot = levelCounter;
				levelCounter++;
				levelSet.Add(new ArrayList());
			} else if(line.StartsWith("'")) { // we found a comment, ignore it
				continue;
			} else {
				(levelSet[currentLevelSlot] as ArrayList).Add (line);
			}
		}
		Debug.Log("Found " + levelCounter + " levels from "+ lines.Length + " lines");
	}
	
	public void loadLevel(string level) {
		UnityEngine.Debug.Log("Loaded level: " + level);
		if(level == "1") {
			loadString ("setup,1,0");
			loadString ("base ,01100111011101,0");
			loadString ("paper,00010000000000,9");
			loadString ("paper,10000000000000,8");
			loadString ("stone,00000000100000,6");
			loadString ("paper,00000000000010,7");
			loadString ("paper,00001000000000,8");
		} else if(level == "2") {
			loadString ("setup,2,0");
			loadString ("base ,11111111011101,0");
			loadString ("stone,00000000100000,6");
			loadString ("paper,00000000000010,7");
			
			loadString ("base ,01100011001101,1");
			loadString ("paper,10000000000000,14");
			loadString ("stone,00001000000000,16");
			loadString ("stone,00010000000000,10");
			loadString ("paper,00000100000000,15");
			loadString ("paper,00000000100000,12");
			loadString ("stone,00000000010000,18");
			loadString ("paper,00000000000010,11");
		}
	}
	
	private void loadString(string levelString) {
		string[] tokens = levelString.Split(',');
		string header = tokens[0].Trim();
		if(header == "base") {
			loadBaseLayer(int.Parse(tokens[2]), tokens[1]);
		} else if(header == "setup") {
			MasterGameController.Instance.SetLevelLineGoal(int.Parse(tokens[1]));
		} else if(header == "player") {
			characterSprite = Instantiate(Resources.Load ("CharacterPrefab")) as GameObject;
			characterSprite.transform.parent = gameLayer.transform;
			float colParse = float.Parse(tokens[1]);
			float lineParse = float.Parse(tokens[2]);
			Debug.Log("Moving player to " + colParse + ", " + lineParse);
			float col = MasterGameController.Instance.GetPositionForColumn(colParse-1) + glpos.x;
			float line = MasterGameController.Instance.GetPositionForLine(lineParse) + glpos.y;
			characterSprite.transform.position = (new Vector3(col, line, characterSprite.transform.position.z));
		} else if(header == "text") {
			MasterGameController.Instance.ShowLevelText(tokens[1]);
		}  else if(header == "end") {
			MasterGameController.Instance.ShowMessageText(tokens[1]);
		} else {
			loadBlockString(header, tokens[1], int.Parse(tokens[2]));	
		}
	}
	
	public void loadBlockString(string blockType, string col, int line) {
		for(int i = 0; i < col.Length; i++) {
			string data = col.Substring(i,1);
			if(data == "1"){
				loadBlock (blockType, i, line);
			}
		}
	}
	
	public void loadBlock(string blockType, int col, int line) {
		GameObject newBlock = null;
		Vector2 tpos = new Vector2(MasterGameController.Instance.GetPositionForColumn(col),
								   MasterGameController.Instance.GetPositionForLine(line));
		if(blockType == "paper") {
			newBlock = Instantiate(Resources.Load ("PaperBlockPrefab")) as GameObject;
			newBlock.transform.parent = gameLayer.transform;
			newBlock.transform.position = new Vector3(tpos.x + glpos.x,
												      tpos.y + glpos.y, 0.0f);
		} else if (blockType == "stone") {
			newBlock = Instantiate(Resources.Load ("StoneBlockPrefab")) as GameObject;
			newBlock.transform.parent = gameLayer.transform;
			newBlock.transform.position = new Vector3(tpos.x + glpos.x,
												      tpos.y + glpos.y, 0.0f);
		} else if (blockType == "spike") {
			newBlock = Instantiate(Resources.Load ("SpikeBlockPrefab")) as GameObject;
			newBlock.transform.parent = gameLayer.transform;
			newBlock.transform.position = new Vector3(tpos.x + glpos.x,
												      tpos.y + glpos.y, 0.0f);
		} else if (blockType == "egg") {
			newBlock = Instantiate(Resources.Load ("EggBlockPrefab")) as GameObject;
			newBlock.transform.parent = gameLayer.transform;
			newBlock.transform.position = new Vector3(tpos.x + glpos.x,
												      tpos.y + glpos.y, 0.0f);
		}
		if(newBlock) {
			createdObjects.Add(newBlock);
			MasterGameController.Instance.RegisterBlock(newBlock);	
		}
	}
	
	public void loadBaseLayer(int layer, string layerData) {
		for(int i = 0; i < layerData.Length; i++) {
			string data = layerData.Substring(i,1);
			GameObject newBlock = null;
			if(data == "0") {
				
			} else if(data == "1"){
				newBlock = Instantiate(Resources.Load ("BaseBlockPrefab")) as GameObject;
				newBlock.transform.parent = gameLayer.transform;
				newBlock.transform.position = new Vector3(i*32.0f+glpos.x,
													      layer*32.0f+glpos.y, 0.0f);
			}
			if(newBlock) {
				createdObjects.Add(newBlock);
			}
		}
	}
	
}
