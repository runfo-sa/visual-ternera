﻿using Core;
using Core.Logic;
using Core.MVVM;
using Editor.Model;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Editor.ViewModel
{
    public class PreviewViewModel(Settings settings) : ViewModelBase(settings)
    {
        public static CollectionView DpiList { get; set; } = new(DpiConstants.All);
        public static CollectionView SizeList { get; set; } = new(SizeConstants.GetList(File.ReadAllText("SizeList.xml")));

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

        public BitmapImage RotateLeftIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-rotate-left.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }

        public BitmapImage RotateRightIcon
        {
            get
            {
                var uri = $"/Editor;component/Resources/{Enum.GetName(Settings.Theme)!.ToLower()}-rotate-right.png";
                return new BitmapImage(new Uri(uri, UriKind.Relative));
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

        public async Task<string> GeneratePreview(TabItem? item)
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

            return labelary.Error;
        }

        public static void DownSize()
        {
            SizeList.MoveCurrentToNext();
            if (SizeList.IsCurrentAfterLast)
            {
                SizeList.MoveCurrentToFirst();
            }
            SizeList.Refresh();
        }

        public static void UpSize()
        {
            SizeList.MoveCurrentToPrevious();
            if (SizeList.IsCurrentBeforeFirst)
            {
                SizeList.MoveCurrentToLast();
            }
            SizeList.Refresh();
        }
    }
}
