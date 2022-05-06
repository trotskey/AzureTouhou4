using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace AzureTouhou4.Marisa
{
    public class OrdinaryMagicianCardController : CardController
    {
        public OrdinaryMagicianCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private MarisaCharacterCardController marisa
        {
            get { return (MarisaCharacterCardController)this.HeroTurnTakerController.CharacterCardController; }
        }

        //"One-shots {Marisa} plays may go under the mini-Hakkero instead of the discard"
        public override void AddTriggers()
        {
            AddTrigger(new Trigger<PlayCardAction>(
                gameController: this.GameController,
                criteria: pca => IsThisPlayerOneShot(pca.CardToPlay),
                response: r => PrimeCard(GameController.FindCardController(r.CardToPlay)),
                types: new List<TriggerType>() { TriggerType.ActivateTriggers },
                timing: TriggerTiming.After,
                cardSource: GetCardSource()
                ));

        }

        private bool IsThisPlayerOneShot(Card c)
        {
            return c.IsOneShot && c.Owner == this.TurnTaker;
        }

        private IEnumerator PrimeCard(CardController c)
        {
            yield return c.AddAfterLeavesPlayAction(action: () => MoveCardToMiniHak(c.Card));
        }

        private IEnumerator MoveCardToMiniHak(Card c)
        {
            List<YesNoCardDecision> ynd = new List<YesNoCardDecision>();
            IEnumerator e = this.GameController.MakeYesNoCardDecision(
                hero: this.DecisionMaker,
                type: SelectionType.Custom,
                card: c,
                storedResults: ynd,
                cardSource: GetCardSource());

            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }

            if (DidPlayerAnswerYes(ynd))
            {
                e = this.GameController.MoveCard(
                    taker: this.TurnTakerController,
                    cardToMove: c,
                    destination: marisa.MiniHak.Card.UnderLocation,
                    showMessage: true,
                    cardSource: GetCardSource()
                    );

                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(e);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(e);
                }

            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (decision is YesNoCardDecision yesNoCard)
            {
                return new CustomDecisionText("Do you want to send {0} below mini-Hakkero?", "Should they send {0} below mini-Hakkero?", "Vote to move below mini-Hakkero", "mini-Hakkero");
            }
            return null;
        }
    }
}
