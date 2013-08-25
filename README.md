# KeywordClassifier Visual Studio Extension

This is a sample extension for Visual Studio that shows how to 
to customize the syntax highlighting. The current version supports
VS2010, VS2012 and the VS2013 preview.

The extension was originally built as a custom Clasiffier component,
but later changed to a custom Tagger component after beta 2 was released,
because it offered better performance.

The extension supports C#, C/C++, and JavaScript, but support for other languages
can be added easily. Each type of keyword highlighted can also be 
customized through new classifications in the Fonts and Colors section
of the Visual Studio Options dialog.

Keywords that are customized include:

1. *Control Flow Keywords* (if, foreach, while, etc.). Customized through the "Keyword - Control Flow" classification.
2. *LINQ Keywords* (select, from, where, join, etc.). Customized through the "Operator - LINQ" classification.
3. *Visibility Keywords* (public, private, etc.). Customized through the "Keyword - Visibility" classification.

![Extension sample](http://winterdom.com/wp-content/uploads/2009/06/vs10_kc_1_thumb.png)

Starting with version 1.4, syntax highlighting of escape sequences in strings is also 
supported, through the "String Escape Sequence" classification:

![Extension sample](http://winterdom.com/images/kc_string_escape.png)

The extension is posted on the Visual Studio Gallery at 
http://visualstudiogallery.msdn.microsoft.com/862fbd13-4a20-44db-b94c-5854e2672b0d

## Build and Installation

1. Install the Visual Studio 2010 SDK. You'll need it for building custom
   extensions.
2. Open the solution in Visual Studio 2010 and build it.
3. Close all open VS2010 instances.
4. Using Windows Explorer, navigate to the project's output folder and double
   click on the KeywordClassifier.vsix generated.

That's it!

## Customization

It is now possible to customize which keywords get highlighted for each of the 3
categories the extension supports for each supported language:

1. Create a new registry key in HKEY_CURRENT_USER:
   Software\Winterdom\VS Extensions\KeywordClassifier
2. Create a new string value under this key for each language/category you want
   to customize. Possible languages are CSharp and Cpp. Possible categories are
   ControlFlow, Linq and Visibility.
   So, for example, to change which Linq keywords
   get highlighted for C#, just create a value named CSharp_Linq and set it to a
   comma-separated list of keywords.

