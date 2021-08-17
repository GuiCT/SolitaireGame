using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TooltipSystem : MonoBehaviour
{
    public Text textField;
    public Tooltip tooltip;
    private static TooltipSystem current;

    public void Awake()
    {
        current = this;
        Hide();
    }

    public static void Show(string content)
    {
        current.tooltip.SetText(content);
        current.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if (current.tooltip != null) {
            current.tooltip.gameObject.SetActive(false);
        }
    }
}
