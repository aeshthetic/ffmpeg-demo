using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using ExtensionMethods;

namespace FfmpegDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var (ffmpegPath, tmpPath) = InitializeApp();
            string videoPath = "";
            do {
                new OpenFileDialog()
                .To(it => {
                    if (it.ShowDialog() == true)
                        videoPath = it.FileName;
                    return new Unit();
                });
            } while (!File.Exists(videoPath));
            videoPath.Split('\\')
            .To(it => it[it.Length - 1])
            .Split('.')[0]
            .Replace(' ', '_')
            .To(it => GenerateThumbnail(ffmpegPath, videoPath, tmpPath + $"\\{it}.jpg"))
            .To(it => new Image { Source = it })
            .To(MainGrid.Children.Add);
        }

        (string, string) InitializeApp()
        {
            InstallFfmpeg();
            string ffmpegPath = @"C:\ffmpeg-3.4-win32-shared\bin\ffmpeg.exe";
            string tmpPath = Path.GetTempPath();
            Directory.CreateDirectory(tmpPath + @"\aeshTmp");
            return (ffmpegPath, tmpPath + @"\aeshTmp");
        }

        static BitmapImage LoadImage(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path));
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private static void InstallFfmpeg()
        {
            var zipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "contrib", "ffmpeg-3.4-win32-shared.zip");
            var extractPath = @"C:\";
            var ffmpegPath = @"C:\ffmpeg-3.4-win32-shared\bin\ffmpeg.exe";
            if (!File.Exists(ffmpegPath))
                ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        private static BitmapImage GenerateThumbnail(string ffmpegPath, string videoPath, string path)
        {
            if (!File.Exists(path))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C {ffmpegPath} -i \"{videoPath}\" -vframes 1 -an -s 400x222 -ss 30 \"{path}\""
                };
                System.Diagnostics.Process process = new System.Diagnostics.Process()
                {
                    StartInfo = startInfo
                };
                process.Start();
                process.WaitForExit();
            }
            return LoadImage(path);
        }
    }
}
