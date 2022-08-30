using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnight : MonoBehaviour
{
    float      m_speed = 2f;
    float      m_jumpForce = 7.5f;
    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_timeSinceJump = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private GameObject player;
    private float distanceToEnemy;
    private Vector2 target;
    private float farToEnemy;
    private float farToEnemyVert;
    private float farToEnemyAbs;
    private bool m_FacingRight = true;
    private bool isMoving;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    void Die(){
        m_animator.SetTrigger("Death");
        Destroy(gameObject, 1);
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
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
            if (farToEnemyAbs < 12f && farToEnemyAbs > 1.5f)
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
            if(m_timeSinceAttack > 2f && farToEnemyAbs <= 1.5)
            {
                m_currentAttack++;

                // Loop back to one after third attack
                if (m_currentAttack > 3)
                m_currentAttack = 1;

                // Reset Attack combo if time since last attack is too large
                if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

                // Call one of three attack animations "Attack1", "Attack2", "Attack3"
                m_animator.SetTrigger("Attack" + m_currentAttack);

                // Reset timer
                m_timeSinceAttack = 0.0f;
            }
            
            //Jump
            else if (farToEnemyVert > 1f && !isMoving && m_grounded && player.GetComponent<HeroKnight>().onPlatform && m_timeSinceJump > 2f)
            {
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
}
