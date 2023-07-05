using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject MenuObjects;
    public GameObject InfoObjects;

    public void BSPScene()
    {
        SceneManager.LoadScene(1);
    }

    public void InfoScene()
    {
        MenuObjects.SetActive(false);
        InfoObjects.SetActive(true);
    }

    public void MenuScene() 
    {
        InfoObjects.SetActive(false);
        MenuObjects.SetActive(true);
    }
}
