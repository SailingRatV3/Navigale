using NUnit.Framework.Constraints;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
   // [Header("Inventory Item Reference")]
   private InventoryItem inventoryItem;
    [Header("UI Elements")]
    // Item Slot
    [SerializeField]
    private TMP_Text quantityText;
    
    [SerializeField]
    private Image itemImage;
/*
 *  [Header("Inventory Item Reference")]
    // Call item Name, Quantity, sprite, 
    ItemData data; 
    InventoryItem inventoryItem;
    
    [Header("UI Elements")]
    // Item Slot
    [SerializeField]
    private TMP_Text quantityText;
    
    [SerializeField]
    private Image itemImage;
 */
    public bool isFull;

    [Header("Item Slot Select Reference")]
    public GameObject selectedShader;
    public bool isSelected;
    
    private Inventory inventory;
    private InventoryManager inventoryManager;
    
    //===Item Description===//
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionText;
    public TMP_Text itemDescriptionNameText;
    public Sprite emptySprite;
    
    void Start()
    {
       inventory = GameObject.Find("Player").GetComponent<Inventory>();
     //  data = GetComponentInParent<ItemData>();
     //  inventoryItem = GetComponentInParent<InventoryItem>();
       inventoryManager = GameObject.Find("Menu-Canvas").GetComponent<InventoryManager>();
       /*
      string itemNameDats =  data.itemName;
      Sprite itemIconData =  data.itemIcon;
      int itemQuantityData = inventoryItem.quantity;
      */
    }

    void Update()
    {
        
    }

    /*public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        string itemNameData =  data.itemName;
        Sprite itemIconData =  data.itemIcon;
        int itemQuantityData = inventoryItem.quantity;

       //  this.data.itemDescription
        this.data.itemIcon= itemIconData;
        this.inventoryItem.quantity = quantity;
        this.data.itemName = itemName;
        this.data.itemDescription = itemDescription;
        isFull = true;
        
        quantityText.text = quantityText.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;
        
    }*/

    // Might need to update this function to add with the input system for controllers input
    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventoryItem == null) return;
        // Whenever the item is clicked 
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
         if (inventoryItem == null) return;
        
        inventoryManager.DeselectAllSlots();
        
        selectedShader.SetActive(true);
        isSelected = true;
        
        ItemData data = inventoryItem.itemData;
        if (data == null) return;
        
        itemDescriptionNameText.text = data.itemName;
        itemDescriptionImage.sprite = data.itemIcon;
        itemDescriptionText.text = data.itemDescription;
/*
        if (itemDescriptionImage.sprite == null)
        {
            itemDescriptionImage.sprite = emptySprite;
        }*/
    }

    public void OnRightClick()
    {
        if(inventoryItem == null) return;
       inventory.EquipItem(inventoryItem.itemData); 
    }

    public void ClearSlot()
    {
       inventoryItem = null;
    
       itemImage.sprite = null;
       quantityText.text = "";
       quantityText.enabled = false;
       
       isFull = false;
       isSelected = false;
       selectedShader.SetActive(false);
    }

    public void SetItem(InventoryItem item)
    {
        Debug.Log("Setting item: " + item.itemData.itemName);
        
      inventoryItem = item;
      
      itemImage.sprite = item.itemData.itemIcon; // maybe change later (adding item icon and inventory icon)
      quantityText.text = item.quantity.ToString();
      quantityText.enabled = item.quantity >= 1;
      isFull = true;
    }
}
