<h1 align="center">Legacy AssemblyPower</h1> <br>

<br/>
<p align="center">
    <a href="LICENSE" target="_blank">
        <img src="https://img.shields.io/github/license/DIGITALLNature/LegacyAssemblyPower.svg" alt="GitHub license">
    </a>
    <a href="https://github.com/DIGITALLNature/LegacyAssemblyPower/graphs/contributors" target="_blank">
        <img src="https://img.shields.io/github/contributors-anon/DIGITALLNature/LegacyAssemblyPower.svg" alt="GitHub contributors">
    </a>
</p>
<br/>

# Introduction
This is a reduced copy of our older plugin framework for Dataverse plugins.
To reduce dependencies, it has been scaled down a bit compared to the original repository (which is still publicly online at [dev.azure.com/ec4u](https://dev.azure.com/ec4u/Dynamics%20DevLab/_git/d365.extension)).

## 🕳️&nbsp; Missing modules
The following modules have been removed to maintain integrity of this repository without old dependencies
* ConfigService
* AzureKeyVault
* TextMessages

## ⚠️&nbsp; Still existing dependencies
Please do not use the LoggingService module if you have not installed the `ec4u_log_message` entities in Dataverse.

## 🔜&nbsp; Follow-up version
As this is the legacy framework we recommend having a look at the repository containing the new framework. This is currently under development, so keep an eye on it: [DIGITALLNature/DigitallAssemblyPower](/DIGITALLNature/DigitallAssemblyPower)

## 🚀&nbsp; Installation and documentation

The [Wiki](/DIGITALLNature/LegacyAssemblyPower/wiki) contains a short introduction including a how-to-get-started.

## 🤝&nbsp; Found a bug? Missing a specific feature?

Please be aware that this repository contains legacy code. Accordingly, the code is no longer maintained and it cannot be guaranteed that bugs will be fixed in a timely manner, or at all.

## 📘&nbsp; License

Digitall Legacy AssemblyPower is released under the under terms of the [MS PL License](LICENSE).
