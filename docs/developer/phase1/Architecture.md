# Software Architecture

## Threads

The software consist of two or three threads

1- DAQ (Data Acusation) thread:
this thread is resposible for getting DAC (Digital 2 Analog converter) data from device.

2- Render thread

3- Main UI thread (WPF)

These threads do use same data, but not work much together. they work independently. It means there is no event or direct calls between them.


## DAQ Thread

this thread is initialized on `App.StartUp()` (App.xaml.cs file) which a new instance of `DaqInterface` is created and `DaqInterface.StartSync` called in new thread. `DaqInterface.StartSync` in a infinite loop will get data from `DAQ` hardware and do process like bit shifting, etc, and finally will put the data into `UiState.CurrentRepo` which is an instance of `DataRepository` class. This way only one thread is engaged with details of interfacing the hardware.

`DaqInterface` takes care of details of hardware, like serial connection, baud rate, comunication protocol, bit shifting (from RPIPico 12 bit data to .NET's 16 bit data - `short` data type) etc.

## Data Flow

Data from DAQ device could have any rate. Say if an arduino is a DAQ, then few killo bytes per second, or if rpi pico 1 is DAQ (curretly is) there would be at max 500K sample per second, each sample is 12 bit. but in .net smallest data type would be short (int16).

500K*2byte = 1MB is required for data for each second. we only keep a fixed amount of data (in a circular array). this is called `DataRepository` class. and the length is set via `DataRepository.RepoLength`. A fixed length list is used to store the data.

the fixed length list uses a circular array to store data. the `FixedLengthList` is supposed to take care of replacaing old data with new data. no need any type of dequeuing by programmer.


`DataRepository` is the common class which is used by all threads.

`DataRepository` have a property of `double SampleRate` which is set by `DaqInterface` once, it is sample rate of samples stored in the array in sample per second.

## Rendering

To do a render our source is a data repository, and targeting a bitmap. there would be probably severaly types of rendering.

MainWindowDataContext.Init start a thread with timer for rendering


### Periodic Rendering

Which means there are periodic signals stored in `DataRepository`, first we need to find the frequency of each signal to show 1-2 cycle of it. User can also define it but if we find an acurate algorithm, then software can do it faster...

Finding frequency of signal
FFT is good? seems no. look at here 

https://stackoverflow.com/a/6288230
https://pages.mtu.edu/~suits/autocorrelation.html
http://www.appstate.edu/~grayro/comphys/lecture10c_11.pdf
https://dsp.stackexchange.com/questions/8432/how-to-get-fundamental-frequency-of-a-signal-using-autocorrelation
https://stackoverflow.com/questions/3949324/calculate-autocorrelation-using-fft-in-matlab
https://stackoverflow.com/questions/51352890/is-this-autocorrelation-formula-correctly-programmed

what we need is find the autocorrelation: the interval at which the signal becomes most like itself



## Existing osciloscopes

https://github.com/OpenHantek/OpenHantek6022/blob/main/docs/developer_info.md
