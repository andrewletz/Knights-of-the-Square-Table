using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(0,100)]
    public int fillPercentage, cavernThreshold;
    public int width, height, smoothingIterations;
    public string randomSeed;
    public bool useRandomSeed;

    private int[,] floorMap;

    void Start(){
        GenMap();
    }

    void GenMap(){
        floorMap = new int[width, height];
        RenderMap();
        for (int i=0; i<smoothingIterations; i++){
            CellularSmooth();
        }
        removeCaverns();
    }

    void RenderMap(){
        if (useRandomSeed){
            randomSeed = Time.time.ToString();
        }

        System.Random randNumGen = new System.Random(randomSeed.GetHashCode());

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
        int i=0, mainCavern=-1;
        (int,int) mainCoords=(5,5);
        Dictionary<(int,int), int> caverns = new Dictionary<(int,int), int>();

        for (int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                if (!visited[x,y] && floorMap[x,y] == 0){
                    visited[x,y] = true;
                    caverns[(x,y)] = 1;
                    Queue<(int,int)> neighbors = new Queue<(int,int)>();
                    exploreNeighbors(x,y,neighbors);

                    while (neighbors.Count > 0){
                        caverns[(x,y)]++;
                        (int next_x, int next_y) = neighbors.Dequeue();
                        exploreNeighbors(next_x,next_y,neighbors);
                    }
                }
            }
        }

        var caverns_sorted = from pair in caverns orderby pair.Value descending select pair;

        foreach (KeyValuePair<(int, int), int> pair in caverns_sorted){
            if (i++ == 0){
                mainCavern = pair.Value;
                mainCoords = pair.Key;
                continue;
            }

            if (pair.Value / mainCavern >= cavernThreshold/100){
                createPath(pair.Key, mainCoords);
            }
        }

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
    }

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
}