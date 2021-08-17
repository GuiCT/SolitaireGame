using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spider
{
    public class CardManager : MonoBehaviour
    {
        SpriteRenderer cardSpriteRenderer;
        [SerializeField]
        Color selectionHighlight;
        [SerializeField]
        Sprite cardBack;
        Sprite cardFront;
        static DataManager dataManager;

        public int value { get; private set; } // A 2 3 4 5 6 7 8 9 10 J Q K

        public int pile
        {
            get => _pile;
            set
            {
                if (value != _pile)
                {
                    if (_pile == -1)
                    {
                        _pile = value;
                    }

                    else if ((value >= 0) && (value < 10))
                    {
                        dataManager.MoveCardIntoPile(transform, value);

                        _pile = value;
                    }
                }
            }
        }
        // Which pile it is / Em qual pilha está
        public int position
        {
            get => transform.GetSiblingIndex();
        } // Which position it is / Em qual posição está

        public bool visible
        {
            get => _visible;
            set
            {
                if (value)
                {
                    _visible = true;
                    cardSpriteRenderer.sprite = cardFront;
                }
                else
                {
                    _visible = false;
                    cardSpriteRenderer.sprite = cardBack;
                }
            }
        } // If it is facing up / Se está virada para cima
        public bool selected
        {
            get => _selected;
            set
            {
                if (value)
                {
                    cardSpriteRenderer.color = selectionHighlight;
                }
                else
                {
                    cardSpriteRenderer.color = Color.white;
                }
            }
        } // If it is selected / Se está selecionada

        private int _pile;
        private bool _visible;
        private bool _selected;

        public void InitializeCard(int card, int pile, bool visible)
        {
            value = card % 13;
            cardSpriteRenderer = GetComponent<SpriteRenderer>();
            cardFront = FindObjectOfType<SpriteController>().availableSprites[value];

            this.visible = visible;
            _pile = pile;

            if (!dataManager)
            {
                dataManager = FindObjectOfType<DataManager>();
            }
        }
    }
}
