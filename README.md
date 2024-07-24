# LC-VEGA

![logo](https://i.imgur.com/GUkudM9.png)

## Description

VEGA is a sentient AI from the modern DOOM games that can assist you in many different ways through voice commands. You can ask him to do almost anything, and can even give you info about moons, creatures and a lot more.

<details>

<summary><b>Showcase videos</b></summary>

<br>

[![demo](https://i.imgur.com/Wtr3mBP.png)](https://www.youtube.com/watch?v=rEAtcF38e_Q)

[![viperianShowcase](https://i.imgur.com/RPpE1Wd.png)](https://www.youtube.com/watch?v=UJv7HbzkoCQ)

</details>

<br>

<details>

<summary><b>What can I ask VEGA to do?</b></summary>

<br>

VEGA is able to perform a very wide variety of actions. From disabling hazards, opening doors, to even giving you an Advanced Scanner that can detect nearby items and entities.

Here's the full list of all the things you can ask him to do:

### Moons
---
You can ask VEGA to give you information about any of the base game's moons with the following voice command:

- VEGA, info about [name of the moon]

### Creatures
---
You can ask VEGA to read out a creature's bestiary entry with the command:

- VEGA, read [creature's name] entry

You can also ask him to give you a summary of the creature by saying:

- VEGA, info about [creature's name]

<br>

Some creature mods are compatible with these commands. Check the list of [compatible creature mods](#mods-that-add-new-creatures) that is at the end of the page for more details.

### Advanced Scanner
---
VEGA is capable of adding two trackers to your helmet's display that will give you information on how many items and creatures are near you in real time, similarly to the ship's minimap. The item tracker doesn't just give you the number of nearby items however, it also displays how much they are worth in total. Also, items held by you or other players do not get tracked by the Advanced Scanner.

![advancedScannergif](https://i.imgur.com/07hOUo0.gif)

To activate the Advanced Scanner, use the following command:

- VEGA, activate scanner / enable scanner / turn on scanner / activate advanced scanner / enable advanced scanner / scan

To disable it, you can just say:

- VEGA, disable scanner / turn off scanner / disable advanced scanner / turn off advanced scanner / disable scan

### Secure doors
---
VEGA can open and close the closest secure door to you with the following commands:

- VEGA, open secure door / open door / open the door / open the secure door
- VEGA, close secure door / close door / close the door / close the secure door

Alternatively, you can also ask him to open or close all secure doors in the facility with these commands:

- VEGA, open all secure doors / open all doors
- VEGA, close all secure doors / close all doors

### Hazards
---
You can ask VEGA to disable the nearest turret, mine or spike trap with the following commands:

- VEGA, disable turret / disable the turret
- VEGA, disable the mine / disable mine / disable the landmine / disable landmine
- VEGA, disable the trap / disable trap / disable the spike trap / disable spike trap

And, as is the case with [Secure Doors](#secure-doors), he can also disable all mines, traps and turrets inside the facility with the following commands:

- VEGA, disable all turrets
- VEGA, disable all mines / disable all landmines
- VEGA, disable all traps / disable all spike traps

### Teleporter
---
VEGA is capable of using the ship's teleporter. To have him do so, you just have to say:

- VEGA, tp / teleport / activate tp / activate teleporter

![VEGAusingteleporter](https://i.imgur.com/84Ctfcq.gif)

I heavily recommend you always use the [Radar Switch](#radar-switch) command before asking him to use the teleporter if you aren't playing solo, as you could otherwise accidentally teleport someone else back to the ship.

### Radar Switch
---
VEGA can focus the ship's radar / cameras / minimap on you by saying:

- VEGA, focus / focus on me / switch to me / switch radar / switch radar to me

### Reports
#### Crew Status
---
You can ask VEGA to give you a status report of your crew that tells you how many players are left with the following command:

- VEGA, crew status / team status / crew info / team info / crew report / team report

Here's an example of what it can look like:

![crewreportexample](https://i.imgur.com/v6VwZ4v.png)

#### Scrap Scan
---
You can ask VEGA to perform a moon wide scrap scan that tells you how many items are inside and how many items you have in the ship as well as the total value for both with the following command:

- VEGA, scrap left / items left / scan for scrap / scan for items

Here's an example of what it can look like:

![scrapscanexample](https://i.imgur.com/SuH9Xy1.png)

#### Who's in the ship?
---
You can ask VEGA to give you a report with the names of the people that are in the ship with the following command:

- VEGA, crew in ship / people in ship / get crew in ship / get people in ship / how many people are in the ship? / is anyone in the ship? / is anybody in the ship?

Here's an example of what it can look like:

![crewinshipexample](https://i.imgur.com/Yrj3aKV.png)

### Radar Boosters
---
VEGA is capable of interacting with the nearest Radar Booster to you. You can have him use the flash command by saying:

- VEGA, flash

And the ping command with:

- VEGA, ping

### Signal Translator
---
You can ask VEGA to transmit a wide variety of different signals to the signal translator with the command:

- VEGA, transmit / send [message]

This is the defualt set of messages you can send:

- YES
- NO
- OKAY
- HELP
- THANKS
- ITEMS
- MAIN
- FIRE
- GIANT
- GIANTS
- DOG
- DOGS
- WORM
- WORMS
- BABOONS
- HAWKS
- DANGER
- GIRL
- GHOST
- BRACKEN
- BUTLER
- BUTLERS
- BUG
- BUGS
- YIPPEE
- SNARE
- FLEA
- COIL
- JESTER
- SLIME
- THUMPER
- MIMIC
- MIMICS
- MASKED
- SPIDER
- SNAKES
- OLD BIRD
- HEROBRINE
- FOOTBALL
- FIEND
- SLENDER
- LOCKER
- SHY GUY
- SIRENHEAD
- DRIFTWOOD
- WALKER
- WATCHER
- LOST
- INSIDE
- TRAPPED
- LEAVE
- GOLD
- APPARATUS

You can add your own custom messages in the configuration file under the **Dialogue & Interactions** section.

### Time
---
You can ask VEGA what the current time of day is by saying:

- VEGA, time / time of day / current time / current time of day / what time is it? / what's the current time of day?

### Ship
---
VEGA can interact with the ship's doors, allowing you to close or open them, as well as being able to turn on or off the ship's lights.

To interact with the doors, you have to say:

- VEGA, open ship doors / open the ship's doors / open hangar doors
- VEGA, close ship doors / close the ship's doors / close hangar doors

To interact with the lights, you need to say:

- VEGA, lights out / lights off / turn the lights off
- VEGA, lights on / turn the lights on

To interact with the magnet that was added in v56, you have to say:

- VEGA, activate magnet / enable magnet / turn on magnet
- VEGA, deactivate magnet / disable magnet / turn off magnet

### Weather
---
VEGA can give you information on all weather phenomena from the base game by saying:

- VEGA, info about rainy / stormy / foggy / flooded / eclipsed weather

### Miscellaneous
---
There are some additional interactions you can have with VEGA, such as being able to thank him or telling him to stop talking.

To tell him to stop talking, you can use one of the following commands:

- VEGA, shut up / stop / stop talking
- Shut up, VEGA / Stop, VEGA / Stop talking, VEGA

And to thank him, you can say:

- VEGA, thank you / thanks
- Thank you, VEGA / Thanks, VEGA

</details>

<br>

<details>

<summary><b>What can VEGA do by himself?</b></summary>

<br>

On his own, VEGA can:
- Let you know it's getting late when inside a moon's facility.
- Give you a brief and informative warning when taking out the Apparatus.
- When you scan a creature for the first time, he will give you a short summary about it, with some helpful tips on how to avoid / deal with them as well.
- Read bestiary entries out loud.
- Give you some information on the current weather of the moon you're going to as you are landing if you have 150 experience points or less.

</details>

<br>

<details>

<summary><b>Replacing VEGA's audio files</b></summary>

<br>

To be able to replace VEGA's voice lines and other sounds, you can download a SoundAPI template that has everything you need to be able to create a basic audio replacement pack for VEGA as well as all of VEGA's audio files in [**here**](https://github.com/03-JS/LC-VEGA/releases/tag/SoundAPI_Template).

Feel free to do whatever you want with both the template and the audio files. You can use them and upload them wherever you want, you can edit them, improve them, anything. Go crazy.

</details>

<br>

<details>

<summary><b>Supported Mods</b></summary>

<br>

VEGA **v2.0 or above** is compatible with some of the most popular and used Lethal Company mods on Thunderstore.

I have separated the list of supported mods into two different categories based on the features VEGA has support for:

### Mods that add new voice commands, features or interactions
---

- Malfunctions:

    VEGA can warn you and give you information about a malfunction the moment it happens. His capabilities are also affected by them.

- Toil-Head:

    VEGA can disable the turrets on Toil-Heads.

- Lategame Upgrades:
    
    You can ask VEGA to use the **Discombobulator** ship upgrade by saying:
    
    - VEGA, attack / stun / shock
    
- Ship Windows:
    
    You can ask VEGA to open or close the ship's shutters with the following commands:
        
    - VEGA, open shutters / open window shutters / open ship shutters
    - VEGA, close shutters / close window shutters / close ship shutters
    
- Diversity:
    
    VEGA will sometimes reply to some of the speaker lines from Diversity. By default the chance of him replying is 40%. This can be configured.
    
- Facility Meltdown:
    
    VEGA will give you different warnings when pulling the apparatus.

### Mods that add new creatures
---

- Giant Specimens
- Football
- Locker
- Peepers
- Rolling Giant
- Shy Guy / SCP-096
- Siren Head
- Faceless Stalker
- Moonswept
- Shockwave Drone
- Lethal Things (Boombas & Maggie)
- LC_Office (Shrimp)

</details>

<br>

<details>

<summary><b>Credits</b></summary>

<br>

- id Software for creating the original character of VEGA.
- My friend Nico and Mina for helping me test and polish the mod, as well as giving me suggestions and ideas to further improve it.
- JacuJ for showing me how to properly compress the audio files and compressing them for me.
- All the people that use the mod and have constantly been giving me ideas, suggestions and support. I appreciate it a lot <3

</details>