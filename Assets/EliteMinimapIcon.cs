using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMinimapIcon : MonoBehaviour
{
    private Unit _eliteUnit;
    
    void Start()
    {
        _eliteUnit = GetComponentInParent<Unit>();
        if (_eliteUnit == null)
        {
            Debug.LogError("EliteMinimapIcon: EliteUnit is null");
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (_eliteUnit is not {IsDead: false, CurrentFaction: Faction.Human})
        {
            gameObject.SetActive(false);
        }
    }
}
