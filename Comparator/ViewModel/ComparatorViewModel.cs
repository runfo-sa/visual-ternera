using Comparator.View;
using Core;
using Core.Logic;
using Core.MVVM;
using ICSharpCode.AvalonEdit.Document;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Comparator.ViewModel
{
    public class ComparatorViewModel : ViewModelBase
    {
        private bool _textMode = true;

        public bool TextMode
        {
            get => _textMode;
            set
            {
                _textMode = value;
                OnPropertyChanged(nameof(TextMode));
            }
        }

        private Visibility _textModeVisibility = Visibility.Visible;

        public Visibility TextModeVisibility
        {
            get => _textModeVisibility;
            set
            {
                _textModeVisibility = value;
                OnPropertyChanged(nameof(TextModeVisibility));
            }
        }

        private bool _imageMode = false;

        public bool ImageMode
        {
            get => _imageMode;
            set
            {
                _imageMode = value;
                OnPropertyChanged(nameof(ImageMode));
            }
        }

        private Visibility _imageModeVisibility = Visibility.Hidden;

        public Visibility ImageModeVisibility
        {
            get => _imageModeVisibility;
            set
            {
                _imageModeVisibility = value;
                OnPropertyChanged(nameof(ImageModeVisibility));
            }
        }

        private TextDocument _firstLabelText = null!;

        public TextDocument FirstLabelText
        {
            get => _firstLabelText;
            set
            {
                _firstLabelText = value;
                OnPropertyChanged(nameof(FirstLabelText));
            }
        }

        private TextDocument _secondLabelText = null!;

        public TextDocument SecondLabelText
        {
            get => _secondLabelText;
            set
            {
                _secondLabelText = value;
                OnPropertyChanged(nameof(SecondLabelText));
            }
        }

        private string _firstFilename = null!;

        public string FirstFilename
        {
            get => _firstFilename;
            set
            {
                _firstFilename = value;
                OnPropertyChanged(nameof(FirstFilename));
            }
        }

        private string _secondFilename = null!;

        public string SecondFilename
        {
            get => _secondFilename;
            set
            {
                _secondFilename = value;
                OnPropertyChanged(nameof(SecondFilename));
            }
        }

        private BitmapSource _leftImage;

        public BitmapSource LeftImage
        {
            get => _leftImage;
            set
            {
                _leftImage = value;
                OnPropertyChanged(nameof(LeftImage));
            }
        }

        private BitmapSource _rightImage;

        public BitmapSource RightImage
        {
            get => _rightImage;
            set
            {
                _rightImage = value;
                OnPropertyChanged(nameof(RightImage));
            }
        }

        private BitmapSource _centerImage;

        public BitmapSource CenterImage
        {
            get => _centerImage;
            set
            {
                _centerImage = value;
                OnPropertyChanged(nameof(CenterImage));
            }
        }

        public RelayCommand SwitchToImageMode => new(_ =>
        {
            TextMode = false;
            TextModeVisibility = Visibility.Hidden;
            ImageMode = true;
            ImageModeVisibility = Visibility.Visible;
        });

        public RelayCommand SwitchToTextMode => new(_ =>
        {
            ImageMode = false;
            ImageModeVisibility = Visibility.Hidden;
            TextMode = true;
            TextModeVisibility = Visibility.Visible;
        });

        public RelayCommand AddComparation => new(_ => OpneComparation());

        private string _leftText = string.Empty;
        private string _rightText = string.Empty;

        public ComparatorViewModel(Settings settings) : base(settings)
        {
            OpneComparation();
        }

        private void OpneComparation()
        {
            SelectLabelsDialog dialog = new(Settings.EtiquetasDir);
            if (dialog.ShowDialog() == true)
            {
                FirstLabelText = new TextDocument(File.ReadAllText(dialog.FirstLabel.Path));
                SecondLabelText = new TextDocument(File.ReadAllText(dialog.SecondLabel.Path));
                FirstFilename = dialog.FirstLabel.Name;
                SecondFilename = dialog.SecondLabel.Name;
                _leftText = FirstLabelText.Text;
                _rightText = SecondLabelText.Text;

                var timer = new System.Timers.Timer(500) { AutoReset = false };
                timer.Elapsed += LoadImages;
                timer.Start();
            }
            else
            {
                FirstLabelText = new TextDocument($"^XA\n\n^XZ");
                SecondLabelText = new TextDocument($"^XA\n\n^XZ");
            }
        }

        private BitmapSource? GenerateImage(string content)
        {
            var labelary = new Labelary(content)
                .FillVariables(Settings)
                .LoadFonts();
            var task = Task.Run(() => labelary.Post("12", "4x6"));
            task.Wait();
            var bytes = task.Result;
            if (bytes is not null)
            {
                using MemoryStream stream = new(bytes);
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            return null;
        }

        private void LoadImages(object? sender, ElapsedEventArgs e)
        {
            var limg = GenerateImage(_leftText);
            if (limg is not null)
            {
                LeftImage = limg;
            }

            var rimg = GenerateImage(_rightText);
            if (rimg is not null)
            {
                RightImage = rimg;
            }

            var firstLines = _leftText.Split(Environment.NewLine);
            var secondLines = _rightText.Split(Environment.NewLine);
            StringBuilder centerContent = new("^XA");

            int i = 0, j = 0;
            for (; i < firstLines.Length && j < secondLines.Length; i++, j++)
            {
                if (firstLines[i] != secondLines[j])
                {
                    centerContent.AppendLine(secondLines[j]);
                }
            }

            for (; j < secondLines.Length; j++)
            {
                centerContent.AppendLine(secondLines[j]);
            }

            centerContent.AppendLine("^XZ");
            var cimg = GenerateImage(centerContent.ToString());
            if (cimg is not null)
            {
                CenterImage = cimg;
            }
        }
    }
}
