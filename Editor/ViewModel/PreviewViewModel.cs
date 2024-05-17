using Core;
using Editor.Model;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Editor.ViewModel
{
    public class PreviewViewModel(Settings settings) : ViewModelBase(settings)
    {
        public static CollectionView DpiList { get; } = new(DpiConstants.All);
        public static CollectionView SizeList { get; } = new(SizeConstants.All);

        private BitmapSource _previewImage = null!;

        public BitmapSource PreviewImage
        {
            get => _previewImage;
            set
            {
                _previewImage = value;
                OnPropertyChanged(nameof(PreviewImage));
            }
        }

        private double _previewAngle = 0.0;

        public double PreviewAngle
        {
            get => _previewAngle;
            set
            {
                _previewAngle = value;
                OnPropertyChanged(nameof(PreviewAngle));
            }
        }

        public RelayCommand RotateRight => new(_ =>
        {
            PreviewImage = new TransformedBitmap(PreviewImage, new RotateTransform(90.0));
            var angle = PreviewAngle + 90.0;
            PreviewAngle = angle >= 360.0 ? 0.0 : angle;
        }, p => PreviewImage is not null);

        public RelayCommand RotateLeft => new(_ =>
        {
            PreviewImage = new TransformedBitmap(PreviewImage, new RotateTransform(-90.0));
            var angle = PreviewAngle - 90.0;
            PreviewAngle = angle < 0.0 ? 270.0 : angle;
        }, p => PreviewImage is not null);

        public async void GeneratePreview(TabItem? item)
        {
            var content = item!.EditorBody.Text;
            var labelary = new Labelary(content)
                .FillVariables(Settings)
                .LoadFonts();
            var bytes = await labelary.Post(((LabelDpi)DpiList.CurrentItem).Value, ((LabelSize)SizeList.CurrentItem).Value);
            if (bytes is not null)
            {
                using MemoryStream stream = new(bytes);
                PreviewImage = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
    }
}
