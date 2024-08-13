pkg load symbolic
clear
syms rg rv
vv = 3.3
vg = 0
syms vi
syms ri 

vo=(vg/rg+vi/ri+vv/rv)/(1/rg+1/ri+1/rv)
adco = vo/3.3*4096

res = 0_001

adc0Val=14
adc33Val=4083

e1=subs(adco,[vi,ri],[0,res])
e2=subs(adco,[vi,ri],[3.3,res])

res = solve(e1 == adc0Val, e2 == adc33Val,[rg,rv] )

round(res.rg/1000)
round(res.rv/1000)