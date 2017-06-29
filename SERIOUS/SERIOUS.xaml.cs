using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using DevExpress.Xpf.Core;
using Microsoft.Kinect;
using Label = System.Windows.Controls.Label;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Point = System.Windows.Point;
using Timer = System.Timers.Timer;


namespace Serious.IHM
{
    /// <summary>
    /// Interaction logic for SERIOUS.xaml
    /// </summary>
    public partial class SERIOUS : DXWindow
    {
        KinectSensor sensor;
        byte[] pixelData;
        Skeleton[] skeletons;
        StreamWriter swOutputFile = null;
        //swOutputFile = new StreamWriter(new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "Capture" + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString() + ".csv", FileMode.OpenOrCreate, FileAccess.ReadWrite));
        String tempWriter = "Capture" + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString() +
                            "_Training.csv";

        string reco;
        int i = 0;
        bool ready = false;
        bool reckon = false;
        double[] cpreco = new double[60];
        Serious.RF.RF oneRf = new RF.RF();

        public SERIOUS()
        {
            InitializeComponent();
            Joker.Source = new BitmapImage(new Uri("pack://application:,,,/SERIOUS;component/seriousface.png", UriKind.Absolute));
            try
            {
                sensor = KinectSensor.KinectSensors[0];
                BitmapImage co =
                    new BitmapImage(new Uri("pack://application:,,,/SERIOUS;component/connected.png", UriKind.Absolute));
                connectStatus.Source = co;
                kinectStatus.Content = "Kinect Connectée";
                sensor.ColorStream.Enable();
                sensor.SkeletonStream.Enable();
                sensor.SkeletonFrameReady += runtime_SkeletonFrameReady;
                sensor.ColorFrameReady += runtime_VideoFrameReady;
                sensor.Start();
            }
            catch (Exception)
            {
                BitmapImage notco =
                    new BitmapImage(new Uri("pack://application:,,,/SERIOUS;component/disconnected.png",
                        UriKind.Absolute));
                connectStatus.Source = notco;
                kinectStatus.Content = "Kinect Déconnectée";
            }
            KinectSensor.KinectSensors.StatusChanged += Kinect_StatusChanged;
        }

