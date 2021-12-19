using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Random = UnityEngine.Random;

public class CardListView : VisualElement {

    List<CardData> _cardDataList;
    ReactiveCollection<CardData> _reactiveCardData;
    Dictionary<CardData, Card> _cardList;
    private IDisposable _dataSubscription = null;
    System.Collections.ObjectModel.ObservableCollection<CardData> observableCollection;

    int previewElements { get; set; }

    private class EmptyAttributes : IUxmlAttributes {
        public bool TryGetAttributeValue(string attributeName, out string value) {
            value = "";
            return false;
        }
    }

    public List<CardData> CardList {
        get {
            return _cardDataList;
        }
        set {
            if (_dataSubscription != null) {
                _dataSubscription.Dispose();
            }
            _cardDataList = value;

            ListView listView = this.Q<ListView>("list-view-root");

            _cardDataList.ObserveEveryValueChanged((list) => list.Count, FrameCountType.EndOfFrame).Subscribe(_ => {
                Debug.Log("REFRESH!");
                listView.RefreshItems();
            });

            Card.UxmlFactory factory = new Card.UxmlFactory();
            listView.makeItem = () => {
                return factory.Create(new EmptyAttributes(), CreationContext.Default) as Card;
            };
            listView.bindItem = (VisualElement element, int itemIndex) => {
                Card card = (Card)element;
                card.Data = _cardDataList[itemIndex];
            };
            listView.itemsSource = _cardDataList;

            /*
            _reactiveCardData = new ReactiveCollection<CardData>(_cardDataList);

            Card.UxmlFactory factory = new Card.UxmlFactory();

            _dataSubscription1 = _reactiveCardData.ObserveAdd().Subscribe((ev) => {
                CardData data = ev.Value;
                int idx = ev.Index;

                VisualElement previousElement = listView.Children().ElementAt(idx);
                Card card = factory.Create(null, new CreationContext()) as Card;
                // previousElement.PlaceBehind(new Card(data));
                previousElement.PlaceBehind(card);
            });
               */
            // ((card) => card.life, FrameCountType.EndOfFrame).Subscribe(life => { Life = life; });
        }
    }

    // Mandatory in order to instantiate it in an UXML file
    public new class UxmlFactory : UxmlFactory<CardListView, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits {

        UxmlIntAttributeDescription _previewElements = new UxmlIntAttributeDescription { name = "preview-elements" };

        // no child
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            int previewElements = ((CardListView)ve).previewElements = _previewElements.GetValueFromBag(bag, cc);

            CardListView root = (CardListView) ve;
            List<CardData> data = root?._cardDataList;
            // ((FeatureCard)ve).status = m_Status.GetValueFromBag(bag, cc);

            if (data == null) {
                data = new List<CardData>();
                
                for (int i = 0; i < previewElements; i++) {
                    data.Add(new CardData() { life = Random.Range(0, 100), name = $"List Element {i}", stars = Random.Range(0, 5) });
                }
            }

            root._cardDataList = data;
        }

    }

    public CardListView() {
        VisualTreeAsset template = Resources.Load<VisualTreeAsset>("CardListViewUXML");
        template.CloneTree(this);
    }
}
