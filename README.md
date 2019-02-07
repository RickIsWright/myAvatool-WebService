<h1 align="center">
    <img src="https://raw.githubusercontent.com/APrettyCoolProgram/myAvatool/master/Resources/Images/Project/myAvatool-WebService-256.png" alt="myAvatool Environment Compare" width="256">
    <br>
    myAvatool Web Service
    <br>
    <img src="https://img.shields.io/badge/License-Apache%202.0-blue.svg" alt="License">
    <img src="https://img.shields.io/badge/.NET-4.6.1%2B-blue.svg" alt=".NET version">
    <img src="https://img.shields.io/badge/Development%20Status-On%20hold-orange.svg" alt="Development status">
    <a href="https://help.github.com/articles/about-pull-requests/">
        <img src="https://img.shields.io/badge/Pull Requests-Go%20for%20it-brightgreen.svg?style=shields" alt="Pull requests">
    </a>
</h1>

This project is kind of old, and I haven't worked on it in a while. But it works! And it's really, really commented.

## Features
* Scheduling Calendar: Verify that a scheduled appointment is within the duration of it's system code.

## Usage

### Scheduling Calendar Duration Check
1. Log into the Avatar environment
2. Open Form Designer
3. Choose the form/section you want
4. Click "Show Section"
5. Add the WSDL file in the "Import WSDL for Scriptlink" field, then click "Import"
```
https://<WEB_SERVER>/AVWS/Scheduling.asmx?WSDL`
```
6. Click the "Available Scripts" dropdown for the action you want (i.e. "Pre-File")
7. Choose "Schedule"
8. Enter "SchedCalApptDurCheck" in the "Parameter" field
9. Uncheck both "Disable All Scripts..." boxes
10. Click "Return to Designer"
11. Click "Save"
12. Click "Submit"

## The myAvatool Project
The myAvatool Web Service is part of the myAvatool project. Please visit the [myAvatool repository](https://github.com/APrettyCoolProgram/myAvatool) for more utilities you can use with myAvatar.
