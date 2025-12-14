# Poker Game Assignment (VG-Grade) - Advanced Functional Programming with Service Architecture

## Overview
This advanced assignment builds upon the G-grade poker game by refactoring it to use **immutable records**, **extension methods**, and a **configurable service architecture** following the same patterns used in SeederService and EncryptionService. You will transform the object-oriented design into a purely functional approach with strategy pattern configuration.

## Prerequisites
- **Completed G-grade assignment** - You must have a working poker game implementation
- **Understanding of SeederService architecture** (branch: `1-seeder-service-architecture`)
- **Understanding of EncryptionService architecture** (branch: `2-encryption-assignment-answer`)
- **Familiarity with SeederVsEncryptionComparison.md** document
- Advanced knowledge of C# records, immutability, and extension methods

## Learning Objectives
- Transform mutable classes into **immutable records**
- Implement **extension methods** to add functionality to records
- Apply the **Builder pattern** for service configuration
- Apply the **Options pattern** for strategy registration
- Use **higher-order functions** for configurable hand evaluation
- Implement **deferred execution** in dependency injection
- Apply **functional composition** with strategy functions

---

## Architectural Transformation Overview

### From G-Grade to VG-Grade

```
┌─────────────────────────────────────────────────────────────────────────┐
│ G-GRADE (Object-Oriented)                                               │
│ - CardDeck class with methods                                           │
│ - PokerHand class with evaluation logic                                 │
│ - Methods modify internal state                                         │
│ - PokerService simply creates instances                                 │
└─────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────┐
│ VG-GRADE (Functional with Service Architecture)                         │
│ - CardDeck as immutable record                                          │
│ - PokerHand as immutable record                                         │
│ - All operations as extension methods                                   │
│ - PokerService with configurable winning strategies                     │
│ - Builder/Options pattern for configuration                             │
│ - Strategy functions registered via Options                             │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Part 1: Transform CardDeck to Immutable Record

### Task
Refactor `CardDeck` class into an **immutable record** with all functionality moved to **extension methods**.

### 1.1 Create CardDeck Record

**Location:** `2.Domain.Models/Cards/CardDeck.cs`

**Requirements:**
1. Convert the class to a **record** using primary constructor
2. Use `ImmutableList<ICard>` instead of `List<ICard>` for the cards collection
3. Remove all methods - they will become extension methods
4. Keep only properties: `Name` and `Cards`

**Hint:**
```csharp
using System.Collections.Immutable;

namespace Models.Cards;

public record CardDeck(string Name, ImmutableList<ICard> Cards) : ICardDeck
{
    public int Count => Cards.Count;
    
