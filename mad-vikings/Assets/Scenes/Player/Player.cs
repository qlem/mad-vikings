using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	Image  						rageBar;
	Image						lifeBar;
    Text						timerText;

    [SerializeField] float      m_speed = 1.0f;
    [SerializeField] float      m_jumpForce = 2.0f;

    public string levelToRestart = "enter level name";

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
    public int 					normalRageIncValue = 5;
    public int 					highRageIncValue = 20;

    public	float 				maxRageTime = 15.0f;
    private bool 				isEnraged = false;
    private float 				rageTime;
    public int 					normalRageDecValue = 10;
    public int 					highRageDecValue = 30;

    private void updateBars() {
    	lifeBar.fillAmount = ((health * 100) / maxRage) / 100;
    	rageBar.fillAmount = ((rage * 100) / maxRage) / 100;
        timerText.text = string.Format("{0:N2}", rageTime);
    }

    private void decreaseRage(Enemy hitted) {
    	if (hitted.getIsDead() && rage - highRageDecValue >= 0) {
			rage -= highRageDecValue; 
		} else if (!hitted.getIsDead() && rage - normalRageDecValue >= 0) {
			rage -= normalRageDecValue;
		} else {
			rage = 0;
		}
		if (rage == 0) {
			isEnraged = false;
			rageTime = maxRageTime;
		}
    }

    private void increaseRage(Enemy hitted) {
		if (hitted.getIsDead() && rage + highRageIncValue <= maxRage) {
			rage += highRageIncValue;
		} else if (!hitted.getIsDead() && rage + normalRageIncValue <= maxRage) {
			rage += normalRageIncValue;
		} else {
			rage = maxRage;
		}
		if (rage == maxRage) {
			isEnraged = true;
		}
    }

    public void TakeDamage(int damage) {
    	if (health - damage >= 0) {
			health -= damage;
            m_animator.SetTrigger("Hurt");
		} else {
			health = 0;
		}
		if (health == 0) {
			isDead = true;
		}
    }

    private int computeDamageAmount() {
    	if (isEnraged) {
    		return damage * 2;
    	}
    	return damage;
    }

    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensor>();
        rageBar = GameObject.Find("Rage").GetComponent<Image>();
        lifeBar = GameObject.Find("Life").GetComponent<Image>();
        timerText = GameObject.Find("TimerText").GetComponent<Text>();
        health = maxHealth;
        rageTime = maxRageTime;
    }
	
	void Update () {
        updateBars();

		// Check if player is dead
		if (isDead) {
			m_animator.SetTrigger("Death");
			isEnraged = false;
			rage = 0.0f;
			health = 0;
            SceneManager.LoadScene(levelToRestart);
			return;
		}

		// Check rage events
		if (isEnraged && rageTime <= 0) {
    		isDead = true;
    	} else if (isEnraged) {
    		rageTime -= Time.deltaTime;
    	}

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
            transform.localScale = new Vector3(-7.0f, 7.0f, 7.0f);
        }
        else if (inputX < 0) {
            transform.localScale = new Vector3(7.0f, 7.0f, 7.0f);
        }

        // Move
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        // Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // m_animator.SetTrigger("Recover");

        // Jump
        if (Input.GetKeyDown("space") && m_grounded) {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        // Attack
        if (timeBtwAttack <= 0 && Input.GetMouseButtonDown(0)) {
        		m_animator.SetTrigger("Attack");
        		Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);
        		for (int i = 0; i < enemiesToDamage.Length; i++) {
        			Enemy hitted = enemiesToDamage[i].GetComponent<Enemy>();
        			int amount = computeDamageAmount();
        			hitted.TakeDamage(amount);
        			if (!isEnraged) {
        				increaseRage(hitted);
        			} else {
        				decreaseRage(hitted);
        			}
        		}
        		timeBtwAttack = startTimeBtwAttack;
        } else {
        	timeBtwAttack -= Time.deltaTime;
        }


        //Change between idle and combat idle
       	if (Input.GetKeyDown("f")) {
            m_combatIdle = !m_combatIdle;
        }

        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon) {
            m_animator.SetInteger("AnimState", 2);
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

    
}
