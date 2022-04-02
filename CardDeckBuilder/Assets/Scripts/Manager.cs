using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class Manager : MonoBehaviour
{
    public List<GameObject> MenuPanels;
    //public GameObject MainMenu_panel;
    //public GameObject About_panel;
    //public GameObject DeckBuilder_panel;

    public GameObject CardPrefab;
    public GameObject CardTitlePrefab;

    public Transform CollectionContentTransform;
    public Transform DeckContentTransform;
    public Transform DeckDropdownTransform;
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
        ActivatePanel(MenuPanels[0]); // main menu
    }

    public void Start()
    {       
        selectedCards = new List<Data>();
        //AllDecks = new List<DeckInfo>();
        deckDropdown = DeckDropdownTransform.GetComponent<TMP_Dropdown>();
        //request = "https://api.pokemontcg.io/v2/cards/xy1-1";
        request = "https://api.pokemontcg.io/v2/cards";

        deckDropdown.ClearOptions();
        AddDeckOption();
        deckDropdown.value = 0;

    StartCoroutine(Util.ApiRequest(request, SetUpCardCollection));
    }

    /**
     * set active the panel needed and disable the rest
     */
    public void ActivatePanel(GameObject panel)
    {
        foreach (GameObject item in MenuPanels)
        {
            if (item == panel)
                item.SetActive(true);
            else
                item.SetActive(false);
        }
    }

    public void SetUpCardCollection(string _json)
    {
        if (_json != null)
        {
            cards = JsonConvert.DeserializeObject<PokemonMultiData>(_json);

            ClearContentsOf(CollectionContentTransform);

            Debug.Log("cards size: " + cards.data.Count);

            foreach (Data item in cards.data)
            {
                GameObject newCard_GO;// = CheckPoolFor("ImageCard");
                //if(newCard_GO == null)
                    newCard_GO = Instantiate(CardPrefab, CollectionContentTransform);

                newCard_GO.GetComponent<CardInfo>().Init(item);
            }
        }
        //int pageCount; //3ekina me 0

        //int minCount = pageCount * 20;

        //int maxCount = minCount + 20;

        //for (int i = minCount; i < maxCount; i++)
        //{ 

        //}
        // k otan patas next pas pageCount++ k 3anakaneis instanitae ts epomenes
        // 8a kaneis na elegxo an to maxCount einai megalitero apo to length tou 
        //cards array tote to maxCount einai to length tou array. 
        //emmm opws exeis tora to scroll content. na mpoun apo katw. 
        //ena next button de3ia, ena previous button aristera. k sti mesi 8a leei "Page 1/100"
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

        //Debug.Log("dropdown value: " + deckDropdown.value + "which is: " + deckDropdown.options[deckDropdown.value].text);

        deckDropdown.value = Dropdown.allSelectableCount;
        //Debug.Log("deckdropdown value: " +deckDropdown.value);
        //Debug.Log("deckdropdown options: " + deckDropdown.options);
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

        foreach (Data cardData in AllDecks[ActiveDeck_id - 1].cards)
        {
            CreateDeckItem(cardData);
        }
    }

    public void AddSelectedCardsToDeck()
    {
        foreach (Data cardData in selectedCards)
        {
            //Debug.Log("add to deck: " + ActiveDeck_id + " and adding cardID: " + cardData.id);
            AllDecks[ActiveDeck_id - 1].cards.Add(cardData);
            CreateDeckItem(cardData);
        }
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
            {
                Debug.Log("found a tag " + tag + "it's: " + item.name);
                return item.gameObject;
            }
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


    //public int CardExistsInDeck()
    //{
    //    List<Data> ActiveDeckCards = GetActiveDeckCards(); // search at the active deck if this card exists already
    //    for (int i = 0; i < ActiveDeckCards.Count; i++)
    //    {
    //        Data currentCard = ActiveDeckCards[i];
    //        if (cardData.id == currentCard.id)
    //            return i;
    //    }
    //    return -1;
    //}

    private List<Data> GetActiveDeckCards()
    {
        //Debug.Log("---looking for deckID: " + (manager.ActiveDeck_id - 1));
        return AllDecks[ActiveDeck_id - 1].cards;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
