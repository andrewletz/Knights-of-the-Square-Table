using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeshGenerator : MonoBehaviour
{
    public int wallHeight;
    public MeshFilter walls;
    public GameObject floor, wallMesh;
    public Texture floorMat;
    private SquareGrid grid;
    private List<Vector3> vertices;
    private List<int> triangles;
    private Dictionary<int,List<Triangle>> triangleDict = new Dictionary<int,List<Triangle>>();
    private List<List<int>> outlines = new List<List<int>>();
    private HashSet<int> checkedVertices = new HashSet<int>();
    private NavMeshSurface navMeshSurface = GetComponent<NavMeshSurface>();
    
    public void GenerateMesh(int[,] map, float squareSize){
        triangleDict.Clear();
        outlines.Clear();
        checkedVertices.Clear();
        grid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < grid.squares.GetLength(0); x ++) {
            for (int y = 0; y < grid.squares.GetLength(1); y ++) {
                TriangluateSquares(grid.squares[x,y]);
            }
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        int tileAmount = 10;
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i =0; i < vertices.Count; i ++) {
            float percentX = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,vertices[i].x) * tileAmount;
            float percentY = Mathf.InverseLerp(-map.GetLength(1)/2*squareSize,map.GetLength(1)/2*squareSize,vertices[i].z) * tileAmount;
            uvs[i] = new Vector2(percentX,percentY);
        }
        mesh.uv = uvs;

        CreateWallMesh(map, squareSize);
        navMeshSurface.BuildNavMesh();
    }

    public void CreatePlane(int width, int height){
        /*
        int scaleX = (int) width/10, scaleZ = (int) height/10;
        floor.transform.position = new Vector3(0f, (-1f * wallHeight) + 0.75f, 0f);
        floor.transform.localScale = new Vector3(scaleX,0f,scaleZ);
        floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(scaleX, scaleZ);
        */
        floor.SetActive(true);
    }

    public int WallHeight(){
        return wallHeight;
    }

    public class SquareGrid {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize){
            int nodeCountX = map.GetLength(0), nodeCountY = map.GetLength(1), x, y;
            float width = nodeCountX * squareSize, height = nodeCountY * squareSize;

            ControlNode[,] control_nodes = new ControlNode[nodeCountX, nodeCountY];

            for (x=0; x<nodeCountX; x++){
                for (y=0; y<nodeCountY; y++){
                    Vector3 pos = new Vector3(-width/2 + x * squareSize + squareSize/2, 0, -height/2 + y * squareSize + squareSize/2);
                    control_nodes[x,y] = new ControlNode(pos,map[x,y] == 1,squareSize);
                }
            }

            squares = new Square[nodeCountX-1,nodeCountY-1];
            for (x=0; x<nodeCountX-1; x++){
                for (y=0; y<nodeCountY-1; y++){
                    squares[x,y] = new Square(control_nodes[x,y+1],control_nodes[x+1,y+1],control_nodes[x+1,y],control_nodes[x,y]);
                }
            }
        }
    }

    public class Square {
        public ControlNode uL, uR, bR, bL;
        public Node cT, cR, cB, cL;
        public int configuration;

        public Square(ControlNode _uLe, ControlNode _uR, ControlNode _bR, ControlNode _bL){
            uL = _uLe;
            uR = _uR;
            bR = _bR;
            bL = _bL;

            cT = uL.right;
            cR = bR.above;
            cB = bL.right;
            cL = bL.above;

            if (uL.active){
                configuration += 8;
            }
            if (uR.active){
                configuration += 4;
            }
            if (bR.active){
                configuration += 2;
            }
            if (bL.active){
                configuration += 1;
            }
        }
    }

    public class Node {
        public Vector3 position;
        public int index;

        public Node(Vector3 _pos){
            position = _pos;
            index = -1;
        }
    }

    public class ControlNode : Node {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float _size) : base(_pos){
            active = _active;
            above = new Node(position + Vector3.forward * _size/2f);
            right = new Node(position + Vector3.right * _size/2f);
        }
    }

    struct Triangle {
        public int A, B, C;
        int[] vertices;

        public Triangle(int a, int b, int c){
            A = a;
            B = b;
            C = c;

            vertices = new int[3];
            vertices[0] = A;
            vertices[1] = B;
            vertices[2] = C;
        }

        public bool contains(int index){
            return index == A || index == B || index == C;
        }

        public int this[int i]{
            get {
                return vertices[i];
            }
        }
    }

    void TriangluateSquares(Square square){
        switch (square.configuration){
            case 0:
                // Empty, no mesh
                break;

            /* 1 Point */
            case 1:
                // Bottom left triangle
                ConstructMesh(square.cL, square.cB, square.bL);
                break;
            case 2:
                // Bottom right triangle
                ConstructMesh(square.bR, square.cB, square.cR);
                break;
            case 4:
                // Upper right triangle
                ConstructMesh(square.uR, square.cR, square.cT);
                break;
            case 8:
                // Upper Left Triangle
                ConstructMesh(square.uL, square.cT, square.cL);
                break;

            /* 2 Points */
            case 3:
                // Bottom Square
                ConstructMesh(square.cR, square.bR, square.bL, square.cL);
                break;
            case 5:
                // BL-UR hexagon
                ConstructMesh(square.cT, square.uR, square.cR, square.cB, square.bL, square.cL);
                break;
            case 6:
                // Right Square
                ConstructMesh(square.cT, square.uR, square.bR, square.cB);
                break;
            case 9:
                // Left Square
                ConstructMesh(square.uL, square.cT, square.cB, square.bL);
                break;
            case 10:
                // BR - UL Hexagon
                ConstructMesh(square.uL, square.cT, square.cR, square.bR, square.cB, square.cL);
                break;

            case 12:
                // Upper Rectangle
                ConstructMesh(square.uL, square.uR, square.cR, square.cL);
                break;

            /* 3 Points */
            case 7:
                // UL-Pointing Pentagon
                ConstructMesh(square.cT, square.uR, square.bR, square.bL, square.cL);
                break;
            
            case 11:
                // BR-Pointing Pentagon
                ConstructMesh(square.uL, square.cT, square.cR, square.bR, square.bL);
                break;

            case 13:
                // UL-Pointing Pentagon
                ConstructMesh(square.uL, square.uR, square.cR, square.cB, square.bL);
                break;
            
            case 14:
                // MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
                ConstructMesh(square.uL, square.uR, square.bR, square.cB, square.cL);
                break;

            /* 4 Points */
            case 15:
                // Full square (filled in)
                ConstructMesh(square.uL, square.uR, square.bR, square.bL);
                checkedVertices.Add(square.uL.index);
                checkedVertices.Add(square.uR.index);
                checkedVertices.Add(square.bR.index);
                checkedVertices.Add(square.bL.index);
                break;
        }
    }

    void AssignVertices(Node[] points){
        for (int i=0; i <points.Length; i++) {
            if (points[i].index == -1) {
                points[i].index = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node p1, Node p2, Node p3){
        triangles.Add(p1.index);
        triangles.Add(p2.index);
        triangles.Add(p3.index);

        Triangle t = new Triangle(p1.index, p2.index, p3.index);
        AddTriangle(t.A, t);
        AddTriangle(t.B, t);
        AddTriangle(t.C, t);
    }

    int GetConnectedOutlineVertex(int index){
        List<Triangle> t_list = triangleDict[index];

        for (int i=0; i<t_list.Count; i++){
            Triangle t = t_list[i];
            for (int j=0; j<3; j++){
                int B = t[j];
                if (B != index && !checkedVertices.Contains(B) && IsOutline(index, B)){
                    return B;
                }
            }
        }
        return -1;
    }

    bool IsOutline(int A, int B){
        List<Triangle> a_list = triangleDict[A];
        int shared = 0;
        for (int i=0; i<a_list.Count; i++){
            if (a_list[i].contains(B)){
                shared++;
                if (shared > 1){
                    break;
                }
            }
        }
        return shared == 1;
    }

    void AddTriangle(int key, Triangle t){
        if (triangleDict.ContainsKey(key)){
            triangleDict[key].Add(t);
        }
        else {
            List<Triangle> t_list = new List<Triangle>();
            t_list.Add(t);
            triangleDict.Add(key, t_list);
        }
    }

    void CalculateOutlines(){
        for (int index=0; index<vertices.Count; index++){
            if (!checkedVertices.Contains(index)){
                int outlineVertex = GetConnectedOutlineVertex(index);
                if (outlineVertex != -1){
                    checkedVertices.Add(index);
                    List<int> newOutline = new List<int>();
                    newOutline.Add(index);
                    outlines.Add(newOutline);
                    FollowOutline(outlineVertex, outlines.Count-1);
                    outlines[outlines.Count-1].Add(index);
                }
            }
        }
    }

    void FollowOutline(int vertexIndex, int outlineIndex){
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertex != -1){
            FollowOutline(nextVertex, outlineIndex);
        }

    }

    void CreateWallMesh(int[,] map, float squareSize){
        CalculateOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        
        foreach (List<int> outline in outlines){
            for (int i=0; i<outline.Count-1; i++){
                int start = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]);
                wallVertices.Add(vertices[outline[i+1]]);
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight);
                wallVertices.Add(vertices[outline[i+1]] - Vector3.up * wallHeight);

                wallTriangles.Add(start);
                wallTriangles.Add(start+2);
                wallTriangles.Add(start+3);
                
                wallTriangles.Add(start+3);
                wallTriangles.Add(start+1);
                wallTriangles.Add(start+0);
            }
        }
        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();

        int tileAmount = 10;
        Vector2[] uvs = new Vector2[wallVertices.Count];
        for (int i =0; i < wallVertices.Count; i ++) {
            float percentY = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,wallVertices[i].y) * tileAmount;
            float percentX = Mathf.InverseLerp(-map.GetLength(0)/2*squareSize,map.GetLength(0)/2*squareSize,wallVertices[i].x) * tileAmount;
        

            uvs[i] = new Vector2(percentY,percentX);
        }
        wallMesh.uv = uvs;

        walls.mesh = wallMesh;

        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = wallMesh;
    }

    void ConstructMesh(params Node[] points){
        AssignVertices(points);

        if (points.Length >= 3){
            CreateTriangle(points[0], points[1], points[2]);
        }
        if (points.Length >= 4){
            CreateTriangle(points[0], points[2], points[3]);
        }
        if (points.Length >= 5){ 
            CreateTriangle(points[0], points[3], points[4]);
        }
        if (points.Length >= 6){
            CreateTriangle(points[0], points[4], points[5]);
        }
    }
}
