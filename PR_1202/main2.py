import sys
from PyQt5 import QtWidgets
from PyQt5 import QtGui
from PyQt5 import Qt
from PyQt5 import QtCore


class CircleAnimate(QtWidgets.QMainWindow):
    def __init__(self):
        super().__init__()
        self.resize(300, 300)
        self.qp = QtGui.QPainter()
        self.x = int(self.height() / 2)
        self.y = int(self.height() / 2)
        self.r = 1
        self.timer = QtCore.QTimer()
        self.timer.timeout.connect(self.update_values)
        self.timer.start(33)

    def paintEvent(self, event):
        qp = QtGui.QPainter()
        brush = QtGui.QBrush()
        brush.setStyle(QtCore.Qt.SolidPattern)
        brush.setColor(QtGui.QColor(,0,0,0))
        qp.setBrush(brush)
        qp.begin(self)
        # qp.rotate(self.r)
        # p.setBrush(QtGui.QColor(200, 0, 0))
        # qp.drawEllipse(self.x, self.y, 50, 50)
        # qp.drawLine(self.x, self.y, self.x + 10, self.y + 10)
        qp.drawPie(self.x, self.y, 50, 50, 100, 1000)
        qp.end()

    def update_values(self):
        self.r *= -1
        self.update()

    def draw_meme(self, x, y):
        qp = QtGui.QPainter()
        qp.begin()
        qp.drawPoint(x, y)
        # qp.drawPie(x, y, )
        qp.end()


if __name__ == '__main__':
    app = QtWidgets.QApplication(sys.argv)
    w = CircleAnimate()
    w.show()
    app.exec()
