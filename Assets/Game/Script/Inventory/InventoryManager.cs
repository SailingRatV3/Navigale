using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Managing Inventory HUD
 */
public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryMenu;
    private bool menuActivated;
    public InputActionReference inventoryAction;
    
    public ItemSlot[] itemSlot;
    
    
    
    void OnEnable()
    {
        inventoryAction.action.performed += OnInteractPerformed;
        inventoryAction.action.Enable();
    }

    void OnDisable()
    {
        inventoryAction.action.performed -= OnInteractPerformed;
        inventoryAction.action.Disable();
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (menuActivated)
        {
            Time.timeScale = 1;
            inventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (!menuActivated)
        {
            Time.timeScale = 0;
           inventoryMenu.SetActive(true);
           menuActivated = true;
        }
    }

    // Test --
    // Need to add menuing for different items
    // 
   /* public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return;
            }
        }
    }*/

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].isSelected = false;
        }
    }

    public void RefreshUI(List<InventoryItem> items)
    {
        // clear all slots 
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].ClearSlot();
        }
        // Fill Slots
        for (int i = 0; i < items.Count; i++)
        {
            itemSlot[i].SetItem(items[i]);
        }
    }
    
}
