#if !(UNITY_LUMIN && !UNITY_EDITOR)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ObjdetectModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.ImgprocModule;

namespace WebcamFaceDetection
{
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class WebcamFaceDetection : MonoBehaviour
    {
        ///<summary>
        /// The gray mat.
        /// </summary>
        Mat grayMat;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The webcam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The cascade.
        /// </summary>
        CascadeClassifier cascade;

        /// <summary>
        /// LBP_CASCADE_FILENAME
        /// </summary>
        protected static readonly string LBP_CASCADE_FILENAME = "lbpcascade_frontalface_improved.xml";

        /// <summary>
        /// HAAR_CASCADE_FILENAME
        /// </summary>
        protected static readonly string HAAR_CASCADE_FILENAME = "haarcascade_frontalface_alt.xml";

        /// <summary>
        /// The faces.
        /// </summary>
        MatOfRect faces;


        // Use this for initalization
        void Start()
        {
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();

            // Cascade
            cascade = new CascadeClassifier();
            cascade.load(Utils.getFilePath(LBP_CASCADE_FILENAME));
            //cascade.load(Utils.getFilePath(HAAR_CASCADE_FILENAME));
            if (cascade.empty()) {
                Debug.LogError("ascade file is not loaded.");
            }

            webCamTextureToMatHelper.Initialize();
        }

        /// <summary>
        /// Raises the web cam texture to mat helper initalized event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInitalized ()
        { 
            Debug.Log("OnWebCamTexturetoMatHelperInitalized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3 (webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale) {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else {
                Camera.main.orthographicSize = height / 2;
            }

            grayMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);

            faces = new MatOfRect();


        }

        /// <summary>
        /// Raises the webcam texture to mat helper disposed event.
        /// </summary>
        
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            if (grayMat != null)
                grayMat.Dispose();

            if (texture != null) {
                Texture2D.Destroy(texture);
                texture = null;
            }

            if (faces != null)
                faces.Dispose();
        }

        /// <summary>
        /// Raise the web cam texture to mat helper error occured event. 
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTexturetoMatHelperErrorOccured(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccured " + errorCode);
        }

        // Update is called once per frame
        void Update()
        {

            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame()){
                Mat rgbaMat = webCamTextureToMatHelper.GetMat();

                Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);
                Imgproc.equalizeHist(grayMat, grayMat);


                if (cascade != null)
                    cascade.detectMultiScale(grayMat, faces, 1.1, 2, 2,
                        new Size(grayMat.cols() * 0.2, grayMat.rows() * 0.2), new Size());

                OpenCVForUnity.CoreModule.Rect[] rects = faces.toArray();
                for (int i = 0; i < rects.Length; i++) {
                    // Debug.Log ("Detect faces " + rect [i]);

                    Imgproc.rectangle(rgbaMat, new Point(rects[i].x, rects[i].y), new Point(rects[i].x + rects[i].width, rects[i].y + rects[i].height), new Scalar(255, 0, 0, 255), 2);
                }

                Utils.fastMatToTexture2D(rgbaMat, texture);
            }
        }

        /// <summary>
        /// Raise the destroy event.
        /// </summary>
        void OnDestroy()
        {
            webCamTextureToMatHelper.Dispose();

            if (cascade != null)
                cascade.Dispose();
        }
    }
}
#endif