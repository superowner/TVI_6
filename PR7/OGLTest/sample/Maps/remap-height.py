import Image

imageName = 'rockwall_relief.tga'
image     = Image.open ( imageName )
layers    = image.split ()

height    = layers [3].point ( lambda v: 255 - v )

image = Image.merge ( image.mode, (layers [0], layers [1], layers [2], height ) )

image.save ( imageName + '.png', 'PNG' )