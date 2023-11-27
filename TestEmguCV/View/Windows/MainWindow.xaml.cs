using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Windows;

namespace TestEmguCV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Mat image;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                image = CvInvoke.Imread(imagePath, ImreadModes.Color);
                ProcessImage();
            }
        }

        private void ProcessImage()
        {
            if (image != null)
            {
                // Применяем фильтры или методы обработки изображений для выделения пружины
                Mat grayImage = new Mat();
                CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);

                Mat edges = new Mat();
                CvInvoke.Canny(grayImage, edges, 50, 150);

                // Находим контуры
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

                if (contours.Size > 0)
                {
                    // Используем внешний контур
                    VectorOfPoint externalContour = contours[0];

                    // Пример аппроксимации контура
                    double epsilon = 0.02 * CvInvoke.ArcLength(externalContour, true);
                    VectorOfPoint approxContour = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(externalContour, approxContour, epsilon, true);

                    // Вычисляем параметры пружины по контуру
                    var boundingBox = CvInvoke.BoundingRectangle(approxContour);

                    double height = boundingBox.Height;
                    double width = boundingBox.Width;
                    double innerDiameter = boundingBox.Width; // Пример, замените на ваш расчет
                    double outerDiameter = boundingBox.Width + boundingBox.Height; // Пример, замените на ваш расчет

                    // Выводим результаты
                    heightText.Text = height.ToString("F2") + " мм";
                    widthText.Text = width.ToString("F2") + " мм";
                    innerDiameterText.Text = innerDiameter.ToString("F2") + " мм";
                    outerDiameterText.Text = outerDiameter.ToString("F2") + " мм";

                    // Отображаем изображение с аппроксимированным контуром
                    CvInvoke.DrawContours(image, new VectorOfVectorOfPoint(approxContour), -1, new MCvScalar(0, 255, 0), 2);
                    imageView.Source = image.ToImage<Bgr, byte>().ToBitmapSource();
                }
            }
        }
    }
}
