_PS>Attack is available as a beta. You can see where the project is at and download a precompiled binary of the alpha [here](https://github.com/jaredhaight/psattack/releases/)_

_It's under heavy, active development. Version "1.0" will be released at this years [CarolinaCon](http://www.carolinacon.org). I'm writing an ongoing series of articles about where this project is at [here](http://www.psattack.com/tags/psattack/)_

_You can find a list of commands that have been tested [here](https://docs.google.com/spreadsheets/d/10Axl5VE08FJGrAh0NjQ_JEskxDfRvHIgUANdnTH3z3Y/edit?usp=sharing)_

_If you have any questions or suggestions for PS>Attack, feel free to reachout on [twitter](https://www.twitter.com/jaredhaight) or via email: jaredhaight `at` prontonmail.com_

## PS>Attack [![Build status](https://ci.appveyor.com/api/projects/status/x8doqg2vv73f131x?svg=true)](https://ci.appveyor.com/project/jaredhaight/pspunch)

A portable console aimed at making pentesting with PowerShell a little easier.

#### What is it
PS>Attack combines some of the best projects in the infosec powershell community into a self contained executable. It's designed to evade antivirus and Incident Response teams.

1. It doesn't rely on powershell.exe. Instead it calls powershell directly through the dotNet framework.
2. The modules that are bundled with the exe are encrypted. When PS>Attack starts, they are decrypted into memory. The unencrypted payloads never touch disk, making it difficult for most antivirus engines to catch them.

PS>Attack contains over 100 commands for Privilege Escalation, Recon and Data Exfilitration. It does this by including the following modules and commands:

* [Powersploit](https://github.com/PowerShellMafia/PowerSploit)
  - Invoke-Mimikatz
  - Get-GPPPassword
  - Invoke-NinjaCopy
  - Invoke-Shellcode
  - Invoke-WMICommand
  - VolumeShadowCopyTools
* [PowerTools](https://github.com/PowerShellEmpire/PowerTools)
  - PowerUp
  - PowerView
* [Nishang](https://github.com/samratashok/nishang)
  - Gupt-Backdoor
  - Do-Exfiltration
  - DNS-TXT-Pwnage
  - Get-Infromation
  - Get-WLAN-Keys
  - Invoke-PsUACme
* [Powercat](https://github.com/besimorhino/powercat)
* [Inveigh](https://github.com/Kevin-Robertson/Inveigh)

It also comes bundled with `get-attack`, a command that allows you to search through the included commands and find the attack that you're looking for.

![Get-Attack](http://i.imgur.com/XKUEvkl.png)

#### How to use it
PS>Attack works best when you generate your own version through the [PS>Attack Build Tool](https://www.github.com/jaredhaight/PSAttackBuildTool). The build tool will handle downloading PS>Attack, updating the modules to the latest versions, encrypting them with a custom key and then compiling the whole thing.

If you want to just try PS>Attack, you can download a compiled release from the [releases](https://www.github.com/jaredhaight/PSAttack/releases/) tab. This binary will work, but the modules may be out of date and the encrypted files aren't custom so they're going to be much easier to spot by AV or IR teams. 

Of course, you can also just clone the repo and compile the code yourself. You can use Visual Studio Community Edition to work with it and compie it.

#### Gr33tz
PS>Attack was inspired by and benefits from a lot of incredible people in the PowerShell community. Particularly [mattifiestation](https://twitter.com/mattifestation) of PowerSploit and [sixdub](https://twitter.com/sixdub), [engima0x3](https://twitter.com/enigma0x3) and [harmj0y](https://twitter.com/HarmJ0y) of Empire. Besides writing the modules and commands that give PS>Attack it's punch, their various projects have inspired alot of my approach to this project as well as my decision to try and contirbute something back to the community.

A huge thank you to [Ben0xA](https://twitter.com/ben0xa), who's [PoshSecFramework](https://github.com/PoshSec/PoshSecFramework) was used to figure out a lot of things about how to build a powershell console.
