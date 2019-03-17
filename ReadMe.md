# Blackjack
Inspired by: https://codereview.stackexchange.com/questions/214390/my-blackjack-game-in-c-console
Thanks Steve!

## Features
+ Multi-player
+ Multi-deck shoe
+ Player can double down
+ Player can split matching cards 
+ Player can double after split
+ Test mode (stack the shoe to test different scenarios)
+ Dealer burns first card of shoe

## To Play
1. Run the program.
2. Enter your name and a bankroll.
3. Enter and additional players and their bankroll.
4. Bet (whole dollar amounts only, within the table limits).
5. Play your hand. 
    * If you split you'll see multiple hands.
	* Answer dealer prompts with the first letter of the word (e.g. h for hit, s for stand).
6. Play continues until you run out of chips or you bet $0 to stand up from the table.

## Misc  
+ Table rules are set as static properties on the Table class.
+ Dealer speed can be controled via the Dealer class's delay field.
+ Object-oriented design - No object can directly touch the internals of another object (i.e. no public setters).

## Features that would be nice to add
+ Insurance
+ Surrender
+ Deal only a single card after splitting aces