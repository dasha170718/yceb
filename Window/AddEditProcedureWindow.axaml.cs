using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DaripProgrammaUP.DateBase;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DaripProgrammaUP.Window;

public partial class AddEditProcedureWindow :  Avalonia.Controls.Window
{
    private Procedure EditProcedure;

    private List<DiseaseRecord> _DiseaseRecordList { get; set; }
    private List<Status> _StatusList { get; set; }
    private List<Doctor> _DoctorsList { get; set; }
    public AddEditProcedureWindow()
    {
        InitializeComponent();
        EditProcedure = null;
        UpdateComboBox();

    }
    public AddEditProcedureWindow(Procedure editProcedure)
    {
        InitializeComponent();
        EditProcedure = editProcedure;
        UpdateComboBox();
        FillData();
    }

    private void FillData()
    {
        CBoxAttendingDoctor.SelectedItem = _DoctorsList.Where(d => d.Id == EditProcedure.DoctorID).FirstOrDefault();
        CBoxDisiaseRecord.SelectedItem = _DiseaseRecordList.Where(d => d.Id == EditProcedure.DiseaseRecordID).FirstOrDefault();
        TBoxDescription.Text = EditProcedure.Description;
        DPickerDateStart.SelectedDate = EditProcedure.DateStart;
        NUpDownDuration.Value = EditProcedure.Duration;
        NUpDownCost.Value = EditProcedure.Cost;
        CBoxStatus.SelectedItem = _StatusList.Where(d => d.Id == EditProcedure.StatusID).FirstOrDefault();
    }
    private void UpdateComboBox()
    {
        _DiseaseRecordList = DataBaseManager.GetDiseaseRecords();
        _StatusList = DataBaseManager.GetStatusList();
        _DoctorsList = DataBaseManager.GetDoctors();

        CBoxDisiaseRecord.ItemsSource = _DiseaseRecordList;
        CBoxStatus.ItemsSource = _StatusList;
        CBoxAttendingDoctor.ItemsSource = _DoctorsList;
    }
    private void BtnSavet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CBoxAttendingDoctor.SelectedItem == null ||
            CBoxDisiaseRecord.SelectedItem == null ||
            TBoxDescription.Text.Length <= 1 ||
            DPickerDateStart.SelectedDate == null ||
            NUpDownDuration.Value == 0 ||
            NUpDownCost.Value == 0 ||
            CBoxStatus.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }

        if (EditProcedure == null)
        {
            DataBaseManager.AddProcedure(new Procedure(
                0,
                (CBoxAttendingDoctor.SelectedItem as Doctor).Id,
                (CBoxDisiaseRecord.SelectedItem as DiseaseRecord).Id,
                TBoxDescription.Text,
                DPickerDateStart.SelectedDate.Value.Date,
                Convert.ToInt32(NUpDownDuration.Value.Value),
                Convert.ToDecimal(NUpDownCost.Value.Value),
                (CBoxStatus.SelectedItem as Status).Id
            ));
        }
        else
        {
            DataBaseManager.UpdateProcedure(new Procedure(
                EditProcedure.Id,
                (CBoxAttendingDoctor.SelectedItem as Doctor).Id,
                (CBoxDisiaseRecord.SelectedItem as DiseaseRecord).Id,
                TBoxDescription.Text,
                DPickerDateStart.SelectedDate.Value.Date,
                Convert.ToInt32(NUpDownDuration.Value.Value),
                Convert.ToDecimal(NUpDownCost.Value.Value),
                (CBoxStatus.SelectedItem as Status).Id
            ));
        }
        ((ProcedureWindow)this.Owner).DownloadDataGrid();
        Close();
    }
}