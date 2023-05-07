using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour,IPointerEnterHandler, ISelectHandler, IPointerDownHandler,ISubmitHandler,ICancelHandler
{
    [SerializeField] private GameObject selectedImage;
    private bool isSelected = false;

    public void OnCancel(BaseEventData eventData)
    {
        isSelected = false;
        Debug.Log("Je suis cancel, isSelected est " + isSelected);
        ActivateSelectedImage(isSelected);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerExit");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("pointer entered");
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("JE SUIS SELECTED " + this.gameObject.name);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        isSelected = true;
        Debug.Log("J'AI ETE SOUMIS " + this.gameObject.name);
        ActivateSelectedImage(isSelected);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void ActivateSelectedImage(bool active)
    {
        selectedImage.SetActive(active);
    }
}
