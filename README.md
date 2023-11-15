This Project is provided as-is with no warranty or support of any kind.

This project contains a template for development of an RTS game using Unity's DOTS technology.

The following things have already been implemented:

- DOTS based NavMesh navigation based on StarForge's implementation
- Combat logic fully implemented
- spawner Logic
- Factory Logic
- Player interaction and issuing of commands


things which are still to be implemented:

- units that can construct factories
- fog of war system
- networking
- multiplayer
- economy simulation
- game state logic (e.g win conditions)
- audio
- navMesh avoidance


Scenes:

unit ui test - scene used to test factory and unit ui
combat_mass_spawn - scene to test performance of units in combat in editor - the amount of units to be spawend can be changed in scene view
combat_mass_spawn_userInput - same as combat_mass_spawn with added logic relying on userInput_combatMassSpawn_scene to provide user input on how many units the user wants to see on the battlefield. intented to be used in a build and distributed to users to test their system.
userInput_combatMassSpawn_scene - provides an input field to tell the spawners of the combat_mass_spawn_userInput scene how many units shall be spawned

Important notice:

This template is not FULLY implemented with DOTS yet. DOTS does not yet have ways to replace every monoBehaviour based feature. For Example UI and Player interactions still use monobehaviour logic.
DOTS and monobehaviour logic need to communicate with each other in order to create a functioning product.
In the future this will gradually be replaced by DOTS versions.

!!!
As of July 2023 there is an internal unity engine memory leak when using the unity dots packages. Nothing can be done about this until unity fixes it on their side. This bug will impact performance and the ability to create a build from this template.
!!!

Assets used from unity's asset store:

- Cartoon_Tank_Free
- Graphy - Ultimate Stats Monitor
- RTS_Camera


Code used from other projects:
- Forging Station: New-DOTS-Nav-Mesh-Query-Implementation - https://github.com/ForgingStation/New-DOTS-Nav-Mesh-Query-Implementation

