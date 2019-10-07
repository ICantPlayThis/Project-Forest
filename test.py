import cv2

# load the cascade
face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

# capture video from webcam
cap = cv2.VideoCapture(0)

# To use a video file as input
# cap = cv2.VideoCapture('filename.mp4;)


run = 1
while run == 1:
    # read the frame
    _, img = cap.read()

    # convert to grayscale
    gray = cv2.cvtColor(img, cv2.Colour_BGR2GRAY)

    # Detect the faces
    faces = face_cascade.detectMultiScale(gray, 1.1, 4)

    # Draw the rectangle around each face
