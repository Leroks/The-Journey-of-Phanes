using UnityEngine;
using System.Collections;

public class Crossbowman : MonoBehaviour {

    [SerializeField] float      m_speed = 1.4f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] GameObject m_projectile;
    [SerializeField] Vector3    m_projectionSpawnOffset;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Humans     m_groundSensor;
    private bool                m_grounded = false;
    private float               m_delayToIdle = 0.0f;
    private int                 m_facingDirection = 1;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_timeSinceJump = 0.0f;

    private GameObject player;
    // private float distanceToEnemy;
    private Vector2 target;
    private float farToEnemy;
    private float farToEnemyVert;
    private float farToEnemyAbs;
    private bool m_FacingRight = true;
    private bool isMoving;
    public bool canMove = true;
    private int health = 2;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Humans>();
    }

    private IEnumerator DieDelay()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Hurt");
        yield return new WaitForSeconds(0.2f);
        gameObject.GetComponent<Crossbowman>().Die();
    }

    public void Hurt(){
        StartCoroutine(DieDelay());
    }

    private void Die(){
        health--;
        if(health <= 0){
             GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>().Spawn();
            //m_animator.SetTrigger("Death");
            //Destroy(gameObject, 1);
            Destroy(gameObject);
        }
    }

    private void Flip()
    {
        if(m_FacingRight){
            gameObject.transform.Rotate(0, 180, 0);
        }
        else{
            gameObject.transform.Rotate(0, -180, 0);
        }
        m_FacingRight = !m_FacingRight;
        //GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
    }

    

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;
        m_timeSinceJump += Time.deltaTime;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (farToEnemy > 0.2f && !m_FacingRight)
        {
            Flip();
            m_facingDirection = -m_facingDirection;
        }

        else if (farToEnemy < -0.2f && m_FacingRight)
        {
            Flip();
            m_facingDirection = -m_facingDirection;
        }

        if (player != null)
        {
            target = new Vector2(player.transform.position.x, player.transform.position.y);
            farToEnemy = (transform.position.x - target.x);
            farToEnemyVert = (target.y - transform.position.y);
            farToEnemyAbs = Mathf.Abs(farToEnemy);
            //distanceToEnemy = Vector2.Distance(player.transform.position, transform.position);

            // Move
            if (farToEnemyAbs < 40f && farToEnemyAbs > 6f && canMove)
            {
                isMoving = true;
                m_body2d.velocity = new Vector2(-m_facingDirection * m_speed, m_body2d.velocity.y); 
            }
            else{
                isMoving = false;
            }
                
            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

            //Attack
            if(m_timeSinceAttack > 2f && farToEnemyAbs <= 6f)
            {
                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack");
               

                // Reset timer
                m_timeSinceAttack = 0.0f;
            }
            
            //Jump
            else if (farToEnemyVert > 1f & !isMoving & m_grounded & Statics.isPlayerOnPlatform & m_timeSinceJump > 2f)
            {
                // Reset timer for attack
                m_timeSinceAttack = 1.5f;

                m_animator.SetTrigger("Jump");
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                m_groundSensor.Disable(0.2f);
                m_timeSinceJump = 0.0f;
            }

            //Run
            else if (isMoving)
            {
                // Reset timer
                m_delayToIdle = 0.05f;
                m_animator.SetInteger("AnimState", 1);
            }

            //Idle
            else
            {
                // Prevents flickering transitions to idle
                m_delayToIdle -= Time.deltaTime;
                    if(m_delayToIdle < 0)
                        m_animator.SetInteger("AnimState", 0);
            }
        } 
    }
    
    //Called in Attack animation.
    public void SpawnProjectile()
    {
        if (m_projectile != null)
        {
            // Set correct arrow spawn position
            Vector3 facingVector = new Vector3(-m_facingDirection, 1, 1);
            Vector3 projectionSpawnPosition = transform.localPosition + Vector3.Scale(m_projectionSpawnOffset, facingVector);
            GameObject bolt = Instantiate(m_projectile, projectionSpawnPosition, Quaternion.identity) as GameObject;
            // Turn arrow in correct direction
            bolt.transform.localScale = facingVector;
        }
    }
}
