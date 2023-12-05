using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DaripProgrammaUP.DateBase;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DaripProgrammaUP.Window;

public partial class AddPatient : Avalonia.Controls.Window
{
    public AddPatient()
    {
        InitializeComponent();
    }

    private void BtnSavet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TBoxPhoneNumber.Text.Length <= 0 ||
            TBoxFirstName.Text.Length <= 0 ||
            TBoxLastName.Text.Length <= 0 ||
            TBoxPatronymic.Text.Length <= 0 ||
            DPicerDateBirth.SelectedDate == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }
        
        DataBaseManager.AddPatient(new Patient(0,
            TBoxLastName.Text,
            TBoxFirstName.Text,
            TBoxPatronymic.Text,
            DPicerDateBirth.SelectedDate.Value.Date,
            TBoxPhoneNumber.Text));
        Close();
        
    }
}