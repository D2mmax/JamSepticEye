using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen = false;

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        gameObject.SetActive(false); // disables the door so the player/enemy can pass through
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        gameObject.SetActive(true); // re-enables the door
    }
}
