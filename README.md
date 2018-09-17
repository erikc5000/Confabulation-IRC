# Confabulation-IRC
Years ago, I had this grand idea to write a more modern Internet Relay Chat client on Windows, written in C# and WPF.  As IRC waned in popularity and I never really had the time to work on it in my spare time anyway, it was ultimately abandoned.  This code has been sitting around and collecting dust, so I've decided to put it up here on GitHub, as is -- in a somewhat functional state.  While it's far from complete, there may be some parts of it that are useful or interesting to anyone implementing an IRC client.

Looking back, there are certainly a number of things I would have done differently.  The interface between the chat library and UI is a little clumsy and the way the threading is handled seems too complicated and error prone.  I should have made better use of dependency injection to facilitate testing too, though that shouldn't be too hard to fix.

As far as code structure, it's divided into four projects:
- Chat - The core IRC library, implementing a decent chunk of RFC-1459
- Controls - A collection of custom UI controls, designed to be generic enough to use in any WPF project
- Confabulation - The UI and primary Windows executable
- Test - A testing application that I used before the UI was functional enough and probably no longer works (Testing?  Who needs that! ;-))

The project uses Visual Studio 2008 and a now antiquated version of the .NET Framework.

The UI isn't fully functional, but you can connect to IRC servers via the UI.  For most other commands, like joining channels, sending messsages, etc., you'll need to use "/" commands.  For example:

/join #mychannel

/nick newnickname

/msg #mychannel Hello world!

It can also process URLs, email addresses, mIRC color codes, and such.
