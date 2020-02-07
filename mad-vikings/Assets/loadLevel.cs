using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadLevel : MonoBehaviour
{
    public string levelname = "enter level name";
    public string levelToRestart = "enter level name";

    public void loadTheLevel() {
        SceneManager.LoadScene(levelname);
    }

    public void reloadTheLevel() {
        SceneManager.LoadScene(levelToRestart);
    }
}
