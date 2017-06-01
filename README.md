# Inheritance Margin extension for Visual Studio 2012+

[![Build status](https://ci.appveyor.com/api/projects/status/0n5me6pqj21fh0fo/branch/master?svg=true)](https://ci.appveyor.com/project/sharwell/inheritancemargin/branch/master)

This extension adds inheritance glyphs to the glyph margin in the Visual Studio text editor. Glyphs are displayed for the following items:

* Interface methods/properties/events which are implemented by the current item
* Virtual methods/properties/events in base classes which are overridden by the current item
* Types which extend or implement the current type
* For interfaces:
  * Methods/properties/events in derived types which implement the current item
  * Interfaces which extend the current interface
  * Classes which implement the current interface
* For base classes:
  * Methods/properties/events in derived types which override the current item
  * Classes which extend the current class

![Preview](PreviewLarge.png)

## Limitations

* **C# only:** The current version only supports C#. In the future, I plan to support other languages via other MEF extensions.
* **Updates while you type:** The glyphs for a file are only updated when that file is opened and when text in that file changes. If changes in open files are not being reflected, you can either close and reopen the affected file or type in the file to force an update.
* **ReSharper users:** ReSharper provides its own version of a similar feature, so this extension isn't necessary for ReSharper users.

I've tested this extension in one of my reasonably large projects (44 projects, 2000+ C# source files), and the extension should never impact the performance of the editor. Depending on the complexity of the current file (size, number of classes/methods, number of derived classes, etc), it may take several seconds for the glyphs to update after the file is opened or text is changed. During this time you should be able to work like normal. If you experience text editor slowdowns due to this extension, please [let us know](https://github.com/tunnelvisionlabs/InheritanceMargin/issues) and we'll work to correct it.
