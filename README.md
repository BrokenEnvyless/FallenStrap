> [!CAUTION]
> The only official places to download FallenStrap are this GitHub repository and [fallenstraplabs.com](https://fallenstraplabs.com). Any other websites offering downloads or claiming to be us are not owned by us.

> [!IMPORTANT]
> FallenStrap is not under active development anymore. Any issues or pull requests opened may take a while to get a response.

<p align="center">
    <img src="https://github.com/fallenstraplabs/fallenstrap/raw/main/Images/FallenStrap-full-dark.png#gh-dark-mode-only" width="380">
    <img src="https://github.com/fallenstraplabs/fallenstrap/raw/main/Images/FallenStrap-full-light.png#gh-light-mode-only" width="380">
</p>

<div align="center">

[![License][shield-repo-license]][repo-license]
[![GitHub Workflow Status][shield-repo-workflow]][repo-actions]
[![Crowdin][shield-crowdin-status]][crowdin-project]
[![Downloads][shield-repo-releases]][repo-releases]
[![Version][shield-repo-latest]][repo-latest]
[![Discord][shield-discord-server]][discord-invite]
[![lol][shield-tenor-meme]][tenor-gif]

</div>

----

FallenStrap is a third-party replacement for the standard Roblox bootstrapper, providing additional useful features and improvements.

Running into a problem or need help with something? [Check out the Wiki](https://fallenstraplabs.com/wiki). If you can't find anything, or would like to suggest something, please [submit an issue](https://github.com/fallenstraplabs/fallenstrap/issues).

FallenStrap is only supported for PCs running Windows.

## Frequently Asked Questions

**Q: Is this malware?**

**A:** No. The source code here is viewable to all, and it'd be impossible for us to slip anything malicious into the downloads without anyone noticing. Just be sure you're downloading it from an official source. The only two official sources are this GitHub repository and [fallenstraplabs.com](https://fallenstraplabs.com).

**Q: Can using this get me banned?**

**A:** No, it shouldn't. FallenStrap doesn't interact with the Roblox client in the same way that exploits do. [Read more about that here.](https://fallenstraplabs.com/wiki/info/fallenstrap-and-bans)
## Features

- Hassle-free Discord Rich Presence to let your friends know what you're playing at a glance
- Simple support for modding of content files for customizability (death sound, mouse cursor, etc)
- See where your server is geographically located (courtesy of [ipinfo.io](https://ipinfo.io))
- Ability to configure graphics fidelity and UI experience

## Installing
Download the [latest release of FallenStrap](https://github.com/fallenstraplabs/fallenstrap/releases/latest), and run it. Configure your preferences if needed, and install. That's about it!

Alternatively, you can install FallenStrap via [Winget](https://winstall.app/apps/pizzaboxer.FallenStrap) by running this in a Command Prompt window:
```
> winget install fallenstrap
```

You will also need the [.NET 6 Desktop Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&rid=win11-x64&apphost_version=6.0.36&gui=true). If you don't already have it installed, you'll be prompted to install it anyway. Be sure to install FallenStrap after you've installed this.

It's not unlikely that Windows Smartscreen will show a popup when you run FallenStrap for the first time. This happens because it's an unknown program, not because it's actually detected as being malicious. To dismiss it, just click on "More info" and then "Run anyway".

Once installed, FallenStrap is added to your Start Menu, where you can access the menu and reconfigure your preferences if needed.

## Code

FallenStrap uses the [WPF UI](https://github.com/lepoco/wpfui) library for the user interface design. We currently use and maintain our own fork of WPF UI at [fallenstraplabs/wpfui](https://github.com/fallenstraplabs/wpfui).


[shield-repo-license]:  https://img.shields.io/github/license/fallenstraplabs/fallenstrap
[shield-repo-workflow]: https://img.shields.io/github/actions/workflow/status/fallenstraplabs/fallenstrap/ci-release.yml?branch=main&label=builds
[shield-repo-releases]: https://img.shields.io/github/downloads/fallenstraplabs/fallenstrap/latest/total?color=981bfe
[shield-repo-latest]:   https://img.shields.io/github/v/release/fallenstraplabs/fallenstrap?color=7a39fb

[shield-crowdin-status]: https://badges.crowdin.net/fallenstrap/localized.svg
[shield-discord-server]: https://img.shields.io/discord/1099468797410283540?logo=discord&logoColor=white&label=discord&color=4d3dff
[shield-tenor-meme]:     https://img.shields.io/badge/mom_made-pizza_rolls-orange

[repo-license]:  https://github.com/fallenstraplabs/fallenstrap/blob/main/LICENSE
[repo-actions]:  https://github.com/fallenstraplabs/fallenstrap/actions
[repo-releases]: https://github.com/fallenstraplabs/fallenstrap/releases
[repo-latest]:   https://github.com/fallenstraplabs/fallenstrap/releases/latest

[crowdin-project]: https://crowdin.com/project/fallenstrap
[discord-invite]:  https://discord.gg/nKjV3mGq6R
[tenor-gif]:       https://media.tenor.com/FIkSGbGycmAAAAAd/manly-roblox.gif

## Code signing policy

Thanks to [SignPath.io](https://signpath.io/) for providing a free code signing service, and the [SignPath Foundation](https://signpath.org/) for providing the free code signing certificate.
