using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();
}
public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData ItemData;

    public string GetInteractPrompt()
    {
        string str = $"{ItemData.displayName}\n{ItemData.description}";
        return str;
    }

    public void OnInteract()
    {
        if (ItemData.type != ItemType.Box)
        {
            CharcterManager.Instance.player.itemData = ItemData;
            CharcterManager.Instance.player.addItem?.Invoke();
        }
        Destroy(gameObject);
    }
}
