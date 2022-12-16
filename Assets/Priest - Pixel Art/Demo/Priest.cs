using UnityEngine;
using UnityEngine.SceneManagement;

public class Priest : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor              m_groundSensor;
    private bool                m_grounded = false;
    private float               m_delayToIdle = 0.0f;
    private int                 m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float timeSpecialAttack = 0.0f;
    private int healthPoints = 3;
    private Collider2D playerHitBox;
    private bool is_Blocking = false;
    private bool is_Moving = false;
    private int m_facingDirection;
    [SerializeField] Transform attackPoint;
    [SerializeField] GameObject _projectile;
    [SerializeField] Vector3 _projectionSpawnOffset;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();
        playerHitBox = gameObject.GetComponent<Collider2D>();
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
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }


    // Update is called once per frame
    void Update ()
    {
        m_timeSinceAttack += Time.deltaTime;
        timeSpecialAttack += Time.deltaTime;

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

        else if (inputX < 0 & !is_Blocking )
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
        if (!is_Blocking)
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        }

        if (m_body2d.velocity.y < 0) gameObject.GetComponent<Rigidbody2D>().gravityScale = 4.5f;
        else gameObject.GetComponent<Rigidbody2D>().gravityScale = 3f;

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);


        if (Input.GetMouseButtonDown(0) & m_timeSinceAttack > 0.5f & !is_Blocking)
        {
            m_currentAttack++;
        
            m_animator.SetTrigger("Attack");

            m_timeSinceAttack = 0.0f;
        }

        if (Input.GetKeyDown("w") && timeSpecialAttack > 5f)
        {
            m_animator.SetTrigger("Heal");
            // Reset timer
            timeSpecialAttack = 0.0f;
        }

        // Block
        if (Input.GetMouseButtonDown(1))
        {
            is_Blocking = true;
            m_animator.SetTrigger("Block");
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

    public void SpawnProjectile()
    {
        if (_projectile != null)
        {
            // Set correct arrow spawn position
            Vector3 facingVector = new Vector3(m_facingDirection, 1, 1);
            Vector3 projectionSpawnPosition = transform.localPosition + Vector3.Scale(_projectionSpawnOffset, facingVector);
            GameObject bolt = Instantiate(_projectile, projectionSpawnPosition, Quaternion.identity) as GameObject;
            // Turn arrow in correct direction
            bolt.transform.localScale = facingVector;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Platform")) { Statics.isPlayerOnPlatform = true; }
        else { Statics.isPlayerOnPlatform = false; }
    }
    
}