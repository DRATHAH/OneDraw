using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateCards : MonoBehaviour
{
    public Transform cardParent;
    public GameObject cardPrefab;
    [Tooltip("Every card type the upgrade altar can provide.")]
    public List<HazardStats> cards = new List<HazardStats>();
    public int limit = 3;
    public UnityEvent onUpgradeEvents;

    List<UpgradeCard> instantiatedCards = new List<UpgradeCard>();

    public void CreateCardList()
    {
        if (limit < cards.Count) // If there are more available cards to make than to be spawned
        {
            while (instantiatedCards.Count < limit)
            {
                int random = Random.Range(0, cards.Count);
                foreach (UpgradeCard tempCard in instantiatedCards)
                {
                    if (tempCard.stats.type.Equals(cards[random].type))
                    {
                        CreateCardList();
                        return;
                    }
                }

                GameObject card = Instantiate(cardPrefab, cardParent.position, Quaternion.identity, cardParent);
                UpgradeCard upgradeCard = card.GetComponent<UpgradeCard>();
                UnityEngine.UI.Button button = card.GetComponentInChildren<UnityEngine.UI.Button>();
                button.onClick.AddListener(delegate { ClearCards(); }); // Adds the ClearCards method to all cards when pressed
                upgradeCard.Initialize(cards[random]);

                instantiatedCards.Add(upgradeCard);
            }
        }
        else // Create a list of all available cards
        {
            foreach(HazardStats entry in cards)
            {
                GameObject card = Instantiate(cardPrefab, cardParent.position, Quaternion.identity, cardParent);
                UpgradeCard upgradeCard = card.GetComponent<UpgradeCard>();
                UnityEngine.UI.Button button = card.GetComponentInChildren<UnityEngine.UI.Button>();
                button.onClick.AddListener(delegate { ClearCards(); }); // Adds the ClearCards method to all cards when pressed
                upgradeCard.Initialize(entry);

                instantiatedCards.Add(upgradeCard);
            }
        }
    }

    public void ClearCards()
    {
        foreach (UpgradeCard card in instantiatedCards)
        {
            Destroy(card.gameObject);
        }
        
        instantiatedCards.Clear();
        GameManager.instance.SetCursorLock(true);
        GameManager.instance.SetPlayerMove(true);
        onUpgradeEvents.Invoke();
    }
}
