using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endLvl : MonoBehaviour
{
    bool showGui = false;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start() {
        canvas = GameObject.Find("Canvas");
    }
    
    // Update is called once per frame
    void Update() {
        if (showGui) {
            canvas.SetActive(true);
            Time.timeScale = 0;
        } else {
            canvas.SetActive(false);
            Time.timeScale = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            showGui = !showGui;
        }
    }

    // private void OnLevelWasLoaded() {
    //     canvas = GameObject.Find("Canvas");
    // }
}
