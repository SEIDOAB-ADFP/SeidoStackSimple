using Microsoft.Extensions.Logging;
using Models.Cards;
using Models.Cards.Interfaces;
using Services.Cards.Interfaces;

namespace Services.Cards;

public class PokerService : IPokerService
{
    private readonly ILogger<PokerService> _logger;

    public ICardDeck CreateDeck(string name) => null; //new CardDeck(name);
    public IPokerHand CreatePokerHand(string name) => null; //new PokerHand(name);

    public PokerService(ILogger<PokerService> logger)
    {
        _logger = logger;
    }
}
