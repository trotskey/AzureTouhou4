using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace AzureTouhou4.Marisa
{
    public class MarisaCharacterCardController : CharacterCardController
    {
        public MarisaCharacterCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController)
        {
        }

        public MiniHakkeroCardController MiniHak
        {
            get
            {
                var mini = this.TurnTakerController.FindCardController("MiniHakkero");
                if (mini.Card.IsInPlay)
                {
                    return (MiniHakkeroCardController)this.TurnTakerController.FindCardController("MiniHakkero");
                }
                else
                {
                    return (MiniHakkeroCardController)this.TurnTakerController.FindCardController("MiniHakkeroCoolingElementalFurnace");
                }
            }
        }

        //TODO: add rest of power

        public override IEnumerator UsePower(int index = 0)
        {
            Card topCard = this.HeroTurnTakerController.DeckTopCardController.Card;

            IEnumerator e = this.GameController.MoveCard(
                taker: this.HeroTurnTakerController,
                cardToMove: topCard,
                destination: MiniHak.Card.UnderLocation,
                showMessage: true,
                cardSource: GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator e = null;

            switch (index)
            {
                case 0:
                    // One player may play a card now.
                    e = SelectHeroToPlayCard(this.DecisionMaker);
                    break;
                case 1:
                    // One player may draw a card now
                    e = this.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: GetCardSource());
                    break;
                case 2:
                    //Place the mini-hakkero next to a hero character card, treating Marisa as that hero's name and you as that hero's player. 
                    //That hero gains the following power: Discard a card. Place the top 3 cards of your deck under the mini-hakkero. 
                    //You may no longer use incapacitated abilities.
                    e = null;
                    break;
            }

            return e;
        }
    }
}
