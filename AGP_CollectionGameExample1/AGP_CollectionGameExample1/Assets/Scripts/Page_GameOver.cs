using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Page_GameOver : MonoBehaviour
{
    public GameObject returnToTitleMessage;
    [SerializeField]
    TextMeshProUGUI winnerMessage;

    public void SetWinnerMessage(string message)
    {
        winnerMessage.text = message;
    }
}
