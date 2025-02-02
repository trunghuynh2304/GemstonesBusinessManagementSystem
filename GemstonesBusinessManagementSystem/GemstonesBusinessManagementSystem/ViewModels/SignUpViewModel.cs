﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text.RegularExpressions;
using GemstonesBusinessManagementSystem.ViewModels;
using GemstonesBusinessManagementSystem.Views;
using GemstonesBusinessManagementSystem.Models;
using GemstonesBusinessManagementSystem.DAL;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace GemstonesBusinessManagementSystem.ViewModels
{
    class SignUpViewModel : BaseViewModel
    {
        public ICommand SignUpCommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand PasswordChangedCommand { get; set; }
        public ICommand PasswordConfirmChangedCommand { get; set; }
        public ICommand KeyCommand { get; set; }
        public ICommand OpenLoginWindowCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }
        private ObservableCollection<Employee> itemSourceEmployee = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> ItemSourceEmployee { get => itemSourceEmployee; set => itemSourceEmployee = value; }
        private bool isSignUp;
        private string password;
        private string username;
        private string passwordConfirm;

        public string TypeEmployee { get; set; }
        public bool IsSignUp { get => isSignUp; set => isSignUp = value; }
        public string Password { get => password; set => password = value; }
        public string Username { get => username; set => username = value; }
        public string PasswordConfirm { get => passwordConfirm; set => passwordConfirm = value; }
        private Employee selectedEmployee = new Employee();

        public Employee SelectedEmployee { get => selectedEmployee; set => selectedEmployee = value; }

        public SignUpViewModel()
        {
            //SignUpCommand = new RelayCommand<SignUpWindow>((p=>true, p=>))
            SignUpCommand = new RelayCommand<SignUpWindow>((parameter) => true, (parameter) => SignUp(parameter));
            PasswordChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingPassword(parameter));
            PasswordConfirmChangedCommand = new RelayCommand<PasswordBox>((parameter) => true, (parameter) => EncodingConfirmPassword(parameter));
            ChangePasswordCommand = new RelayCommand<ForgotPasswordWindow>((parameter) => true, (parameter) => ChangePassword(parameter));
            OpenLoginWindowCommand = new RelayCommand<Window>(parameter => true, parameter => parameter.Close());
            LoadCommand = new RelayCommand<Window>((parameter) => true, (parameter) => SetItemSourcEmployee());
        }

        public void EncodingPassword(PasswordBox parameter)
        {
            this.password = parameter.Password;
            this.password = MD5Hash(this.Password);
        }
        public void EncodingConfirmPassword(PasswordBox parameter)
        {
            this.passwordConfirm = parameter.Password;
            this.passwordConfirm = MD5Hash(this.passwordConfirm);
        }

        public void SetItemSourcEmployee()
        {
            itemSourceEmployee.Clear();
            List<Employee> employees = EmployeeDAL.Instance.GetEmployeeNonAccount();
            foreach (var employee in employees)
            {
                itemSourceEmployee.Add(employee);
            }
        }

        public void ChangePassword(ForgotPasswordWindow parameter)
        {
            if (string.IsNullOrEmpty(parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mã xác thực!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbKey.Focus();
                return;
            }
            //check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text) || !AccountDAL.Instance.IsExistUsername(parameter.txtUsername.Text))
            {
                parameter.txtUsername.Focus();
                parameter.txtUsername.Text = "";
                return;
            }
            //check password
            if (string.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbPassword.Focus();
                return;
            }
            //check password confirm
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                CustomMessageBox.Show("Vui lòng xác thực mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }
            //kiem tra do chinh xac
            if (!(ParameterDAL.Instance.GetAuthKey().Value == parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Mã xác thực không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbKey.Focus();
                return;
            }

            if (password != passwordConfirm)
            {
                CustomMessageBox.Show("Mật khẩu không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (AccountDAL.Instance.UpdatePasswordByUsername(parameter.txtUsername.Text, password))
            {
                CustomMessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                parameter.txtUsername.Text = null;
                parameter.pwbPassword.Password = "";
                parameter.pwbPasswordConfirm.Password = "";
            }
        }
        public void SignUp(SignUpWindow parameter)
        {
            isSignUp = false;
            if (parameter == null)
            {
                return;
            }
            //check ma xac thuc
            if (string.IsNullOrEmpty(parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mã xác thực!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbKey.Focus();
                return;
            }
            //check loai nhan vien
            if (string.IsNullOrEmpty(parameter.cboSelectEmployee.Text))
            {
                CustomMessageBox.Show("Vui lòng chọn nhân viên để đăng ký!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.cboSelectEmployee.Focus();
                parameter.cboSelectEmployee.Text = "";
                return;
            }
            //check username
            if (string.IsNullOrEmpty(parameter.txtUsername.Text) || AccountDAL.Instance.IsExistUsername(parameter.txtUsername.Text))
            {
                parameter.txtUsername.Focus();
                parameter.txtUsername.Text = "";
                return;
            }
            //check password
            if (string.IsNullOrEmpty(parameter.pwbPassword.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbPassword.Focus();
                return;
            }
            //check password confirm
            if (string.IsNullOrEmpty(parameter.pwbPasswordConfirm.Password))
            {
                CustomMessageBox.Show("Vui lòng nhập mật khẩu xác nhận!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbPasswordConfirm.Focus();
                return;
            }

            if (!(ParameterDAL.Instance.GetAuthKey().Value == parameter.pwbKey.Password))
            {
                CustomMessageBox.Show("Mã xác thực không đúng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                parameter.pwbKey.Focus();
                return;
            }

            if (password != passwordConfirm)
            {
                CustomMessageBox.Show("Mật khẩu xác nhận không trùng khớp!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int idAccount = AccountDAL.Instance.GetNewID();
            if (idAccount != -1)
            {
                Account newAccount = new Account(idAccount, parameter.txtUsername.Text.ToString(), password);
                isSignUp = AccountDAL.Instance.AddintoDB(newAccount);
                if (isSignUp && EmployeeDAL.Instance.UpdateIdAccount(idAccount, selectedEmployee.IdEmployee))
                {
                    
                    CustomMessageBox.Show("Đăng ký thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    parameter.txtUsername.Text = null;
                    parameter.pwbPassword.Password = null;
                    parameter.pwbPasswordConfirm.Password = null;
                    ItemSourceEmployee.Remove(selectedEmployee);
                    isSignUp = true;
                }
                else
                {
                    CustomMessageBox.Show("Đăng ký không thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
