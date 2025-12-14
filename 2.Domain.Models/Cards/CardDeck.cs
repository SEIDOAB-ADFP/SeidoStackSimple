using Microsoft.Extensions.Logging;
using Models.Cards.Interfaces;

namespace Models.Cards;

// Some implementation ideas and hints of a CardDeck class below. 
// CardDeck to be implemented as needed.

/* 
public class CardDeck : IEnumerable<ICard>, ICardDeck
{
    public string Name { get; init; }
    protected List<ICard> cards = new ();

    public IEnumerator<ICard> GetEnumerator() => cards.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        (string sRet, int i) = ("", 0);
        var deck = new List<ICard>(cards);

        foreach (var card in deck)
        {
            sRet += $"{card,-9}";
            if ((i++ + 1) % 13 == 0)
                sRet += "\n";
        }
        return sRet;
    }

    public ICardDeck Sort(Func<IEnumerable<ICard>, IOrderedEnumerable<ICard>> sortFunc)
    {
        if (cards.Count <= 0) return this;

        cards = sortFunc(cards).ToList();
        return this;
    }
  
    public virtual ICardDeck ToLogger<T>(ILogger<T> logger, string message = null)
    {
        var logMessage = $"{Name} holds {cards.Count} cards.\n{ToString()}";
        if (message != null)
            logMessage = $"{message}\n{logMessage}";
        
        
        logger.LogInformation("{LogMessage}", logMessage);
        return this;
    }
}

*/