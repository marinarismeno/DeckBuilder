using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfo : MonoBehaviour
{
    public Data cardData;
    public Image myImage;
    public GameObject loadingPanel;
    public Button thisButton;
    private Manager manager;

    private void Awake()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    public void Init(Data _data)
    {
        cardData = _data;
        loadingPanel.SetActive(true);
        StartCoroutine(Util.LoadImageFromURL(cardData.images.small, SetSprite));
    }

    void SetSprite(Sprite _sprite)
    {
        if (_sprite != null)
        {
            myImage.sprite = _sprite;
            loadingPanel.SetActive(false);
        }
    }

    //public void ShowCardMenu(bool b)
    //{
    //    if (b && CardExistsInDeck() != -1)
    //        removeButton.interactable = false; // disable if doesn't exists in the deck

    //    if (b && cardMenuPanel.activeInHierarchy) // if already open, then close
    //        cardMenuPanel.SetActive(!b);
    //    else
    //        cardMenuPanel.SetActive(b);
    //}

    public void CardGotSelected_to_add()
    {
        if (thisButton.interactable) // if it just got selected
        {
            // add to selected cards
            manager.selectedCards.Add(cardData);
        }
        thisButton.interactable = !thisButton.interactable; 
    }

    public void DeleteCardFromDeck()
    {
        manager.RemoveCardFromDeck(cardData);

        this.transform.SetParent(manager.Pool);
        //Destroy(this.gameObject);
    }
}
