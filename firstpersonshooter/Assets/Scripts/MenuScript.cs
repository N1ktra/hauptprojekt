using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void BSPScene()
    {
        SceneManager.LoadScene(1);
    }

    public void InfoScene()
    {
        SceneManager.LoadScene(2);
    }

    public void MenuScene() { SceneManager.LoadScene(0); }
}
