D:\Apps\ImageMagick\magick.exe blue-raccoon.png -trim +repage logo-tight.png
D:\Apps\ImageMagick\magick.exe logo-tight.png -resize 256x256 logo-tight-256.png
D:\Apps\ImageMagick\magick.exe logo-tight-256.png -define icon:auto-resize=16,32,48,64,128,256 logo.ico
D:\Apps\ImageMagick\magick.exe logo-tight-256.png -define icon:auto-resize=16,32,48 favicon.ico
