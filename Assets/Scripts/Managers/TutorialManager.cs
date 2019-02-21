using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] HUD_Tooltip _move, _use, _jump, _shoot, _reload, _katana, _checkpoint;

    #region Singleton
    static TutorialManager instance;
    public static TutorialManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TutorialManager>();
                if (instance == null)
                {
                    instance = new GameObject("new TutorialManager Object").AddComponent<TutorialManager>().GetComponent<TutorialManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    void Start()
    {
        Invoke("StartMove", .3f);
    }

    IEnumerator WaitForCondition(Func<bool> predicate, Action callback)
    {
        yield return new WaitUntil(predicate);

        callback();
    }

    IEnumerator WaitForConditionOrDelay(Func<bool> predicate, float waitTime, Action callback)
    {
        var elapsed = 0f;

        while (!predicate() && elapsed < waitTime)
        {
            yield return new WaitForEndOfFrame();

            elapsed += Time.deltaTime;
        }

        callback();
    }

    IEnumerator WaitThenCallback(float waitTime, Action callback)
    {
        yield return new WaitForSeconds(waitTime);

        callback();
    }

    public void StartMove()
    {
        _move.gameObject.SetActive(true);

        bool predicate() => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || _use.gameObject.activeInHierarchy;
        void callback() => _move.Hide();

        StartCoroutine(WaitForCondition(predicate, callback));
    }

    public void StartUse()
    {
        _use.gameObject.SetActive(true);

        var door = GameObject.Find("FirstDoor").GetComponent<Door>();

        bool predicate() => door.Opened;
        void callback() => _use.Hide();

        StartCoroutine(WaitForCondition(predicate, callback));
    }

    public void StartJump()
    {
        _jump.gameObject.SetActive(true);

        bool predicate() => Input.GetKeyDown(KeyCode.Space);
        void callback() => _jump.Hide();

        StartCoroutine(WaitForCondition(predicate, callback));
    }

    public void StartShoot()
    {
        _shoot.gameObject.SetActive(true);

        bool predicate() => Input.GetMouseButtonDown(0);
        void callback() => _shoot.Hide();

        StartCoroutine(WaitForCondition(predicate, callback));
    }

    public void StartReload()
    {
        _reload.gameObject.SetActive(true);
        bool predicate() => _use.gameObject.activeInHierarchy;
        void callback() => _reload.Hide();

        StartCoroutine(WaitForConditionOrDelay(predicate, 1f, callback));
    }

    public void StartKatana(DestructibleBase dest)
    {
        _katana.gameObject.SetActive(true);

        bool predicate() => !dest.gameObject.activeInHierarchy;
        void callback() => _katana.Hide();

        StartCoroutine(WaitForCondition(predicate, callback));
    }

    public void StartCheckPoint()
    {
        _checkpoint.gameObject.SetActive(true);
        void callback() => _checkpoint.Hide();

        StartCoroutine(WaitThenCallback(1, callback));
    }

}
