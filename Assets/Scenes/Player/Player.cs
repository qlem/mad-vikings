using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	Image  						rageBar;
	Image						lifeBar;

    [SerializeField] float      m_speed = 1.0f;
    [SerializeField] float      m_jumpForce = 2.0f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private GroundSensor        m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool 				isDead = false;
    private float 				rage = 0.0f;
    private float 				health;

    private float 				timeBtwAttack;
    public float 				startTimeBtwAttack;

    public Transform 			attackPos;
    public LayerMask			whatIsEnemies;
    public float 				attackRange;
    public int 					damage;

    public float 				maxHealth;
    public float 				maxRage;
    public int 					normalRageIncValue;
    public int 					highRageIncValue;

    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensor>();
        rageBar = GameObject.Find("Rage").GetComponent<Image>();
        lifeBar = GameObject.Find("Life").GetComponent<Image>();
        health = maxHealth;
        rageBar.fillAmount = 0.0f;
    }
	
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

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0) {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (inputX < 0) {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        // Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // m_animator.SetTrigger("Recover");

        // Attack
        if (timeBtwAttack <= 0 && Input.GetMouseButtonDown(0)) {
        		Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        		for (int i = 0; i < enemiesToDamage.Length; i++) {
        			enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
        			if (rage < maxRage) {
        				if (enemiesToDamage[i].GetComponent<Enemy>().getIsDead()) {
        					rage += highRageIncValue;
        				} else {
        					rage += normalRageIncValue;
        				}
        				rageBar.fillAmount = ((rage * 100) / maxRage) / 100;
        			}
        		}
        		timeBtwAttack = startTimeBtwAttack;
        		m_animator.SetTrigger("Attack");
        } else {
        	timeBtwAttack -= Time.deltaTime;
        }

        if (isDead) {
        	m_animator.SetTrigger("Death");
        }

        //Change between idle and combat idle
       	else if (Input.GetKeyDown("f")) {
            m_combatIdle = !m_combatIdle;
        }

        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon) {
            m_animator.SetInteger("AnimState", 2);
        }

        // Jump
        else if (Input.GetKeyDown("space") && m_grounded) {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        // Combat Idle
        else if (m_combatIdle) {
            m_animator.SetInteger("AnimState", 1);
        }

        // Idle
        else {
            m_animator.SetInteger("AnimState", 0);
        }
    }

    void OnDrawGizmosSelected() {
    	Gizmos.color = Color.red;
    	Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    public void TakeDamage(int damage) {
    	if (health <= 0 ) {
    		isDead = true;
    	} else {
    		health -= damage;
    		lifeBar.fillAmount = ((health * 100) / maxRage) / 100;;
    		m_animator.SetTrigger("Hurt");
    	}
    }
}
