# UnityProject-310719
RPG character creation, network connection and basic game world interactions, with custom Unity Editor plugins.

PROJECT OVERVIEW:
The project began as a series of plugins for the Unity Editor, allowing the user to build a database of items,
skills and spells, common components of roleplaying games. Additional databases were added over time to allow
for more complexity. A character creation screen was then created, allowing the player to select various details
about their character's history as well as the items that they started out with. The multiplayer functionality 
is based on the Photon Bolt API. Most recently, some RPG features have been added to the project, including 
movement and interaction with non-player characters.

1. UNITY EDITOR PLUGINS
There are several databases in the project which store information used in the roleplaying game: items, skills
(and associated 'perks'), spells (and associated 'auras'), quests and NPCs. These can all be editted in the
Unity Editor. The Editor loads the relevant information from a .json file and stores it in a ScriptableObject,
which the Unity Editor engine can interact with; once the item (et cetera) has been editted, the information in
the ScriptableObject is transcribed into the .json file. When the game is run, an instance in the game repeats
this process and allows scripts in the game to access that information. This means that dialog with things such 
as dialog with non-playable characters, the effects of spells and abilities, and the progress of missions does 
not need to be hard-coded, since it is stored in these databases, making the process of creating content for
the game much faster.

2. CHARACTER CREATION
The player may select a faction to join at the beginning of the game, as well as non-changing information such 
as their race, on the character creation screen. In addition, they may select the items that they begin the
game with, based on a selection of items stored by the server. Choosing different races increases some of the 
player's attributes by small amounts (an example of the Perks created in the Editor).

3. NPC DIALOG
The player may interact with non-playable characters in the game world, although this is not fully implemented
yet. An important aspect of the game is that various pieces of dialog are unlocked at different stages of the
game. This is all handled in the Unity Editor, where each 'dialog stage' has a set of 'dialog links' that the
player can choose from. Some of these links are only accessible if certain conditions are met, such as the
player being on a particular stage of a quest, or having a specific item in their inventory.

4. PREVIOUS VERSIONS
A previous version of the project existed around 2014-2015, however, this has mostly been entirely scrapped. The
only remaining parts of that project are the foundations of the Editor Plugins (which constitute the bulk of the
work) and the quest /dialog system. The old version of the project focussed much more on the game world itself, 
and in it the player was able to fell trees, pick up items and craft items.
