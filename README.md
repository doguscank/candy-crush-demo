# Candy Crush Demo
A clone of Candy Crush Saga mobile game. Different from the original game, this game is an endless game.

# Gameplay
User clicks on two tiles to swap their places on the grid. If there are any matches, these tiles are removed from the grid. Minimum match size is 3, and maximum match size set to be 5. User receives 10 score for each tile removed from the grid. Each tile is spawned using RNG.

## Game Screen
![Gameplay](Images/Gameplay.gif)

# Implemented Features
* A grid class that controls everything about tiles.
* Tiles have swap and drop animations.
  * Each animation is edited using code.
* Debug mode.
  * A history class that saves grid color history to check if everything works correctly.
  * Navigation in history is done by arrow keys.

![Debug Screen](Images/DebugTool.gif)
