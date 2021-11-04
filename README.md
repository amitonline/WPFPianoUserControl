# WPFPianoUserControl
A WPF UserControl which displays a piano keyboard and uses MIDI to play sounds on an actual musical keyboard or synth.

This project is a demo of how to create a piano keyboard as a WFP UserControl. It displays a single octave keyboard and uses NAudio.Midi to play the sounds on a musical 
keyboard or a synth when the keys are pressed. The usercontrol will still work if no MIDI keyboard is connected - it will update the keyboard UI without trying to play any sounds.

**IMPORTANT**
The control only supports sending notes as MIDI out commands, so the connected keyboard should have a built-in speaker or be connected to some audio output for the notes to be heard. Pure MIDI controller keyboards will not generate any audio sound.

**UPDATE 4 NOV, 2021**
The code has now been revamped to handle multiple octaves. The UI creation and MIDI node handling is now dynamic. The control can show up to 7 octaves with C4 being the middle and default octave.
PianoUserControl project is the actual usercontrol. PianoTest project is the demo application which tests the usercontrol. TestDynamic project is a scratchpad application used for testing ideas before putting them into the control. 


**FIRST WORKING COPY**
This source code is a companion to the Youtube Playlist which explains how to create this User Control.
https://www.youtube.com/playlist?list=PLLAs2gIR3bXOfS2JkOKh9U3ahCYp3nDoo


![Screenshot](https://github.com/amitonline/WPFPianoUserControl/blob/master/mq2.jpg)
