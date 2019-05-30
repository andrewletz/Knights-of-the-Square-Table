using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(0,100)]
    public int fillPercentage;
    public int width, height, smoothingIterations, borderSize, cavernThreshold, numEnemies;
    public string randomSeed;
    public bool useRandomSeed;
    public GameObject playerObject, cameraObject, lightObject;

    private int[,] floorMap;
    private System.Random randNumGen;
    private MeshGenerator meshGen;

    void Start(){
        GenMap();
    }

    void GenMap(){
        floorMap = new int[width, height];
        RenderMap();
        for (int i=0; i<smoothingIterations; i++){
            CellularSmooth();
        }

        meshGen = GetComponent<MeshGenerator>();
        meshGen.CreatePlane(width+borderSize*2,height+borderSize*2);
        removeCaverns();

        int[,] borderedMap = new int[width+borderSize*2, height+borderSize*2];

        for (int x=0; x<borderedMap.GetLength(0); x++){
            for (int y=0; y<borderedMap.GetLength(1); y++){
                if (x >= borderSize && x < width+borderSize && y >= borderSize && y < height+borderSize){
                    borderedMap[x,y] = floorMap[x-borderSize,y-borderSize];
                }
                else {
                    borderedMap[x,y] = 1;
                }
            }
        }
        
        meshGen.GenerateMesh(borderedMap, 1);
    }

    void RenderMap(){
        if (useRandomSeed){
            randomSeed = Time.time.ToString();
        }

        randNumGen = new System.Random(randomSeed.GetHashCode());

        for (int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                if (x==0 || x==width-1 || y==0 || y==height-1){
                    floorMap[x,y] = 1;    
                }
                else {
                    floorMap[x,y] = (randNumGen.Next(0,100) < fillPercentage) ? 1 : 0;
                }
            }   
        }
    }

    void CellularSmooth(){
        for (int x=0; x<width; x++) {
            for (int y=0; y<height; y++) {
                int neighbors = NeighborWallCount(x,y);

                if (neighbors > 4){
                    floorMap[x,y] = 1;
                }
                else if (neighbors < 4){
                    floorMap[x,y] = 0;
                }
            }
        }
    }

    int NeighborWallCount(int x, int y){
        int wallCount=0;
        for (int next_x=x-1; next_x<=x+1; next_x++){
            for (int next_y=y-1; next_y<=y+1; next_y++){
                if (next_x >=0 && next_x < width && next_y >=0 && next_y < height){
                    if (next_x != x || next_y != y){
                        wallCount += floorMap[next_x, next_y];
                    }
                }
                else {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void removeCaverns(){
        bool[,] visited = new bool[width,height];
        Dictionary<(int,int), int> caverns = new Dictionary<(int,int), int>();
        List<Room> cavernRooms = new List<Room>();

        for (int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                if (!visited[x,y] && floorMap[x,y] == 0){

                    visited[x,y] = true;
                    caverns[(x,y)] = 1;

                    List<Tile> cavernTiles = new List<Tile>();
                    cavernTiles.Add(new Tile(x,y));

                    Queue<(int,int)> neighbors = new Queue<(int,int)>();
                    exploreNeighbors(x,y,neighbors);

                    while (neighbors.Count > 0){
                        caverns[(x,y)]++;
                        (int next_x, int next_y) = neighbors.Dequeue();

                        cavernTiles.Add(new Tile(next_x, next_y));
                        exploreNeighbors(next_x,next_y,neighbors);
                    }

                    cavernRooms.Add(new Room(cavernTiles, floorMap));
                }
            }
        }

        cavernRooms.Sort();
        cavernRooms[0].mainRoom = true;
        cavernRooms[0].accessible = true;

        ConnectCaverns(cavernRooms);

        CreateExit(cavernRooms[randNumGen.Next(0,cavernRooms.Count)]);
        InitializeMap(cavernRooms[randNumGen.Next(0,cavernRooms.Count)]);

        void exploreNeighbors(int x, int y, Queue<(int,int)> neighbors){
            if (x>0 && x<width-1 && y>0 && y<height-1){
                if (!visited[x-1,y] && floorMap[x-1,y]==0){
                    visited[x-1,y] = true;
                    neighbors.Enqueue((x-1,y));
                }
                if (!visited[x+1,y] && floorMap[x+1,y]==0){
                    visited[x+1,y] = true;
                    neighbors.Enqueue((x+1,y));
                }
                if (!visited[x,y-1] && floorMap[x,y-1]==0){
                    visited[x,y-1] = true;
                    neighbors.Enqueue((x,y-1));
                }
                if (!visited[x,y+1] && floorMap[x,y+1]==0){
                    visited[x,y+1] = true;
                    neighbors.Enqueue((x,y+1));
                }
            }
        }

        void createPath((int,int) point1, (int,int) point2){
            int minX = Math.Min(point1.Item1, point2.Item1), maxX = Math.Max(point1.Item1, point2.Item1), minY = Math.Min(point1.Item2, point2.Item2), maxY = Math.Max(point1.Item2, point2.Item2), it;
            int minX_y = Math.Min(point1.Item1, point2.Item1) == point1.Item1 ? point1.Item2 : point2.Item2;
            int minY_x = Math.Min(point1.Item2, point2.Item2) == point1.Item2 ? point1.Item1 : point2.Item1;

            for (it=minX; it<=maxX; it++){
                floorMap[it, minX_y] = 0;
            }
            for (it=minY; it<=maxY; it++){
                floorMap[minY_x, it] = 0;
            }
        }

        void fillCavern(Room cavern){
            foreach (Tile tile in cavern.tiles){
                floorMap[tile.X, tile.Y] = 1;
            }
        }
    }

    void ConnectCaverns(List<Room> caverns, bool forceAccessibility=false){

        int bestDist = 0;
        bool possibleConnectionFound = false;
        Tile bestA_tile = new Tile(), bestB_tile = new Tile();
        Room bestA = new Room(), bestB = new Room();
        List<Room> a_rooms = new List<Room>(), b_rooms = new List<Room>();

        if (forceAccessibility){
            foreach (Room cavern in caverns){
                if (cavern.accessible){
                    b_rooms.Add(cavern);
                }
                else {
                    a_rooms.Add(cavern);
                }
            }
        }
        else {
            a_rooms = caverns;
            b_rooms = caverns;
        }

        foreach(Room cavern in a_rooms){
            if (!forceAccessibility){
                possibleConnectionFound = false;
                if (cavern.connectedRooms.Count > 0){
                    continue;
                }
            }

            foreach(Room other in b_rooms){
                
                if (cavern == other || cavern.IsConnected(other)){
                    continue;
                }



                for (int A=0; A<cavern.edgeTiles.Count; A++){
                    for (int B=0; B<other.edgeTiles.Count; B++){
                        Tile tileA = cavern.edgeTiles[A];
                        Tile tileB = other.edgeTiles[B];
                        int dist = (int)(Mathf.Pow(tileA.X-tileB.X,2) + Mathf.Pow(tileA.Y-tileB.Y,2));

                        if (dist < bestDist || !possibleConnectionFound){
                            bestDist = dist;
                            possibleConnectionFound = true;
                            bestA_tile = tileA;
                            bestB_tile = tileB;
                            bestA = cavern;
                            bestB = other;
                        }
                    }
                }

            }

            if (possibleConnectionFound && !forceAccessibility){
                CreatePassage(bestA, bestB, bestA_tile, bestB_tile);
            }
        }

        if (possibleConnectionFound && forceAccessibility){
            CreatePassage(bestA, bestB, bestA_tile, bestB_tile);
            ConnectCaverns(caverns, true);
        }

        if (!forceAccessibility){
            ConnectCaverns(caverns, true);
        }
    }

    void CreatePassage(Room A, Room B, Tile A_tile, Tile B_tile){
        Room.Connect(A, B);
        Debug.DrawLine (CoordToWorldPoint(A_tile,2),CoordToWorldPoint(B_tile,2), Color.green, 100);

        List<Tile> line = GetLine(A_tile, B_tile);
        foreach (Tile t in line){
            DrawCircle(t,5);
        }
    }

    void ExitPath(Tile A_tile, Tile B_tile){
        List<Tile> line = GetLine(A_tile, B_tile);
        foreach (Tile t in line){
            DrawCircle(t,5);
        }
    }
    
    void CreateExit(Room cavern){
        Tile exitPos = cavern.edgeTiles[randNumGen.Next(0,cavern.edgeTiles.Count-1)];
        int left = exitPos.X, right = width-exitPos.X, bottom = exitPos.Y, top = height-exitPos.Y, exitPoint = Math.Min(Math.Min(left,right),Math.Min(bottom,top));

        if (exitPoint==left){
            ExitPath(exitPos, new Tile(0, exitPos.Y));
        }
        else if (exitPoint==right){
            ExitPath(exitPos, new Tile(width, exitPos.Y));
        }
        else if (exitPoint==bottom){
            ExitPath(exitPos, new Tile(exitPos.X, 0));
        }
        else if (exitPoint==top){
            ExitPath(exitPos, new Tile(exitPos.X, height));
        }
    }

    void DrawCircle(Tile t, int rad){
        for (int x = -rad; x <= rad; x++){
            for (int y = -rad; y <= rad; y++){
                if (x*x + y*y <= rad*rad){
                    int realX = t.X+x, realY = t.Y+y;
                    if (IsInMapRange(realX,realY)){
                        floorMap[realX,realY] = 0;
                    }
                }
            }
        }
    }

    void DrawSpawnCircle(Tile t, List<Tile> tiles, int rad){
        for (int x = -rad; x <= rad; x++){
            for (int y = -rad; y <= rad; y++){
                if (x*x + y*y <= rad*rad){
                    int realX = t.X+x, realY = t.Y+y;
                    if (IsInMapRange(realX,realY)){
                        floorMap[realX,realY] = 0;
                        for (int i=0; i<tiles.Count; i++){
                            if (tiles[i].X == realX && tiles[i].Y == realY){
                                tiles.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }
    }

    bool IsInMapRange(int x, int y){
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    List<Tile> GetLine(Tile from, Tile to){
        List<Tile> line = new List<Tile>();
        int x = from.X, y = from.Y, dx = to.X - x, dy = to.Y - y, step = Math.Sign(dx), gradientStep = Math.Sign(dy), longest = Math.Abs(dx), shortest = Math.Abs(dy);
        bool inverted = false;

        if (longest < shortest){
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);
            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int accumulation = longest / 2;
        for (int i=0; i<longest; i++){
            line.Add(new Tile(x,y));

            if (inverted){
                y += step;
            }
            else {
                x += step;
            }

            accumulation += shortest;
            if (accumulation >= longest){
                if (inverted){
                    x += gradientStep;
                }
                else {
                    y += gradientStep;
                }
                accumulation -= longest;
            }
        }
        return line;
    }

    Vector3 CoordToWorldPoint(Tile tile, int depth) {
        return new Vector3 (-width / 2 + .5f + tile.X, depth, -height / 2 + .5f + tile.Y);
    }

    void InitializeMap(Room mainCavern){
        Tile player = mainCavern.tiles[randNumGen.Next(0,mainCavern.tiles.Count-1)], exitPoint = mainCavern.tiles[randNumGen.Next(0,mainCavern.tiles.Count-1)];
        double tolerance = .80 * mainCavern.tiles.Count, enemyTolerance = .25 * mainCavern.tiles.Count;
        int wall_height = -1 * meshGen.WallHeight();

        Vector3 playerPos = CoordToWorldPoint(player, wall_height);
        playerPos.y += 0.5f;
        playerObject.transform.position = playerPos;

        List<Tile> spawnLocs = mainCavern.tiles;
        DrawSpawnCircle(player, spawnLocs, 5);
        /*
        for (int i=0; i<numEnemies; i++){
            if (spawnLocs.Count < 1){
                break;
            }

            GameObject enemy = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Enemies/Enemy1.prefab", typeof(GameObject))) as GameObject;
            Vector3 enemyLoc = CoordToWorldPoint(spawnLocs[randNumGen.Next(0,spawnLocs.Count-1)], wall_height);
            enemyLoc.y += 0.5f;
            enemy.transform.position = enemyLoc;
            enemy.GetComponent<EnemyController>().target = playerObject;
            enemy.SetActive(true);
            DrawSpawnCircle(player, spawnLocs, 2);
        }
        */

        playerObject.SetActive(true);
        lightObject.SetActive(true);
        cameraObject.SetActive(true);
        cameraObject.GetComponent<CameraController>().Initialize();

        double coordAbs(Tile A, Tile B){
            return Math.Sqrt(Mathf.Pow(A.X+B.X,2)+Mathf.Pow(A.Y+B.Y,2));
        }

    }

    struct Tile {
        public int X, Y;

        public Tile(int x_coord, int y_coord){
            X = x_coord;
            Y = y_coord;
        }
    }

    class Room : IComparable<Room> {
        public List<Tile> tiles, edgeTiles;
        public List<Room> connectedRooms;
        public int size;
        public bool accessible, mainRoom;

        public Room(){
        }

        public Room(List<Tile> roomTiles, int[,] map){
            tiles = roomTiles;
            size = tiles.Count;
            connectedRooms = new List<Room>();
            edgeTiles = new List<Tile>();

            foreach (Tile tile in tiles){
                for (int x=tile.X-1; x<=tile.X+1; x++){
                    for (int y=tile.Y-1; y<=tile.Y+1; y++){
                        if ((x == tile.X || y == tile.Y) && map[x,y] == 1){
                            edgeTiles.Add(tile);
                        }
                    }
                }
            }
        }

        public void SetAccessible(){
            if (!accessible){
                accessible = true;
                foreach (Room connected in connectedRooms){
                    connected.SetAccessible();
                }
            }
        }

        public static void Connect(Room A, Room B){
            if (A.accessible){
                B.SetAccessible();
            }
            else if (B.accessible){
                A.SetAccessible();
            }

            A.connectedRooms.Add(B);
            B.connectedRooms.Add(A);
        }

        public bool IsConnected(Room other){
            return connectedRooms.Contains(other);
        }

        public int CompareTo(Room other) {
            return other.size.CompareTo(size);
        }
    }

/* 
    void OnDrawGizmos() {
        if (floorMap != null) {
            for (int x = 0; x < width; x ++) {
                for (int y = 0; y < height; y ++) {
                    Gizmos.color = (floorMap[x,y] == 1)?Color.black:Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
                    Gizmos.DrawCube(pos,Vector3.one);
                }
            }
        }
    }
    */
}