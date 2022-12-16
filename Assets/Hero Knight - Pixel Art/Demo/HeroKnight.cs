﻿using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroKnight : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] float m_jumpForce;
    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor m_groundSensor;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float timeSpecialAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private int healthPoints = 3;
    private Collider2D playerHitBox;
    private bool is_Blocking = false;
    private bool is_Moving = false;
    private int m_facingDirection;

    //ATTACK
    [SerializeField] LayerMask attackLayers;
    [SerializeField] Transform attackPoint;

    // TODO:
    public HeroType heroType;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        playerHitBox = gameObject.GetComponent<Collider2D>();
    }

    void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPoint.position, 0.73f, attackLayers);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                // col.gameObject.GetComponent<Animator>().SetTrigger("Hurt");
                col.gameObject.GetComponent<EnemyKnight>().Hurt();
                //StartCoroutine(DieDelay(col));
            }
            else if(col.CompareTag("RangedEnemy")){
                col.gameObject.GetComponent<Crossbowman>().Hurt();
            }

        }
    }

    public void Hurt(int facingDirection)
    {
        //Hurt        
        if (!(is_Blocking && facingDirection == m_facingDirection))
        {
            m_animator.SetTrigger("Hurt");
            if (healthPoints > 0) healthPoints--;
            if (healthPoints <= 0)
            {
                Die();
            }
        }
        else
        {
            m_animator.SetTrigger("Block");
        }

    }

    void Die()
    {
        //Death
        //m_animator.SetTrigger("Death");
        Destroy(gameObject);
        SceneManager.LoadScene(0);
        
        
    }

    // Update is called once per frame
    void Update()
    {

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;
        timeSpecialAttack += Time.deltaTime;

        // // Increase timer that checks roll duration
        // if (m_rolling)
        //     m_rollCurrentTime += Time.deltaTime;

        // // Disable rolling if timer extends duration
        // if (m_rollCurrentTime > m_rollDuration)
        //     m_rolling = false;

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

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 & !is_Blocking)
        {
            attackPoint.transform.localPosition = new Vector3(0.8f, 0.8f, 0);
            GetComponent<SpriteRenderer>().flipX = false;
            is_Moving = true;
            m_facingDirection = 1;
        }

        else if (inputX < 0 & !is_Blocking)
        {
            attackPoint.transform.localPosition = new Vector3(-0.8f, 0.8f, 0);
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            is_Moving = true;
        }

        else if (inputX < Mathf.Epsilon)
        {
            is_Moving = false;
        }

        // Move
        if (!m_rolling & !is_Blocking)
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        }

        if (m_body2d.velocity.y < 0) gameObject.GetComponent<Rigidbody2D>().gravityScale = 4.5f;
        else gameObject.GetComponent<Rigidbody2D>().gravityScale = 3f;

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !is_Blocking)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            if(heroType == HeroType.Warrior)  m_animator.SetTrigger("Attack" + m_currentAttack);
            else
            m_animator.SetTrigger("Attack");
           

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        
        if (Input.GetKeyDown("w") && timeSpecialAttack > 5f)
        {
            
            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("SpecialAttack");

            // Reset timer
            timeSpecialAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1))
        {
            is_Blocking = true;
            m_animator.SetTrigger("TriggerBlock");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
        {
            is_Blocking = false;
            m_animator.SetBool("IdleBlock", false);
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !is_Blocking)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
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
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    // // Animation Events
    // // Called in slide animation.
    // void AE_SlideDust()
    // {
    //     Vector3 spawnPosition;

    //     if (m_facingDirection == 1)
    //         spawnPosition = m_wallSensorR2.transform.position;
    //     else
    //         spawnPosition = m_wallSensorL2.transform.position;

    //     if (m_slideDust != null)
    //     {
    //         // Set correct arrow spawn position
    //         GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
    //         // Turn arrow in correct direction
    //         dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
    //     }
    // }
   

    // // TODO on collision with enemey play hurt animation
    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.tag == "Enemy") ;
    // }
}
