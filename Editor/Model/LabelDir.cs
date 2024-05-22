﻿using Core;
using System.Collections;
using System.Collections.ObjectModel;

namespace Editor.Model
{
    public class LabelDir(string Name)
    {
        public string Name { get; } = Name;
        public ObservableCollection<LabelFile> Labels { get; set; } = [];
        public ObservableCollection<LabelDir> Subfolders { get; set; } = [];
        public IEnumerable Items => Subfolders!.Cast<object>().Concat(Labels);
    }
}
