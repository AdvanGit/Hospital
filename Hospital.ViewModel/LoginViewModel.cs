﻿using Hospital.Domain.Model;
using Hospital.Domain.Security;
using Hospital.EntityFramework;
using Hospital.EntityFramework.Services;
using Hospital.ViewModel.Notificator;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.ViewModel
{
    public class LoginViewModel : MainViewModel
    {
        public async Task<bool> CheckUser(long phoneNumber)
        {
            IsLoading = true;
            try
            {
                var item = (await new GenericDataServices<Staff>(contextFactory)
                    .GetWhere(s => s.PhoneNumber == phoneNumber))
                    .FirstOrDefault();
                if (item != null)
                {
                    CurrentStuffId = item.Id;
                    HeaderCaption = item.FirstName + " " + item.MidName[0] + ". " + item.LastName[0] + ".";
                    return true;
                }
                else
                {
                    NotificationManager.AddItem(new NotificationItem(NotificationType.Information, TimeSpan.FromSeconds(4), "Запись не найдена"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                NotificationManager.AddException(ex, 8);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<IAccount> GetIdentityAccount(long phoneNumber, string password)
        {
            return new Account<Staff>();
        }
    }
}
