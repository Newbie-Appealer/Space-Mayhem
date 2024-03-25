using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemStack;

    [Header("Slot Information")]
    [SerializeField] private bool _usedSlot;


    #region UI Image
    public void F_UpdateSlot(int v_code, int v_stack)
    {
        _itemImage.sprite = ResourceManager.Instance.F_GetInventorySprite(v_code);
        _itemStack.text = v_stack.ToString();
        _usedSlot = true;
    }

    public void F_EmptySlot()
    {
        _itemImage.sprite = ResourceManager.Instance.emptySlotSprite;
        _itemStack.text = "";
        _usedSlot = false;
    }
    #endregion    //인벤토리 슬롯 아이템 드래그 시작

}
