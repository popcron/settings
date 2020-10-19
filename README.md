# Settings
Something that can manage and save user settings editor/builds.

![image](https://media.discordapp.net/attachments/461266635383111680/767508875687100436/unknown.png)

## Installing
To install for use in Unity, copy everything from this repository to `<YourNewUnityProject>/Packages/Popcron.Settings` folder.

If using 2018.3.x, you can add a new entry to the manifest.json file in your Packages folder:
```json
"com.popcron.settings": "https://github.com/popcron/settings.git"
```

If using 2019.x or above, you can click on the + in the package manager UI and add use the following git url:
```json
https://github.com/popcron/settings.git
```

Then, create a few properties and make sure to click on the `Generate` button to create the class file.

## What it does
- Creates a `Settings.cs` class that holds all of the settings
- Saves the settings automatically on change as a json file
- Loads the settings automatically

## Example
Lets imagine your setup resembles the image above, if that is the case, this is how you would access the settings in code:
```cs
Settings.sensitivity = 5f;
Settings.invert = false;
```
