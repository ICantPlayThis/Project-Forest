import cv2
import time

# load the cascade
face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

# capture video from webcam
cap = cv2.VideoCapture(0)
print(cap)

run = 1
while run == 1:
    _, frame = cap.read()
    print(frame)

    # convert to grayscale
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    faces = face_cascade.detectMultiScale(gray, 1.1, 5, flags=cv2.cv.CV_HAAR_SCALE_IMAGE)

    # draw rectangle around faces
    for (x, y, w, h) in faces:
        cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 2)

    # display the frame
    cv2.imshow('Video', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    time.sleep(1)

# When finished, release capture
cap.release()
cv2.destroyAllWindows()




