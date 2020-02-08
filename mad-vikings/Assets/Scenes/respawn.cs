using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class respawn : MonoBehaviour
{
    public string levelToRestart = "enter level name";
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player")
            SceneManager.LoadScene(levelToRestart);
    }
}
