using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class HUDController: MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] Image[] _keys;
    [SerializeField] TextMeshProUGUI _hpText, _ammoText;
    [SerializeField] Animator _damageFeedback;

    #region Singleton
    static HUDController instance;
	public static HUDController Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<HUDController>();
				if(instance == null)
				{
					instance = new GameObject("new HUDController Object").AddComponent<HUDController>().GetComponent<HUDController>();
				}
			}
			return instance;
		}
	}
    #endregion

    public void SetAmmo(string ammo)
    {
        _ammoText.text = ammo;
    }

    public void SetHealth(string health)
    {
        _hpText.text = health;
    }

    public void SetKey(KeysForDoors key)
    {
        _keys[(int)key].gameObject.SetActive(true);
    }

    public void OnDamage()
    {
        _damageFeedback.CrossFadeInFixedTime("Activate", .1f);
    }
}
