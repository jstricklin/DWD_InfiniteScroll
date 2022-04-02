# DWD Infinite Scroll

### Assignment notes:
 
All relevant project code is contained with the `Code` directory.

When viewing the project in the editor, the CardManager will allow you to specify the amount of cards displayed in our card display when Play Mode is entered.

Additional card sprites can be created easily in two steps:

- Enter the Sprite Editor and crop new cards from the master Card sprite sheet in the Graphics folder.
- Add new card's suit/value/face sprite to the Cards Array in the Card Manager.

Acceptance Criteria:

1. Cards are spawned and sorted at runtime by the CardManager.
2. Blank cards are spawned at the the start of PlayMode and repopulated with card data as they are recycled back into the scroll view.
3. Only 4 cards are visible at a time - this variable can be changed in the CardManager before entering PlayMode.
4. Card view can be scrolled infinitely either direction, with spawned cards being repopulated, repositioned and reordered in its sibling hierarchy in order to be recycled.
5. Bonus Sock Pair Probability Simulator included! Press 'spacebar' during runtime to execute, and select the "Probability" game object to update parameters.
