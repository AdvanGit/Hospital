﻿using Hospital.ViewModel.Ambulatory;
using Hospital.WPF.Navigators;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Hospital.WPF.Controls.Ambulatory
{
    public partial class AmbDiagnostic : UserControl, INavigatorItem
    {
        public string Label => "Диагностика";
        public Type Type => GetType();

        public Geometry GeometryIcon => Geometry.Parse("M447.1 112c-34.2.5-62.3 28.4-63 62.6-.5 24.3 12.5 45.6 32 56.8V344c0 57.3-50.2 104-112 104-60 0-109.2-44.1-111.9-99.2C265 333.8 320 269.2 320 192V36.6c0-11.4-8.1-21.3-19.3-23.5L237.8.5c-13-2.6-25.6 5.8-28.2 18.8L206.4 35c-2.6 13 5.8 25.6 18.8 28.2l30.7 6.1v121.4c0 52.9-42.2 96.7-95.1 97.2-53.4.5-96.9-42.7-96.9-96V69.4l30.7-6.1c13-2.6 21.4-15.2 18.8-28.2l-3.1-15.7C107.7 6.4 95.1-2 82.1.6L19.3 13C8.1 15.3 0 25.1 0 36.6V192c0 77.3 55.1 142 128.1 156.8C130.7 439.2 208.6 512 304 512c97 0 176-75.4 176-168V231.4c19.1-11.1 32-31.7 32-55.4 0-35.7-29.2-64.5-64.9-64zm.9 80c-8.8 0-16-7.2-16-16s7.2-16 16-16 16 7.2 16 16-7.2 16-16 16z");

        public AmbDiagnostic()
        {
            InitializeComponent();
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var sortPath = e.Column.SortMemberPath.ToString();
            var colectionView = CollectionViewSource.GetDefaultView(((DataGrid)sender).ItemsSource);

            colectionView.SortDescriptions.Clear();

            if (e.Column.SortDirection == null || e.Column.SortDirection.Value == ListSortDirection.Ascending)
            {
                e.Column.SortDirection = ListSortDirection.Descending;
                colectionView.SortDescriptions.Add(new SortDescription(sortPath, ListSortDirection.Descending));
            }
            else
            {
                e.Column.SortDirection = ListSortDirection.Ascending;
                colectionView.SortDescriptions.Add(new SortDescription(sortPath, ListSortDirection.Ascending));
            }
            e.Handled = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as DiagnosticViewModel).RaiseIsSymptomPropetryChange();
        }
    }
}
