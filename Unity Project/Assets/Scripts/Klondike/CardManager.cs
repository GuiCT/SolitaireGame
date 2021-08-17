using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klondike
{
    public enum CardPosition
    {
        NONE = -1,
        TRIO,
        BOTTOM_PILE,
        TOP_PILE
    }

    public class CardManager : MonoBehaviour
    {
        static DataManager dataManager;
        SpriteRenderer cardSpriteRenderer;
        [SerializeField]
        Color selectionHighlight;
        [SerializeField]
        Sprite cardBack;
        Sprite cardFront;

        public int group { get; private set; } // 0 1 2 3 Clubs Hearts Diamonds Spades Paus Copas Ouros Espadas
        
        public int value { get; private set; } // A 2 3 4 5 6 7 8 9 10 J Q K
        
        public bool isRed
        {
            get => ((group == 1) || (group == 2));
        }
        // If it is red / Se é vermelha

        public int position
        {
            get => transform.GetSiblingIndex();
        }
        // Which position it is / Em qual posição está

        public int pile // Which pile it is / Em qual pilha está
        {
            get => _pile;
            set
            {
                if ((value >= 0) && (value < 7))
                {
                    if (_cardPosition != CardPosition.NONE)
                    {
                        if (value != _pile)
                        {
                            dataManager.MoveCardIntoBottomPile(transform, value);
                            _pile = value;
                            _cardPosition = CardPosition.BOTTOM_PILE;
                        }
                    }
                    else
                    {
                        _cardPosition = CardPosition.BOTTOM_PILE;
                        _pile = value;
                    }
                }
            }
        }

        public bool trio
        {
            get => _cardPosition == CardPosition.TRIO;
            set
            {
                if (value)
                {
                    _pile = -1;
                    _cardPosition = CardPosition.TRIO;
                }
            }
        }
        // If it is on the top trio / Se está no trio de cima
        
        public bool top
        {
            get => _cardPosition == CardPosition.TOP_PILE;
            set
            {
                _pile = -1;
                _cardPosition = CardPosition.TOP_PILE;
            }
        }
        // If it is on the top / Se está no topo

        public bool bottom
        {
            get => _cardPosition == CardPosition.BOTTOM_PILE;
        }

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
        } // If it is face up / Se está virada para cima
        
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

                _selected = value;
            }
        } // If it is selected / Se está selecionada

        private CardPosition _cardPosition;
        private int _pile;
        private bool _visible;
        private bool _selected;

        public void InitializeCard(int card, bool visible)
        {
            if ((card >= 0) && (card < 13))
            {
                group = 0;
            }
            else if ((card >= 13) && (card < 26))
            {
                group = 1;
            }
            else if ((card >= 26) && (card < 39))
            {
                group = 2;
            }
            else if ((card >= 39) && (card < 52))
            {
                group = 3;
            }
            value = card % 13;
            cardSpriteRenderer = GetComponent<SpriteRenderer>();
            cardFront = FindObjectOfType<SpriteController>().availableSprites[card];
            this.visible = visible;
            _cardPosition = CardPosition.NONE;
            _pile = -1;
            if (dataManager == null)
            {
                dataManager = FindObjectOfType<DataManager>();
            }
        }
    }
}
