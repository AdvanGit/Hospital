﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace Hospital.Domain.Model
{
    public enum Gender : byte { male, female }
    public enum WeekDays : byte { FiveTwo, TwoTwo, FourTwo, Even, Odd }

    public class User : DomainObject
    {
        private string _firstName;
        private string _midName;
        private string _lastName;
        private long _phoneNumber;
        private DateTime _birthDay;
        private Gender _gender;
        private DateTime _createDate;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string MidName
        {
            get { return _midName; }
            set
            {
                _midName = value;
                OnPropertyChanged("MidName");
            }
        }
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public long PhoneNumeber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged("PhoneNumber");
            }
        }
        public DateTime BirthDay
        {
            get => _birthDay;
            set
            {
                _birthDay = value;
                OnPropertyChanged("BirthDay");
            }
        }
        public DateTime CreateDate
        {
            get => _createDate;
            set
            {
                _createDate = value;
                OnPropertyChanged("CreateDate");
            }
        }

        public string _Adress { get; set; }
        public Adress Adress
        {
            get { return _Adress == null ? null : JsonConvert.DeserializeObject<Adress>(_Adress); }
            set { _Adress = JsonConvert.SerializeObject(value); }
        }

        public Gender Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged("Gender");
            }
        }
    }

    public class Staff : User
    {
        private bool _isEnabled;
        private string _password;
        private string _qualification;
        private WeekDays _weekDays;
        private int _cabinet;
        private Department _department;

        public ObservableCollection<Entry> Registrators { get; set; }
        public ObservableCollection<Entry> DoctorDestinations { get; set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        public string Qualification
        {
            get { return _qualification; }
            set
            {
                _qualification = value;
                OnPropertyChanged("Qualification");
            }
        }
        public WeekDays WeekDays
        {
            get => _weekDays;
            set
            {
                _weekDays = value;
                OnPropertyChanged("WeekDays");
            }
        }
        public int Cabinet
        {
            get => _cabinet;
            set
            {
                _cabinet = value;
                OnPropertyChanged("Cabinet");
            }
        }
        public Department Department
        {
            get
            {
                return _department;
            }
            set
            {
                _department = value;
                OnPropertyChanged("Department");
            }
        }
    }

    public class Patient : User
    {
        private Belay _belay;
        private int _belayCode;
        private DateTime _belayDateOut;
        private bool _isMarried;
        private bool _hasChild;

        public Belay Belay
        {
            get => _belay;
            set
            {
                _belay = value;
                OnPropertyChanged("Belay");
            }
        }
        public int BelayCode
        {
            get => _belayCode;
            set
            {
                _belayCode = value;
                OnPropertyChanged("BelayCode");
            }
        }
        public DateTime BelayDateOut
        {
            get => _belayDateOut;
            set
            {
                _belayDateOut = value;
                OnPropertyChanged("BelayDateOut");
            }
        }
        public bool IsMarried
        {
            get => _isMarried;
            set
            {
                _isMarried = value;
                OnPropertyChanged("IsMarried");
            }
        }
        public bool HasChild
        {
            get => _hasChild;
            set
            {
                _hasChild = value;
                OnPropertyChanged("HasChild");
            }
        }
    }
}