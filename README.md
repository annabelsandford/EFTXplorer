# ![EFTXplorer](https://github.com/annabelsandford/EFTXplorer/raw/main/readme_img/eftx-git.png)

[![Version 1.1](https://img.shields.io/badge/Version-1.1-blueviolet)](https://img.shields.io/badge/Version-1.1-blueviolet) [![Windows](https://svgshare.com/i/ZhY.svg)](https://svgshare.com/i/ZhY.svg) [![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://GitHub.com/annabelsandford/EFTXplorer/graphs/commit-activity)

EFTXplorer (EFTX) is a tool designed to analyze, modify, decompress and convert Emergency Format Texture files (.EFT texture files) back to an easily readable and editable format (eg. bitmaps).

## Context 📚

Games like the [Emergency Series](https://en.wikipedia.org/wiki/Emergency_(video_game_series)) are using a proprietary texture compression format called "Emergency Format Texture" or ".EFT" for important / critical in-game textures. The games (and their editor modes) can convert Targa Image files (.TGA) to .EFT format, however they cannot (or simply don't have an option to) convert these .EFT textures back to a readable format. The biggest headache here was that you couldn't examine or alter textures unless you had access to the original .TGA file.

## Installation 🚀
#### ➡️ Preliminary Information:
EFTXplorer has only been tested on Windows 10 (x86 & x64). As of right now we do not know whether it works on any Windows version preceeding or succeeding W10. If you have any information thru personal testing, feel free to let us know and we will update this readme text accordingly. 

#### ➡️ Step #1 / Prerequisites:
- Know whether you're on a 32-bit (x86) or 64-bit (x64) machine
- [.NET Framework 4.7.2 or newer](https://support.microsoft.com/en-us/topic/microsoft-net-framework-4-7-2-offline-installer-for-windows-05a72734-2127-a15d-50cf-daf56d5faec2)
- Yeah.. that's basically it

#### ➡️ Step #2 / Download release:
Go to [releases]() and download the newest version of EFTX. Then run the version right for you.
## Features ✨
![EFTXplorer Screenshot 1](https://github.com/annabelsandford/EFTXplorer/raw/main/readme_img/animation.gif)

Explanation / list of features from top to bottom, left to right:

1️⃣ Action Menu:
- **About** - Opens window with credits and links to our socials.
- **Import** - Lets you *import* any given .EFT format file.
- **Export** - Lets you *export* any imported .EFT format file.
- **Search Updates** - Looks for EFTX updates if there's an internet connection to do so.

2️⃣ Main Interface:
- **Image Information** - Shows information like dimensions, filename & type of imported .EFT.
- **Overview** - Shows an overview of the imported file's base folder for potential quick access to other .EFT's of interest.
- **_Right-clicking Overview_** - Right-clicking the "Overview" panel reveales a context menu with the following options:
  - **Reload File Explorer**  - Updates the Overview panel and reloads the folder to show any potential new files.
  - **Import \*.eft** - The same as the "Import" button in the Action Menu. Lets you import any given .EFT format file.
  - **Clear** - Clears EFTX. Also removes any temporary files and sweeps the cache.
- **Preview** - Shows a thumbnail / preview of the imported .EFT format file.
- **_Right-clicking Preview_** - Right-clicking the "Preview" panel reveales a context menu with the following option:
  - **Rotate Image**  - Rotates the image 90 degrees. Will affect the export.
- **Edit** - Nonfunctional. Will be removed in future versions.

3️⃣ Console Interface:
- **Console** - Shows everything EFTX is doing. Highly useful to find if something is not working correctly.

![EFTXplorer Screenshot 2](https://github.com/annabelsandford/EFTXplorer/raw/main/readme_img/eftx_2.PNG)

## Known issues with EFTXplorer:
- [ ] Some EFT files have the tiles in the wrong order when imported (Especially Emergency 3 .EFT's)
- [ ] All EFT files bigger than 512x512 have weird seams.
- [X] (Fixed 1.2) Importing an EFT file then clicking "About" results in EFTXplorer crashing with an exception related to the tmp bitmap
- [ ] (Fixes pending) memory leak in EFT loading: the pointer with the RGBA/BGRA data from load_eft_file_rgba/bgra isn't freed after use
- [ ] (Fixes pending) Several memory leaks in load_eft_file_rgba/bgra
- [ ] (WINE specific) the update checker fails to work
- [X] The update checker reports there's an update when there isn't.

## Contributing ✏️
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.
Please make sure to update tests as appropriate.
You are allowed to use code as per our license. We urge you to read it if you plan on using or improving our code. 

## Credits ✏️
Jean-Luc Mackail ([Twitter > @FuzzyQuills](https://twitter.com/FuzzyQuills))
- The brain behind all this. EFTX wouldn't be here without him. He's the best (I love him <3)

Annabel Jocelyn Sandford ([Twitter > @annie_sandford](https://twitter.com/annie_sandford))
- Me. Nothing to say lol. C# / .NET wrapper, came up with a few good ideas

## License 📜
[GNU AFFERO GENERAL PUBLIC LICENSE 3.0 (AGPL)](https://www.gnu.org/licenses/agpl-3.0.txt)
