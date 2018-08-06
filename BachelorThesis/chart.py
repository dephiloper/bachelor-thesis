#!/usr/bin/env python3
import matplotlib.pyplot as plt
from scipy import interpolate

with open('log.txt') as f:
    lines = f.readlines()

y = []

for line in lines:
	y.append(float(line[-10:].replace(',','.')))

x = range(len(y))

#smooth = interpolate.interp1d(x,y, kind='linear')

plt.plot(x, y)
plt.show()
