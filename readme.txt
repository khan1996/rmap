rMap - ryl map editor


How to run:
To run rMap you will need XNA runtime 3.1 libraries.
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=53867a2a-e249-4560-8011-98eb3e799ef2&displaylang=en

How to edit:
Needed are Visual Studio 2008 C# and XNA Game Studio 3.1
http://www.microsoft.com/downloads/en/details.aspx?FamilyID=80782277-d584-42d2-8024-893fcd9d3e82&displaylang=en

Also note that the main exe should be built for x86 and not "Any CPU".
That is because if any cpu is selected then it will run as 64bit
in 64bit windows but the "3d window" library which is built in XNA
doesn't support 64. So you need to build it to force it to load
in 32bit on 32bit cpus and on 32bit emulation on 64bit cpus.

Useful hotkeys:
Ctlr+S saves and exports the open map. The first time you open it
you have to show both locations, after that they will be overwritten.
So to edit a map and test it use this hotkey.

*.rmap vs *.z3s
i made the rmap file extension for the reason of fast loading
and later to disable "importing" if i would release it to public.
When you import a z3s file it will take alot of time to create the "minimap".
So always use the rmap file for faster loading.

The import may take up to 30sec on a bit slower cpus so have patience.