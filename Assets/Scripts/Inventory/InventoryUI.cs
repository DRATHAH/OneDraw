using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public Transform itemsParent;
    public RectTransform descriptionPanel;

    Inventory inventory;
    InventorySlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        inventoryCanvas.SetActive(false);
        descriptionPanel.gameObject.SetActive(false);
        inventory.onItemChangedCallback.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !inventoryCanvas.activeSelf)
        {
            inventoryCanvas.SetActive(true);
            GameManager.instance.SetCursorLock(false);
            PauseManager.instance.pausable = false;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryCanvas.SetActive(false);
            GameManager.instance.SetCursorLock(true);
            PauseManager.instance.pausable = true;
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.uniqueItems.Count)
            {
                slots[i].AddItem(inventory.uniqueItems[i], descriptionPanel, inventory.items[inventory.uniqueItems[i]]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
