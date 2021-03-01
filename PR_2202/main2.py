import sys
from PyQt5 import QtWidgets
from PyQt5 import QtGui
from PyQt5 import Qt
from PyQt5 import QtCore
import numpy as np


class CircleAnimate(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.resize(510, 480)
        self.qp = QtGui.QPainter()
        self.rotate = 0

        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.update_values)
        self.timer.start(33)

    def paintEvent(self, event):
        self.drawGear(50, 50, 50, 30, 10, True)

    def drawGear(self, x, y, r_out, r_in, n, to_left):
        triangle_points = []
        for i in range(n):
            triangle_points.append([r_in * np.cos(i * 2 * np.pi / n),
                                    r_in * np.sin(i * 2 * np.pi / n)]),
            triangle_points.append([r_out * np.cos(i * 2 * np.pi / n + np.pi / n),
                                    r_out * np.sin(i * 2 * np.pi / n + np.pi / n)]),
        triangle_points.sort(key=lambda c: np.arctan2(c[0], c[1]), reverse=to_left)

        transform = QtGui.QTransform(1, 0, 0, 1, x, y)
        transform.rotate(100 / r_out * self.rotate * (1 if to_left else -1))


        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(50, 50, 50))

        path = QtGui.QPainterPath()
        for i in triangle_points:
            path.lineTo(i[0], i[1])
        path.lineTo(triangle_points[0][0], triangle_points[0][1])
        path.addEllipse(r_in / -2, r_in / -2, r_in, r_in)
        path.addRect(r_in / -16, r_in / -2, r_in / 8, r_in)
        path.addRect(r_in / -2, r_in / -16, r_in, r_in / 8)
        qp.fillPath(path, QtGui.QBrush(QtGui.QColor(50, 50, 50)))

    def update_values(self):
        self.rotate += 1
        self.update()


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)
    w = CircleAnimate()
    w.show()
    app.exec()
