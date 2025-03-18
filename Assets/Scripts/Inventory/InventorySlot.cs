using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Item item;
    public Image icon;
    public GameObject removeButton;
    public RectTransform description;
    public TMP_Text number;
    public int amount = 0;

    bool mouseOverlap = false;

    public void AddItem(Item newItem, RectTransform panel, int num)
    {
        description = panel;

        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        amount = num;
        removeButton.SetActive(true);
        number.transform.parent.gameObject.SetActive(true);
        number.text = amount.ToString();
    }

    public void UseItem()
    {
        if (item)
        {
            item.Use();
            Inventory.instance.Remove(item);
        }
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.SetActive(false);
        if (description)
        {
            description.GetComponentInChildren<TMP_Text>().text = "";
        }
        number.transform.parent.gameObject.SetActive(false);
        amount = 0;
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
        mouseOverlap = false;
        description.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item)
        {
            mouseOverlap = true;
            description.GetComponentInChildren<TMP_Text>().text = item.desc;
            description.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (mouseOverlap && item && description.gameObject.activeSelf)
        {
            Vector3 pos = Input.mousePosition;
            description.position = pos;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item && mouseOverlap)
        {
            mouseOverlap = false;
            description.gameObject.SetActive(false);
        }
    }
}
