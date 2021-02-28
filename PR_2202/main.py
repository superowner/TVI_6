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
        self.draw_gear(r=66, b=133, g=244,
                       center_x=255, center_y=245,
                       radius=75, to_left=True)
        self.draw_gear(r=52, b=168, g=83,
                       center_x=200, center_y=200,
                       radius=50, to_left=False)
        self.draw_gear(r=251, b=188, g=5,
                       center_x=275, center_y=200,
                       radius=25, to_left=True)
        self.draw_gear(r=234, b=67, g=53,
                       center_x=255, center_y=240,
                       radius=25, to_left=False)
        self.draw_arrow(r=100, b=100, g=100,
                        x=255, y=240, w=200, h=5,
                        radius=10)
        self.draw_arrow(r=150, b=150, g=150,
                        x=255, y=240, w=150, h=10,
                        radius=60)
        self.draw_arrow(r=200, b=200, g=200,
                        x=255, y=240, w=100, h=15,
                        radius=60*12)

    def draw_arrow(self, r, b, g, x, y, w, h, radius):
        transform = QtGui.QTransform(1, 0, 0, 1, x, y)
        transform.rotate(100 / radius * self.rotate)

        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(r, b, g))
        qp.drawRect(0, h / -2, w, h)
        qp.end()

    def draw_gear(self, r, b, g, center_x, center_y, radius, to_left):
        transform = QtGui.QTransform(1, 0, 0, 1, center_x, center_y)
        transform.rotate(100 / radius * self.rotate * (1 if to_left else -1))

        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(r, b, g))

        points_on_circle = []
        for x in range(-(radius + 1), radius + 1):
            for y in range(-(radius + 1), radius + 1):
                if x ** 2 + y ** 2 == radius ** 2:
                    points_on_circle.append([x, y])

        points_on_circle.sort(key=lambda c: np.arctan2(c[0], c[1]))

        for i in range(len(points_on_circle)):
            a_point = points_on_circle[i - 1]
            b_point = points_on_circle[i]
            bc_line = radius // 5
            ab_line = np.sqrt((a_point[0] - b_point[0]) ** 2 + (a_point[1] - b_point[1]) ** 2)

            vec_x = (a_point[0] - b_point[0]) / ab_line
            vec_y = (a_point[1] - b_point[1]) / ab_line
            c_point = [
                b_point[0] + (vec_y * bc_line),
                b_point[1] + (-vec_x * bc_line)
            ]
            path = QtGui.QPainterPath()
            path.moveTo(a_point[0], a_point[1])
            path.lineTo(c_point[0], c_point[1])
            path.lineTo(b_point[0], b_point[1])
            qp.fillPath(path, QtGui.QBrush(QtGui.QColor(r, b, g)))

        path = QtGui.QPainterPath()
        path.addText(radius * -1.31, radius * 0.67, QtGui.QFont('Georgia', 1.45 * radius), "⭕")
        path.addText(radius * -1.31, radius * 0.67, QtGui.QFont('Georgia', 1.45 * radius), "➕")
        qp.drawPath(path)
        qp.end()

    def update_values(self):
        self.rotate += 1
        self.update()

    def draw_meme(self, x, y):
        pass


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)
    w = CircleAnimate()
    w.show()
    app.exec()
