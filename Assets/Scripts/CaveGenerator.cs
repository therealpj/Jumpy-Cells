using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CaveGenerator : MonoBehaviour
{
    public GameObject[] gameObjects;
    public int width, height;

    [Range(0.0f, 1f)]
    public float tileChance;
    public int deathLimit;
    public int birthLimit;
    public int timesToSmooth;
    public int randomTile;
    private float previousTileChance;
    public int enemyCount;

    private string color;
    private GameObject[,] tiles;
    private string tile_middle, tile_right, tile_left, tile_single, tile_wall_1, tile_wall_2, tile_wall_3;
    private string background;
    private GameObject backgroundClone;
    private int emptyTiles;
    private bool[,] visited;
    public TextMeshProUGUI levelNumber;

    private Dictionary<string, GameObject> backgroundItems = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    private enum TileColors {
        blue,
        brown,
        green,
        yellow,
    }
    // Start is called before the first frame update
    void Start() {
        tiles = new GameObject[width, height];
        levelNumber.enabled = false;
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        LoadLevel();
    }

    public void SpawnBackgroundItem(Vector3 position) {
        if (color == "brown") {
            color = "red";
        } else if (color == "yellow") {
            color = "dark";
        }
        string type = "plant";
        string variety = (Mathf.RoundToInt(Random.Range(1, 7))).ToString();
        string name = type + "_" + color + "_" + variety;

        if (!backgroundItems.ContainsKey(name)) {
            var go2 = Resources.Load(name);
            GameObject go = go2 as GameObject;
            if (go == null) {
                Debug.Log("Tried to instantiate item " + name + ", but it does not exist");
                return;
            }
            backgroundItems.Add(name, go);
        }
        spawnedPrefabs.Add(Instantiate(backgroundItems[name], position, Quaternion.identity));

    }

    void AssignTiles() {
        randomTile = Mathf.RoundToInt(Random.Range(0,4));
        color = ((TileColors) randomTile).ToString();
        tile_middle = "tile_" + color + "_middle";
        tile_right = "tile_" + color + "_right";
        tile_left = "tile_" + color + "_left";
        tile_single = "tile_" + color + "_single";
        tile_wall_1 = "tile_" + color + "_wall_1";
        tile_wall_2 = "tile_" + color + "_wall_2";
        tile_wall_3 = "tile_" + color +"_wall_3";

        // assigning the background
        background = color + "_tiles_background";
    }

    void LoadLevel() {
        previousTileChance = tileChance;
        emptyTiles = 0;
        visited = new bool[width, height];

        // generating a random level out of the 4 possible ones
        DestroyTiles();
        AssignTiles();
        FillMap();

        // spawning the player at the correct position
        GameObject player = GameObject.Find("player");
        player.transform.position = new Vector3(width / 2, height / 2, -5);
        previousTileChance = tileChance;
        StartCoroutine(ShowLevelText());
    }

    public IEnumerator ShowLevelText() {
        levelNumber.text = "Level:   " + PlayerPrefs.GetInt("level");
        levelNumber.enabled = true;
        yield return new WaitForSeconds(3.0f);
        levelNumber.enabled = false;
    }


    void FillMap() {
        // instantiating the background
        backgroundClone = Instantiate(gameObjects[FindGameObject(background)], new Vector2(0, 0), Quaternion.identity);

        // now the tiles
        bool[,] cellMap = new bool[width, height];
        cellMap = InitializeMap(cellMap);

        for(int i = 0; i < timesToSmooth; i++) {
            cellMap = Smoothmap(cellMap);
        }

        FloodFill(cellMap, width / 2, height / 2);
        if (emptyTiles < (width * height) / 100 ) {
            LoadLevel();
        } else {
            SpawnEnemies(cellMap);
            PlaceExitFlag(cellMap);
            RenderMap(cellMap);
        }
    }

    void PlaceExitFlag(bool[,] map) {
        int x = Random.Range(1, width-1);
        int y = 1;
        map[x,y] = false;
        map[x,y+1] = false;
        spawnedPrefabs.Add(Instantiate(gameObjects[FindGameObject("flag_big")], new Vector3(x, y, -5), Quaternion.identity));

    }

    public bool[,] InitializeMap(bool[,] map) {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < width; y++) {
                visited[x, y] = false;
                if(Random.value < tileChance || x == 0 || y == 0 || x == width - 1 || y == height - 1) {
                    map[x, y] = true;
                }
            }
        }
        return map;
    }

    public bool[,] Smoothmap(bool[,] oldMap) {
        bool[,] newMap = new bool[width, height];
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                int neighbors = CountTileNeighbors(oldMap, x, y);
                if(oldMap[x, y]) {
                    if (neighbors < deathLimit) {
                        newMap[x, y] = false;
                    } else {
                        newMap[x, y] = true;
                    }
                } else {
                    if (neighbors > birthLimit) {
                        newMap[x, y] = true;
                    } else {
                        newMap[x, y] = false;
                    }
                }
            }
        }
        return newMap;
    }

    public int CountTileNeighbors(bool[,] map, int x, int y ) {
        int count = 0;
        for(int i=-1; i<2; i++){
            for(int j=-1; j<2; j++){
                int neighbour_x = x+i;
                int neighbour_y = y+j;
                //If we're looking at the middle point
                if(i == 0 && j == 0){
                }
                //In case the index we're looking at it off the edge of the map
                else if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= width || neighbour_y >= height){
                    count = count + 1;
                }
                //Otherwise, a normal check of the neighbour
                else if(map[neighbour_x, neighbour_y]){
                    count = count + 1;
                }
            }
        }
        return count;
    }

    private void SpawnEnemies(bool[,] map) {
        for(int i = 0; i < enemyCount; i++) {
            // spawn an enemy if tile is empty
            int enemyPositionX = Random.Range(0, width);
            int enemyPositionY = Random.Range(0, height);
            string enemy = "";
            if(!map[enemyPositionX, enemyPositionY]) {
                // choosing a random enemy from the 3 possible enemies
                switch(Random.Range(1, 3)) {
                    case 1: enemy = "enemy_flying";
                    break;
                    case 2: enemy = "enemy_floating";
                    break;
                    case 3: enemy = "enemy_floating";
                    break;
                }
                spawnedPrefabs.Add(Instantiate(gameObjects[FindGameObject(enemy)], new Vector3(enemyPositionX, enemyPositionY, -5), Quaternion.identity));
            }
        }
    }
    void RenderMap(bool[,] map) {
        GameObject tileToRender;
        int z = -2;
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                if(map[x, y]) {
                    tileToRender = CorrectTileToRender(map, x, y);

                    // chance to render a different variety of wall
                    if (tileToRender.name == tile_wall_1 && Random.value < 0.1) {
                        if (Random.value < 0.5) {
                            tileToRender = gameObjects[FindGameObject(tile_wall_2)];
                        }else {
                            tileToRender = gameObjects[FindGameObject(tile_wall_3)];
                        }
                    }
                    tiles[x, y] = Instantiate(tileToRender,
                    new Vector3(x, y, z),
                    Quaternion.identity);
                }
            }
        }
    }

    void DestroyTiles() {
        //remove the background
        Destroy(backgroundClone);

        //remove the tiles
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y ++){
                Destroy(tiles[x, y]);
            }
        }

        // remove the background items
        foreach(GameObject go in spawnedPrefabs) {
            Destroy(go);
        }
    }

    GameObject CorrectTileToRender(bool[,]map, int x, int y) {

        // if border tile, render the wall tile
        if( x == 0 || x == width - 1 || y == 0 || y == height - 1) {
            return gameObjects[FindGameObject(tile_wall_1)];
        } else  {
            if (map[x, y + 1] == true ) {
                return gameObjects[FindGameObject(tile_wall_1)];
            } else {

                // chance to render a background item
                if (Random.value < 0.2) {
                    SpawnBackgroundItem(new Vector2(x, y + 0.7f));
                }

                if (map[x - 1, y] == true && map[x + 1, y] == true) {
                    return gameObjects[FindGameObject(tile_middle)];
                } else if (map[x - 1, y] == true && map[x + 1, y] == false) {
                    return gameObjects[FindGameObject(tile_right)];
                } else if (map[x - 1, y] == false && map[x + 1, y] == true) {
                    return gameObjects[FindGameObject(tile_left)];
                } else if (map[x - 1, y] == false && map[x + 1, y] == false) {
                    return gameObjects[FindGameObject(tile_single)];
                }
            }
        }
        return gameObjects[FindGameObject(tile_wall_1)];
    }


    // given the name of a prefab, returns its index our gameObjects array
    int FindGameObject(string name) {
        for(int i = 0; i < gameObjects.Length; i++) {
            if (name == gameObjects[i].name) {
                return i;
            }
        }
        return -1;
    }



    void FloodFill(bool[,] map, int x, int y) {
        if(x < 0 || y < 0 || x > width - 1 || y > height - 1) {
            return;
        }

        // check if we have already visited this tile
        if(visited[x, y]) {
            return;
        }

        // now we have visited this tile
        visited[x, y] = true;

        if(map[x, y]) {
            return;
        } else {
            // we have found an empty space
            emptyTiles += 1;
            FloodFill(map, x+1, y);
            FloodFill(map, x-1, y);
            FloodFill(map, x, y-1);
            FloodFill(map, x, y+1);
        }
    }
}
