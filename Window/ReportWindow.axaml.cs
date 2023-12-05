using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DaripProgrammaUP.DateBase;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace DaripProgrammaUP.Window;

public partial class ReportWindow : Avalonia.Controls.Window
{
    private List<Status> _StatusList { get; set; }
    private List<Doctor> _DoctorsList { get; set; }
    private List<Patient> _PatientsList { get; set; }

    public ReportWindow()
    {
        InitializeComponent();
        UpdateComboBox();
    }

    private void UpdateComboBox()
    {
        _StatusList = DataBaseManager.GetStatusList();
        _DoctorsList = DataBaseManager.GetDoctors();
        _PatientsList = DataBaseManager.GetPatients();
        CBoxStatus.ItemsSource = _StatusList;
        CBoxDoctor.ItemsSource = _DoctorsList;
        CBoxPatient.ItemsSource = _PatientsList;
    }

    private async Task GenerateReport()
    {
        // Строка для сохранения
        string OutputString = "";
        // Стартовые переменные
        decimal price = 0;
        List<Procedure> procedures = DataBaseManager.GetProcedures();
        // Фильрация по ComboBox
        if (CBoxDoctor.SelectedItem != null)
            procedures = procedures.Where(c => c.DoctorID == (CBoxDoctor.SelectedItem as Doctor).Id).ToList();
        if (CBoxPatient.SelectedItem != null)
            procedures = procedures.Where(c => c.DoctorID == (CBoxPatient.SelectedItem as Patient).Id).ToList();
        if (CBoxStatus.SelectedItem != null)
            procedures = procedures.Where(c => c.DoctorID == (CBoxStatus.SelectedItem as Status).Id).ToList();

        // Проверка периода
        OutputString += "Пациент: \n";
        if (CBoxPatient.SelectedItem == null)
            OutputString += "Все\n";
        else
            OutputString += (CBoxPatient.SelectedItem as Patient).FirstName + " " +
                            (CBoxPatient.SelectedItem as Patient).LastName+"\n";
        OutputString += "Доктор: \n";
        if (CBoxDoctor.SelectedItem == null)
            OutputString += "Все\n";
        else
            OutputString += (CBoxDoctor.SelectedItem as Doctor).FirstName + " " +
                            (CBoxDoctor.SelectedItem as Doctor).LastName +"\n";
        OutputString += "Статус: \n";
        if (CBoxStatus.SelectedItem == null)
            OutputString += "Все\n";
        else
            OutputString += (CBoxStatus.SelectedItem as Status).Name;
        
        // Фильтрация по датам

        if (DPicerStart.SelectedDate == null)
            DPicerStart.SelectedDate = DateTime.Today.AddYears(-1);
        if (DPicerEnd.SelectedDate == null)
            DPicerEnd.SelectedDate = DateTime.Now;
        if (DPicerEnd.SelectedDate != null && DPicerStart.SelectedDate == null)
            DPicerStart.SelectedDate = DPicerEnd.SelectedDate.Value.Date.AddYears(-1);
        if (DPicerEnd.SelectedDate > DPicerStart.SelectedDate)
        {
            DateTime cash = DPicerEnd.SelectedDate.Value.Date;
            DPicerEnd.SelectedDate = DPicerStart.SelectedDate;
            DPicerStart.SelectedDate = cash;
        }

        OutputString += "Период: \n";
        OutputString += 
            DPicerStart.SelectedDate.Value.ToString() + " - " + DPicerEnd.SelectedDate.Value.ToString()+ "\n";
        OutputString += "Кол-во: \n";
        OutputString +=  procedures.Count.ToString()+ "\n";
        
        
        // Заполнение строк
        OutputString += "Процедуры: \n";
        foreach (Procedure value in procedures)
        {
            OutputString +=  value.Id + " | " + value.DateStart.Date + " | " + value.Cost.ToString() + " | " +
                             value.OutputStatusName + "\n";
            price += value.Cost;
        }
        OutputString += "Итоговая цена: \n";
        OutputString +=  price.ToString()+ "\n";
        
        // Сохранение
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Title = "Сохранить отчет";
        dialog.InitialFileName = "Отчет.txt";
    
        string result = await dialog.ShowAsync(this);

        if (result != null)
        {
            File.WriteAllText(result, OutputString);
        }


    }
    private void BtnPatientClear_OnClick(object? sender, RoutedEventArgs e)
    {
        CBoxPatient.SelectedItem = null;
    }

    private void BtnDoctorClear_OnClick(object? sender, RoutedEventArgs e)
    {
        CBoxDoctor.SelectedItem = null;
    }

    private void BtnStatusClear_OnClick(object? sender, RoutedEventArgs e)
    {
        CBoxStatus.SelectedItem = null;
    }

    private void BtnReset_OnClick(object? sender, RoutedEventArgs e)
    {
        CBoxPatient.SelectedItem = null;
        CBoxDoctor.SelectedItem = null;
        CBoxStatus.SelectedItem = null;
    }

    private void BtnGenerate_OnClick(object? sender, RoutedEventArgs e)
    {
        GenerateReport();
    }
    
}