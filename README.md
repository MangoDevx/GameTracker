# ![](https://github.com/MangoDevx/GameTracker/blob/master/Images/GTLogo.png?raw=true) GameTracker
GameTracker is a fairly lightweight application which will show you how many hours you play your games or use applications for and display them in charted data.
The project was built with cross-platform capabilities, but it is untested. If you'd like to try to compile it for cross-platform let me know!

## Features 
|Features           | Description                                      |TODO                           |
|-------------------|--------------------------------------------------|-------------------------------|
|Auto Detect        |Automatically detects most Steam games            |Add more automatic game support|
|Modern Webpage     |Modern webpage to display the data for you        |                               |
|Friendly Menu      |Easy & straightforward to use console based UI    |Complete CT impl               |
|Whitelist          |Easily add applications that are not auto-detected|                               |
|Blacklist          |Easily block applications from being auto-detected|                               |

## Direction
I'd like to take this project in the direction of a GUI rather than console based menu. I may revisit this project and remake it with Avalonia

## Screenshots
![main-menu](https://github.com/MangoDevx/GameTracker/blob/master/Images/MainMenu.png?raw=true, "Main Menu")

![list-menu](https://github.com/MangoDevx/GameTracker/blob/master/Images/ProcessMenu.png?raw=true, "List Menu")

![web-view](https://github.com/MangoDevx/GameTracker/blob/master/Images/WebView.png?raw=true, "Web View")

![social-card](https://github.com/MangoDevx/GameTracker/blob/master/Images/GameTrackerSocialCard.png?raw=true, "Social Card")

## Dependencies
- You will need Node.js w/ npm if you want to use the webpage
## How to run
- Self Compile:
    - (WINDOWS) If you choose to compile the code yourself, the build needs to follow the same structure as the Windows Build in the Tests folder. 
        - 3 folders next to each other, api, tracker, and web with the appropriate files inside of them
- Downloading:
    - (WINDOWS) You can download the repository then copy the Windows Build folder that is inside Tests anywhere you'd like
    - (WIP) You can also find the release in the Releases button of Github

## Credits
The menu was created with [Spectre.Console](https://github.com/spectreconsole/spectre.console)