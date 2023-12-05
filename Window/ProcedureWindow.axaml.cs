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

public partial class ProcedureWindow : Avalonia.Controls.Window
{
    private List<Procedure> _DataProcedure { get; set; }
    private List<Procedure> _ViewProcedure { get; set; }

    private Patient authPatient { get; set; }

    public ProcedureWindow()
    {
        InitializeComponent();
        DownloadDataGrid();
    }

    public ProcedureWindow(Patient patient)
    {
        InitializeComponent();
        authPatient = patient;
        PanelAdmin.IsEnabled = false;
        DownloadDataGrid();
    }

    public void DownloadDataGrid()
    {
        _DataProcedure = DataBaseManager.GetProcedures();
        UpdateDataGrid();
    }

    private void UpdateDataGrid()
    {
        _ViewProcedure = _DataProcedure;
        if (authPatient != null)
        {
            List<DiseaseRecord> filterDiseaseRecords = DataBaseManager.GetDiseaseRecords();
            filterDiseaseRecords = filterDiseaseRecords.Where(c => c.PatientID == authPatient.Id).ToList();
            List<int> idDisiaseRecrod = new List<int>();
            foreach (DiseaseRecord value in filterDiseaseRecords)
                idDisiaseRecrod.Add(value.Id);
            _ViewProcedure = _ViewProcedure.Where(c => idDisiaseRecrod.Contains(c.DiseaseRecordID)).ToList();
        }

        if (SearchBox.Text.Length > 0)
            _ViewProcedure = _ViewProcedure.Where(c =>
                c.Id.ToString().Contains(SearchBox.Text) ||
                c.DoctorID.ToString().Contains(SearchBox.Text) ||
                c.DiseaseRecordID.ToString().Contains(SearchBox.Text) ||
                c.Description.ToString().Contains(SearchBox.Text) ||
                c.DateStart.ToString().Contains(SearchBox.Text) ||
                c.Duration.ToString().Contains(SearchBox.Text) ||
                c.Cost.ToString().Contains(SearchBox.Text) ||
                c.StatusID.ToString().Contains(SearchBox.Text)
            ).ToList();
        DataGrid.ItemsSource = _ViewProcedure;
    }



    private void ResetBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
    }

    private void BtnDelet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
            return;
        DataBaseManager.RemoveProcedure(DataGrid.SelectedItem as Procedure);
        DownloadDataGrid();
    }

    private void BtnRemoveSelect_OnClick(object? sender, RoutedEventArgs e)
    {
        DataGrid.SelectedItem = null;
    }

    

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateDataGrid();
    }

    private void BtnAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        AddEditProcedureWindow adw = new AddEditProcedureWindow();
        adw.ShowDialog(this);
    }

    private void BtnEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Строка не выбрана", ButtonEnum.Ok).ShowAsync();
            return;
        }

        AddEditProcedureWindow adw = new AddEditProcedureWindow(DataGrid.SelectedItem as Procedure);
        adw.ShowDialog(this);
    }
}