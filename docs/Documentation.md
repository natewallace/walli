# Walli User Guide

## Overview

Walli is a SalesForce IDE application used by SalesForce developers.  The application uses the various SalesForce APIs to interact with SalesForce and allow users to perform a number of developer related tasks such as creating and updating triggers and classes or querying and modifying data using SOQL statements.

## Install

The installer for Walli is an MSI setup file which can be found in the downloads section of this site.  The installer will copy all the necessary files to your program files directory, create a shortcut on your desktop and start menu, and then launch Walli as soon as the install is complete.

## Using Walli
Detailed documentation on how to use Walli is split out into several sections listed below.

### The basics

* [Project management](Project-management)

* [Project layout](Project-layout)

### The features

* [Data query tool](Data-query-tool)

* [Class and trigger editor](Class-and-trigger-editor)

* [VisualForce and Component editor](VisualForce-and-Component-editor)

* [Reports](Reports)

* [Deployment Tools](Deployment-Tools)

* [Team](Team)

## Credits

Walli is made up of several open source and free components.  Listed below are those items.

**AvalonEdit**
[https://github.com/icsharpcode/SharpDevelop/wiki/AvalonEdit](https://github.com/icsharpcode/SharpDevelop/wiki/AvalonEdit)
The text editor in Walli is built on top of the excellent open source AvalonEdit control.

**Gardens Point**
[https://gplex.codeplex.com/](https://gplex.codeplex.com/)
[https://gppg.codeplex.com/](https://gppg.codeplex.com/)
The lexing and parsing engine in Walli was built using the open source generators written by Gardens Point.

**Json.NET**
[http://james.newtonking.com/json](http://james.newtonking.com/json)
This open source library is used for JSON parsing.

**Source Control**
There are a number of third party open source libraries used to enable support for Git source control.
[https://libgit2.github.com/](https://libgit2.github.com/) - The underlying library that is used to support Git.
[https://github.com/libgit2/libgit2sharp](https://github.com/libgit2/libgit2sharp) - A library that wraps the libgit2 library with nice .NET code.
[http://www.libssh2.org/](http://www.libssh2.org/) - Used to support SSH protocol with Git.

**Comparison Tools**
There are two difference algorithms a user can choose from when doing a comparison.  The following librarires are used to calculate the differences in text:
[https://code.google.com/p/google-diff-match-patch/](https://code.google.com/p/google-diff-match-patch/) - An implementation of the Myer difference algorithm maintained by Google.
[https://github.com/jcdickinson/difflib](https://github.com/jcdickinson/difflib) - An implementation of the Patience difference algorithm written by Jonathan Dickinson.

**File Search**
[http://lucenenet.apache.org/](http___lucenenet.apache.org_)
The global file search feature is built on top of the .NET version of Apache's Lucene search index library.

**WiX Toolset**
[http://wixtoolset.org/](http://wixtoolset.org/)
The installer for Walli is built using the WiX Toolset.

**Cloud Icon**
[http://www.iconarchive.com/show/colorful-long-shadow-icons-by-graphicloads/Cloud-icon.html](http://www.iconarchive.com/show/colorful-long-shadow-icons-by-graphicloads/Cloud-icon.html)
[http://graphicloads.com/](http://graphicloads.com/)
The cloud icon used for the Walli application is a freeware image found on iconarchive.

**Hamster Icon**
[http://www.iconarchive.com/show/kids-icons-icons-by-everaldo/mascot-icon.html](http___www.iconarchive.com_show_kids-icons-icons-by-everaldo_mascot-icon.html)
[http://www.everaldo.com/](http___www.everaldo.com_)
The hamster icon used for the upgrade notification is a freeware image found on iconarchive.

**Other Icons**
[http://msdn.microsoft.com/en-us/library/vstudio/ms247035(v=vs.100).aspx](http://msdn.microsoft.com/en-us/library/vstudio/ms247035(v=vs.100).aspx)
The vast majority of icons used in Walli come from the Microsoft Visual Studio Image Library.
 
 
