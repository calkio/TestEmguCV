using System.Windows;
using Emgu;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System.Windows.Media.Imaging;
using System.IO;
using SixLabors.ImageSharp;
using Point = SixLabors.ImageSharp.Point;
using System;

namespace TestEmguCV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image<Bgr, byte> inputImage = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                inputImage = new Image<Bgr,byte>(openFileDialog.FileName);
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            // Преобразование изображения в оттенки серого
            Image<Gray, byte> grayImage = inputImage.Convert<Gray, byte>();

            // Применение адаптивной пороговой обработки
            CvInvoke.AdaptiveThreshold(grayImage, grayImage, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary, 11, 2);

            // Морфологическая операция эрозии для уменьшения шума
            CvInvoke.Erode(grayImage, grayImage, null, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0));

            // Морфологическая операция дилатации для восстановления контуров
            CvInvoke.Dilate(grayImage, grayImage, null, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0));

            // Поиск контуров
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(grayImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Tree, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            // Рисование контуров на исходном цветном изображении
            CvInvoke.DrawContours(inputImage, contours, -1, new MCvScalar(200, 200, 200), 1);

            // Создание диалогового окна для сохранения результата
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            // Сохранение результата при выборе места сохранения
            if (saveFileDialog.ShowDialog() == true)
            {
                grayImage.Save(saveFileDialog.FileName);
            }
        }
    }
}
