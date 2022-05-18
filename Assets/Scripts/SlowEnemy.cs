using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEnemy : EnemyBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        attackTimer = 0.7f;
        attackDamage = 5;
        movementSpeed = 0.3f;
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
            Debug.Log("Muahahahaha");
        }
    }
}
