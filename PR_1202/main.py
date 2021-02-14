from PyQt5 import QtWidgets, QtGui
import sys
import numpy as np


class ColorCircle(QtWidgets.QWidget):
    def __init__(self):
        super().__init__()
        self.h = 0

        self.radius_in = 80
        self.radius_out = 100

        alpha = 30
        a = self.radius_in * np.sin(np.deg2rad(alpha))
        b = self.radius_in * np.cos(np.deg2rad(alpha))
        self.triangle_a = (2 * (self.radius_in + a)) / np.sqrt(3)
        self.ax = self.radius_out + a
        self.ay = self.radius_out - b
        self.bx = self.radius_out - self.radius_in
        self.by = self.radius_out
        self.cx = self.radius_out + a
        self.cy = self.radius_out + b

        palette = QtGui.QPalette()
        palette.setColor(QtGui.QPalette.Background, QtGui.QColor('#4d4d4d'))
        self.setPalette(palette)
        self.setFixedSize(self.radius_out * 2, self.radius_out * 2)

    def paintEvent(self, event):
        super().paintEvent(event)
        self.paint_circle()
        self.paint_triangle(self.h)

    def mousePressEvent(self, event):
        super().mousePressEvent(event)
        x = event.x()
        y = event.y()
        s = np.sqrt(np.power(y - self.radius_out, 2) + np.power(x - self.radius_out, 2)) / self.radius_out
        if self.radius_out / 100 > s > self.radius_in / 100:
            self.h = (np.arctan2(x - self.radius_out, y - self.radius_out) + np.pi) / (2. * np.pi)
            self.update()

    def paint_circle(self):
        painter = QtGui.QPainter(self)
        for y in range(self.radius_out * 2):
            for x in range(self.radius_out * 2):
                s = np.sqrt(np.power(y - self.radius_out, 2) + np.power(x - self.radius_out, 2)) / self.radius_out
                if self.radius_out / 100 > s > self.radius_in / 100:
                    h = (np.arctan2(y - self.radius_out, x - self.radius_out) + np.pi) / (2. * np.pi)
                    color = QtGui.QColor(255, 255, 255, 255)
                    color.setHsvF(h, s, 1, 1)
                    painter.setPen(color)
                    painter.drawPoint(y, x)

    def paint_triangle(self, h):
        painter = QtGui.QPainter(self)
        for y in range(self.radius_out * 2):
            for x in range(self.radius_out * 2):
                bx = self.bx - self.ax
                by = self.by - self.ay
                cx = self.cx - self.ax
                cy = self.cy - self.ay
                px = x - self.ax
                py = y - self.ay
                m = (px * by - bx * py) / (cx * by - bx * cy)
                if 0 < m < 1:
                    i = (px - m * cx) / bx
                    if 0 < i and m + i < 1:
                        v = np.sqrt((self.ax - x) ** 2 + (self.ay - y) ** 2) / self.triangle_a
                        s = np.sqrt((self.cx - x) ** 2 + (self.cy - y) ** 2) / self.triangle_a
                        color = QtGui.QColor(255, 255, 255, 255)
                        color.setHsvF(h, s, v, 1)
                        painter.setPen(color)
                        painter.drawPoint(y, x)


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)
    w = ColorCircle()
    w.show()
    app.exec()
