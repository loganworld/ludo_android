using UnityEngine;
using UnityEngine.SceneManagement;

public class Playsceneloader : MonoBehaviour
{
    public void LoadMainPlayScene()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }

}