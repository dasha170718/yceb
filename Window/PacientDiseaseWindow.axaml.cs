using System;
using System.Collections.Generic;
using System.Linq;
using DaripProgrammaUP.DateBase;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DaripProgrammaUP.Window;

public partial class PacientDiseaseWindow : Avalonia.Controls.Window
{
    private List<DiseaseRecord> _DataDiseaseRecord { get; set; }
    private List<DiseaseRecord> _ViewDiseaseRecord { get; set; }


    public PacientDiseaseWindow()
    {
        InitializeComponent();
        DownloadDataGrid();
    }

    public void DownloadDataGrid()
    {
        _DataDiseaseRecord = DataBaseManager.GetDiseaseRecords();
        UpdateDataGrid();
    }

    private void UpdateDataGrid()
    {
        _ViewDiseaseRecord = _DataDiseaseRecord;
        if (SearchBox.Text.Length >= 1)
        {
            string filters = SearchBox.Text.ToLower();
            _ViewDiseaseRecord = _ViewDiseaseRecord.Where(c =>
                c.Id.ToString().Contains(filters) ||
                c.PatientID.ToString().Contains(filters) ||
                c.FinalPrice.ToString().Contains(filters) ||
                c.DateStart.ToString().Contains(filters) ||
                c.DateEnd.ToString().Contains(filters) ||
                c.StatusID.ToString().Contains(filters) ||
                c.AttendingDoctorID.ToString().Contains(filters) ||
                c.DiseaseID.ToString().Contains(filters) ||
                c.OutputPatientFIO.ToLower().Contains(filters) ||
                c.OutputDiseaseName.ToLower().Contains(filters) ||
                c.OutputStatusName.ToLower().Contains(filters) ||
                c.OutputDoctorFIO.ToLower().Contains(filters)
            ).ToList();
        }
        DataGrid.ItemsSource = _ViewDiseaseRecord;
    }





    private void BtnDelet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
            return;
        DataBaseManager.RemoveDiseaseRecord(DataGrid.SelectedItem as DiseaseRecord);
        DownloadDataGrid();
    }



    private void ResetBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
    }
    




    private void UpdateFinalPrice()
    {
        if (DataGrid.SelectedItem == null)
            return;
        DiseaseRecord diseaseRecord = DataGrid.SelectedItem as DiseaseRecord;
        List<Procedure> procedures = DataBaseManager.GetProcedures().Where(
            p => p.DiseaseRecordID == diseaseRecord.Id).ToList();
        decimal finalPrice = 0;
        foreach (Procedure value in procedures)
        {
            if (value.StatusID == 4)
                finalPrice += value.Cost;
        }

        diseaseRecord.FinalPrice = finalPrice;
        MessageBoxManager.GetMessageBoxStandard("Обновлено", "Итоговая стоимость услуг:" + finalPrice, ButtonEnum.Ok)
            .ShowAsync();
        DataBaseManager.UpdateDiseaseRecord(diseaseRecord);
        DownloadDataGrid();
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateDataGrid();
    }

    private void BtnAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        AddEditDiseaseRecord adw = new AddEditDiseaseRecord();
        adw.ShowDialog(this);
    }

    private void BtnEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Строка не выбрана", ButtonEnum.Ok).ShowAsync();
            return;
        }

        AddEditDiseaseRecord adw = new AddEditDiseaseRecord(DataGrid.SelectedItem as DiseaseRecord);
        adw.ShowDialog(this);
    }

    private void BtnAddPatient_OnClick(object? sender, RoutedEventArgs e)
    {
        AddPatient adw = new AddPatient();
        adw.ShowDialog(this);
    }

    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateFinalPrice();
    }
}