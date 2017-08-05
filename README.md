# SSVEP Keyboard
v0.06

This is a repository for the SSVEP keyboard built primarily for the Smartphone BCI

[Smartphone-bci](https://jmanart.github.io/smartphone-bci-hardware) is a hardware / software platform born with the intention of Developing an EEG under £20.

Other repositories that are a part of the smartphone BCI Project:

* [SMARTPHONE-BCI](https://github.com/icibici/Android-diagnostic-app)
* [SMARTPHONE-BCI-HARDWARE](https://github.com/icibici/smartphone-bci-hardware)

---------------------------------------------------------------------------------------------------------------------------------------------------------

**WARNING! THIS APP USES FLICKERING LIGHTS! DO NOT USE THIS APP IF YOU HAVE PHOTOSENSITIVE EPILEPSY**
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------------------------------------------------------------

## What is SSVEP?

- [A Quick Intro to SSVEP](http://synaptitude.me/blog/a-quick-intro-to-ssvep-steady-state-visually-evoked-potential/)

## The Unity App

The **SSVEP Keyboard** app is built in Unity (currently version 2017.1.0f3) and can run on following platforms:

- iOS
- Android (untested but should work)

You can download builds for Android [here](https://drive.google.com/drive/folders/0B4W4Pn0tIMBXbGUtdmJCMW02dk0?usp=sharing))
For the iOS version please contact me directly and I can add you to the beta testers via TestFlight

### Current functions of the app

To use the app, it must be connected to the Smartphone BCI (see instructions [here](https://jmanart.github.io/smartphone-bci-hardware))

The Keyboard app can do the following:

- Output a 1Khz tone via the headphone jack to act as a carrier wave for the Smartphone BCI
- Read a signal from the microphone and output the frequecy bands from 940-1060Hz
- Toggle the mode to display these signal bands with various processing methods applied to view peaks (Sorry as modes are currently unlabelled. The default mode is the one with no zero values, please use that one)
- Flickering two sets of bands at 985/1015Hz and 980/1020Hz. This acts as a visual guide for the location of SSVEP signals at 15Hz and 20Hz (Look for peaks at these points)
- Use demo signal toggle: Turn this on to play a pre-recorded EEG signal (from my own brain) instead of using the signal from your device
- Detect toggle: Turn on to drive the keyboard with your signal by comparing the levels of two frequency bands:
  - If (level at 20Hz > level at 15Hz) for longer than 1 second then High Frequency is triggered
  - If (level at 20Hz < level at 15Hz) for longer than 1 second then Low Frequency is triggered
  - If the levels are roughly equal then nothing is triggered
  - This detection method may or may not work for you as I have only tested it a few times.
- The keyboard can be operated manually by clicking the low frequecy and high frequecy buttons, this will simulate what would happen if the device had interpreted a low or high frequency signal and reduce the letter choices until one is found.
- Click Idle Reset to reset the input. As the input averages over time this will clear out old data and start fresh.
- Click the text are at the top to clear it.
- The framerate needs to be 60 fps for it to work properly. The signal output should be 11025 Hz

Using two devices:

- The keyboard keys are very small to see. If you would like to view an SSVEP signal or operate the keyboard with larger flickering squares you can setup a second device running the flicker app at 15Hz and 20Hz. This may be more reliable.

The Flicker app can do the following:

- Display two squares that flicker at two different Hz rates set by the user.
- Be sure to choose frequencies that divide evenly into 60Hz for accurate flickers (15Hz and 20Hz are used in the keyboard)
- Note that the screen refresh rate (displayed in the app) should be 60fps for this to work correctly. (For example 30fps will not display 20Hz correctly as it does not divide evenly)

### iOS build issues
This is in case you have errors when building with XCode. On iOS you need to request permission to use the microphone. For some reason Unity isn't adding this request but you can add it manually. Look for the error and see if it says anything about adding a key to Info.plist

You may need to manually add a key to Info.plist:
NSMicrophoneUsageDescription

with a string explaining the use:
Microphone data used as an input for EEG data from the BCI device

You should probably add this one with a string description while you're there. It prevents other potential build errors.
NSCameraUsageDescription

### To Do

There's lots still to do. All tasks are tracked in the issues section.