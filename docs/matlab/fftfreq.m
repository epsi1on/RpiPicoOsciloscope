#https://dsp.stackexchange.com/questions/1919/efficiently-calculating-autocorrelation-using-ffts
pkg load signal

clc;
%% Cross correlation through a FFT
n = 1024;


xx = transpose( [ 0 : n-1]);

freq = 0.010;

x = sin(2*pi*freq*xx);# randn(n,1);



% cross correlation reference
xref = xcorr(x,x);

plot(xref);

[val, idx] = max(xref)



#[row,col] = find(xref,mx)

%FFT method based on zero padding
fx = fft([x; zeros(n,1)]); % zero pad and FFT
x2 = ifft(fx.*conj(fx)); % abs()^2 and IFFT
% circulate to get the peak in the middle and drop one
% excess zero to get to 2*n-1 samples

x2 = [x2(n+2:end); x2(1:n)];


[xs,ys] = findpeaks(x2(:,1));

#plot(x2(:,1))
#plot(xs,ys)

% calculate the error
d = x2-xref; % difference, this is actually zero
#fprintf('Max error = %6.2f\n',max(abs(d)));

