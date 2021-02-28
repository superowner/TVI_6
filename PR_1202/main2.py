import sys
from PyQt5 import QtWidgets
from PyQt5 import QtGui
from PyQt5 import Qt
from PyQt5 import QtCore
import numpy as np


class CircleAnimate(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.resize(300, 300)
        self.qp = QtGui.QPainter()

        self.rotate = 0
        self.radius = 100
        self.center_x = self.height() // 2
        self.center_y = self.height() // 2

        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.update_values)
        self.timer.start(10)

    def paintEvent(self, event):
        transform = QtGui.QTransform(1, 0, 0, 1, self.center_x, self.center_y)
        transform.rotate(self.rotate)

        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(0, 150, 0))

        points_on_circle = []
        for x in range(-(self.radius + 1), self.radius + 1):
            for y in range(-(self.radius + 1), self.radius + 1):
                if x ** 2 + y ** 2 == self.radius ** 2:
                    points_on_circle.append([x, y])

        points_on_circle.sort(key=lambda c: np.arctan2(c[0], c[1]))

        for i in range(len(points_on_circle)):
            a_point = points_on_circle[i - 1]
            b_point = points_on_circle[i]
            bc_line = 20
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
            qp.fillPath(path, QtGui.QBrush(QtGui.QColor(0, 150, 0)))

        qp.drawEllipse(-self.radius, -self.radius, self.radius * 2, self.radius * 2)
        qp.setBrush(self.palette().color(1))
        qp.drawPie(
            int(self.radius * -2.5 / 4),
            int(self.radius * -2.5 / 4),
            self.radius, self.radius,
            90 * 16, 90 * 16)
        qp.drawPie(
            int(self.radius * -1.5 / 4),
            int(self.radius * -2.5 / 4),
            self.radius, self.radius,
            0, 90 * 16)
        qp.drawPie(
            int(self.radius * -1.5 / 4),
            int(self.radius * -1.5 / 4),
            self.radius, self.radius,
            270 * 16, 90 * 16)
        qp.drawPie(
            int(self.radius * -2.5 / 4),
            int(self.radius * -1.5 / 4),
            self.radius, self.radius,
            180 * 16, 90 * 16)

        qp.setBrush(QtGui.QColor(0, 150, 0))
        qp.drawEllipse(self.radius // -4, self.radius // -4, self.radius // 2, self.radius // 2)

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
