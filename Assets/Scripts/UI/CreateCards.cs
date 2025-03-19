using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateCards : MonoBehaviour
{
    public Transform cardParent;
    public List<UpgradeCard> cards = new List<UpgradeCard>();
    public int limit = 3;
    public UnityEvent onUpgradeEvents;

    List<UpgradeCard> instantiatedCards = new List<UpgradeCard>();

    public void CreateCardList()
    {
        if (limit > cards.Count)
        {
            for (int i = 0; i < limit; i++)
            {
                int random = Random.Range(0, cards.Count);
                foreach (UpgradeCard tempCard in instantiatedCards)
                {
                    if (tempCard.cardName.Equals(cards[i].cardName))
                    {
                        CreateCardList();
                        return;
                    }
                }

                GameObject card = Instantiate(cards[random].gameObject, cardParent.position, Quaternion.identity, cardParent);
                UnityEngine.UI.Button button = card.GetComponentInChildren<UnityEngine.UI.Button>();
                button.onClick.AddListener(delegate { ClearCards(); }); // Adds the ClearCards method to all cards when pressed
                instantiatedCards.Add(card.GetComponent<UpgradeCard>());
            }
        }
        else
        {
            foreach(UpgradeCard entry in cards)
            {
                GameObject card = Instantiate(entry.gameObject, cardParent.position, Quaternion.identity, cardParent);
                UnityEngine.UI.Button button = card.GetComponentInChildren<UnityEngine.UI.Button>();
                button.onClick.AddListener(delegate { ClearCards(); }); // Adds the ClearCards method to all cards when pressed
                instantiatedCards.Add(card.GetComponent<UpgradeCard>());
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
