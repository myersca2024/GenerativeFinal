using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerItemsBehavior : MonoBehaviour
{
    public int items = 0;
    public TMP_Text text;

    private void Update()
    {
        text.text = items.ToString();
    }

    public void AddItem()
    {
        items++;
    }
}
