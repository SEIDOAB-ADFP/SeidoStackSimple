# Poker Game Assignment (G-Grade) - Functional Programming Style

## Overview
In this assignment, you will implement a poker game using **functional programming principles** in C#. You will work with LINQ, lambda expressions, method chaining, and higher-order functions to create a card game system that can shuffle, deal, evaluate poker hands, and determine winners.

## Learning Objectives
- Apply functional programming concepts in C# (LINQ, lambdas, pure functions)
- Use method chaining for fluent interfaces
- Implement discriminated unions using inheritance
- Work with enumerations and pattern matching
- Use higher-order functions (functions that take or return functions)
- Aggregate and transform collections functionally

## Prerequisites
- Understanding of C# records, classes, and interfaces
- Familiarity with LINQ operators (Select, Where, OrderBy, Aggregate, etc.)
- Knowledge of lambda expressions and pattern matching
- Basic understanding of poker hand rankings

## Project Structure

You will work in the following files:
- `2.Domain.Models/Cards/Card.cs` - Card record implementation
- `2.Domain.Models/Cards/CardDeck.cs` - Deck class with functional operations
- `2.Domain.Models/Cards/PokerHand.cs` - Poker hand evaluation logic
- `1.Domain.Services/Cards/PokerService.cs` - Service to create decks and hands
- `0.App.AppWorker/Workers/UsingPoker.cs` - Demonstration code (already provided)

## Part 1: Implement the Card Record

### Task
Create a **record** called `Card` that implements the `ICard` interface.

### Requirements
1. Use the **primary constructor** syntax with `CardSuit` and `CardRank` parameters
2. Implement the `ICard` interface from `Models.Cards.Interfaces`
3. Override `ToString()` to display cards using Unicode symbols:
   - ♣ (Clubs): `\x2663`
   - ♦ (Diamonds): `\x2666`
   - ♥ (Hearts): `\x2665`
   - ♠ (Spades): `\x2660`
4. Implement `ToLogger<T>` method to log the card information

### Hints
```csharp
public record Card (CardSuit Suit, CardRank Rank): ICard
{
    public override string ToString() => Suit switch
    {
        // Your code: Use pattern matching to return the appropriate symbol
    };

    public ICard ToLogger<T>(ILogger<T> logger, string message = null)
    {
        // Your code: Log the card with optional message prefix
    }
}
```

### Expected Output Example
```
♠ Ace
♥ King
♦ Five
```

---

## Part 2: Implement the CardDeck Class

### Task
Create a `CardDeck` class that implements `ICardDeck` interface with functional deck manipulation methods.

### Requirements

#### 2.1 Basic Structure
- Implement `IEnumerable<ICard>` and `ICardDeck`
- Use a `protected List<ICard> cards` field
- Implement `Name` property and `Count` property
- Implement `GetEnumerator()` for iteration

#### 2.2 Deck Creation
- Create a private `Create()` method that generates all 52 cards
- Use nested loops to iterate through all suits and ranks
- Constructor should accept a `name` parameter and call `Create()`

#### 2.3 Functional Operations

Implement the following methods using **functional programming style**:

**Shuffle()**
- Return a new shuffled deck using LINQ's `OrderBy` with random values
- Should return `ICardDeck` for method chaining

**Sort(Func<IEnumerable<ICard>, IOrderedEnumerable<ICard>> sortFunc)**
- Accept a **higher-order function** as parameter
- Apply the sorting function to the cards
- Return `ICardDeck` for chaining

**Keep(Func<ICard, bool> predicate)**
- Filter cards using `Where` with the provided predicate
- Keep only cards that match the predicate
- Return `ICardDeck` for chaining

**Remove(Func<ICard, bool> predicate)**
- Remove cards that match the predicate
- Use `Where` with negated predicate (!predicate)
- Return `ICardDeck` for chaining

**Draw()**
- Return the last card and remove it from the deck
- Modify the internal list

