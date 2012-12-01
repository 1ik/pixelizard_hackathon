using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using System.Threading;
using System.IO;
using Microsoft.Speech.AudioFormat;
using System.Diagnostics;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace KinectPowerPointControl
{
    public partial class MainWindow : Window
    {

        #region Varibales
        KinectSensor sensor;
        SpeechRecognitionEngine speechRecognizer;
        private Stopwatch stopwatch;
        private int lastElaspedTime;

        // Timer Code
        protected System.Timers.Timer timer;

        Double score;
        // End Timer Code
        DispatcherTimer readyTimer;

        byte[] colorBytes;
        Skeleton[] skeletons;
        
        bool isCirclesVisible = true;

        bool isForwardGestureActive = false;
        bool isBackGestureActive = false;
        bool isLeftLegKicking = false;
        SolidColorBrush activeBrush = new SolidColorBrush(Colors.Green);
        SolidColorBrush inactiveBrush = new SolidColorBrush(Colors.Red);
        #endregion

        #region Initializarion Code
        public MainWindow()
        {
            InitializeComponent();

            //Runtime initialization is handled when the window is opened. When the window
            //is closed, the runtime MUST be unitialized.
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            //Handle the content obtained from the video camera, once received.

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            timer = new System.Timers.Timer();

            initialSetting();
            dc = new DataConnection();
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.KinectSensors.FirstOrDefault();

            if (sensor == null)
            {
                MessageBox.Show("This application requires a Kinect sensor.");
                this.Close();
            }
            
            sensor.Start();

            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);

            sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            sensor.ElevationAngle = 0;

            Application.Current.Exit += new ExitEventHandler(Current_Exit);

            InitializeSpeechRecognition();
        }

        
        
        void Current_Exit(object sender, ExitEventArgs e)
        {
            if (speechRecognizer != null)
            {
                speechRecognizer.RecognizeAsyncCancel();
                speechRecognizer.RecognizeAsyncStop();
            }
            if (sensor != null)
            {
                sensor.AudioSource.Stop();
                sensor.Stop();
                sensor.Dispose();
                sensor = null;
            }
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                ToggleCircles();
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenColorImageFrame())
            {
                if (image == null)
                    return;

                if (colorBytes == null ||
                    colorBytes.Length != image.PixelDataLength)
                {
                    colorBytes = new byte[image.PixelDataLength];
                }

                image.CopyPixelDataTo(colorBytes);

                //You could use PixelFormats.Bgr32 below to ignore the alpha,
                //or if you need to set the alpha you would loop through the bytes 
                //as in this loop below
                int length = colorBytes.Length;
                for (int i = 0; i < length; i += 4)
                {
                    colorBytes[i + 3] = 255;
                }

                BitmapSource source = BitmapSource.Create(image.Width,
                    image.Height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    colorBytes,
                    image.Width * image.BytesPerPixel);
                videoImage.Source = source;
            }
        }


        // Wash room elements
        void initializeWashRoomElements() 
        {
            
        }

        void initialSetting() 
        {
            score = 10;
            hideDoor();
            hideSandal();
            showBasin();
            showChappal();
            showToilet();
            showTissue();
            showSoap();
            showFlush();
            showRectStart();
            hideMainToilet();
            hideBasin();
            hideMainTissue();
            hideMainBasin();
            hideExitDoor();

            firstTimeOnDoorSandalLess = true;
            firstTimeOnDoorSandalLessDoubleZ = 0;
            firstTimePush = true;
            globalHandZ = 0;
            globalPush = false;
            doorVisible = true;
            usedWashRoom = false;
            usedTisssue = false;
            usedbasinMain = false;
            gamePlayMode = false;
            hideSandalBoolean = false;

            elipseToilet.Visibility = Visibility.Collapsed;
            ellipseBasin.Visibility = Visibility.Collapsed;
            ellipseTissue.Visibility = Visibility.Collapsed;
        }

        void gameStartSetting() 
        {
            score = 10;
            hideRectStart();
            hideBasin();
            hideChappal();
            hideToilet();
            hideSoap();
            hideFlush();
            hideTissue();
            showDoor();
            showSandal();
            hideMainToilet();
            gamePlayMode = true;


        }

        void hideExitDoor() 
        {
            imageExitDoor.Visibility = Visibility.Collapsed;
            rectExitDoor.Visibility = Visibility.Collapsed;
        }
        void showExitDoor()
        {
            imageExitDoor.Visibility = Visibility.Visible;
            rectExitDoor.Visibility = Visibility.Visible;
        }

        void showMainBasin()
        {
            imageBasinMain.Visibility = Visibility.Visible;
            rectBasinMain.Visibility = Visibility.Visible;
        }
        void hideMainBasin()
        {
            imageBasinMain.Visibility = Visibility.Collapsed;
            rectBasinMain.Visibility = Visibility.Collapsed;
        }

        
       
        void showMainTissue() 
        {
            imageTissueMain.Visibility = Visibility.Visible;
            rectTissueMain.Visibility = Visibility.Visible;
        }
        void hideMainTissue()
        {
            imageTissueMain.Visibility = Visibility.Collapsed;
            rectTissueMain.Visibility = Visibility.Collapsed;
        }
        void showMainToilet() 
        {
            imageMainToilet.Visibility = Visibility.Visible;
            rectToiletMain.Visibility = Visibility.Visible;
        }
        void hideMainToilet()
        {
            imageMainToilet.Visibility = Visibility.Collapsed;
            rectToiletMain.Visibility = Visibility.Collapsed;
        }
        void showSandal() 
        {
            rectSandal.Visibility = Visibility.Visible;
            imageSandalLeft.Visibility = Visibility.Visible;
            imageSandalRight.Visibility = Visibility.Visible;
        }
        void hideSandal()
        {
            rectSandal.Visibility = Visibility.Collapsed;
            imageSandalLeft.Visibility = Visibility.Collapsed;
            imageSandalRight.Visibility = Visibility.Collapsed;
        }
        void hideRectStart() 
        {
            rectStart.Visibility = Visibility.Collapsed;
            labelStart.Visibility = Visibility.Collapsed;
        }
        void showRectStart() 
        {
            rectStart.Visibility = Visibility.Visible;
            labelStart.Visibility = Visibility.Visible;
        }

        void hideDoor() 
        {
            recDoor.Visibility = Visibility.Hidden;
            imageDoor.Visibility = Visibility.Hidden;
        }
        void showDoor() 
        {
            recDoor.Visibility = Visibility.Visible;
            imageDoor.Visibility = Visibility.Visible;
        }

        void hideBasin() 
        {
            rectBasin.Visibility = Visibility.Collapsed;
            imageBasin.Visibility = Visibility.Collapsed;
            imageBasinDetails.Visibility = Visibility.Collapsed;
        }
        void showBasin() 
        {
            rectBasin.Visibility = Visibility.Visible;
            imageBasin.Visibility = Visibility.Visible;
            imageBasinDetails.Visibility = Visibility.Visible;
        }

        void hideChappal() 
        {
            rectChappal.Visibility = Visibility.Collapsed;
            imageChappal.Visibility = Visibility.Collapsed;
            imageChappalDetails.Visibility = Visibility.Collapsed;
        }
        void showChappal()
        {
            rectChappal.Visibility = Visibility.Visible;
            imageChappal.Visibility = Visibility.Visible;
            imageChappalDetails.Visibility = Visibility.Visible;
        }

        void showToilet()
        {
            rectToilet.Visibility = Visibility.Visible;
            imageToilet.Visibility = Visibility.Visible;
            imageToiletDetails.Visibility = Visibility.Visible;
        }

        void hideToilet()
        {
            rectToilet.Visibility = Visibility.Collapsed;
            imageToilet.Visibility = Visibility.Collapsed;
            imageToiletDetails.Visibility = Visibility.Collapsed;
        }


        void hideSoap() 
        {
            rectSoap.Visibility = Visibility.Collapsed;
            imageSoap.Visibility = Visibility.Collapsed;
            imageSoapDetails.Visibility = Visibility.Collapsed;
        }
        void showSoap()
        {
            rectSoap.Visibility = Visibility.Visible;
            imageSoap.Visibility = Visibility.Visible;
            imageSoapDetails.Visibility = Visibility.Visible;
        }

        void hideFlush()
        {
            rectFlush.Visibility = Visibility.Collapsed;
            imageFlush.Visibility = Visibility.Collapsed;
            imageFlushDetails.Visibility = Visibility.Collapsed;
        }
        void showFlush()
        {
            rectFlush.Visibility = Visibility.Visible;
            imageFlush.Visibility = Visibility.Visible;
            imageFlushDetails.Visibility = Visibility.Visible;
        }

        void showTissue()
        {
            rectTissue.Visibility = Visibility.Visible;
            imageTissue.Visibility = Visibility.Visible;
            imageTissueDetails.Visibility = Visibility.Visible;
        }
        void hideTissue()
        {
            rectTissue.Visibility = Visibility.Collapsed;
            imageTissue.Visibility = Visibility.Collapsed;
            imageTissueDetails.Visibility = Visibility.Collapsed;
        }

        // end wash room elements
        #endregion
        


        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                if (skeletons == null ||
                    skeletons.Length != skeletonFrame.SkeletonArrayLength)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(skeletons);

                Skeleton closestSkeleton = (from s in skeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked &&
                                                  s.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked
                                            select s).OrderBy(s => s.Joints[JointType.Head].Position.Z)
                                                    .FirstOrDefault();

                if (closestSkeleton == null)
                    return;

                var head = closestSkeleton.Joints[JointType.Head];
                var rightHand = closestSkeleton.Joints[JointType.HandRight];
                var leftHand = closestSkeleton.Joints[JointType.HandLeft];

                var leftLeg = closestSkeleton.Joints[JointType.FootLeft];

                
                
                if (head.TrackingState != JointTrackingState.Tracked ||
                    rightHand.TrackingState != JointTrackingState.Tracked ||
                    leftHand.TrackingState != JointTrackingState.Tracked ||
                    leftLeg.TrackingState!=JointTrackingState.Tracked)
                {
                    //Don't have a good read on the joints so we cannot process gestures
                    return;
                }

                SetEllipsePosition(ellipseHead, head, false);
                SetEllipsePosition(ellipseLeftHand, leftHand, isBackGestureActive);
                SetEllipsePosition(ellipseRightHand, rightHand, isForwardGestureActive);
                SetEllipsePosition(leftLegEllipse, leftLeg, isLeftLegKicking);

                ProcessForwardBackGesture(head, rightHand, leftHand,leftLeg);
                setImagePosition(leftHand,rightHand,head,leftLeg);

                labelScore.Content = score;
                
            }
        }
        bool gamePlayMode = false;
        
        void setImagePosition(Joint leftHand,Joint rightHand, Joint head,Joint leftLeg) 
        {
            var pointLeftHand = sensor.MapSkeletonPointToColor(leftHand.Position, sensor.ColorStream.Format);
            Canvas.SetLeft(imageLeftHand, pointLeftHand.X - imageLeftHand.ActualWidth / 2);
            Canvas.SetTop(imageLeftHand, pointLeftHand.Y - imageLeftHand.ActualHeight / 2);

            var pointRightHand = sensor.MapSkeletonPointToColor(rightHand.Position, sensor.ColorStream.Format);
            Canvas.SetLeft(imageRightHand, pointRightHand.X - imageLeftHand.ActualWidth / 2);
            Canvas.SetTop(imageRightHand, pointRightHand.Y - imageLeftHand.ActualHeight / 2);

            if (hovered(imageRightHand, rectStart))
            {
                if (!firstTouce) 
                {
                    firstTouce = true;
                    rightHandZ= rightHand.Position.Z;
                }
                
                if (rightHandZ - rightHand.Position.Z > .25)
                {
                    gamePlayMode=true;
                   
                    
                }
                else 
                {
                   
                }
            }
            else 
            {
                firstTouce = false;       
            }

            if (!gamePlayMode)
            {
                washRoomElementHoverEffectleftHand(rectBasin, imageBasinDetails);
                washRoomElementHoverEffectleftHand(rectChappal, imageChappalDetails);
                washRoomElementHoverEffectleftHand(rectToilet, imageToiletDetails);
                
                washRoomElementHoverEffectRightHand(rectSoap, imageSoapDetails);
                washRoomElementHoverEffectRightHand(rectFlush, imageFlushDetails);
                washRoomElementHoverEffectRightHand(rectTissue, imageTissueDetails);
            }
            else 
            {
                gameStartSetting();

                if (leftLeg.Position.X < head.Position.X - 0.30)
                {
                    hideSandalBoolean = true;
                    score+=100;
                }

                if (hideSandalBoolean) 
                {
                    hideSandal();
                }
                if (rectSandal.Visibility == Visibility.Visible)
                {
                    if (hovered(imageRightHand, recDoor))
                    {

                        /*labelOutput.Content = "Hovered";
                        if (firstTimeOnDoor)
                        {
                            firstTimeOnDoor = false;
                            rightHandOnDoorZ = rightHand.Position.Z;
                        }
                        else
                        {
                            if (rightHandOnDoorZ - rightHand.Position.Z > .25)
                            {
                                labelOutput.Content = "doom";
                                score -= 1;
                            }
                        }*/

                        //labelOutput.Content = rightHand.Position.X - leftHand.Position.X;
                        if (rightHand.Position.X > head.Position.X + .20)
                        {
                            if (!isBackGestureActive && !isForwardGestureActive)
                            {
                                isForwardGestureActive = true;
                                //System.Windows.Forms.SendKeys.SendWait("{HOME}");
                                //Scroll();
                                labelOutput.Content = "Wear Shoes";
                            }
                        }
                        else
                        {
                            isForwardGestureActive = false;
                        }
                          
                    }
                }
                else if (rectSandal.Visibility == Visibility.Collapsed)
                {
                    if (hovered(imageRightHand, recDoor)) 
                    {
                       /* if (firstTimeOnDoorSandalLess)
                        {
                            firstTimeOnDoorSandalLess = false;
                            firstTimeOnDoorSandalLessDoubleZ = rightHand.Position.Z;
                        }
                        else
                        {
                            if (firstTimeOnDoorSandalLessDoubleZ - rightHand.Position.Z > .25) 
                            {
                                doorVisible = false;
                                score += 10;
                            }
                        }  */
                        if (rightHand.Position.X > head.Position.X + 0.20)
                        {
                            if (!isBackGestureActive && !isForwardGestureActive)
                            {
                                isForwardGestureActive = true;
                                doorVisible = false;
                            }
                        }
                        else
                        {
                            isForwardGestureActive = false;
                        }
                    }

                    if (!doorVisible) 
                    {
                        hideDoor();

                        showMainToilet();
                        showMainBasin();
                        showMainTissue();
                        showExitDoor();

                        if (hovered(imageRightHand, rectToiletMain) && hovered(imageLeftHand, rectToiletMain))
                        {
                            usedWashRoom = true;
                            labelOutput.Content = "Wash room";
                            elipseToilet.Visibility = Visibility.Visible;

                        }
                        else 
                        {
                            elipseToilet.Visibility = Visibility.Collapsed;
                        }


                        if (hovered(imageRightHand, rectTissueMain) && hovered(imageLeftHand, rectTissueMain))
                        {
                            usedTisssue = true;
                            labelOutput.Content = "Tissue";
                            ellipseTissue.Visibility = Visibility.Visible;
                        }
                        else 
                        {
                            ellipseTissue.Visibility = Visibility.Collapsed;
                        }
                        if (hovered(imageRightHand, rectBasinMain) && hovered(imageLeftHand, rectBasinMain))
                        {
                            usedbasinMain = true;
                            labelOutput.Content = "Basin";
                            ellipseTissue.Visibility = Visibility.Visible;
                        }
                        else 
                        {
                            ellipseTissue.Visibility = Visibility.Collapsed;
                        }
                        labelOutput.Content = "Basin: " + usedbasinMain + " Tissue: " + usedTisssue + " Wash: " + usedWashRoom;
                        if (hovered(imageRightHand, rectExitDoor) && hovered(imageLeftHand, rectExitDoor))
                        {
                            if (usedWashRoom && usedbasinMain && usedTisssue)
                            {
                                labelOutput.Content = "Perfect";
                                score+= 100;
                                dc.updateScore(""+score, userid);
                                initialSetting();
                            }
                            else 
                            {
                                labelOutput.Content = "bad";
                                score -= 1;
                            }
                        }

                        

                        

                    }

                    
                }
                
                
            }

 
        }

        
        Boolean firstTimeOnDoorSandalLess = true;
        Double firstTimeOnDoorSandalLessDoubleZ = 0;
        Boolean firstTimePush = true;
        Double globalHandZ = 0;
        Boolean globalPush = false;
        Boolean doorVisible = true;
        Boolean usedWashRoom = false;
        Boolean usedTisssue = false;
        Boolean usedbasinMain = false;
        void pushed(Rectangle targetRectangle, Joint rightHand) 
        {
            if (firstTimePush)
            {
                firstTimePush = false;
                globalHandZ = rightHand.Position.Z;
                globalPush = false;
                return;
            }
            else 
            {
                if (globalHandZ - rightHand.Position.Z > .25) 
                {
                    globalPush = true;
                   
                }
            }
        
        }
        Boolean hideSandalBoolean = false;
        Double rightHandOnDoorZ = 0.00; 
        Boolean firstTimeOnDoor = true;

        
        #region HoverEffectRegion
        void washRoomElementHoverEffectleftHand(Rectangle washroomElementRect,Image imageDetail) 
        {
            if (hovered(imageLeftHand, washroomElementRect))
            {
                imageDetail.Visibility = Visibility.Visible;
            }
            else
            {
                imageDetail.Visibility = Visibility.Hidden;
            }
        }
        void washRoomElementHoverEffectRightHand(Rectangle washroomElementRect, Image imageDetail)
        {
            if (hovered(imageRightHand, washroomElementRect))
            {
                imageDetail.Visibility = Visibility.Visible;
            }
            else
            {
                imageDetail.Visibility = Visibility.Collapsed;
            }
        }
        #endregion


        double rightHandZ = 0;
        bool firstTouce = false;
        Boolean clicked(Joint leftHand, Joint rightHand) 
        {
            Boolean click = false;
            
            if (Math.Abs(leftHand.Position.Y - rightHand.Position.Y)<=.04) 
            {
                click = true;
            }
            return click;
        }
        

        Boolean hovered(Image handImage, Rectangle targetRectangle) 
        {

            Boolean touchedLeft = false;
            Boolean touchedTop = false;
            Double rightHandLeft = Canvas.GetLeft(handImage);
            Double rightHandTop = Canvas.GetTop(handImage);
            //  Double rightHandBottom = Canvas.GetBottom(image2);


            Double rec1Left = Canvas.GetLeft(targetRectangle);
            Double rec1Top = Canvas.GetTop(targetRectangle);
            //Double rec1Bottom = Canvas.GetBottom(rec1);

            if (rightHandLeft > rec1Left)
            {
                if (rightHandLeft <= rec1Left + targetRectangle.Width)
                {
                    touchedLeft = true;
                }
            }
            if (rec1Top <= rightHandTop)
            {
                if (rec1Top + targetRectangle.Height >= rightHandTop)
                {
                    touchedTop = true;
                }
            }
            

            if (touchedLeft && touchedTop)
            {

               // labelOutput.Content = "Touched";
                return true;
            }
            else 
            {

                //labelOutput.Content = "Not Touched";
                return false;
            }
        }
        
        
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public void Scroll()
        {
            // this will cause a vertical scroll
            mouse_event(0x0800, 0, 0, 500, 0);
        }

        private void ProcessForwardBackGesture(Joint head, Joint rightHand, Joint leftHand,Joint leftLeg)
        {
           /* Console.WriteLine(rightHand.Position.X + " - " + leftHand.Position.Y+" = "+(rightHand.Position.X-leftHand.Position.Y));
            if ((rightHand.Position.X - leftHand.Position.Y) < 0)
            {
                System.Windows.Forms.SendKeys.SendWait("{ADD}");
            }
            else 
            {
                System.Windows.Forms.SendKeys.SendWait("{SUBTRACT}");
            }*/


            /*
            if (rightHand.Position.X > head.Position.X + 0.45)
            {
                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isForwardGestureActive = true;
                    //System.Windows.Forms.SendKeys.SendWait("{HOME}");
                    //Scroll();
                }
            }
            else
            {
                isForwardGestureActive = false;
            }
            */
            /*
            if (leftHand.Position.X < head.Position.X - 0.45)
            {
                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isBackGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{Left}");
                    
                }
            }
            else
            {
                isBackGestureActive = false;
            }
            
            if (leftLeg.Position.X < head.Position.X - 0.45)
            {
                   isLeftLegKicking=true;
                   hideSandal();
            }
            else
            {
                    isLeftLegKicking = false;
            }
            */
            
            

        }

        //This method is used to position the ellipses on the canvas
        //according to correct movements of the tracked joints.
        private void SetEllipsePosition(Ellipse ellipse, Joint joint, bool isHighlighted)
        {
            var point = sensor.MapSkeletonPointToColor(joint.Position, sensor.ColorStream.Format);

            if (isHighlighted)
            {
                ellipse.Width = 60;
                ellipse.Height = 60;
                ellipse.Fill = activeBrush;
            }
            else
            {
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = inactiveBrush;
            }

            Canvas.SetLeft(ellipse, point.X - ellipse.ActualWidth / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.ActualHeight / 2);
        }

        void ToggleCircles()
        {
            if (isCirclesVisible)
                HideCircles();
            else
                ShowCircles();
        }
        
        void HideCircles()
        {
            isCirclesVisible = false;
            ellipseHead.Visibility = System.Windows.Visibility.Collapsed;
            ellipseLeftHand.Visibility = System.Windows.Visibility.Collapsed;
            ellipseRightHand.Visibility = System.Windows.Visibility.Collapsed;
        }

        void ShowCircles()
        {
            isCirclesVisible = true;
            ellipseHead.Visibility = System.Windows.Visibility.Visible;
            ellipseLeftHand.Visibility = System.Windows.Visibility.Visible;
            ellipseRightHand.Visibility = System.Windows.Visibility.Visible;
        }

        #region Speech Recognition Methods

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private void InitializeSpeechRecognition()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            try
            {
                speechRecognizer = new SpeechRecognitionEngine(ri.Id);
            }
            catch
            {
                MessageBox.Show(
                    @"There was a problem initializing Speech Recognition.
Ensure you have the Microsoft Speech SDK installed and configured.",
                    "Failed to load Speech SDK",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            var phrases = new Choices();
            phrases.Add("computer show window");
            phrases.Add("computer hide window");
            phrases.Add("computer show circles");
            phrases.Add("computer hide circles");

            var gb = new GrammarBuilder();
            //Specify the culture to match the recognizer in case we are running in a different culture.                                 
            gb.Culture = ri.Culture;
            gb.Append(phrases);

            // Create the actual Grammar instance, and then load it into the speech recognizer.
            var g = new Grammar(gb);

            speechRecognizer.LoadGrammar(g);
            speechRecognizer.SpeechRecognized += SreSpeechRecognized;
            speechRecognizer.SpeechHypothesized += SreSpeechHypothesized;
            speechRecognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

            this.readyTimer = new DispatcherTimer();
            this.readyTimer.Tick += this.ReadyTimerTick;
            this.readyTimer.Interval = new TimeSpan(0, 0, 4);
            this.readyTimer.Start();

        }

        private void ReadyTimerTick(object sender, EventArgs e)
        {
            this.StartSpeechRecognition();
            this.readyTimer.Stop();
            this.readyTimer = null;
        }

        private void StartSpeechRecognition()
        {
            if (sensor == null || speechRecognizer == null)
                return;

            var audioSource = this.sensor.AudioSource;
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            var kinectStream = audioSource.Start();
                
            speechRecognizer.SetInputToAudioStream(
                    kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            
        }

        void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Trace.WriteLine("\nSpeech Rejected, confidence: " + e.Result.Confidence);
        }

        void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Trace.Write("\rSpeech Hypothesized: \t{0}", e.Result.Text);
        }

        void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //This first release of the Kinect language pack doesn't have a reliable confidence model, so 
            //we don't use e.Result.Confidence here.
            if (e.Result.Confidence < 0.70)
            {
                Trace.WriteLine("\nSpeech Rejected filtered, confidence: " + e.Result.Confidence);
                return;
            }

            Trace.WriteLine("\nSpeech Recognized, confidence: " + e.Result.Confidence + ": \t{0}", e.Result.Text);

            if (e.Result.Text == "computer show window")
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                    {
                        this.Topmost = true;
                        this.WindowState = System.Windows.WindowState.Normal;
                    });
            }
            else if (e.Result.Text == "computer hide window")
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.Topmost = false;
                    this.WindowState = System.Windows.WindowState.Minimized;
                });
            }
            else if (e.Result.Text == "computer hide circles")
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.HideCircles();
                });
            }
            else if (e.Result.Text == "computer show circles")
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.ShowCircles();
                });
            }
        }
        
        #endregion
        String userid = "";
        DataConnection dc;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            


            dc = new DataConnection();

            if (dc.getPassword(textboxUname.Text)!=null) 
            {
                if (dc.getPassword(textboxUname.Text).Equals(textboxPass.Password))
                {
                    textboxPass.Visibility = Visibility.Hidden;
                    textboxUname.Visibility = Visibility.Hidden;
                    labelUsername.Visibility = Visibility.Hidden;
                    labelPassword.Visibility = Visibility.Hidden;

                    recLogin.Visibility = Visibility.Hidden;
                    butLogin.Visibility = Visibility.Hidden;

                    userid = textboxUname.Text;
                }
                else 
                {
                    MessageBox.Show("Password Mismatched " + textboxPass.Password + " " + dc.getPassword(textboxUname.Text));
                }
            }
            
        }

        private void imageExitDoor_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void imageSoap_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

    }
}
