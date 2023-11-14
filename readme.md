A very simple osciloscope, uses RPI pico as data acusation (DAQ) unit, and wpf for displaying signal

Downloadable Releases:
- Proof of Concept: [DOWNLOAD](https://github.com/epsi1on/SimpleOscilloscope/releases/download/POC/release.zip)

Here is output for osciloscope for a PWM signal with 25% duty cycle

![Screen Shot](POC.png?raw=true "Screnshot")

# How to start


1- Download POC version of osciloscope from here [DOWNLOAD](https://github.com/epsi1on/SimpleOscilloscope/releases/download/POCv2/release.zip), unzip. 

2- Decompress the file, locate `rp2daq.uf2` in the extracted content

3- Now we need to upload `rp2daq.uf2` to Pico. Unplug RPI pico from PC, hold the button on the RPI PICO and while pressing it down, connect the usb cable to PICO. copy the rp2daq.uf2 file from previous step into the drive shown in the explorer. keyword for search: "upload uf2 file to raspberrypi pico"

4- The probe is GP26 of RPI PICO (pin #31 of 40 pins) also called ADC0 in datasheet ([pinout](https://www.raspberrypi.com/documentation/microcontrollers/images/pico-pinout.svg)). connect the probe to your signal source, also connect ground of signal to RPI Pico ground (pico ground pins: #3,#8,#13,#18,#23,#28,#33,#38) in order to have commonn ground

5- Run the binary file `SimpleOsciloscope.UI.exe` file from zipped file, select the COM port of RPI pico and choose a sampling rate (default is 500'000 which is 500K sample per second) and click the connect button.


hopefully everything works as expected and you'll see the waveform on the screen. PWM signals are tested and works good, maybe on more complex waveform application have some bugs, as it is proof of concept still.

Please report issues in the Issue section.
