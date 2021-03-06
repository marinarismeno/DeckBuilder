using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public List<GameObject> MenuPanels;

    public GameObject CardPrefab;
    public GameObject CardTitlePrefab;

    public Transform CollectionContentTransform;
    public Transform DeckContentTransform;
    public Transform DeckDropdownTransform;
    public Transform SortingDropdown;
    public Transform Pool;

    private string request;
    PokemonMultiData cards;
    public List<DeckInfo> AllDecks;
    private TMP_Dropdown deckDropdown;
    public List<Data> selectedCards;
    public List<Data> selectedCards_to_delete;


    public int ActiveDeck_id;
    private void Awake()
    {
        ActivatePanel(MenuPanels[3]); // loading panel
    }

    public void Start()
    {       
        selectedCards = new List<Data>();
        deckDropdown = DeckDropdownTransform.GetComponent<TMP_Dropdown>();
        request = "https://api.pokemontcg.io/v2/cards?page=1&pageSize=60";

        deckDropdown.ClearOptions();
        ClearContentsOf(DeckContentTransform);
        AddDeckOption();
        deckDropdown.value = 0;

        StartCoroutine(Util.ApiRequest(request, OnCardsReceived));
        
    }

    /**
     * set active the panel needed and disable the rest
     */
    public void ActivatePanel(GameObject panel)
    {
        foreach (GameObject item in MenuPanels)
        {
            if (item == panel)
                item.SetActive(true); // enable the needed one 
            else
                item.SetActive(false); // disable the rest
        }
    }

    public void OnCardsReceived(string _json)
    {
        if (_json != null)
        {
            cards = JsonConvert.DeserializeObject<PokemonMultiData>(_json);
            ActivatePanel(MenuPanels[0]); // main menu
            
        }
    }

    /**
     * called by the create a deck button on the main menu (OnClick)
     */
    public void SetUpCardCollection()
    {       
        SetUpRarityNo();
           
        ClearContentsOf(CollectionContentTransform);            

        Debug.Log("cards size: " + cards.data.Count);

        foreach (Data item in cards.data)
        {
            GameObject newCard_GO;
            newCard_GO = Instantiate(CardPrefab, CollectionContentTransform);

            newCard_GO.GetComponent<CardInfo>().Init(item);
        }
        SortCollectionCards(0);
       
    }


    public IEnumerator LoadImageFromURL(string URL, Image cell)
    {
        if (!string.IsNullOrEmpty(URL))
        {
            // Check internet connection
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return null;
            }
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL);
            yield return www.SendWebRequest();

            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            cell.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
        }
    }

    /**
     * Pooling - reusing the items instead of 
     * destroying them and instantiating new ones
     */
    public void ClearContentsOf(Transform item)
    {
        for (int i = item.childCount - 1; i >= 0; i--)
        {
            item.GetChild(i).SetParent(Pool);
            //GameObject.Destroy(item.GetChild(i).gameObject);
        }
    }

    public void AddDeckOption()
    {
        int id = AllDecks.Count + 1;

        AllDecks.Add(new DeckInfo(id));

        AddDropdownOption(id.ToString()); // add new deck's id to the dropdown

        deckDropdown.value = Dropdown.allSelectableCount;
    }

    public void AddDropdownOption(string optionName)
    {
        deckDropdown.options.Add(new TMP_Dropdown.OptionData() { text = optionName });
    }

    public void SetSelectedDeck()
    {
        ActiveDeck_id = deckDropdown.value + 1;
        LoadDeckCards();
    }

    public void LoadDeckCards()
    {
        ClearContentsOf(DeckContentTransform);
        SortDeck();

        foreach (Data cardData in AllDecks[ActiveDeck_id - 1].cards)
        {
            CreateDeckItem(cardData);
        }
    }

    public void AddSelectedCardsToDeck()
    {
        foreach (Data cardData in selectedCards)
        {
            AllDecks[ActiveDeck_id - 1].cards.Add(cardData);
            CreateDeckItem(cardData);
        }
        SortDeck(true);

        ClearSelections();

    }
    public void ClearSelections()
    {
        selectedCards.Clear();
        SetAllCardsInteractable();
    }

    private void CreateDeckItem(Data cardData)
    {
        GameObject newCardGO = CheckPoolFor("DeckCard");
        if (newCardGO == null)
            newCardGO = Instantiate(CardTitlePrefab, DeckContentTransform); // create a new
        else
            newCardGO.transform.SetParent(DeckContentTransform); // use the onr from the pool

        newCardGO.GetComponent<CardInfo>().cardData = cardData; // add this cards data to the deck's card
        newCardGO.GetComponentInChildren<TMP_Text>().text = cardData.name; // title of the card
    }

    public GameObject CheckPoolFor(string tag)
    {
        foreach (Transform item in Pool)
        {
            if(item.CompareTag(tag))
                return item.gameObject;
        }
        return null;
    }

    public void RemoveCardFromDeck(Data cardData)
    {

        GetActiveDeckCards().Remove(cardData);

    }
    public void SetAllCardsInteractable()
    {
        foreach (Transform item in CollectionContentTransform)
        {
            item.GetComponent<Button>().interactable = true;
        }
    }

    private List<Data> GetActiveDeckCards()
    {
        //Debug.Log("---looking for deckID: " + (manager.ActiveDeck_id - 1));
        return AllDecks[ActiveDeck_id - 1].cards;
    }

    public static int SortByName(Data c1, Data c2)
    {
        return c1.name.CompareTo(c2.name);
    }
    public static int SortByType(Data c1, Data c2)
    {
        return c1.types[0].CompareTo(c2.types[0]);
    }
    public static int SortByHP(Data c1, Data c2)
    {
        return int.Parse(c1.hp).CompareTo(int.Parse(c2.hp));
    }
    public static int SortByRarityNo(Data c1, Data c2)
    {
        //Debug.Log(c1.rarity);
        return c1.rarityNo.CompareTo(c2.rarityNo);
    }

    private void SetUpRarityNo()
    {
        foreach (Data cardData in cards.data)
        {
            switch (cardData.rarity)
            {
                case "Common":
                    cardData.rarityNo = 0;
                    break;
                case "Uncommon":
                    cardData.rarityNo = 1;
                    break;
                case "Rare":
                    cardData.rarityNo = 2;
                    break;
                case "Rare Holo":
                    cardData.rarityNo = 3;
                    break;
                case "Rare Holo GX":
                    cardData.rarityNo = 4;
                    break;
                case "Rare Holo EX":
                    cardData.rarityNo = 5;
                    break;
                case "Rare Holo V":
                    cardData.rarityNo = 6;
                    break;
                case "Rare Ultra":
                    cardData.rarityNo = 7;
                    break;
                case "Promo":
                    cardData.rarityNo = 8;
                    break;
                default: // empty
                    cardData.rarityNo = 9;
                    break;
            }
        }
    }

    public void SortCollectionCards(int dropdownValue)
    {
        if (dropdownValue == 0)
            cards.data.Sort(SortByType);
        else if (dropdownValue == 1)
            cards.data.Sort(SortByHP);
        else
            cards.data.Sort(SortByRarityNo);

        RearrangeCards(cards.data, CollectionContentTransform);       
    }

    /**
     * sort deck cards alphabetically and rearrange them
     */
    public void SortDeck(bool rearrange = false)
    {
        AllDecks[ActiveDeck_id - 1].cards.Sort(SortByName);

        if(rearrange)
            RearrangeCards(AllDecks[ActiveDeck_id - 1].cards, DeckContentTransform);
    }


    public void RearrangeCards(List<Data> cardData, Transform contentParent)
    {
        int childNo;

        for (int i = 0; i < cardData.Count; i++)
        {
            childNo = FindCardIn(cardData[i], contentParent);
            
            if(childNo != -1)
                contentParent.GetChild(childNo).SetSiblingIndex(i);
            else
                Debug.Log("card name, type, hp, rarityNo that is not in the content: " 
                    + cardData[i].name + "-" + cardData[i].types[0] + "-" + cardData[i].hp + "-" + cardData[i].rarityNo);
        }
    }

    /**
     * 
     */
    private int FindCardIn(Data cardData, Transform contentParent)
    {
        Transform cardGO;
        for (int i = 0; i < contentParent.childCount; i++)
        {
            cardGO = contentParent.GetChild(i);
            if (cardGO.GetComponent<CardInfo>().cardData == cardData)
                return i;
        }
        return -1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
