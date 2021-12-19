using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class Card : VisualElement {

    CardData _data;
    IDisposable _dataSubscription;

    public CardData Data {
        get {
            return _data;
        }
        set {
            if (_dataSubscription != null) {
                _dataSubscription.Dispose();
            }
            _data = value;
            _dataSubscription = _data.ObserveEveryValueChanged((card) => card.life, FrameCountType.EndOfFrame).Subscribe(life => { Life = life; });
        }
    }

    private float _life;
    public float Life {
        get {
            return _life;
        }
        set {
            _life = value;
            VisualElement filler = this.Q<VisualElement>("life-bar-filler");
            if (filler != null) {
                filler.style.flexBasis = new StyleLength(new Length(value, LengthUnit.Percent));
            }
        }
    }

    // Mandatory in order to instantiate it in an UXML file
    public new class UxmlFactory : UxmlFactory<Card, UxmlTraits> { }

    // 
    public new class UxmlTraits : VisualElement.UxmlTraits {

        UxmlFloatAttributeDescription _Life = new UxmlFloatAttributeDescription { name = "Life" };

        // no child
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            // ((FeatureCard)ve).status = m_Status.GetValueFromBag(bag, cc);

            ((Card)ve).Life = ((Card)ve)._data?.life ?? _Life.GetValueFromBag(bag, cc);
        }

    }

    public Card() {
        VisualTreeAsset template = Resources.Load<VisualTreeAsset>("CardUXML");
        template.CloneTree(this);
    }

    public Card(CardData data) {
        VisualTreeAsset template = Resources.Load<VisualTreeAsset>("CardUXML");
        template.CloneTree(this);
        Data = data;
    }
}
