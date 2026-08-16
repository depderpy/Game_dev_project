using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryScreen;
    [SerializeField] private Text inventoryText;
    [SerializeField] private GameManager gameManager;

    private bool isOpen = false;

    private void Start()
    {
        inventoryScreen.SetActive(false);
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        inventoryScreen.SetActive(isOpen);

        if (isOpen)
        {
            UpdateInventory();
        }
    }

    private void UpdateInventory()
    {
        inventoryText.text = "===== INVENTORY =====\n\n";

        foreach (Item item in gameManager.Inventory.items)
        {
            inventoryText.text +=
                item.itemName + " x" + item.quantity + "\n";
        }

        inventoryText.text += "\nPress I to close";
    }
}