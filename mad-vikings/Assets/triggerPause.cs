using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerPause : MonoBehaviour
{
    bool showGui = false;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start() {
        canvas = GameObject.Find("CanvasPause");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            showGui = !showGui;
        }

        if (showGui) {
            canvas.SetActive(true);
            Time.timeScale = 0;
        } else {
            canvas.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void returnInTheGame() {
        showGui = !showGui;
    }
}
