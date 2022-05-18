using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

	public Vector2 gridWorldSize;
	public float nodeRadius;
	public Node[,] grid;
	public float waterLevel = .4f;
	float nodeDiameter;
	public int gridSizeX, gridSizeY;
	public Material edgeMaterial;
	public Material terrainMaterial;
	public static int enemiesCount;
	public List<Node>[] paths;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
		enemiesCount = GetComponent<Pathfinding>().seeker.Length;
		//CreateGrid();
		

	}
	 void Start()
	{
		//perlin noise map to randomly select walkables 
		float[,] noiseMap = new float[gridSizeX, gridSizeY];
		(float xOffset, float yOffset) = (Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				float noiseValue = Mathf.PerlinNoise(x * (nodeRadius/5) + xOffset, y * (nodeRadius / 5) + yOffset);
				noiseMap[x, y] = noiseValue;
			}
		}
		
		grid = new Node[gridSizeX, gridSizeY];
		//ge lower corner to place grid on it
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				//Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				Vector3 worldPoint = transform.position + Vector3.right * (x * nodeDiameter + nodeRadius) + transform.position + Vector3.forward * (y * nodeDiameter + nodeRadius); ;
				float noiseValue = noiseMap[x, y];
				// noiseValue -= falloffMap[x, y];
				bool walkable = noiseValue > waterLevel ;
				//Node node = new Node(walkable,worldPoint,x,y);
				grid[x, y] = new Node(walkable, worldPoint, x, y);

			}
		}
		DrawTerrainMesh(grid);
		DrawEdgeMesh(grid);
		DrawTexture(grid);
		//CreateGrid();
		paths = new List<Node>[enemiesCount];
		for (int i = 0; i < paths.Length; i++)
		{
			paths[i] = new  List<Node>();
		}
	}

	//We need a grid (Squares) fear nothing as the square is two triangles!!
	 
	void DrawTerrainMesh(Node[,] grid)
	{
		Mesh mesh = new Mesh();
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Vector2> uvs = new List<Vector2>();
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Node node = grid[x, y];
				if (node.walkable)
				{
					Vector3 a = new Vector3(x - .5f, 0, y + .5f);
					Vector3 b = new Vector3(x + .5f, 0, y + .5f);
					Vector3 c = new Vector3(x - .5f, 0, y - .5f);
					Vector3 d = new Vector3(x + .5f, 0, y - .5f);
					Vector2 uvA = new Vector2(x / (float)gridSizeX, y / (float)gridSizeY);
					Vector2 uvB = new Vector2((x + 1) / (float)gridSizeX, y / (float)gridSizeY);
					Vector2 uvC = new Vector2(x / (float)gridSizeX, (y + 1) / (float)gridSizeY);
					Vector2 uvD = new Vector2((x + 1) / (float)gridSizeX, (y + 1) / (float)gridSizeY);
					Vector3[] v = new Vector3[] { a, b, c, b, d, c };
					Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
					for (int k = 0; k < 6; k++)
					{
						vertices.Add(v[k]);
						triangles.Add(triangles.Count);
						uvs.Add(uv[k]);
					}
				}
			}
		}
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
	}
	///We detect nearby cells if not walkable and add edges 
	void DrawEdgeMesh(Node[,] grid)
	{
		Mesh mesh = new Mesh();
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Node node = grid[x, y];
				if (node.walkable)
				{
					if (x > 0)
					{
						Node left = grid[x - 1, y];
						if (!left.walkable)
						{
							Vector3 a = new Vector3(x - .5f, 0, y + .5f);
							Vector3 b = new Vector3(x - .5f, 0, y - .5f);
							Vector3 c = new Vector3(x - .5f, -1, y + .5f);
							Vector3 d = new Vector3(x - .5f, -1, y - .5f);
							Vector3[] v = new Vector3[] { a, b, c, b, d, c };
							for (int k = 0; k < 6; k++)
							{
								vertices.Add(v[k]);
								triangles.Add(triangles.Count);
							}
						}
					}
					if (x < gridSizeX - 1)
					{
						Node right = grid[x + 1, y];
						if (!right.walkable)
						{
							Vector3 a = new Vector3(x + .5f, 0, y - .5f);
							Vector3 b = new Vector3(x + .5f, 0, y + .5f);
							Vector3 c = new Vector3(x + .5f, -1, y - .5f);
							Vector3 d = new Vector3(x + .5f, -1, y + .5f);
							Vector3[] v = new Vector3[] { a, b, c, b, d, c };
							for (int k = 0; k < 6; k++)
							{
								vertices.Add(v[k]);
								triangles.Add(triangles.Count);
							}
						}
					}
					if (y > 0)
					{
						Node down = grid[x, y - 1];
						if (!down.walkable)
						{
							Vector3 a = new Vector3(x - .5f, 0, y - .5f);
							Vector3 b = new Vector3(x + .5f, 0, y - .5f);
							Vector3 c = new Vector3(x - .5f, -1, y - .5f);
							Vector3 d = new Vector3(x + .5f, -1, y - .5f);
							Vector3[] v = new Vector3[] { a, b, c, b, d, c };
							for (int k = 0; k < 6; k++)
							{
								vertices.Add(v[k]);
								triangles.Add(triangles.Count);
							}
						}
					}
					if (y < gridSizeY - 1)
					{
						Node up = grid[x, y + 1];
						if (!up.walkable)
						{
							Vector3 a = new Vector3(x + .5f, 0, y + .5f);
							Vector3 b = new Vector3(x - .5f, 0, y + .5f);
							Vector3 c = new Vector3(x + .5f, -1, y + .5f);
							Vector3 d = new Vector3(x - .5f, -1, y + .5f);
							Vector3[] v = new Vector3[] { a, b, c, b, d, c };
							for (int k = 0; k < 6; k++)
							{
								vertices.Add(v[k]);
								triangles.Add(triangles.Count);
							}
						}
					}
				}
			}
		}
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		GameObject edgeObj = new GameObject("Edge");
		edgeObj.transform.SetParent(transform);

		MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
		meshRenderer.material = edgeMaterial;
	}
	//We add the assigned material to each node
	void DrawTexture(Node[,] grid)
	{
		Texture2D texture = new Texture2D(gridSizeX, gridSizeY);
		Color[] colorMap = new Color[gridSizeX * gridSizeY];
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Node node = grid[x, y];
				if (!node.walkable)
					colorMap[y * gridSizeY + x] = Color.blue;
				else
					colorMap[y * gridSizeX+ x] = Color.green;
			}
		}
		texture.filterMode = FilterMode.Point;
		texture.SetPixels(colorMap);
		texture.Apply();

		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.material = terrainMaterial;
		meshRenderer.material.mainTexture = texture;
	}
	//We get the cells using Manhattan Distance to not move diagonally since the player cant also 
	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{

				
					int isDiagonalNode = Mathf.Abs(x + y);
					if (isDiagonalNode == 0 || isDiagonalNode == 2)
						continue;
				

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}
		return neighbours;
	}

	//position in vectors from cells
	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		/*float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];
		*/////
		float percentX = (worldPosition.x) / gridWorldSize.x;
		float percentY = (worldPosition.z) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];
	}
	
	//correct one public List<Node> path;
	
	//public  List<Node> [] path;
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

		if (grid != null)
		{

			foreach (Node n in grid)
			{

				Gizmos.color = (n.walkable) ? Color.green : Color.blue;
				for (int i = 0; i < paths.Length; i++)
				{


					if (paths[i] != null)
						if (paths[i].Contains(n))
							Gizmos.color = Color.black;
				}
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}

			/*	foreach (Node n in grid)
				{

					Gizmos.color = (n.walkable) ? Color.white : Color.red;
					if (path != null)
						if (path.Contains(n))
							Gizmos.color = Color.black;
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			*/
		}
	}
	/*void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y< gridSizeY; y++)
			{
				Node node = grid[x, y];
				if (!node.walkable)
					Gizmos.color = Color.blue;
				else
					Gizmos.color = Color.green;
				Vector3 pos = new Vector3(x, 0, y);
				Gizmos.DrawCube(pos, Vector3.one);
			}
		}
	}*/
}