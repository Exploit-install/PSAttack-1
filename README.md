_PS>Punch is in a super limited Proof of Concept state right now. You can see where the project is at and download a precompiled binary of the alpha [here](https://github.com/jaredhaight/PSPunch/releases/)_

_It's under heavy, active development and should be taking shape rapidly, with a "1.0" release planned early 2016. I'm writing an ongoing series of articles about where this project (and it's parent, [PS>Attack](https://github.com/jaredhaight/PSAttack)) is at [here](http://www.psattack.com/tags/psattack/)_

_You can find a list of commands that have been tested [here](https://docs.google.com/spreadsheets/d/10Axl5VE08FJGrAh0NjQ_JEskxDfRvHIgUANdnTH3z3Y/edit?usp=sharing)_

_If you have any questions or suggestions for PS>Attack and PSPunch, feel free to reachout on [twitter](https://www.twitter.com/jaredhaight) or via email: jaredhaight `at` prontonmail.com_

## PS>Punch [![Build status](https://ci.appveyor.com/api/projects/status/x8doqg2vv73f131x?svg=true)](https://ci.appveyor.com/project/jaredhaight/pspunch)

A portable console aimed at making pentesting with PowerShell a little easier.

#### What is it
PS>Punch combines some of the best projects in the infosec powershell community into a self contained executable. It's designed to evade antivirus and Incident Response teams.

1. It doesn't rely on powershell.exe. Instead it calls powershell directly through the dotNet framework.
2. The modules that are bundled with the exe are encrypted. When PS>Punch starts, they are decrypted into memory. The unencrypted payloads never touch disk, making it difficult for most antivirus engines to catch them.

Offensively, PS>Punch contains commands over 100 commands for Privilege Escalation, Recon and Data Exfilitration. It does this by including the following modules and commands:

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

#### How to use it
PS>Punch works best when you generate your own version through [PS>Attack](https://www.github.com/jaredhaight/PSAttack). PS>Attack will handle downloading PS>Punch, updating the modules to the latest versions, encrypting them with a custom key and then compiling the whole thing into an executable.

If you want to just try PS>Punch, you can download a compiled release from the [releases](https://www.github.com/jaredhaight/PSPunch/releases/) tab. This binary will work, but the modules may be out of date and the encrypted files aren't custom so they're going to be much easier to spot by AV or IR teams. 

Of course, you can also just clone the repo and compile the code yourself. You can use Visual Studio Community Edition to work with it and compie it.

#### Gr33tz
PS>Punch was inspired by and benefits from a lot of incredible people in the PowerShell community. Particularly [mattifiestation](https://twitter.com/mattifestation) of PowerSploit and [sixdub](https://twitter.com/sixdub), [engima0x3](https://twitter.com/enigma0x3) and [harmj0y](https://twitter.com/HarmJ0y) of Empire. Besides writing the modules and commands that give PS>Punch it's.. punch, their various projects have inspired alot of my approach to PS>Attack and PS>Punch as well as my decision to try and contirbute something back to the community.

A huge thank you to [Ben0xA](https://twitter.com/ben0xa), who's [PoshSecFramework](https://github.com/PoshSec/PoshSecFramework) was used to figure out a lot of things about how to build a powershell console.
