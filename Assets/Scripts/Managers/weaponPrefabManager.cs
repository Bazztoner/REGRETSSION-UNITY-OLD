using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponPrefabManager: MonoBehaviour
{
	static WeaponPrefabManager instance;
	public static WeaponPrefabManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<WeaponPrefabManager>();
				if(instance == null)
				{
					instance = new GameObject("new weaponPrefabManager Object").AddComponent<WeaponPrefabManager>().GetComponent<WeaponPrefabManager>();
				}
			}
			return instance;
		}
	}


	WeaponBase[] _allWeapons;

    void Awake()
    {
		_allWeapons = Resources.LoadAll<WeaponBase>("Prefabs/Weapons");
    }


    public WeaponBase GetWeapon(string wpnName)
    {
		return _allWeapons.FirstOrDefault(x => x.gameObject.name == wpnName);
    }
}
