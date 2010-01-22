# KeywordClassifier VS2010 Extension

This is a sample extension for Visual Studio 2010 that shows how to 
to customize the syntax highlighting used for certain keywords.

The extension was originally built as a custom Clasiffier component,
but later changed to a custom Tagger component after beta 2 was released,
because it offered better performance.

The extension supports C# and C/C++, but support for other languages
can be added easily. Each type of keyword highlighted can also be 
customized through new classifications in the Fonts and Colors section
of the Visual Studio 2010 Options dialog.

Keywords that are customized include:

1. *Control Flow Keywords* (if, foreach, while, etc.). Customized through the "Keyword - Control Flow" classification.
2. *LINQ Keywords* (select, from, where, join, etc.). Customized through the "Operator - LINQ" classification.
3. *Visibility Keywords* (public, private, etc.). Customized through the "Keyword - Visibility" classification.

![Extension sample](http://winterdom.com/wp-content/uploads/2009/06/vs10_kc_1_thumb.png)

## Installation

1. Install the Visual Studio 2010 SDK. You'll need it for building custom
   extensions.
2. Open the solution in Visual Studio 2010 and build it.
3. Close all open VS2010 instances.
4. Using Windows Explorer, navigate to the project's output folder and double
   click on the KeywordClassifier.vsix generated.

That's it!
