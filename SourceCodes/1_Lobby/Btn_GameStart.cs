using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn_GameStart : Btn_Parent
{
    [SerializeField] GameObject difficultySelectPanel;

    public override void Init()
    {
        GetComponent<Button>().onClick.AddListener(() => difficultySelectPanel.SetActive(true));
    }
}
