using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastEnemy : EnemyBase
{
    public override void Start()
    {
        base.Start();
        attackTimer = 0.5f;
        attackDamage =2;
        movementSpeed = 0.5f;
    }



    // Update is called once per frame
    void Update()
    {
       PlayerDetection();
       Attack();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Hit(attackDamage);
            Debug.Log("Hehehehe");
        }
    }
}
