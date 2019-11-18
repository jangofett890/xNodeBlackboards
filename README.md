# xNodeBlackboards
A basic Blackboard system for xNode. Just a straight port of Blackboard from my personal node system at the moment, also added Custom Blackboard Value Editors Support. Some bugs and or redundancies may exist, if this is the case please report them!

## Getting Started
<p align="center">
  <img src="https://i.imgur.com/A5bS0RK.png">
</p>
Blackboards are just a representative for BlackBoardGraph.
A blackboard graph is a normal graph, however it contains a List of BlackboardObjects, as well as some Actions, and it ensures that a Blackboard is created on creation of the graph.

Once created, you can create variables on the blackboard, then drag and drop them onto the graph.

You can access these variables from the Graph Asset Itself.

BlackBoardGraph itself isn't creatable, so you will need to make your own graph type and extend BlackBoardGraph.

Currently custom Type support isn't in yet, so the best case would be to use ScriptableObject under Basic Types for most things, although this can be buggy and there are kinks that need to be worked out.

### Key Features
* Dynamic Graphs based on Blackboard Variables
* Serialize almost anything (Including Prefabs, Scene GameObjects, and Scene Components References.)
* Custom BlackboardObject editors based on xNode custom editor design.

### TODO
* Blackboard Variable Type Attribute (To allow using your type as a Variable)
* Reflection based Variable Display (Instead of switching between types use a general property field for everything)
* Fix everything to do with GUILayout Argument Exception, Hotcontrol, and Getting Control X's position while repainting


### Final Notes
This is in early development and pull requests/issues are well appreciated! 
