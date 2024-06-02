# NPC_System

This is a NPC Sytem I've made for unity 3D games, free to use for educational purpose only (may change in the future).

## How to setup:

-- Step 1:
    \n - Create an empty named NPCSystem in your scene
    \n - Add the NPCSPawner component to the NPCSystem empty. Everything on the script is pretty self explaining, but you'll find "documentation" at the end of the readme.
    \n - Add all your NPC prefabs to the Base NPC Prefab list and add their animation state controller to NPCAnimatorStateController Field.

-- Step 2:
    - Create an empty child of NPCSystem and name it SpawnpointManager.
    - Add the Spawnpoint manager component to it.
    - Do the same with an empty child of NPCSystem named WaypointsManager, and add the WaypointManager script instead of SpawnpointManager. 

-- Step 3:
    - See the how to use section


## How to use:

The system is pretty eazy to understand, if you want to add Spawnpoints (where the NPC are going to spawn) you select the SpawnpointManager empty and
you hold left shit + press right clic to create a spawnpoint where you want in the scene.
If you want to add waypoints it's the same process hold left shift + press right clic. I recommand to duplicate the Waypoints Manager if you wants to have
multiple path for npcs. You should not have only one WaypointManager for all your paths, because if you do that the NPCs are going to walk everywhere except
where you want.


## Errors you might run into

When your adding waypoints to the scene, the Editor scripts (waypointEditor and spawnpointeditor) must be in the same folders than others monobehaviour scripts.
But if you want to make a build of your game, you must move these script into the Editor Folder, if you don't, your build will be cancelled.


## NPC Spawner Component """Explained"""

Here's a very quick explication of what the field of NPC Spawner script does:

- NPCAnimatorStateController (RuntimeAnimatorController): The Animation State Controller of all NPCs, you create one with the animations you want and you put it in there.
- BaseNpcPrefabs (List): A list of all the prefabs you can have for NPCs, there's will be chosed randomly before spawning.
- NPCCount (Int): The number of NPC you want to Spawn.
- RespawnCheckInterval (Int): How many seconds before checking if NPC are dead and if we should make them disappear and respawn.
- PlayerProximityRadius (Float): The radius of the sphere where we check if the player is in. If is not, we can make NPC respawn.
- Min NPC Speed (Float): The minimum speed NPCs can have.
- Max NPC Speed (Float): The maximum speed NPCs can have.
