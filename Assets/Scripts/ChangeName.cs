using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeName : MonoBehaviour
{
    public TMP_InputField inputField;
    public void ChangePlayerName()
    {
        MenuManager.Instance.PlayerName = inputField.text;
    }
}
