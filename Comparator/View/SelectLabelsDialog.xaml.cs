using AdonisUI.Controls;
using Core.Git;
using Core.Logic;
using Core.Model;
using Core.ViewLogic;
using System.IO;
using System.Windows.Data;

namespace Comparator.View
{
    public partial class SelectLabelsDialog : AdonisWindow
    {
        public ListCollectionView DpiList { get; set; } = new(DpiConstants.All);
        public ListCollectionView SizeList { get; set; } = new(SizeConstants.GetList(File.ReadAllText("SizeList.xml")));

        public ListCollectionView LeftGitVer { get; set; }
        public ListCollectionView RightGitVer { get; set; }

        public LabelFile LeftLabel
        {
            get => (LabelFile)leftLabel.SelectedItem;
        }

        public LabelFile RightLabel
        {
            get => (LabelFile)rightLabel.SelectedItem;
        }

        private readonly IEnumerable<LabelFile> _files;
        private readonly Settings _settings;

        public SelectLabelsDialog(Settings settings)
        {
            InitializeComponent();
            _settings = settings;

            _files = Directory
                .GetFiles(settings.EtiquetasDir, $"*.{settings.EtiquetasExtension}")
                .Select(f => new LabelFile(f));

            leftLabel.ItemsSource = _files;
            rightLabel.ItemsSource = _files;

            dpiList.ItemsSource = DpiList;
            sizeList.ItemsSource = SizeList;

            var tags = Git.RunGitCommand(
                "for-each-ref",
                "--format=\"%(refname:short)|%(creatordate:format:%Y/%m/%d %I:%M)|%(subject)\\n\" \"refs/tags/*\"",
                settings.EtiquetasDir)
            .Split("\\n", StringSplitOptions.RemoveEmptyEntries).Select(GitTag.Parse)
            .Prepend(GitTag.Local);

            LeftGitVer = new(tags.ToList());
            leftGitVer.ItemsSource = LeftGitVer;

            RightGitVer = new(tags.ToList());
            rightGitVer.ItemsSource = RightGitVer;
        }

        private void AcceptDialog(Object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void LeftFetchFiles(Object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            leftLabel.IsEnabled = false;
            acceptButton.IsEnabled = false;

            leftLabel.ItemsSource = FetchFiles((GitTag)LeftGitVer.CurrentItem);

            if (leftLabel.ItemsSource is not null)
            {
                leftLabel.IsEnabled = true;
                acceptButton.IsEnabled = true;
                leftLabel.SelectedIndex = 0;
            }
        }

        private void RightFetchFiles(Object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            rightLabel.IsEnabled = false;
            acceptButton.IsEnabled = false;

            rightLabel.ItemsSource = FetchFiles((GitTag)RightGitVer.CurrentItem);

            if (rightLabel.ItemsSource is not null)
            {
                rightLabel.IsEnabled = true;
                acceptButton.IsEnabled = true;
                rightLabel.SelectedIndex = 0;
            }
        }

        private IEnumerable<LabelFile>? FetchFiles(GitTag current)
        {
            if (current == GitTag.Local)
            {
                return _files;
            }

            return LoadGitFile(current);
        }

        private IEnumerable<LabelFile> LoadGitFile(GitTag git)
        {
            string path = Path.Combine(Path.GetTempPath(), $"Visual Ternera - {git.Tag}");
            Directory.CreateDirectory(path);

            if (Git.RunGitCommand("tag", "--points-at HEAD", path) != git.Tag)
            {
                Git.RunGitCommand("init", "", path);
                Git.RunGitCommand("remote add origin", _settings.GitRepo, path);
                Git.RunGitCommand("fetch", "--all --tags --prune", path);
                Git.RunGitCommand("checkout", $"tags/{git.Tag}", path);
            }

            return Directory
                .GetFiles(path, $"*.{_settings.EtiquetasExtension}")
                .Select(f => new LabelFile(f));
        }
    }
}
