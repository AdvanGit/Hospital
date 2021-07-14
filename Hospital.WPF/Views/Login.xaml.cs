﻿using Hospital.ViewModel;
using Hospital.ViewModel.Notificator;
using Hospital.WPF.Commands;
using Hospital.WPF.Navigators;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Hospital.WPF.Views
{
    public partial class Login : UserControl, INavigatorItem
    {
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        public string Label => "Авторизация";
        public Type Type => GetType();

        public Login(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public Command Init => new Command(async obj =>
        {
            if (long.TryParse(obj.ToString(), out long phone))
            {
                if (await (DataContext as LoginViewModel).CheckUser(phone))
                {
                    Main.MenuNavigator.Bodies.Clear();
                    Main.MenuNavigator.Bodies.Add(new Registrator());
                    Main.MenuNavigator.Bodies.Add(new Schedule());
                    Main.CurrentPage = Main.MenuNavigator.Bodies[0];
                }
            }
            else NotificationManager.AddItem(new NotificationItem(NotificationType.Error, TimeSpan.FromSeconds(3), "Номер введен неверно", true));
        }, obj => (obj != null && obj.ToString().Length > 5));
    }
}
