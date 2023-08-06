# Render
We have 
- xs : double[l] array contains time of sample in seconds
- ys : double[l] array contains value of sample in volts
- l: predefined length, like 100K or 1M, is not variable
- W: width of bitmap (canvas)
- H: height of bitmap (canvas)
- M: margin for drawing - distance from bitmap edges to inside. make graph more elegant


so how to render?!length can be so high, like 1M. seems need some windowing also some frequency detection.

for detecting frequency:


## Finding frequency of signal

FFT is good? no. look at here https://stackoverflow.com/a/6288230
what we need is find the autocorrelation: the interval at which the signal becomes most like itself


we could use zero crossing or avg crossing! which is when signal crosses the average of itself.
a list of double values which time of signal crosses the average line. lets try

let define a list L1, where signal cross the AVG line
let define a list L2, difference of L1[i] with L1[i+1].

We want to understand autocorrelation of L2, simply try different shifts, for 1 to 50 and find the nor2 of err for each shift.
Lets select minimum of errs, say shift j have minimum err.
it is possible that a division of J be our target frequency, or J is a multiply of F.

the minimum J which L2[J} is near enough to Min[L2] agains Max[L2], could be our choise...


note that this algorithm assumes signal do at least 3 times cross its average at its period. A normal sin wave only cross two times, so this algorithm is not good with simple sin waves.


After we've found  the frequency, we need to window the whole signal, itterate through windows and snapshot each window into the bitmap screen.  there is start  parameter, which is start of first window, and a width paramter which is width of window.
Maybe we do not want to snapshot 100K windows. Also need a method to prevent phase shift of signal on screen. the signal wants to move  always on the screen if we do not fix it.

General method for autocorrelation can be : phi | int(Abs(f(x)-f(x+phi)))~=0 let call it eq1


## Rendering

### Render Signal Shape

once we have found the frequency, we do not need find it anymore, for next scene we can use latest frequency, and just optimize it with above formula.

So realtime state of render engine after each render is:

- Last frequency: double (`lastFreq`)
- Last signal shape: double[1000][2] (shape of last drawn signal x,y values) used for align new shape. if phase of new signal changes, then it will appear to move on screen. (`lastShape`)

What we need to define in each render:

0- new frequency `fNew` which is optimized version of `lastFreq`. and should be fairly accurate.
1- Start Parameter (`t0`).
2- Window Width parameter (1 / fNew) (`w`).
3- Window count parameter (constant) (`cnt`).

`t0` should be calculated in a way that new graph vs `lastShape` have minimum difference (eq1)

There sould be a transformation for x:
- 0 maps to margin
- w maps to bitmap.w-margin

For y (need to reverse Y for display)
- minY map to bitmap.H - margin
- maxY map to margin

### Render Grids

the grids are not depended on t0, it depend on fNew.