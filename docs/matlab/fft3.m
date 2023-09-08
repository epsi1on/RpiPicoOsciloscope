#https://www.educba.com/matlab-autocorrelation/

clc;
clear all;
close all;
t= 0:500;
x= sin(pi/4*t);
subplot(2,1,1)
plot(t,x)
subplot(2,1,2)
autocorr(x)