using Models.Cards.Interfaces;

namespace Services.Cards.Interfaces;

public interface IPokerService
{
    ICardDeck CreateDeck(string name);
    public IPokerHand CreatePokerHand(string name);
}