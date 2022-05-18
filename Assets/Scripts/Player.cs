using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
	//movement 
	public Transform TransformToMove;
	public Vector2 Spacing;
	public Vector3 WorldOrigin;	
	public float movementSpeed;
	public Vector2 CellPosition;
	public Grid grid;
	//health
	public int health;
	public int maxHealth = 20;
	public HpBar hpBar;
	//attack
	public GameObject hit;
	public Collider hitCollider;
	public int attackDamge = 4;
	void Reset()
	{
		// defaults
		Spacing = new Vector2(1, 1);
		movementSpeed = 5.0f;
	}

	// this maps the cell to the world
	Vector3 ComputeWorldPositionFromGridCell(int cellX, int cellY)
	{
		
		// this maps X,Y cell to world, in this case X/Z
		return WorldOrigin + new Vector3(cellX * Spacing.x, 0, cellY * Spacing.y);
	}

	// drives the cell position to the world position
	// returns true if you're done moving and have arrived.
	bool DriveGridCellPositionToWorldPosition(bool instantaneous = false)
	{
		Vector3 worldPosition = ComputeWorldPositionFromGridCell((int)CellPosition.x, (int)CellPosition.y);

		Vector3 position = TransformToMove.position;

		if (instantaneous)
		{
			position = worldPosition;
		}

		position = Vector3.MoveTowards(position, worldPosition, movementSpeed * Time.deltaTime);

		float distance = Vector3.Distance(position, worldPosition);

		TransformToMove.position = position;

		return distance < 0.01f;
	}

	void Start()
	{
		health = maxHealth;
		hpBar.SetMaxHealth(maxHealth);
		hitCollider = hit.GetComponent<SphereCollider>();//GetComponent<SphereCollider>();
		hitCollider.enabled = false;
		// if you don't set this, we'll move ourselves
		if (!TransformToMove)
		{
			TransformToMove = transform;
		}
		// initial position.
		DriveGridCellPositionToWorldPosition(instantaneous: true);
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			hitCollider.enabled = true;
		}
		if(Input.GetMouseButtonUp(0))
		{
			hitCollider.enabled = false;

		}
		// only consider the next move once we've arrived at the cell we're going to
		if (DriveGridCellPositionToWorldPosition())
		{
			int xmove = 0;
			int ymove = 0;

			// we only need to change the cell; the above function
			// will notice our world position is different
			if (Input.GetKeyDown(KeyCode.W) ||
				Input.GetKeyDown(KeyCode.UpArrow))
			{
				ymove = +1;
			}
			if (Input.GetKeyDown(KeyCode.S) ||
				Input.GetKeyDown(KeyCode.DownArrow))
			{
				ymove = -1;
			}
			if (Input.GetKeyDown(KeyCode.A) ||
				Input.GetKeyDown(KeyCode.LeftArrow))
			{
				xmove = -1;
			}
			if (Input.GetKeyDown(KeyCode.D) ||
				Input.GetKeyDown(KeyCode.RightArrow))
			{
				xmove = +1;
			}

			// process the move, if any
			if (xmove != 0 || ymove != 0)
			{
				Vector2 newCellPosition = CellPosition;

				newCellPosition.x += xmove;
				newCellPosition.y += ymove;
				//this will put our hit to last move direction 
				hit.transform.position = new Vector3((int)newCellPosition.x , 0, (int)newCellPosition.y);
				if (grid.grid[(int)newCellPosition.x, (int)newCellPosition.y].walkable)
				{
					CellPosition = newCellPosition;
					

				}

			
				//CellPosition = newCellPosition;

			}
		}
	}
	public void Hit(int damage)
	{
		if (health >= 0)
		{
			health -= damage;
			hpBar.SetHealth(health);
		}
		if (health <= 0)
		{

			//Destroy(gameObject);
			gameObject.SetActive(false);


		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ( "Enemy"))
		{
			other.GetComponent<EnemyBase>().Hit(attackDamge);
		//	Debug.Log("WEEE HITTT themmm");
		}
	}
}