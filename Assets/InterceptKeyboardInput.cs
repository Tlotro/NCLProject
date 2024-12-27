using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderMouseOnly : MonoBehaviour
{
    private Selectable selectable;

    void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    void Update()
    {
        if (selectable.IsInteractable() && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (Input.anyKeyDown)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}