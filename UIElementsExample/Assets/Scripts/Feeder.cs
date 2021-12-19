using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Feeder : MonoBehaviour {

    CardListView cardList;
    List<CardData> cardListData = new List<CardData>();


    void Start() {
        UIDocument doc = GetComponent<UIDocument>();
        VisualElement root = doc.rootVisualElement;
        cardList = root.Q<CardListView>();

        for (int i = 0; i < 3; i++) {
            cardListData.Add(new CardData() { life = Random.Range(70, 100), name = $"List Element {i}", stars = Random.Range(0, 5) });
        }

        cardList.CardList = cardListData;
    }

    float time = 0;
    int idx = 0;


    void Update() {
        time += Time.deltaTime;

        if (time > 1) {
            int elemCount = cardListData.Count;
            time -= 1;
            idx += 1;
            idx %= elemCount;

            cardList.CardList[idx].life = Random.Range(0, 100);

            if (idx == 0) {
                cardListData.Insert(Random.Range(0, elemCount), new CardData() { life = 100, name = $"List Element {elemCount}", stars = Random.Range(0, 5) });
            }
        }
    }
}
