# FsmTool
Experimental .fsm chunk extractor.

Experimental, and mostly untested MGSV:TPP .fsm chunk extractor. Can currently be used to extract the .demo and .snd chunks from .fsm files.
It also automatically builds the SND chunks into a Wwise Vorbis (.wem) audio file, and splits it back into SND chunks when repacking.

## Usage

### Unpacking

Drag, or use Open With on, a .fsm file onto the tool to unpack.

### Repacking

Open the .fsm.xml file, next to the unpacked _fsm folder, with the tool to repack.
The .wem file with the same name as the folder it's in, without "_fsm", will be used as the repacked sound file.

If you want to generate a custom .wem file to repack into a .fsm file, use [vgmstream](https://github.com/vgmstream/vgmstream/releases/latest) to convert vanilla .wem files into .wav, and to convert your edited .wav back into .wem, use [Wwise 2013.2.9](https://www.nexusmods.com/witcher3/mods/3234).

Create a new project for custom cutscene audio. You can keep the same one if you plan to do more. 
In the Actor-Mixer Hierarchy, create a new Actor-Mixer object. Edit its "Source Settings" > "Conversion Settings" to use Vorbis. I recommend "Vorbis Auto Detect High".

Then click "Edit", and in the Windows platform line of settings, click "Edit" under "Adv." to open the Vorbis Encoder Parameters window. 
At the bottom of the window, set "Seek table granularity (sample frames)" to 4096.

Now that you've configured an Actor-Mixer for exporting cutscene audio files, drag your edited .wav onto the Actor-Mixer, importing it as a child of that Actor-Mixer. Importing is as SFX is fine.

When you import it, it should be blue under the Actor-Mixer Hierarchy. Right click it, and click "Convert...". 
This will create your .wem file in your Wwise project's "\.cache\Windows\SFX\" directory.

Copy the converted .wem file into your unpacked _fsm folder, rename after the name of the folder it's in, but without "_fsm". 
Now you can repack the .fsm by opening the .fsm.xml file with FsmTool, or dragging the .fsm.xml over the executable.
