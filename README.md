# Decision Grids #

### Overview ###

This is a project created for Pathing, Navigation, and threat detection.
The package uses Node based grid collections to map out the environment in an area.
Locations can be mapped out on a grid using different shapes and sizes, with a positive or negative value. 
The positive and negative values on the grid can be used to read back broad information from different locations and directions on the Grid.
As well as being used for path finding for agents to navigate an environment

This project is built to be pulled in a unity project from the package manager.

### Project Notes ###

GridCollections should be created from static method call on 'GridCollection.cs' & 'GridCollectionDetail.cs'.
There is no limit on GridCollections to be created.
Call 'Cleanup()' on GridCollections when finished with use.

On GridCollection, when checking PathFinding, set 'PathFinding.NavPersistence' to change the amount of time the algorythm will spend moving away from the destination
in hopes that it will start to move toward the destination.

Currently the value of a node location can only be -1, 0, or 1.

### Example ###
Examples of using the scripts are in the folder called 'Examples' along with an example scene.

### Developer ###
Name                | E-mail
:-------------------|:------------------------
Luke Garnsey        | <lukegarnsey@gmail.com>
