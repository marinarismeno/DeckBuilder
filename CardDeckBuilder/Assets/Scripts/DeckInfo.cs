using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckInfo
{
    public int deckID;
    public List<Data> cards;

    public DeckInfo(int deckID)
    {
        this.deckID = deckID;
        this.cards = new List<Data>();
    }

    public void AddToDeck(Data cardData)
    {
        cards.Add(cardData);
    }

    public void RemoveFromDeck(Data cardInfo)
    {
        foreach (Data item in cards)
        {
            if(item.id == cardInfo.id)
            {
                cards.Remove(item);
            }
        }        
    }

}
