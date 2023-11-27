using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.ObjectModel;

namespace Video
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCapture capture;
        private ObservableCollection<int> availableCameras;

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            // Заполняем ComboBox доступными камерами
            availableCameras = new ObservableCollection<int>();
            for (int i = 0; i < 10; i++) // Проверяем первые 10 камер
            {
                VideoCapture testCapture = new VideoCapture(i);
                if (testCapture.IsOpened)
                {
                    availableCameras.Add(i);
                    testCapture.Dispose(); // Освобождаем тестовый захват
                }
            }

            cameraComboBox.ItemsSource = availableCameras;
            if (availableCameras.Count > 0)
            {
                cameraComboBox.SelectedIndex = 0;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            ProcessFrame();
        }

        private void ProcessFrame()
        {
            if (capture != null && capture.IsOpened)
            {
                Mat frame = new Mat();
                Mat gray = new Mat();
                Mat edge = new Mat();

                capture.Read(frame);
                CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
                CvInvoke.Canny(gray, edge, 100, 200);

                // Ваш код обработки изображения...

                UpdateImage(videoImage, frame);
                UpdateImage(processedImage, edge);
            }
        }

        private void UpdateImage(Image imageControl, Mat mat)
        {
            BitmapSource bitmapSource = ToBitmapSource(mat);
            imageControl.Source = bitmapSource;
        }

        private BitmapSource ToBitmapSource(Mat mat)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                mat.ToImage<Bgr, byte>().ToBitmap().Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void CameraComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Обновляем VideoCapture при изменении выбранной камеры
            if (cameraComboBox.SelectedItem != null)
            {
                int selectedCamera = (int)cameraComboBox.SelectedItem;
                if (capture != null)
                {
                    capture.Dispose(); // Освобождаем предыдущий захват
                }
                capture = new VideoCapture(selectedCamera);
            }
        }
    }
}

