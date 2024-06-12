using Comparator.View;
using Core;
using Core.Git;
using Core.Logic;
using Core.Model;
using Core.MVVM;
using Core.ViewLogic;
using ICSharpCode.AvalonEdit.Document;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Comparator.ViewModel
{
    public delegate void ComparatorDelegate();

    /// <summary>
    /// <para><b>View Model</b> para la ventana de Comparación de etiquetas.</para>
    /// <para>TODO!: Enlace a la descripcion del manual, 5.Desarrollo/Comparador.</para>
    /// </summary>
    public class ComparatorViewModel(Settings settings) : ViewModelBase(settings)
    {
        private bool _textMode = true;

        /// <summary>
        /// Indica si la ventana esta comparando codigo o no.
        /// </summary>
        public bool TextMode
        {
            get => _textMode;
            private set
            {
                _textMode = value;
                OnPropertyChanged(nameof(TextMode));
            }
        }

        private Visibility _textModeVisibility = Visibility.Visible;

        /// <summary>
        /// Indica la visibilidad de <see cref="TextMode"/>.
        /// </summary>
        public Visibility TextModeVisibility
        {
            get => _textModeVisibility;
            private set
            {
                _textModeVisibility = value;
                OnPropertyChanged(nameof(TextModeVisibility));
            }
        }

        private bool _imageMode = false;

        /// <summary>
        /// Indica si la ventana esta comparando imagenes o no.
        /// </summary>
        public bool ImageMode
        {
            get => _imageMode;
            private set
            {
                _imageMode = value;
                OnPropertyChanged(nameof(ImageMode));
            }
        }

        private Visibility _imageModeVisibility = Visibility.Hidden;

        /// <summary>
        /// Indica la visibilidad de <see cref="ImageMode"/>.
        /// </summary>
        public Visibility ImageModeVisibility
        {
            get => _imageModeVisibility;
            private set
            {
                _imageModeVisibility = value;
                OnPropertyChanged(nameof(ImageModeVisibility));
            }
        }

        private TextDocument _leftText = null!;

        /// <summary>
        /// Codigo a comparar del lado izquierdo, considerado como el 'codigo viejo'.
        /// </summary>
        public TextDocument LeftText
        {
            get => _leftText;
            set
            {
                _leftText = value;
                OnPropertyChanged(nameof(LeftText));
            }
        }

        private TextDocument _rightText = null!;

        /// <summary>
        /// Codigo a comparar del lado derecho, considerado como el 'codigo nuevo'.
        /// </summary>
        public TextDocument RightText
        {
            get => _rightText;
            set
            {
                _rightText = value;
                OnPropertyChanged(nameof(RightText));
            }
        }

        private string _leftFilename = null!;

        /// <summary>
        /// Nombre del archivo del lado izquierdo.
        /// </summary>
        public string LeftFilename
        {
            get => _leftFilename;
            set
            {
                _leftFilename = value;
                OnPropertyChanged(nameof(LeftFilename));
            }
        }

        private string _rightFilename = null!;

        /// <summary>
        /// Nombre del archivo del lado derecho.
        /// </summary>
        public string RightFilename
        {
            get => _rightFilename;
            set
            {
                _rightFilename = value;
                OnPropertyChanged(nameof(RightFilename));
            }
        }

        private BitmapSource _leftImage = null!;

        /// <summary>
        /// Imagen a comparar del lado izquierdo, considerada como la 'imagen vieja'.
        /// </summary>
        public BitmapSource LeftImage
        {
            get => _leftImage;
            set
            {
                _leftImage = value;
                OnPropertyChanged(nameof(LeftImage));
            }
        }

        private BitmapSource _rightImage = null!;

        /// <summary>
        /// Imagen a comparar del lado derecho, considerada como la 'imagen nueva'.
        /// </summary>
        public BitmapSource RightImage
        {
            get => _rightImage;
            set
            {
                _rightImage = value;
                OnPropertyChanged(nameof(RightImage));
            }
        }

        private BitmapSource _centerImage = null!;

        /// <summary>
        /// Imagen del centro, solamente incluye las lineas de diferencia entre ambos lados.
        /// </summary>
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

        public RelayCommand ChangeComparation => new(_ => OpenSelector());

        /// <summary>
        /// Evento que dispara una petición para actualizar el renderizado de las lineas diferentes.
        /// La vista esta encargada de recibir este evento y procesar el renderizado.
        /// </summary>
        public event ComparatorDelegate? CalculateDiffEvent;

        private LabelDpi _dpi;
        private LabelSize _size;

        public void OpenSelector()
        {
            SelectLabelsDialog dialog = new(Settings);

            if (dialog.ShowDialog() == false)
            {
                Closed = true;
                return;
            }

            /*LoadFiles(
                new TagFile((GitTag)dialog.LeftGitVer.CurrentItem, dialog.LeftLabel),
                new TagFile((GitTag)dialog.RightGitVer.CurrentItem, dialog.RightLabel)
            );*/

            LeftText = new TextDocument(File.ReadAllText(dialog.LeftLabel.Path));
            RightText = new TextDocument(File.ReadAllText(dialog.RightLabel.Path));
            LeftFilename = dialog.LeftLabel.Name;
            RightFilename = dialog.RightLabel.Name;

            _dpi = (LabelDpi)dialog.DpiList.CurrentItem;
            _size = (LabelSize)dialog.SizeList.CurrentItem;

            // Copiamos el texto de estos 'TextDocument' acá porque,
            // solo el hilo dueño de estas variables puede accederlos.
            var leftText = LeftText.Text;
            var rightText = RightText.Text;

            // Para evitar esperar a que se generen las imagenes para cargar la ventana,
            // ponemos un timer para que se generen 0.5 segundos despues de cargar la ventana.
            var timer = new System.Timers.Timer(500) { AutoReset = false };
            timer.Elapsed += (o, e) => LoadImages(leftText, rightText);
            timer.Start();

            CalculateDiffEvent?.Invoke();
        }

        private BitmapFrame? GenerateImage(string content)
        {
            var dpiValue = _dpi.Value;
            var sizeValue = _size.Value;
            var labelary = new Labelary(content)
                .FillTestVariables(Settings)
                .LoadFonts();

            using var task = Task.Run(() => labelary.Build(dpiValue, sizeValue));
            task.Wait();

            var bytes = task.Result;
            if (bytes is not null)
            {
                using MemoryStream stream = new(bytes);
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

            return null;
        }

        private void LoadImages(string leftText, string rightText)
        {
            var leftImg = GenerateImage(leftText);
            if (leftImg is not null)
            {
                LeftImage = leftImg;
            }

            var rightImg = GenerateImage(rightText);
            if (rightImg is not null)
            {
                RightImage = rightImg;
            }

            StringBuilder centerContent = new("^XA");
            var leftLines = leftText.Split(Environment.NewLine);
            var rightLines = rightText.Split(Environment.NewLine);

            for (int i = 0, j = 0; i < leftLines.Length && j < rightLines.Length; i++, j++)
            {
                if (leftLines[i] != rightLines[j])
                {
                    centerContent.AppendLine(rightLines[j]);
                }
            }

            var centerImg = GenerateImage(centerContent.AppendLine("^XZ").ToString());
            if (centerImg is not null)
            {
                CenterImage = centerImg;
            }
        }

        private void LoadFiles(TagFile left, TagFile right)
        {
            // Version local, no hace falta traer nada desde el repositorio global.
            if (left.Tag == GitTag.Local && right.Tag == GitTag.Local)
            {
                LeftText = new TextDocument(File.ReadAllText(left.File.Path));
                RightText = new TextDocument(File.ReadAllText(right.File.Path));
                LeftFilename = left.File.Name;
                RightFilename = right.File.Name;
            }
            // Etiqueta izquierda es local, derecha se traen desde el repositorio global.
            else if (left.Tag == GitTag.Local)
            {
                LeftText = new TextDocument(File.ReadAllText(left.File.Path));
                LeftFilename = left.File.Name;

                var rightFile = new LabelFile(LoadGitFile(right));
                RightText = new TextDocument(File.ReadAllText(rightFile.Path));
                RightFilename = rightFile.Name;
            }
            // Etiqueta derecha es local, izquierda se traen desde el repositorio global.
            else if (right.Tag == GitTag.Local)
            {
                RightText = new TextDocument(File.ReadAllText(right.File.Path));
                RightFilename = right.File.Name;

                var leftFile = new LabelFile(LoadGitFile(left));
                LeftText = new TextDocument(File.ReadAllText(leftFile.Path));
                LeftFilename = leftFile.Name;
            }
            // Ambas se traen desde el repositorio global.
            else
            {
                var leftFile = new LabelFile(LoadGitFile(left));
                LeftText = new TextDocument(File.ReadAllText(leftFile.Path));
                LeftFilename = leftFile.Name;

                var rightFile = new LabelFile(LoadGitFile(right));
                RightText = new TextDocument(File.ReadAllText(rightFile.Path));
                RightFilename = rightFile.Name;
            }
        }

        private string LoadGitFile(TagFile tagFile)
        {
            string path = Path.Combine(Path.GetTempPath(), "Visual Ternera - Git Temp");
            Directory.CreateDirectory(path);
            Git.RunGitCommand("init", "", path);
            Git.RunGitCommand("remote add origin", Settings.GitRepo, path);
            Git.RunGitCommand("fetch", "--all --tags --prune", path);
            Git.RunGitCommand("checkout", $"tags/{tagFile.Tag.Tag} {tagFile.File.Name}", path);
            return Path.Combine(path, tagFile.File.Name);
        }
    }

    record struct TagFile(GitTag Tag, LabelFile File);
}