        private void Kinect_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                default:
                    try
                    {
                        sensor = KinectSensor.KinectSensors[0];
                        BitmapImage co =
                            new BitmapImage(new Uri("pack://application:,,,/SERIOUS;component/connected.png", UriKind.Absolute));
                        connectStatus.Source = co;
                        kinectStatus.Content = "Kinect Connectée";
                        sensor.ColorStream.Enable();
                        sensor.SkeletonStream.Enable();
                        sensor.SkeletonFrameReady += runtime_SkeletonFrameReady;
                        sensor.ColorFrameReady += runtime_VideoFrameReady;
                        sensor.Start();
                    }
                    catch(Exception)
                    {
                        BitmapImage notco = new BitmapImage(new Uri("pack://application:,,,/SERIOUS;component/disconnected.png", UriKind.Absolute));
                        connectStatus.Source = notco;
                        kinectStatus.Content = "Kinect Déconnectée";
                        sensor.Stop();
                    }
                    break;
            }
        }

        void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
            {
                if (SFrame == null)
                {
                }
                else
                {
                    skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(skeletons);
                    receivedData = true;
                }
            }

            if (receivedData)
            {

                Skeleton currentSkeleton = (from s in skeletons
                    where s.TrackingState == SkeletonTrackingState.Tracked
                    select s).FirstOrDefault();

                if (currentSkeleton != null)
                {
                    SetEllipsePosition(Head, currentSkeleton.Joints[JointType.Head]);
                    SetEllipsePosition(HipCenter, currentSkeleton.Joints[JointType.HipCenter]);
                    SetEllipsePosition(Spine, currentSkeleton.Joints[JointType.Spine]);
                    SetEllipsePosition(ShoulderCenter, currentSkeleton.Joints[JointType.ShoulderCenter]);
                    SetEllipsePosition(ShoulderLeft, currentSkeleton.Joints[JointType.ShoulderLeft]);
                    SetEllipsePosition(ElbowLeft, currentSkeleton.Joints[JointType.ElbowLeft]);
                    SetEllipsePosition(WristLeft, currentSkeleton.Joints[JointType.WristLeft]);
                    SetEllipsePosition(HandLeft, currentSkeleton.Joints[JointType.HandLeft]);
                    SetEllipsePosition(ShoulderRight, currentSkeleton.Joints[JointType.ShoulderRight]);
                    SetEllipsePosition(ElbowRight, currentSkeleton.Joints[JointType.ElbowRight]);
                    SetEllipsePosition(WristRight, currentSkeleton.Joints[JointType.WristRight]);
                    SetEllipsePosition(HandRight, currentSkeleton.Joints[JointType.HandRight]);
                    SetEllipsePosition(HipLeft, currentSkeleton.Joints[JointType.HipLeft]);
                    SetEllipsePosition(KneeLeft, currentSkeleton.Joints[JointType.KneeLeft]);
                    SetEllipsePosition(AnkleLeft, currentSkeleton.Joints[JointType.AnkleLeft]);
                    SetEllipsePosition(FootLeft, currentSkeleton.Joints[JointType.FootLeft]);
                    SetEllipsePosition(HipRight, currentSkeleton.Joints[JointType.HipRight]);
                    SetEllipsePosition(KneeRight, currentSkeleton.Joints[JointType.KneeRight]);
                    SetEllipsePosition(AnkleRight, currentSkeleton.Joints[JointType.AnkleRight]);
                    SetEllipsePosition(FootRight, currentSkeleton.Joints[JointType.FootRight]);
                    recognize(currentSkeleton);
                }

            }
        }

        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            SkeletonPoint skeletonPoint = joint.Position;
            Point point = new Point();
            ColorImagePoint colorPoint = sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint,
                ColorImageFormat.RgbResolution640x480Fps30);
            //Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            //vector.X = ScaleVector(640, joint.Position.X);
            //vector.Y = ScaleVector(480, -joint.Position.Y);
            point.X = colorPoint.X;
            point.Y = colorPoint.Y;

            //Joint updatedJoint = new Joint();
            //updatedJoint = joint;
            //updatedJoint.TrackingState = JointTrackingState.Tracked;
            //updatedJoint.Position = vector;

            Canvas.SetLeft(ellipse, point.X - ellipse.Width/2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height/2);
            exporterPoints(joint, ready);
        }

        private float ScaleVector(int length, float position)
        {
            float value = (((((float) length)/1f)/2f)*position) + (length/2);
            if (value > length)
            {
                return (float) length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            sensor.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor.SkeletonFrameReady += runtime_SkeletonFrameReady;
            sensor.ColorFrameReady += runtime_VideoFrameReady;
            sensor.Start();
        }

        void runtime_VideoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (ColorImageFrame CFrame = e.OpenColorImageFrame())
            {
                if (CFrame == null)
                {

                }
                else
                {
                    pixelData = new byte[CFrame.PixelDataLength];
                    CFrame.CopyPixelDataTo(pixelData);
                    receivedData = true;
                }
            }

            if (receivedData)
            {
                BitmapSource source = BitmapSource.Create(640, 480, 96, 96,
                    PixelFormats.Bgr32, null, pixelData, 640*4);
                videoImage.Source = source;
                DemoImage.Source = source;
            }
        }

        void exporterPoints(Joint j, bool rdy)
        {
            if (rdy == true)
            {
                string strComma = ";";
                if (j.JointType.ToString() == "Head")
                {
                    swOutputFile.WriteLine(" ");

                }
                swOutputFile.Write(j.Position.X + strComma + j.Position.Y + strComma +
                                       j.Position.Z + strComma+i+strComma);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ready == true)
            {
                i++;
                idposition.Content = "ID position : " + i;
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ready = true;
            //swOutputFile = new StreamWriter(new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "Capture" + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString() + ".csv", FileMode.OpenOrCreate, FileAccess.ReadWrite));
            try
            {
                swOutputFile = new StreamWriter(tempWriter,true);
            }
            catch (Exception)
            {
                tempWriter = "Capture" + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString() +
                             "_Training.csv";
                swOutputFile = new StreamWriter(tempWriter, false);
            }
            swOutputFile.Write("HeadX;HeadY;HeadZ;HipCenterX;HipCenterY;HipCenterZ;SpineX;SpineY;SpineZ;");
            swOutputFile.Write("ShoulderCenterX;ShoulderCenterY;ShoulderCenterZ;ShoulderLeftX;ShoulderLeftY;ShoulderLeftZ;");
            swOutputFile.Write("ElbowLeftX;ElbowLeftY;ElbowLeftZ;WristLeftX;WristLeftY;WristLeftZ;HandLeftX;HandLeftY;HandLeftZ;");
            swOutputFile.Write("ShoulderRightX;ShoulderRightY;ShoulderRightZ;ElbowRightX;ElbowRightY;ElbowRightZ;");
            swOutputFile.Write("WristRightX;WristRightY;WristRightZ;HandRightX;HandRightY;HandRightZ;");
            swOutputFile.Write("HipLeftX;HipLeftY;HipLeftZ;KneeLeftX;KneeLeftY;KneeLeftZ;");
            swOutputFile.Write("AnkleLeftX;AnkleLeftY;AnkleLeftZ;FootLeftX;FootLeftY;FootLeftZ;");
            swOutputFile.Write("HipRightX;HipRightY;HipRightZ;KneeRightX;KneeRightY;KneeRightZ;AnkleRightX;AnkleRightY;AnkleRightZ;");
            swOutputFile.Write("FootRightX;FootRightY;FootRightZ;IDPOS");
        }


        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ready = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog therf = new OpenFileDialog();
            therf.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            if (therf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                reco = therf.FileName;
                recolabel.Content = "Jeu de données chargé : " + reco;
                recolabel.Foreground = Brushes.Blue;
            }
            else
            {
                recolabel.Content = "Jeu de données chargé : Aucun";
            }
        }

        private void startReco_Click(object sender, RoutedEventArgs e)
        {
            oneRf.load(reco);
            reckon = true;
            try
            {
                double res = oneRf.classify(cpreco)[0];
                setPositionLabel(res);
            }
            catch(Exception)
            {
                recolabel.Foreground=Brushes.Red;
            }
            


        }

        private void setPositionLabel(double res)
        {
            if ((res>1.5)&&(res < 1.7))
            {
                recoPosition.Content = "Bras Gauche levé :" + res;
            }
            if ((res >= 2) && (res <= 2.15))
            {
                recoPosition.Content = "Bras Droit levé :" + res;
            }
            if (3< res)
            {
                recoPosition.Content = "Bras droit Jambe gauche levés :" + res;
            }
        }

        /**while (reckon == true)
            {
                Timer t = new Timer(3000);
                t.AutoReset = true;
                t.Elapsed += new ElapsedEventHandler(timerout);
           }
            
        }

        private void timerout(object sender, ElapsedEventArgs e)
        {
            recoPosition.Content = oneRf.classify(cpreco)[0];
        }
        */


        private void recognize(Skeleton sk)
        {
            cpreco[0] = sk.Joints[JointType.Head].Position.X;
            cpreco[1] = sk.Joints[JointType.Head].Position.Y;
            cpreco[2] = sk.Joints[JointType.Head].Position.Z;
            cpreco[3] = sk.Joints[JointType.HipCenter].Position.X;
            cpreco[4] = sk.Joints[JointType.HipCenter].Position.Y;
            cpreco[5] = sk.Joints[JointType.HipCenter].Position.Z;
            cpreco[6] = sk.Joints[JointType.Spine].Position.X;
            cpreco[7] = sk.Joints[JointType.Spine].Position.Y;
            cpreco[8] = sk.Joints[JointType.Spine].Position.Z;
            cpreco[9] = sk.Joints[JointType.ShoulderCenter].Position.X;
            cpreco[10] = sk.Joints[JointType.ShoulderCenter].Position.Y;
            cpreco[11] = sk.Joints[JointType.ShoulderCenter].Position.Z;
            cpreco[12] = sk.Joints[JointType.ShoulderLeft].Position.X;
            cpreco[13] = sk.Joints[JointType.ShoulderLeft].Position.Y;
            cpreco[14] = sk.Joints[JointType.ShoulderLeft].Position.Z;
            cpreco[15] = sk.Joints[JointType.ElbowLeft].Position.X;
            cpreco[16] = sk.Joints[JointType.ElbowLeft].Position.Y;
            cpreco[17] = sk.Joints[JointType.ElbowLeft].Position.Z;
            cpreco[18] = sk.Joints[JointType.WristLeft].Position.X;
            cpreco[19] = sk.Joints[JointType.WristLeft].Position.Y;
            cpreco[20] = sk.Joints[JointType.WristLeft].Position.Z;
            cpreco[21] = sk.Joints[JointType.HandLeft].Position.X;
            cpreco[22] = sk.Joints[JointType.HandLeft].Position.Y;
            cpreco[23] = sk.Joints[JointType.HandLeft].Position.Z;
            cpreco[24] = sk.Joints[JointType.ShoulderRight].Position.X;
            cpreco[25] = sk.Joints[JointType.ShoulderRight].Position.Y;
            cpreco[26] = sk.Joints[JointType.ShoulderRight].Position.Z;
            cpreco[27] = sk.Joints[JointType.ElbowRight].Position.X;
            cpreco[28] = sk.Joints[JointType.ElbowRight].Position.Y;
            cpreco[29] = sk.Joints[JointType.ElbowRight].Position.Z;
            cpreco[30] = sk.Joints[JointType.WristRight].Position.X;
            cpreco[31] = sk.Joints[JointType.WristRight].Position.Y;
            cpreco[32] = sk.Joints[JointType.WristRight].Position.Z;
            cpreco[33] = sk.Joints[JointType.HandRight].Position.X;
            cpreco[34] = sk.Joints[JointType.HandRight].Position.Y;
            cpreco[35] = sk.Joints[JointType.HandRight].Position.Z;
            cpreco[36] = sk.Joints[JointType.HipLeft].Position.X;
            cpreco[37] = sk.Joints[JointType.HipLeft].Position.Y;
            cpreco[38] = sk.Joints[JointType.HipLeft].Position.Z;
            cpreco[39] = sk.Joints[JointType.KneeLeft].Position.X;
            cpreco[40] = sk.Joints[JointType.KneeLeft].Position.Y;
            cpreco[41] = sk.Joints[JointType.KneeLeft].Position.Z;
            cpreco[42] = sk.Joints[JointType.AnkleLeft].Position.X;
            cpreco[43] = sk.Joints[JointType.AnkleLeft].Position.Y;
            cpreco[44] = sk.Joints[JointType.AnkleLeft].Position.Z;
            cpreco[45] = sk.Joints[JointType.FootLeft].Position.X;
            cpreco[46] = sk.Joints[JointType.FootLeft].Position.Y;
            cpreco[47] = sk.Joints[JointType.FootLeft].Position.Z;
            cpreco[48] = sk.Joints[JointType.HipRight].Position.X;
            cpreco[49] = sk.Joints[JointType.HipRight].Position.Y;
            cpreco[50] = sk.Joints[JointType.HipRight].Position.Z;
            cpreco[51] = sk.Joints[JointType.KneeRight].Position.X;
            cpreco[52] = sk.Joints[JointType.KneeRight].Position.Y;
            cpreco[53] = sk.Joints[JointType.KneeRight].Position.Z;
            cpreco[54] = sk.Joints[JointType.AnkleRight].Position.X;
            cpreco[55] = sk.Joints[JointType.AnkleRight].Position.Y;
            cpreco[56] = sk.Joints[JointType.AnkleRight].Position.Z;
            cpreco[57] = sk.Joints[JointType.FootRight].Position.X;
            cpreco[58] = sk.Joints[JointType.FootRight].Position.Y;
            cpreco[59] = sk.Joints[JointType.FootRight].Position.Z;
        }

        private void stopReco_Click(object sender, RoutedEventArgs e)
        {
            reckon = false;
        }

/*
 private void DXTabControl_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            if (DemoTab.IsSelected == true)
            {
                DemoImage.Source=videoImage.Source;
                videoImage.Source = null;
            }
            else
            {
                if (DemoImage.Source != null)
                {
                    videoImage.Source=DemoImage.Source;
                    DemoImage.Source = null;
                }
            }
        }*/
        
    }
}
