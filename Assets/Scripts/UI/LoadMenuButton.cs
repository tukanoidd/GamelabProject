using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuButton : MonoBehaviour
{
    public int selectedMenuIndex = 0;
    public string menuName = "";

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuName);
    }
}
