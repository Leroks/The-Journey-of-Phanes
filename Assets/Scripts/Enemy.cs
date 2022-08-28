using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float boomerSpeed = 10f;
    public Rigidbody2D rb;
    Rigidbody2D playerRB;
    public GameObject deathEffect;
    private float farToEnemy;
    private bool m_FacingRight = true;
    GameObject player;
    private Vector2 target;
    private float distanceToEnemy;
    private bool canMove = true;
    private bool canAttack = true;
    private bool canJump=true;
    public Animator animator;
    private bool isDead=false;
    private bool isEnemyDead;
    public CapsuleCollider2D Capsulecollider;
    private bool collided = false;
    private float posy;

    private Vector3 pivot;

    private bool isDied = false;

    public GameObject raycaster;
    private bool raycasterMove;

    private bool playedOnce = true;

    public void TakeDamage()
    {    
        Die();
    }
        void Die()
    {
        
        isDead = true;

        posy = transform.position.y + 1;
        pivot = new Vector3(transform.position.x, posy, 0f);

        GetComponent<CapsuleCollider2D>().enabled = false;
        Capsulecollider.enabled = false;
      
        rb.gravityScale = 0;

        animator.SetTrigger("isDead");
        //AudioManagerScript.PlaySound(AudioManagerScript.Sound.deathSound1, pivot);
        GameObject effects = Instantiate(deathEffect, pivot, Quaternion.identity);

        Destroy(effects, 5.0f);
        
    
        Destroy(gameObject, 1f);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        raycasterMove = raycaster.GetComponent<EnemyMovementBlocker>().raycastMove;
        
        if (player != null)
            {
                farToEnemy = (transform.position.x - target.x);
                target = new Vector2(player.transform.position.x, player.transform.position.y);
                distanceToEnemy = Vector2.Distance(player.transform.position, transform.position);
                if (distanceToEnemy < 12f)
                {
                    
                /*
                if (GameObject.FindGameObjectWithTag("Player") != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target, boomerSpeed * Time.deltaTime);
                }
                */

                if (canMove == true && isDied==false)
                {
                    if (raycasterMove)
                    {
                        animator.SetBool("isWalking", true);
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, transform.position.y, transform.position.z), boomerSpeed * Time.deltaTime);
                    }
                    else
                    {
                        animator.SetBool("isWalking", false);
                    }
                    //transform.position += new Vector3(-boomerSpeed * Time.deltaTime * multiplier, 0f,0f);
                    if (farToEnemy > 0 && !m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();

                    }
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (farToEnemy <= 0 && m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                        
                    }
                }

                if (player.transform.position.x == transform.position.x)
                {
                    
                    if(Mathf.Abs(playerRB.velocity.x) > 1 && !raycasterMove)
                    {
                        animator.SetBool("isWalking", true);
                    }
                    else
                        animator.SetBool("isWalking", false);
                }
                }


            if (canAttack == true && isDead == false && isEnemyDead == false && collided)
            {
                //AudioManagerScript.PlaySound(AudioManagerScript.Sound.boomerBlast, transform.position);
                

                //charactercontroller2d.TakeDamage(boomDamage);

                posy = transform.position.y + 1;
                pivot = new Vector3(transform.position.x, posy, 0f);

                GameObject effects = Instantiate(deathEffect, pivot, Quaternion.identity);

                Destroy(effects, 5.0f);

                Destroy(gameObject);

            }


        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
   
    IEnumerator JumpWaiter()
    {
        animator.SetTrigger("isJumped");
        rb.AddForce(new Vector2(0f, 400f));
        canJump = false;
        yield return new WaitForSeconds(1);
        canJump = true;
        
    }

    void OnCollisionEnter2D(Collision2D hitInfo)
    {
      if(hitInfo.gameObject.CompareTag("Player"))
        {
            //isEnemyDead = charactercontroller2d.isDead;
            collided = true;
        }
    }
  
    void OnTriggerStay2D(Collider2D hitInfo)
    {

        if (hitInfo.CompareTag("Map"))
        {
            if (canJump)
            {

                StartCoroutine(JumpWaiter());
            }

              
        }

    }

    void Walk1()
    {

        //AudioManagerScript.PlayEnemyWalkSound(AudioManagerScript.Sound.enemyBoomerWalk, pivot);

    }

    void Walk2()
    {

        //AudioManagerScript.PlayEnemyWalkSound(AudioManagerScript.Sound.enemyBoomerWalk2, pivot);

    }


}
