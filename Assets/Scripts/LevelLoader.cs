using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColorToPrefab {
	public Color32 color;
	public GameObject prefab;
}

public class LevelLoader : MonoBehaviour {

	public string levelFileName;

	public ColorToPrefab[] colorToPrefab;

	void Start () {
		
		LoadMap();
	
	}

	void ClearMap() {

		while(transform.childCount > 0) {
			Transform c = transform.GetChild(0);
			c.SetParent(null);
			Destroy(c.gameObject);
		}

	}

	void LoadAllLevelNames() {

	}

	void LoadMap() {
		
		ClearMap();

		string filePath = Application.dataPath + "/StreamingAssets/" + levelFileName;
		byte[] bytes = System.IO.File.ReadAllBytes(filePath);
		Texture2D levelMap = new Texture2D(2, 2);
		levelMap.LoadImage(bytes);

		Color32[] allPixels = levelMap.GetPixels32();
		int width = levelMap.width;
		int height = levelMap.height;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				SpawnTileAt( allPixels[(y * width) + x], x, y );
			}
		}

	}

	void SpawnTileAt( Color32 c, int x, int y ) {

		if(c.a <= 0) {
			return;
		}

		foreach(ColorToPrefab ctp in colorToPrefab) {
			
			if( c.Equals(ctp.color) ) {
				GameObject go = (GameObject)Instantiate(ctp.prefab, new Vector3(x, y, 0), Quaternion.identity );
				go.transform.SetParent(this.transform);

				return;
			}
		}

		Debug.LogError("No color to prefab found for: " + c.ToString() );

	}
	
}
