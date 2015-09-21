```
/**************************************************************************
 * 
 * Copyright (c) Unterrainer Informatik OG.
 * This source is subject to the Microsoft Public License.
 * 
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * (In other words you may copy, use, change and redistribute it without
 * any restrictions except for not suing me because it broke something.)
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 * 
 ***************************************************************************/

```

# General  

This section contains various useful projects that should help your development-process.  

This section of our GIT repositories is free. You may copy, use or rewrite every single one of its contained projects to your hearts content.  
In order to get help with basic GIT commands you may try [the GIT cheat-sheet][coding] on our [homepage][homepage].  

This repository located on our  [homepage][homepage] is private since this is the master- and release-branch. You may clone it, but it will be read-only.  
If you want to contribute to our repository (push, open pull requests), please use the copy on github located here: [the public github repository][github]  

# SplitStopWatch  

This class implements a stopWatch.  

Additionally to the normal stopWatch-functionality it may be used to debug out split-times as well. It measures the split-times and keeps track of the overall times in a variable.  
Don't be afraid to stop the watch. Stopping doesn't mean you loose any value whatsoever. Think of it as a real-life stopWatch where you may press the start-button at any time after previously pressing the stop-button.  

This class provides useful overloads that allow writing to a stream in a way that your measurement doesn't get compromised (the stopWatch is paused while writing to the stream). You may initialize it with a stream so that you can use all the overloads that take a string-argument or Console.Out is used as a default.  
All the write-operations are performed as a printLine-call, so you don't need to close your assigned text with a newline-character.  

It has a property 'isActive' that defaults to true. When this is set to false all calls to this class are aborted within a single if-statement in the called method. This is a convenience function so that you may leave your logging-code in the production code.  

#### Example  
    
```csharp
SplitStopWatch ssw = new SplitStopWatch();
ssw.start("started.");
  Thread.sleep(10);
ssw.split("split.");
  Thread.sleep(10);
ssw.stop("stopped.");
```


[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/splitstopwatch