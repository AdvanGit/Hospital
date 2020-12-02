﻿using Hospital.Domain.Model;
using Hospital.EntityFramework;
using Hospital.UI.Controls.Registrator;
using Hospital.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Hospital.UI.ViewModels.RegistratorViewModel
{
    public class RegistratorViewModel : INotifyPropertyChanged
    {
        public RegistratorViewModel()
        {
            CurrentRegTable = RegTables[0];
            CurrentPatientState = RegPatientTable;
        }

        private UserControl _currentRegTable;
        private UserControl _currentPatientState;
        public UserControl CurrentRegTable { get => _currentRegTable; set { _currentRegTable = value; OnPropertyChanged(nameof(CurrentRegTable)); } }
        public UserControl CurrentPatientState { get => _currentPatientState; set { _currentPatientState = value; OnPropertyChanged(nameof(CurrentPatientState)); } }

        public UserControl RegPatientTable { get; } = new RegPatientTable();
        public List<UserControl> RegTables { get; } = new List<UserControl> { new Controls.Registrator.RegDoctorTable(), new Controls.Registrator.RegTimeTable() };

        private readonly GenericDataServices<Patient> dataServicesPatient = new GenericDataServices<Patient>(new HospitalDbContextFactory());
        private readonly GenericDataServices<Entry> dataServiceEntry = new GenericDataServices<Entry>(new HospitalDbContextFactory());

        private Patient _selectedPatient;
        private Patient _editingPatient;
        private Entry _selectedEntry;
        public Patient SelectedPatient { get => _selectedPatient; set { _selectedPatient = value; OnPropertyChanged(nameof(SelectedPatient)); } }
        public Patient EditingPatient { get => _editingPatient; set { _editingPatient = value; OnPropertyChanged(nameof(EditingPatient)); } }
        public Entry SelectedEntry { get => _selectedEntry; set { _selectedEntry = value; OnPropertyChanged(nameof(SelectedEntry)); } }

        public ObservableCollection<Patient> Patients { get; } = new ObservableCollection<Patient>();
        public ObservableCollection<Belay> Belays { get; } = new ObservableCollection<Belay>();
        public ObservableCollection<Entry> Doctors { get; } = new ObservableCollection<Entry>();
        public ObservableCollection<Entry> Entries { get; } = new ObservableCollection<Entry>();

        private RelayCommand _selectRow;
        private RelayCommand _selectEntry;
        private RelayCommand _insertData;
        private RelayCommand _editPatient;
        private RelayCommand _editCancel;
        private RelayCommand _createPatient;
        private RelayCommand _savePatient;
        private RelayCommand _pageBack;
        private RelayCommand _createEntry;

        public RelayCommand SelectRow
        {
            get => _selectRow ??= new RelayCommand(async obj =>
            {
                if (obj != null)
                    if (obj.GetType() == typeof(Entry))
                    {
                        SelectedEntry = (Entry)obj;
                        await GetEntries(((Entry)obj).DoctorDestination, ((Entry)obj).TargetDateTime);
                        CurrentRegTable = RegTables[1];
                    }
                    else if (obj.GetType() == typeof(Patient)) SelectedPatient = (Patient)obj;
            });
        }
        public RelayCommand SelectEntry
        {
            get => _selectEntry ??= new RelayCommand(obj =>
            {
                if (obj != null) SelectedEntry = (Entry)obj;
            });
        }
        public RelayCommand InsertData { get => _insertData ??= new RelayCommand(async obj => { await GetPatients(); await GetFreeEntries(); }); }
        public RelayCommand EditPatient
        {
            get => _editPatient ??= new RelayCommand(async obj =>
            {
                EditingPatient = (Patient)SelectedPatient.Clone();
                CurrentPatientState = new RegEditPanel();
                await GetBelays();
            });
        }
        public RelayCommand EditCancel { get => _editCancel ??= new RelayCommand(obj => CurrentPatientState = RegPatientTable); }
        public RelayCommand CreatePatient
        {
            get => _createPatient ??= new RelayCommand(async obj => { EditingPatient = new Patient(); CurrentPatientState = new RegEditPanel(); await GetBelays(); });
        }
        public RelayCommand SavePatient
        {
            get => _savePatient ??= new RelayCommand(async obj =>
            {
                await dataServicesPatient.Update(EditingPatient.Id, EditingPatient);
                CurrentPatientState = RegPatientTable;
                SelectedPatient = EditingPatient;
            });
        }
        public RelayCommand PageBack { get => _pageBack ??= new RelayCommand(obj => CurrentRegTable = RegTables[0]); }
        public RelayCommand CreateEntry { get => _createEntry ??= new RelayCommand(async obj =>
        {
            SelectedEntry.Patient = SelectedPatient;
            SelectedEntry.EntryStatus = EntryStatus.Await;
            await dataServiceEntry.Update(SelectedEntry.Id, SelectedEntry);
            SelectedEntry = null;
            //Entries.Remove(Entries.Where(e => e.TargetDateTime == SelectedEntry.TargetDateTime).FirstOrDefault());
            //Entries.Add(SelectedEntry);
        }); }


        private async Task GetFreeEntries()
        {
            Doctors.Clear();
            using (HospitalDbContext db = new HospitalDbContextFactory().CreateDbContext())
            {
                List<Change> allChanges = await db.Changes
                    .Include(c => c.Staff).ThenInclude(s => s.Department).ThenInclude(d => d.Title)
                    .ToListAsync();

                for (int i = 0; i < allChanges.Count; i++)
                {
                    Change change = allChanges[i];

                    List<Entry> emptyEntries = new List<Entry>();
                    foreach (DateTime time in change.GetTimes()) emptyEntries
                            .Add(new Entry { CreateDateTime = DateTime.Now, TargetDateTime = time, DoctorDestination = change.Staff});

                    List<Entry> entries = await db.Entries
                        .Where(e => e.DoctorDestination == change.Staff)
                        .Where(e => e.TargetDateTime.Date == change.DateTimeStart.Date)
                        .ToListAsync();

                    emptyEntries.AddRange(entries);

                    var result = emptyEntries
                        .OrderBy(e => e.TargetDateTime)
                        .GroupBy(e => e.TargetDateTime)
                        .Select(e => e.Last())
                        .Where(e => e.EntryStatus == EntryStatus.Open)
                        .GroupBy(r => r.DoctorDestination)
                        .Select(r => r.FirstOrDefault());

                    if (result.Count() != 0)
                    {
                        allChanges.RemoveAll(c => c.Staff == change.Staff);
                        i--;
                    }

                    foreach (Entry entry in result) Doctors.Add(entry);
                }

            }
        }
        private async Task GetEntries(Staff selectedStaff, DateTime date)
        {
            Entries.Clear();
            using (HospitalDbContext db = new HospitalDbContextFactory().CreateDbContext())
            {

                List<Entry> entries = await db.Entries
                    .Where(e => e.DoctorDestination == selectedStaff)
                    .Where(e => e.TargetDateTime.Date == date.Date)
                    .Include(e => e.Patient)
                    .Include(e=>e.Registrator)
                    .Include(e=>e.MedCard)
                    .ToListAsync();
                List<Entry> emptyEntries = new List<Entry>();

                foreach (Change change in db.Changes
                    .Include(c => c.Staff).ThenInclude(s => s.Department).ThenInclude(d => d.Title)
                    .Where(c => c.Staff == selectedStaff)
                    .Where(c => c.DateTimeStart.Date == date.Date)
                    .ToList())
                    foreach (DateTime time in change.GetTimes()) emptyEntries.Add(new Entry
                    {
                        CreateDateTime = DateTime.Now,
                        TargetDateTime = time,
                        DoctorDestination = change.Staff,
                        Registrator = change.Staff, //заглушка
                    });

                emptyEntries.AddRange(entries);
                var result = emptyEntries.OrderBy(e => e.TargetDateTime).GroupBy(e => e.TargetDateTime).Select(e => e.Last());
                foreach (Entry entry in result) Entries.Add(entry);
            }
        }
        private async Task GetPatients()
        {
            using (HospitalDbContext db = new HospitalDbContextFactory().CreateDbContext())
            {
                Patients.Clear();
                var _patients = await db.Patients.Include(p => p.Belay).ToListAsync();
                foreach (Patient patient in _patients) Patients.Add(patient);
            }
        }
        private async Task GetBelays()
        {
            if (Belays.Count == 0)
                using (HospitalDbContext db = new HospitalDbContextFactory().CreateDbContext())
                {
                    var _belays = await db.Belays.ToListAsync();
                    foreach (Belay belay in _belays) Belays.Add(belay);
                }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
