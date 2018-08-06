#!/usr/bin/env python
from __future__ import division
from keras import regularizers
from keras.models import Sequential
from keras.layers import Dense, Dropout
from keras.callbacks import TensorBoard
import KerasModelToJSON as js
import numpy as np
import json
import random
import sys

def load_data():
    # read json lines to content array
    with open('decision.txt') as f:
        content = f.readlines()
        content = [json.loads(x) for x in content]

    # seperate content into features and labels
    features = []
    labels = []
    for i, data in enumerate(content):
        if i % 2 == 0:
            features.append(data)
        else:
            labels.append(data)

    # shuffle features and labels together
    zipped = list(zip(features, labels))
    random.shuffle(zipped)
    features, labels = zip(*zipped)

    return np.array(features), np.array(labels)

# create model
model = Sequential()
model.add(Dense(units=20, activation='relu', input_dim=37))
model.add(Dense(units=2, activation='tanh'))

model.compile(loss='mse', optimizer='adam')

# save train model
wrt = js.JSONwriter(model, "train_model.json")
wrt.save()

sys.exit(0)

x, y = load_data()
train_count = int(len(x) * 0.8)
test_count = int(len(x) * 0.15)

x_train = x[:train_count]
y_train = y[:train_count]
x_test = x[train_count:train_count + test_count]
y_test = y[train_count:train_count + test_count]
x_eval = x[train_count + test_count:]
y_eval = y[train_count + test_count:]

tbCallBack = TensorBoard(log_dir='./Graph', histogram_freq=0, write_graph=True, write_images=True)
model.fit(x_train, y_train, epochs=10000, batch_size=128, callbacks=[tbCallBack])

loss_and_metrics = model.evaluate(x_test, y_test, batch_size=128)
print("loss and metrics: {}".format(loss_and_metrics))

preds = model.predict(x_eval, batch_size=128)
sum_pred = 0
for (y, p) in zip(y_eval, preds):
    print("predicted:{} actual: {}".format(y, p))
    if abs(y[0] - round(p[0])) < 0.0001 and abs(y[1] - round(p[1])) < 0.0001:
        sum_pred += 1
print("accuracy: {} %".format(int(sum_pred / len(y_eval) * 100)))

wrt = js.JSONwriter(model, "model.json")
wrt.save()
