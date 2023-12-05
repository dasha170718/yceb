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

public partial class AddEditDiseaseRecord : Avalonia.Controls.Window
{
    private DiseaseRecord EditDiseaseRecord { get; set; }
    private List<Disease> _DiseaseList { get; set; }
    private List<Doctor> _DoctorsList { get; set; }
    private List<Status> _StatusList { get; set; }
    private List<Patient> _PatientList { get; set; }
    public AddEditDiseaseRecord()
    {
        InitializeComponent();
        EditDiseaseRecord = null;
        UpdateComboBox();
    }
    public AddEditDiseaseRecord(DiseaseRecord editDiseaseRecord)
    {
        InitializeComponent();
        EditDiseaseRecord = editDiseaseRecord;
        UpdateComboBox();
        FillData();

    }
    private void FillData()
    {
        DiseaseRecord value = EditDiseaseRecord;
        CBoxPatient.SelectedItem = _PatientList.Where(c => value.PatientID == c.Id).FirstOrDefault();
        NUpDownFinalPrice.Value = value.FinalPrice;
        DPickerDateStart.SelectedDate = value.DateStart;
        DPickerDateEnd.SelectedDate = value.DateEnd;
        CBoxStatus.SelectedItem = _StatusList.Where(c => value.StatusID == c.Id).FirstOrDefault();
        CBoxDisease.SelectedItem = _DiseaseList.Where(c => value.DiseaseID == c.Id).FirstOrDefault();
        CBoxAttendingDoctor.SelectedItem = _DoctorsList.Where(c => value.AttendingDoctorID == c.Id).FirstOrDefault();
    }
    private void UpdateComboBox()
    {
        _PatientList = DataBaseManager.GetPatients();
        _StatusList = DataBaseManager.GetStatusList();
        _DoctorsList = DataBaseManager.GetDoctors();
        _DiseaseList = DataBaseManager.GetDiseases();

        CBoxPatient.ItemsSource = _PatientList;
        CBoxStatus.ItemsSource = _StatusList;
        CBoxAttendingDoctor.ItemsSource = _DoctorsList;
        CBoxDisease.ItemsSource = _DiseaseList;
    }
    private void BtnSavet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CBoxPatient.SelectedItem == null ||
            DPickerDateStart.SelectedDate == null ||
            CBoxStatus.SelectedItem == null ||
            CBoxAttendingDoctor.SelectedItem == null ||
            CBoxDisease.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }

        if (EditDiseaseRecord == null)
        {
            DataBaseManager.AddDiseaseRecord(new DiseaseRecord(
                0,
                (CBoxPatient.SelectedItem as Patient).Id,
                Convert.ToDecimal(NUpDownFinalPrice.Value.Value),
                DPickerDateStart.SelectedDate.Value.Date,
                DPickerDateEnd.SelectedDate.Value.Date,
                (CBoxStatus.SelectedItem as Status).Id,
                (CBoxAttendingDoctor.SelectedItem as Doctor).Id,
                (CBoxDisease.SelectedItem as Disease).Id
            ));
        }
        else
        {
            DataBaseManager.UpdateDiseaseRecord(new DiseaseRecord(
                EditDiseaseRecord.Id,
                (CBoxPatient.SelectedItem as Patient).Id,
                Convert.ToDecimal(NUpDownFinalPrice.Value.Value),
                DPickerDateStart.SelectedDate.Value.Date,
                DPickerDateEnd.SelectedDate.Value.Date,
                (CBoxStatus.SelectedItem as Status).Id,
                (CBoxAttendingDoctor.SelectedItem as Doctor).Id,
                (CBoxDisease.SelectedItem as Disease).Id
            ));
        }

        ((PacientDiseaseWindow)this.Owner).DownloadDataGrid();
        Close();
    }
}