using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldInfo : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitUntil(()=> Player.Instance.initialized);
        
        PlayerStats.Instance.onGoldChanged.AddListener(ChangeGoldUI);
        ChangeGoldUI(0,0,PlayerStats.Instance.CurrGold);
    }

    void ChangeGoldUI(int amount, int before, int after)
    {
        GetComponentInChildren<TextMeshProUGUI>().SetText( $"<sprite name=\"0\">{after}");
    }
}
