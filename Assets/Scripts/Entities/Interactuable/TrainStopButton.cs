using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class TrainStopButton : MonoBehaviour, IInteractuable
{
    bool _endLvl = false;

    void Awake()
    {
        //StartCoroutine(LoadAsync());
    }

    public void Use()
    {
        _endLvl = true;
        SceneManager.LoadScene("Endgame");
    }

    IEnumerator LoadAsync()
    {
        var asyncOp = SceneManager.LoadSceneAsync("Endgame", LoadSceneMode.Single);
        asyncOp.allowSceneActivation = false;

        while (!asyncOp.isDone && !_endLvl)
        {
            yield return new WaitForEndOfFrame();
        }

        asyncOp.allowSceneActivation = true;
    }
}