    // Implement IEnumerable<ICard>
    public IEnumerator<ICard> GetEnumerator() => Cards.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

### 1.2 Create CardDeckExtensions

**Location:** `2.Domain.Models/Cards/Extensions/CardDeckExtensions.cs`

**Requirements:**
Transform all CardDeck methods into **extension methods** that return **new instances**:

**CreateDeck() - Factory Extension**
```csharp
public static class CardDeckExtensions
{
    public static CardDeck CreateDeck(this string name)
    {
        // Generate all 52 cards
        var cards = Enum.GetValues<CardSuit>()
            .SelectMany(suit => Enum.GetValues<CardRank>()
                .Select(rank => (ICard)new Card(suit, rank)))
            .ToImmutableList();
        
        return new CardDeck(name, cards);
    }
}
```

**Shuffle() - Returns new shuffled deck**
```csharp
public static CardDeck Shuffle(this CardDeck deck)
{
    // TODO: Create new CardDeck with shuffled cards
    // Hint: Use OrderBy with random values and ToImmutableList()
    // Return: new CardDeck(deck.Name, shuffledCards)
}
```

**Keep() - Returns new filtered deck**
```csharp
public static CardDeck Keep(this CardDeck deck, Func<ICard, bool> predicate)
{
    // TODO: Create new CardDeck with filtered cards
    // Hint: Use Where and ToImmutableList()
}
```

**Remove() - Returns new filtered deck**
```csharp
public static CardDeck Remove(this CardDeck deck, Func<ICard, bool> predicate)
{
    // TODO: Similar to Keep but negate the predicate
}
```

**Sort() - Returns new sorted deck**
```csharp
public static CardDeck Sort(this CardDeck deck, 
    Func<IEnumerable<ICard>, IOrderedEnumerable<ICard>> sortFunc)
{
    // TODO: Apply sorting function and return new deck
}
```

**Draw() - Returns tuple with card and new deck**
```csharp
public static (ICard card, CardDeck remainingDeck) Draw(this CardDeck deck)
{
    if (deck.Cards.IsEmpty)
        throw new InvalidOperationException("Cannot draw from empty deck");
    
    // TODO: Return last card and new deck without that card
    // Hint: var lastCard = deck.Cards[^1];
    // Hint: var remaining = deck.Cards.RemoveAt(deck.Cards.Count - 1);
    // Return: (lastCard, new CardDeck(deck.Name, remaining))
}
```

**Add() - Returns new deck with added card(s)**
```csharp
public static CardDeck Add(this CardDeck deck, ICard card)
{
    // TODO: Return new deck with card added
    // Hint: deck.Cards.Add(card)
}

public static CardDeck Add(this CardDeck deck, IEnumerable<ICard> cards)
{
    // TODO: Return new deck with all cards added
    // Hint: deck.Cards.AddRange(cards)
}
```

---

## Part 2: Transform PokerHand to Immutable Record

### Task
Refactor `PokerHand` into an **immutable record** with evaluation logic in extension methods.

### 2.1 Create PokerHand Record

**Location:** `2.Domain.Models/Cards/PokerHand.cs`

```csharp
using System.Collections.Immutable;

namespace Models.Cards;

public record PokerHand(string Name, ImmutableList<ICard> Cards) : IPokerHand
{
    public int Count => Cards.Count;
    
    // Implement IEnumerable<ICard>
    public IEnumerator<ICard> GetEnumerator() => Cards.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

### 2.2 Create PokerHandExtensions

**Location:** `2.Domain.Models/Cards/Extensions/PokerHandExtensions.cs`

**Requirements:**
Transform all PokerHand logic into extension methods:

**CreatePokerHand() - Factory Extension**
```csharp
public static PokerHand CreatePokerHand(this string name)
{
    return new PokerHand(name, ImmutableList<ICard>.Empty);
}
```

**Grouping Properties as Extension Methods**
```csharp
public static IEnumerable<IGrouping<CardSuit, ICard>> GetSuits(this PokerHand hand)
{
    return hand.Cards.GroupBy(card => card.Suit);
}

public static IEnumerable<IGrouping<CardRank, ICard>> GetRanks(this PokerHand hand)
{
    return hand.Cards.GroupBy(card => card.Rank);
}

public static IOrderedEnumerable<CardRank> GetSortedRanks(this PokerHand hand)
{
    return hand.Cards.Select(c => c.Rank).OrderBy(r => r);
}
```

**IsRanksStraight Extension**
```csharp
public static bool IsRanksStraight(this PokerHand hand)
{
    // TODO: Implement straight detection
    // Hint: Use GetSortedRanks() and check consecutive values
}
```

**EvaluateRank Extension**
```csharp
public static IPokerHand EvaluateRank(this PokerHand hand)
{
    if (hand.Cards.Count != 5)
        return new NoPokerRank() { Name = hand.Name, cards = hand.Cards.ToList() };

    // TODO: Implement hand evaluation using extension methods
    // Use: hand.GetRanks(), hand.GetSuits(), hand.IsRanksStraight()
    // Return appropriate IPokerHand discriminator
}
```

**Add() - Returns new hand with added card(s)**
```csharp
public static PokerHand Add(this PokerHand hand, ICard card)
{
    return new PokerHand(hand.Name, hand.Cards.Add(card));
}

public static PokerHand Add(this PokerHand hand, IEnumerable<ICard> cards)
{
    return new PokerHand(hand.Name, hand.Cards.AddRange(cards));
}
```

**Remove() - Returns new hand without matching cards**
```csharp
public static PokerHand Remove(this PokerHand hand, Func<ICard, bool> predicate)
{
    return new PokerHand(hand.Name, 
        hand.Cards.Where(c => !predicate(c)).ToImmutableList());
}
```

---

## Part 3: Update UsingPoker Worker

### Task
Update `UsingPoker.cs` to use the new immutable record-based approach.

**Key Changes:**

### 3.1 Deck Manipulation with Extension Methods
```csharp
private async Task DemonstrateDeckManipulation()
{
    // TODO: Create a deck using the string extension method "Deck 1".CreateDeck()
    // TODO: Chain .Shuffle() to shuffle the deck
    // TODO: Chain .Sort() with a lambda that orders by Suit then by Rank
    // TODO: Chain .Remove() to remove Aces and Kings using a predicate
    // TODO: Chain .Keep() to keep only cards with ranks 2, 3, 5, 7
    
    // TODO: Use tuple deconstruction to draw a card: var (card, remainingDeck) = deck.Draw();
    // TODO: Reassign deck variable to the remainingDeck to work with the updated immutable deck
    // Hint: Remember that all operations return NEW deck instances, not modified ones
}
```

### 3.2 Playing Poker with Immutable Hands
```csharp
private async Task PlayPokerGame(int nrPlayers = 4)
{
    // TODO: Create and shuffle a deck using "Poker Deck".CreateDeck().Shuffle()
    
    // TODO: Create players using Enumerable.Range(1, nrPlayers)
    // TODO: Use .Select() to create a PokerHand for each player: $"Player {i}".CreatePokerHand()
    // TODO: Convert to List with .ToList()
    
    // TODO: Deal cards using nested Aggregate (functional approach - no for loops!)
    // Outer Aggregate: Enumerable.Range(1, 5) - deals 5 cards
    // Inner Aggregate: Iterate through all players for each card
    // Hint: Aggregate should carry state as tuple: (deck, players)
    // Hint: For each player, Draw() a card, update the deck, and Add() card to player
    // Hint: Return new state tuple with updated deck and updated player list
    
    // TODO: Extract the updated players from the aggregate result
    
    // TODO: Evaluate all hands using .Select(p => p.EvaluateRank())
    
    // TODO: Determine winner using LINQ:
    // Hint: OrderByDescending by hand's RankScore
    // Hint: Use .First() to get the top hand
    
    // TODO: Log the winner's name
}
```

---


## Functional Programming Patterns in VG Assignment

### 1. Immutability with Records
```csharp
// Every operation returns a NEW record
public static CardDeck Shuffle(this CardDeck deck)
{
    var shuffledCards = deck.Cards.OrderBy(_ => Random.Shared.Next()).ToImmutableList();
    return new CardDeck(deck.Name, shuffledCards);  // New instance!
}
```

### 2. Extension Methods for Operations
```csharp
// Instead of: deck.Shuffle()  (method on class)
// We use: deck.Shuffle()      (extension method on record)
// This keeps the record pure and behavior separate
```

### 3. Tuple Returns for State Changes
```csharp
// Returns both the card and the new deck state
public static (ICard card, CardDeck deck) Draw(this CardDeck deck)
{
    return (deck.Cards[^1], new CardDeck(deck.Name, deck.Cards.RemoveAt(deck.Cards.Count - 1)));
}
```

---

## Completion Checklist

### Part 1: Immutable CardDeck
- [ ] CardDeck converted to record with ImmutableList
- [ ] All methods moved to CardDeckExtensions
- [ ] Shuffle returns new deck
- [ ] Keep/Remove return new decks
- [ ] Draw returns tuple (card, new deck)
- [ ] Add returns new deck with added cards

### Part 2: Immutable PokerHand
- [ ] PokerHand converted to record with ImmutableList
- [ ] All logic moved to PokerHandExtensions
- [ ] GetSuits/GetRanks/GetSortedRanks as extensions
- [ ] EvaluateRank as extension method
- [ ] Add/Remove return new hands

### Part 3: UsingPoker Updated
- [ ] Uses extension methods for deck creation
- [ ] Handles immutability (reassigning variables)
- [ ] Uses tuple deconstruction for Draw
- [ ] Uses PokerService.DetermineWinner

---

## Grading Criteria (VG-Grade)

To achieve VG grade, you must complete ALL of the following:

1. **G-grade assignment completed** - All G-grade requirements met
2. **Immutable records** - CardDeck and PokerHand as records with ImmutableList
3. **Extension methods** - All functionality implemented as extensions
4. **Configuration** - Custom poker strategies configured
5. **Functional consistency** - All operations return new instances
6. **Working demonstration** - UsingPoker runs with new architecture

---

## Tips for Success

1. **Transform incrementally:**
   - Start with immutable records
   - Convert methods to extensions one at a time
   - Test after each change

2. **Embrace immutability:**
   - Never modify existing records
   - Always return new instances
   - Use `with` expressions for record copying when needed

3. **Follow the pattern exactly:**
   - The service architecture has four components (Service, Options, Builder, Extensions)
   - Use the same deferred configuration approach
   - Register strategies in dictionaries

4. **Use ImmutableList correctly:**
   - `Add()` returns new list
   - `RemoveAt()` returns new list
   - `AddRange()` returns new list
   - Always call `ToImmutableList()` when needed

---

**Good luck with your advanced functional programming implementation!** 🃏♠♥♦♣
