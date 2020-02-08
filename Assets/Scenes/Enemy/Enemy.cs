using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float      m_speed = 1.0f;
    [SerializeField] float      m_jumpForce = 2.0f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private GroundSensor        m_groundSensor;
    private bool                m_grounded = false;
    private bool 				isDead = false;

    public float               	timeIsDead;
    public int 					health;

    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<GroundSensor>();
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
}
