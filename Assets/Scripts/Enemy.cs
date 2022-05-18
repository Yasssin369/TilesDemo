using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Grid path;
    public Vector3 position;
    public float movementSpeed;

    public Vector2 Spacing;
    public Vector3 WorldOrigin;
    public Vector2 CellPosition;
    public Node startPosition;
    public Player player;
    public float Range = 5f;
    public int index;
    public int health = 10;
    public GameObject hit;
    public Collider hitCollider;
    public float hitRange = 0.2f;
    public int attackDamage = 1;
    private void Awake()
    {
     

    }
    void Start()
    {
       //We get a random number of cells X,Y grid
        CellPosition.x = Random.Range(0, path.gridSizeX);
        CellPosition.y = Random.Range(0, path.gridSizeY);
     
        //We move the cell to world position in the generated map
        transform.position = ComputeWorldPositionFromGridCell((int)CellPosition.x, (int) CellPosition.y);
        index = System.Array.IndexOf(path.GetComponent<Pathfinding>().seeker, transform);
        //Spawn();
        hitCollider = hit.GetComponent<SphereCollider>();//GetComponent<SphereCollider>();
        hitCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //we check if its range move and stop if player gets away 
        PlayerDetection();
        Attack();
       /* if(path.path!=null)
        { 
        Movement();
        }*/
    }
    void Spawn()
    {
        CellPosition.x = Random.Range(0, path.gridSizeX);
        CellPosition.y = Random.Range(0, path.gridSizeY);
        if (path.grid[(int)CellPosition.x, (int)CellPosition.y].walkable)
        {
            transform.position = ComputeWorldPositionFromGridCell((int)CellPosition.x, (int)CellPosition.y);
        }
      
    }
   
    Vector3 ComputeWorldPositionFromGridCell(int cellX, int cellY)
    {

        // this maps X,Y cell to world, in this case X/Z
        return WorldOrigin + new Vector3(cellX * Spacing.x, 0, cellY * Spacing.y);
    }
    void Movement()
    {
        //We get the Calculated path and just follow it 
        foreach (Node n in path.paths[index])
        {
            if (path.paths[index].Contains(n))
            {
                Vector3 moveDirection = n.worldPosition - new Vector3(path.nodeRadius, 0, path.nodeRadius);
                position = Vector3.MoveTowards(transform.position, moveDirection, movementSpeed * Time.deltaTime);
               
                transform.position = position;
                hit.transform.position = n.worldPosition;// moveDirection;
               
            }

        }
      
        
    }
    void Attack()
    {
         if (Vector3.Distance(hit.transform.position, transform.position) <= hitRange)
         {
            //  hitCollider.enabled = true;
            StartCoroutine(AttackPlayer(0.5f));
         }
        else
        {
           // hitCollider.enabled = false;
        }
    }
   
    void PlayerDetection()
    {
        if(Vector3.Distance(player.transform.position,transform.position)<=Range)
        {
            Movement();
        }
    }
    public void Hit(int damage)
    {
        if (health >= 0)
        {
            health -= damage;
           
        }
        if (health <= 0)
        {

            //Destroy(gameObject);
            gameObject.SetActive(false);


        }
    }
    IEnumerator AttackPlayer(float seconds)
    {
        hitCollider.enabled = true;
        yield return new WaitForSeconds(seconds);
        hitCollider.enabled = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Hit(attackDamage);
            Debug.Log("Muahahahaha");
        }
    }
}
