#https://www.tutorialspoint.com/programming_example/7NOnPF/autocorrelation
pkg load signal




#n=-5:5;
N=12;

t= 0:N;
x=sin(pi/4*t);


#x=ones(1,N+1);
r=xcorr(x);
disp('autocorrelation sequence r=');
disp(r);
subplot(2,1,1)
stem(t,x);
title('square sequence r=');
subplot(2,1,2);
k=-N:N;
stem(k,r);
title('auto correlation output');
xlabel('lag index');
ylabel('amplitude');