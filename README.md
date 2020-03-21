# LlamaLibrary

This is a collection of botbases and general code which fills in some of the functionality missing from RB.

## The Main Botbases

### Reduce/Desynth
Reduce just aetherial reduces any available item and will desynth the trust loot items. Specifically items that contain these words 
```cs
private static readonly List<string> desynthList = new List<string>
{
    "Warg",
    "Amaurotine",
    "Lakeland",
    "Voeburtite",
    "Fae",
    "Ravel",
    "Nabaath",
    "Anamnesis"
};
```
you can add to that list in Reduce\Reduce.cs if you want but right now I don't have a UI based system for defining what to desynth. I used to also have it go for items under a certain desynth difficultly but client changes and RB's item db messed that up recently. There are settings to "Stay Running" and only reduce/desynth in a certain zone...these are mostly used when running it along side minion's trust addon or forager.

### Retainers
This botbase needs to be started within sight of a retainer summoning bell. It will go cycle through your retainers and deposit and items from player to retainer that the retainer already has. Ie if the player has Walnut Logs and the retainer has a stack of 23 it will move the players stack to the retainer. It will also combine item stacks between retainers, for example if 2 differnt both has stacks of Walnut Logs it will take it out of 1 retainer to put it in the other. Also collects gil from each retainer. Each of these settings can be turned on/off in settings.

*Note: There is Retainer Pull as well as setttings for category checks and other stuff not mentioned. The settings are for retainer pull which is more of a proof of concept than working/updated botbase.*

### IshgardHandin
Well the name says it all. This will go to Ishgard and hand-in crafting and gathering skybuilders items. Kupo tickets will be redeemed after hitting 9 (so start it with under 9 tickets) and for crafting it'll just keep handing in when you're maxed on scrips....gathering just kind of breaks there. The code for the botbase includes Tasks which can be called with runcode's in orderbot to do the handins as well as a `<BuyWhiteScriptItem>` tag. There is also a bool in IshgardHandinBase.cs which you can set to true to have it discard all collectables which don't meet the cutoff. 
```cs 
public static bool DiscardCollectable = true;
```

### DiademGather
Need some Umbral weather nodes from Diadem? Goes in to Diadem and sits in 1 of 3 spots in the middle of the map and waits for the weather to change and when it does it goes and gathers that node. Simple and dumb...will leave a random time between 10-20 min before the timer runs out then goes back in and continues. You need to start it on a gathering class but it'll switch between BTN/MIN as needed.

### Materia
Remove all materia from an item or Affix materia to an item. *This slowed down after a recent patch but still gets the job done*

### Housing Checker
Goes from housing zone to housing zone and checks the wards for houses for sale. When it finishes it'll print out the list of plots for sale in the console/log.

## The Lesser Botbases

### Autotrade
Well it just auto accepts trade requests and any items being traded. Use it to pass items between accounts.

### Autofollow
Offmesh follow the leader. You can use this to have an alt follow you through a dungeon running unsynced but stay close. It doesn't know what the map looks like, just turns towards you and walks forward.

### Out on a limb
This is really a main botbase but down here because I'm not happy with it. It will win the game but after a random number of wins it starts winning 0 MGP. Seems like a server side restriction, so i have it stop once it starts winning 0. 

https://discord.gg/DT9geAp
