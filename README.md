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
