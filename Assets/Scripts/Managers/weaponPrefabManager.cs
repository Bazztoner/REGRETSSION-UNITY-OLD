using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class weaponPrefabManager: MonoBehaviour
{
	static weaponPrefabManager instance;
	public static weaponPrefabManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<weaponPrefabManager>();
				if(instance == null)
				{
					instance = new GameObject("new weaponPrefabManager Object").AddComponent<weaponPrefabManager>().GetComponent<weaponPrefabManager>();
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
