using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float      m_speed = 5.0f;
    [SerializeField] float      m_jumpForce = 17.0f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private GroundSensor        m_groundSensor;
    private bool                m_grounded = false;
    private bool 				isDead = false;

    public float               	timeIsDead;
    public int 					health;

    private float 				timeBtwAttack = 0.0f;
    private float 				startTimeBtwAttack = 0.9f;
    private Vector2 basePositions;
    private float distance;
    private float distanceBase;
    //target of the ennemy
    public Transform Target;

    // Distance poursuit
    public float chaseRange = 10;

    // attack scope
    public float attackRange = 2.2f;
    public Transform 			attackPos;
    public LayerMask			whatIsEnemies;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensor>();
        basePositions = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        // Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Check if character just started falling
        if (m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (!isDead) {
            // search player
            Target = GameObject.Find("Player").transform;

            // Compute distance player / ennemy
            distance = Vector2.Distance(Target.position, transform.position);

            // Compute distance bettwen ennemy and base position ennemy
            distanceBase = Vector2.Distance(basePositions, transform.position);

            // When ennemy is near but can't attack
            if (distance <= chaseRange && distance > attackRange) {
                chase();
            }

            // When ennemy is near for attack
            if (distance < attackRange) {
                attack();
            }
            
            // When player escape
            if (distance > chaseRange && distanceBase > 1) {
                backBase();
            }
        }

        if (isDead) {
        	m_animator.SetTrigger("Death");
        	if (timeIsDead < 0) {
        		Destroy(gameObject);
    		} else {
    			timeIsDead -= Time.deltaTime;
    		}
        }
    }

    public void TakeDamage(int damage) {
    	if (health <= 0 ) {
    		isDead = true;
    	} else {
    		health -= damage;
    		m_animator.SetTrigger("Hurt");
    	}
    }

    public bool getIsDead() {
    	return isDead == true;
    }

    // Back initial position ennemy
    public void backBase() {

    }

    public void attack() {
        if (timeBtwAttack <= 0) {
            Target.GetComponent<Player>().TakeDamage(3);
            timeBtwAttack = startTimeBtwAttack;
            m_animator.SetTrigger("Attack");
        } else {
        	timeBtwAttack -= Time.deltaTime;
        }
    }

    void chase() {
        float targetX = Target.position.x;

        if (targetX > transform.position.x) {
            transform.localScale = new Vector3(-7.0f, 7.0f, 7.0f);
            m_body2d.velocity = new Vector2(1 * m_speed, m_body2d.velocity.y);
        }
        else if (targetX < transform.position.x) {
            transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);
            m_body2d.velocity = new Vector2(-1 * m_speed, m_body2d.velocity.y);
        }
        m_animator.SetInteger("AnimState", 2);
    }
}
