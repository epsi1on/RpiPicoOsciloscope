%https://stackoverflow.com/questions/3949324/calculate-autocorrelation-using-fft-in-matlab

n = 1024;


xx = transpose( [ 0 : n-1]);

freq = 0.010;

x = sin(2*pi*freq*xx);# randn(n,1);


x_pad = [x zeros(size(x))];
X     = fft(x_pad);
X_psd = abs(X).^2;
r_xx = ifft(X_psd)