**Add() and InsertAtBottom()**
- Implement both single card and collection overloads
- `Add` adds to the end, `InsertAtBottom` inserts at position 0

### Hints
```csharp
public ICardDeck Shuffle()
{
    if (cards.Count <= 0) return this;
    
    var rnd = new Random();
    cards = cards.OrderBy(c => rnd.Next()).ToList();
    return this;
}

public ICardDeck Keep(Func<ICard, bool> predicate)
{
    // Your code: Use Where to filter
}

public ICardDeck Remove(Func<ICard, bool> predicate)
{
    // Your code: Use Where with !predicate
}
```

### Example Usage
```csharp
var deck = new CardDeck("My Deck")
    .Shuffle()
    .Sort(cards => cards.OrderBy(c => c.Suit).ThenBy(c => c.Rank))
    .Remove(c => c.Rank == CardRank.Ace)
    .Keep(c => c.Suit == CardSuit.Hearts);
```

---

## Part 3: Implement PokerHand Class

### Task
Create a `PokerHand` class that inherits from `CardDeck` and evaluates poker hand rankings.

### Requirements

#### 3.1 Grouping Properties
Implement these properties using LINQ `GroupBy`:

```csharp
public IEnumerable<IGrouping<CardSuit, ICard>> Suits =>  
    this.cards.GroupBy(card => card.Suit);

public IEnumerable<IGrouping<CardRank, ICard>> Ranks => 
    this.cards.GroupBy(card => card.Rank);

public IOrderedEnumerable<CardRank> SortedRanks => 
    this.cards.Select(c => c.Rank).OrderBy(r => r);
```

#### 3.2 Straight Detection
Implement `IsRanksStraight` property:
- Check if 5 consecutive ranks exist
- Use `Enumerable.Range` and `All` to verify consecutive values

```csharp
public bool IsRanksStraight { get 
{
    var sRanks = SortedRanks.ToList();
    // Your code: Check if each rank is exactly 1 higher than the previous
}}
```

#### 3.3 Poker Hand Evaluation
Implement the `EvaluateRank` property to detect all poker hands:

**Hand Rankings (highest to lowest):**
1. **Royal Flush** - A, K, Q, J, 10 of same suit
2. **Straight Flush** - 5 consecutive cards of same suit
3. **Four of a Kind** - 4 cards of same rank
4. **Full House** - 3 of a kind + pair
5. **Flush** - 5 cards of same suit
6. **Straight** - 5 consecutive cards
7. **Three of a Kind** - 3 cards of same rank
8. **Two Pair** - 2 different pairs
9. **One Pair** - 2 cards of same rank
10. **High Card** - No matching ranks

**Implementation Requirements:**
- Use boolean variables to detect each hand type
- Use LINQ methods: `Any()`, `Count()`, `Where()`
- Use **pattern matching with tuples** in a switch expression
- Return the appropriate discriminator class instance

### Hints
```csharp
public IPokerHand EvaluateRank { get
{
    if (cards.Count != 5)
        return new NoPokerRank() {Name = this.Name, cards = this.cards};

    //... code ...
    bool isThreeOfAKind = Ranks.Any(group => group.Count() >= 3);
    //... code ...    
        
    return (isRoyalFlush, isStraightFlush, isFourOfAKind, isFullHouse, 
            isFlush, isStraight, isThreeOfAKind, isTwoPair, isOnePair) switch
    {
        (true, _, _, _, _, _, _, _, _) => new RoyalFlush() {Name = this.Name, cards = this.cards},
        // Your code: Complete the pattern matching for all hand types
        _ => new HighCard() {Name = this.Name, cards = this.cards}
    };  
}}
```

---

## Part 4: Complete the UsingPoker Demonstration

### Task
Complete the missing code in `UsingPoker.cs` to demonstrate deck manipulation and play a poker game.

### Requirements

