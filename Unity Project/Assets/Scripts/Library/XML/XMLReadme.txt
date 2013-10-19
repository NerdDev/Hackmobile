----------------
XML Usage in our Project:
----------------

To get a piece of data, there is two ways to go. Assuming a method has an XMLNode in it, you can either use:
x.SelectSomething(key);
or
XMLNifty.SelectSomething(x, key);

The former is fine if you can guarantee x is not null. If you cannot guarantee that, use the latter.

In an NPC, parseXML is called on a node with a key of "npc". It passes that node into WorldObject.parseXML -
 which then checks for nullity - and then continues to parse the rest. The next line is this:

 this.role = x.SelectEnum<Role>("role");

 This cannot guarantee that x is not null, so it uses the latter. This selects
 <role>someRole</role>
 in the NPC's xml definition, and if it does not exist, sets it to a default.

 Down the line, there is the line:
 this.stats.parseXML(x);

 This passes the node x to the stats class. In the stats class, the first line is this:

	//Assignation
    XMLNode x = xnode.select("stats");

This assigns takes the top NPC node and selects the stats node in it. Next, it does this:

	//Variables
	MaxHealth = x.SelectInt("maxhealth");

Since x is guaranteed to not be null, it uses the first assignation.
The chain of operations here is the npc node -> stats node -> maxhealth node -> maxhealth's text -> to integer.

----------------
XML Basics:
----------------

There's multiple components to the parser. Starting from the top, the XMLReader takes in a file location,
 and from that location, it parses the XML into a tree. The rest of the syntax is devoted to understanding that tree.

 At the top of the tree, there exists a root node. This is the top of the file.
 In an example XML syntax;
 <topNode>
	<middleNode1> text1 </middleNode1>
	<middleNode2> text2 </middleNode2>
 </topNode>

 The root node generated from this XML is the topNode. This root node then stores a list of all the nodes under it -
  correspondingly, the two middle nodes. This is contained in a list that is obtained with XMLNode.get(). Each XMLNode
  also contains two other strings - a key, and the text. The key is the particular name of the node. In the case of the root,
  the key is called "topNode". However, there is no text on the root node - there is no text existing for it that means anything.

On the two middle nodes, the key and text are more defined. The key for the first is "middleNode1", and the text is " text1 ".
 This tree will continue on down through whatever is defined in the file. There exists multiple options to get data for the text;
 ex from above: topNode.SelectString("middleNode2") will return "text2". This finds the node with the key "middleNode2" and then
 returns a string version of the text. You can also return integers, floats, and more.

----------------
XML Usage:
----------------
To create an XML, simply pass in a file path to the XMLReader.
XMLReader xreader = new XMLReader(filePath);

To get the root node, access the reader.
XMLNode root = xreader.getRoot();

Now you have the top of the tree and can search through it as needed. To return all the XMLNodes under a node,
 simply use:
List<XMLNode> nodes = root.get();

You can parse down as needed. This is how the DataManager passes around data - when it finds an XML headed by:
 <npcs> - it matches that to a parsing function for NPC's, and returns all the nodes under it and parses them into
 NPC objects.
