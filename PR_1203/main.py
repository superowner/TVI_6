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

        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.update_values)
        self.timer.start(33)

    def paintEvent(self, event):
        class Vertex:
            def __init__(self, position, color, normal):
                self.position = position
                self.color = color
                self.normal = normal

        ludo = [
            [
                Vertex(position=QtGui.QVector4D(-1, -1, -1, 1), color=QtGui.QColor('orange'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, -1, 1), color=QtGui.QColor('orange'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, -1, 1), color=QtGui.QColor('orange'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(1, -1, -1, 1), color=QtGui.QColor('orange'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, -1, 1), color=QtGui.QColor('orange'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, -1, 1), color=QtGui.QColor('orange'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, -1, 1, 1), color=QtGui.QColor('violet'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, 1, 1), color=QtGui.QColor('violet'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, 1, 1), color=QtGui.QColor('violet'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(1, -1, 1, 1), color=QtGui.QColor('violet'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, 1, 1), color=QtGui.QColor('violet'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, 1, 1), color=QtGui.QColor('violet'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, -1, -1, 1), color=QtGui.QColor('blue'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, -1, 1, 1), color=QtGui.QColor('blue'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, -1, 1), color=QtGui.QColor('blue'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, -1, 1, 1), color=QtGui.QColor('blue'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, 1, 1), color=QtGui.QColor('blue'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, -1, 1), color=QtGui.QColor('blue'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(1, -1, -1, 1), color=QtGui.QColor('pink'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, 1, 1), color=QtGui.QColor('pink'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, -1, 1), color=QtGui.QColor('pink'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(1, -1, 1, 1), color=QtGui.QColor('pink'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, 1, 1), color=QtGui.QColor('pink'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, -1, 1), color=QtGui.QColor('pink'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, -1, -1, 1), color=QtGui.QColor('magenta'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, -1, 1, 1), color=QtGui.QColor('magenta'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, -1, 1), color=QtGui.QColor('magenta'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, -1, 1, 1), color=QtGui.QColor('magenta'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, 1, 1), color=QtGui.QColor('magenta'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, -1, -1, 1), color=QtGui.QColor('magenta'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, 1, -1, 1), color=QtGui.QColor('red'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(-1, 1, 1, 1), color=QtGui.QColor('red'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, -1, 1), color=QtGui.QColor('red'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
            [
                Vertex(position=QtGui.QVector4D(-1, 1, 1, 1), color=QtGui.QColor('red'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, 1, 1), color=QtGui.QColor('red'), normal=QtGui.QVector3D(0, 0, -1)),
                Vertex(position=QtGui.QVector4D(1, 1, -1, 1), color=QtGui.QColor('red'), normal=QtGui.QVector3D(0, 0, -1)),
            ],
        ]

    def draw(self, x_center, y_center):
        transform = QtGui.QTransform(1, 0, 0, 1, x_center, y_center)
        qp = QtGui.QPainter()
        qp.begin(self)
        qp.setRenderHint(QtGui.QPainter.Antialiasing)
        qp.setTransform(transform)

        path = QtGui.QPainterPath()

        # qp.fillPath(path, QtGui.QBrush(QtGui.QColor(r_color, g_color, b_color)))

    def update_values(self):
        self.rotate += 1
        self.update()


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)
    w = CircleAnimate()
    w.show()
    app.exec()
