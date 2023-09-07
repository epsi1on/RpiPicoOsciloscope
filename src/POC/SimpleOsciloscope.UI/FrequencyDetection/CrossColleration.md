octave code

syms x1 x2 y1 y2
dx=x2-x1
dy=y2-y1
m=dy/dx
syms x y

Y = solve(y-y1 == m*(x-x1), y)

x_r = solve(Y==0 ,x)
x_r2 = x_r - x1

l1 = x_r - x1
l2 = x2 - x_r


s1 = (y1*l1/2)
s2 = (y2*l2/2)

s = factor(expand(s1-s2))