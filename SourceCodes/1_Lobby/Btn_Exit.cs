using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Exit : Btn_Parent
{
    public override void Init()
    {
        GetComponent<Button>().onClick.AddListener(GameManager.Instance.QuitGame);
    }
}