#### 4.1 DemonstrateDeckManipulation() (5 points)
Complete the following operations:
1. Shuffle the deck
2. Sort the deck by suit, then by rank
3. Remove Aces and Kings
4. Keep only cards with ranks 2, 3, 5, 7
5. Draw 4 cards into a poker hand
6. Evaluate the hand (should show NoPo
7. Draw 1 more card
8. Evaluate again (should show a valid hand)

**Use method chaining and lambda expressions!**

Example:
```csharp
var deck = deck1.Shuffle()
    .Sort(cards => cards.OrderBy(c => c.Suit).ThenBy(c => c.Rank))
    .Remove(c => c.Rank switch { 
        CardRank.Ace or CardRank.King => true, 
        _ => false 
    })
    .Keep(c => c.Rank switch { /* your code */ })
    .ToLogger(_logger, "Filtered deck:");
```

#### 4.2 PlayPokerGame() (5 points)
Complete the poker game implementation using **functional programming**:

1. **Create players** - Use `Enumerable.Range` and `Select`
```csharp
var players = Enumerable.Range(1, nrPlayers)
    .Select(i => _pokerService.CreatePokerHand($"Player {i}"))
    .ToList();
```

2. **Deal cards** - Use nested `Aggregate` to deal 5 cards to each player
```csharp
Enumerable.Range(1, 5)
    .Aggregate(players, (allPlayers, cardNumber) =>
        allPlayers.Select(player =>
        {
            player.Add(deckForDealing.Draw());
            return player;
        }).ToList()
    );
```

3. **Evaluate hands** - Use `Select` and `ForEach`
```csharp
players.Select(player => player.EvaluateRank)
    .ToList()
    .ForEach(rank => rank.ToLogger(_logger));
```

4. **Determine winner** - Chain LINQ operators
```csharp
var winner = players
    .Select(player => new { 
        Player = player, 
        Rank = player.EvaluateRank, 
        Score = player.EvaluateRank.RankScore 
    })
    .OrderByDescending(p => p.Score)
    .ThenByDescending(p => p.Player.SortedRanks.Max())
    .First();
```

5. **Clear hands** - Use `Remove` with always-true predicate
```csharp
players.ForEach(player => player.Remove(c => true));
```

---

## Part 5: Update PokerService (Bonus - 5 points)

### Task
Update `PokerService.cs` to return actual instances instead of `null`.

```csharp
public ICardDeck CreateDeck(string name) => new CardDeck(name);
public IPokerHand CreatePokerH

### Task
Update `PokerService.cs` to return actual instances instead of `null`.

```csharp
public ICardDeck CreateDeck(string name) => new CardDeck(name);
public IPokerHand CreatePokerHand(string name) => new PokerHand(name);
```

---

## Completion Checklist

Ensure you have completed all of the following:

- [ ] **Card Record** - Correct record syntax, ToString(), ToLogger()
- [ ] **CardDeck Class** - All methods implemented functionally with chaining
  - [ ] Basic structure (properties, enumeration)
  - [ ] Deck creation (52 cards generated)
  - [ ] Shuffle and Sort using LINQ
  - [ ] Keep and Remove with functional filtering
  - [ ] Draw, Add, and InsertAtBottom
- [ ] **PokerHand Class** - Complete hand evaluation logic
  - [ ] Grouping properties (Suits, Ranks, SortedRanks)
  - [ ] Straight detection (IsRanksStraight)
  - [ ] Hand evaluation (all 10 hand types detected)
- [ ] **UsingPoker Demo** - Completed demonstration code
  - [ ] Deck manipulation with chaining operations
  - [ ] Poker game with functional dealing and winner detection
- [ ] **PokerService** - Returns actual instancesdesign your methods to:
- Return new instances or modified collections
- Use `ToList()` to create new collections
- Support method chaining by returning `this` or new instances

### 2. **Higher-Order Functions**
Functions that accept other functions as parameters:
```csharp
public ICardDeck Sort(Func<IEnumerable<ICard>, IOrderedEnumerable<ICard>> sortFunc)
public ICardDeck Keep(Func<ICard, bool> predicate)
```

### 3. **LINQ Operators (Functional Transformations)**
- `Select` - Transform/map elements
- `Where` - Filter elements
- `OrderBy` / `ThenBy` - Sort elements
- `GroupBy` - Group elements by key
- `Aggregate` - Reduce/fold operation
- `Any` / `All` - Existential checks

### 4. **Lambda Expressions**
Inline anonymous functions:
```csharp
.Remove(c => c.Rank == CardRank.Ace)
.OrderBy(c => c.Suit).ThenBy(c => c.Rank)
```

### 5. **Pattern Matching**
Modern C# switch expressions:
```csharp
Suit switch
{
    CardSuit.Clubs => "♣",
    CardSuit.Diamonds => "♦",
    _ => "♠"
}
```

### 6. **Method Chaining (Fluent Interface)**
```csharp
deck.Shuffle()
    .Sort(cards => cards.OrderBy(c => c.Rank))
    .Remove(c => c.Rank == CardRank.Ace)
    .ToLogger(_logger);
```

---

## Testing Your Implementation

### Expected Console Output
When running the `UsingPoker` worker, you should see:

1. **Deck Manipulation:**
   - Original deck (52 cards)
   - Sorted deck
   - Filtered decks
   - Hand evaluation with 4 cards (NoPokerRank)
   - Hand evaluation with 5 cards (actual poker hand)

2. **Poker Game:**
   - Multiple rounds of poker
   - Each player's hand and rank
   - Winner announcement each round
   - Game continues until not enough cards remain

### Sample Output
```
Creating and manipulating decks of cards.
Deck 1 holds 52 cards.
♣ Two     ♣ Three   ♣ Four    ... (all 52 cards)

Sorted Deck:
Deck 1 holds 52 cards.
♣ Two     ♣ Three   ♣ Four    ... (sorted by suit and rank)

Only 2,3,5,7:
Deck 1 holds 16 cards.
♥ Two     ♠ Five    ♦ Seven   ...

Hand 1 evaluated rank after 4 cards:
Hand 1: NoPokerRank
♣ Two     ♥ Five    ♠ Seven   ♦ Three   

Hand 1 evaluated rank after 5 cards:
Hand 1: OnePair
♣ Two     ♥ Two     ♥ Five    ♠ Seven   ♦ Three   

---
Round 1
----------
Player 1: ThreeOfAKind
♠ King    ♥ King    ♣ King    ♦ Five    ♠ Two    

Winner: Player 1 with ThreeOfAKind
```

---

## Tips for Success

1. **Start with the basics:** Implement `Card` and basic `CardDeck` structure first
2. **Test incrementally:** Test each method as you implement it
3. **Use LINQ extensively:** This is a functional programming exercise
4. **Method chaining is key:** Every deck operation should return `ICardDeck`
5. **Pattern matching:** Use switch expressions for clean, readable code
6. **Think declaratively:** Describe WHAT you want, not HOW to do it
7. **Avoid loops when possible:** Use LINQ operators instead

---

## Submission Requirements

1. All files should compile without errors
2. The `UsingPoker.ExecuteAsync()` method should run successfully
3. Code should follow functional programming principles
4. Include comments explaining complex LINQ operations
5. Ensure proper method chaining throughout

---

## Additional Challenges (Extra Credit)

1. **Add Jokers** - Extend the deck to include jokers and handle them in hand evaluation
2. **Better Tie-Breaking** - When two players have the same hand type, compare kickers
3. **Statistics** - Add LINQ queries to show statistics (most common hand, win rates, etc.)
4. **Functional Deck Merging** - Implement a method to merge two decks functionally
5. 
---

**Good luck and have fun with functional programming!** 🃏♠♥♦♣
