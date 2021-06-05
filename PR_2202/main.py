import sys
from PyQt5 import QtWidgets
from PyQt5 import QtGui
from PyQt5 import QtCore
import numpy as np


class CircleAnimate(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle('Килюп Л.А. ИКБО-08-18')
        
        self.resize(510, 480)
        self.qp = QtGui.QPainter()
        self.rotate = 0

        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.update_values)
        self.timer.start(33)

    def paintEvent(self, event):
        self.draw_notation(x_center=255, y_center=240, radius=200,
                           angle=6, radius_point=5,
                           r_color=150, g_color=150, b_color=150)
        self.draw_notation(x_center=255, y_center=240, radius=200,
                           angle=30, radius_point=10,
                           r_color=100, g_color=100, b_color=100)

        self.draw_gear(x_center=255, y_center=240, radius=60, coefficient=60,
                       n_teeth=15, to_left=True,
                       r_color=66, g_color=133, b_color=244)
        self.draw_gear(x_center=255, y_center=240, radius=40, coefficient=7.5,
                       n_teeth=15, to_left=True,
                       r_color=251, g_color=188, b_color=5)
        self.draw_gear(x_center=255, y_center=240, radius=20, coefficient=0.25,
                       n_teeth=15, to_left=True,
                       r_color=234, g_color=67, b_color=53)

        self.draw_gear(x_center=320, y_center=255, radius=40, coefficient=7.5,
                       n_teeth=15, to_left=False,
                       r_color=52, g_color=168, b_color=83)
        self.draw_gear(x_center=382, y_center=282, radius=40, coefficient=7.5,
                       n_teeth=15, to_left=True,
                       r_color=234, g_color=67, b_color=53)

        self.draw_glass(x_center=255, y_center=240, radius=420,
                        r_color=200, b_color=200, g_color=200, a_color=50)

        self.draw_arrow(x_center=255, y_center=240, radius=3600, coefficient=1,
                        width=12, height=100,
                        r_color=200, g_color=200, b_color=200)
        self.draw_arrow(x_center=255, y_center=240, radius=300, coefficient=1,
                        width=6, height=150,
                        r_color=150, g_color=150, b_color=150)
        self.draw_arrow(x_center=255, y_center=240, radius=5, coefficient=1,
                        width=5, height=200,
                        r_color=100, g_color=100, b_color=100)

    def draw_gear(self, x_center, y_center, radius, coefficient, n_teeth, to_left, r_color, g_color, b_color):
        triangle_points = []
        for i in range(n_teeth):
            triangle_points.append([radius * 3 / 4 * np.cos(i * 2 * np.pi / n_teeth),
                                    radius * 3 / 4 * np.sin(i * 2 * np.pi / n_teeth)]),
            triangle_points.append([radius * np.cos(i * 2 * np.pi / n_teeth + np.pi / n_teeth),
                                    radius * np.sin(i * 2 * np.pi / n_teeth + np.pi / n_teeth)]),
        triangle_points.sort(key=lambda c: np.arctan2(c[0], c[1]), reverse=to_left)

        transform = QtGui.QTransform(1, 0, 0, 1, x_center, y_center)
        transform.rotate(100 / (radius * coefficient) * self.rotate * (1 if to_left else -1))
        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)

        path = QtGui.QPainterPath()
        path.clear()
        for i in triangle_points:
            path.lineTo(i[0], i[1])
        path.lineTo(triangle_points[0][0], triangle_points[0][1])
        path.addEllipse(radius * -0.6, radius * -0.6, radius * 1.2, radius * 1.2)
        qp.fillPath(path, QtGui.QBrush(QtGui.QColor(r_color, g_color, b_color)))

        path.clear()
        path.setFillRule(QtCore.Qt.WindingFill)
        path.addRect(radius / -8, radius / -1.6, radius / 4, radius * 1.3)
        path.addRect(radius / -1.6, radius / -8, radius * 1.3, radius / 4)
        path.addEllipse(radius / -4, radius / -4, radius / 2, radius / 2)
        qp.fillPath(path, QtGui.QBrush(QtGui.QColor(r_color, g_color, b_color)))

    def draw_arrow(self, x_center, y_center, radius, coefficient, width, height, r_color, b_color, g_color):
        transform = QtGui.QTransform(1, 0, 0, 1, x_center, y_center)
        transform.rotate((100 / (radius * coefficient)) * self.rotate)

        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(r_color, b_color, g_color))
        qp.drawRect(width / -2, 0, width, -height)
        qp.end()

    def draw_notation(self, x_center, y_center, radius, angle, radius_point, r_color, b_color, g_color):
        transform = QtGui.QTransform(1, 0, 0, 1, x_center, y_center)
        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(r_color, b_color, g_color))
        for alpha in range(0, 360, angle):
            x = radius * np.cos(np.radians(alpha) - np.pi / 2)
            y = radius * np.sin(np.radians(alpha) - np.pi / 2)
            qp.drawEllipse(x - radius_point / 2, y - radius_point / 2, radius_point, radius_point)

    def draw_glass(self, x_center, y_center, radius, r_color, b_color, g_color, a_color):
        transform = QtGui.QTransform(1, 0, 0, 1, x_center, y_center)
        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)
        qp.setPen(QtGui.QColor(0, 0, 0, 0))
        qp.setBrush(QtGui.QColor(r_color, b_color, g_color, a_color))
        qp.drawEllipse(radius / -2, radius / -2, radius, radius)

    def update_values(self):
        self.rotate += 1
        self.update()


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)
    w = CircleAnimate()
    w.show()
    app.exec()
