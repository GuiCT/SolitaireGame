using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour
{
public Text textField;

    public void SetText(string content)
    {
        textField.text = content;
    }

    private void Update()
    {
    Vector2 position = Input.mousePosition;

    transform.position = position;

    }
}
