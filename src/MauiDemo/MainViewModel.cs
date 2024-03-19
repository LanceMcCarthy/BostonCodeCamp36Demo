using System.Collections.Specialized;
using CommonHelpers.Collections;
using CommonHelpers.Common;
using CommonHelpers.Models;
using CommonHelpers.Services;

namespace MauiDemo;

public class MainViewModel : ViewModelBase
{
    private ObservableRangeCollection<Employee> employees;

    public MainViewModel()
    {
        var data = SampleDataService.Current.GenerateEmployeeData();

        Employees.AddRange(data.Skip(Employees.Count).Take(5), NotifyCollectionChangedAction.Reset);
        
        AddRangeCommand = new Command(() => { Employees.AddRange(data.Skip(Employees.Count).Take(5), NotifyCollectionChangedAction.Reset); });

        ClearItemsCommand = new Command(() => { Employees.Clear(); });
    }

    public ObservableRangeCollection<Employee> Employees
    {
        get => employees ??= new ObservableRangeCollection<Employee>();
        set => SetProperty(ref employees, value);
    }

    public Command AddRangeCommand { get; set; }

    public Command ClearItemsCommand { get; set; }
}