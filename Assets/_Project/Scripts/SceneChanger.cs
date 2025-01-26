using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadGamesURL()
    {
        Application.OpenURL("https://alphabeticus.com/product/a2z-card-game/");
    }
}
