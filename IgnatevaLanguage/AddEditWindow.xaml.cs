using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IgnatevaLanguage
{
    /// <summary>
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private Client currentClient = new Client();
        //private Client newClient = new Client();

        public AddEditWindow(Client SelectedClient)
        {
            InitializeComponent();

            IDTextBlock.Visibility = Visibility.Visible;
            IDTextBox.Visibility = Visibility.Visible;
            IDTextBox.Opacity = 1.0;

            if (SelectedClient != null)
            {
                currentClient = SelectedClient;

                if (currentClient.GenderCode == "ж")
                    RadioBtnFemale.IsChecked = true;
                else
                    RadioBtnMale.IsChecked = true;

                IDTextBox.IsReadOnly = true;
                IDTextBox.Opacity = 0.5;

                BDayDatePicker.Text = currentClient.Birthday.ToString();
            }
            else
            {
                IDTextBlock.Visibility = Visibility.Hidden;
                IDTextBox.Visibility = Visibility.Hidden;
                BDayDatePicker.Text = "";
            }

            DataContext = currentClient;

        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //private string MakeRelativePath(string filePath, string baseDirectory)
        //{
        //    // Убедимся, что путь заканчивается слешем
        //    baseDirectory = System.IO.Path.GetFullPath(baseDirectory);
        //    if (!baseDirectory.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
        //    {
        //        baseDirectory += System.IO.Path.DirectorySeparatorChar;
        //    }

        //    // Преобразуем путь в URI
        //    Uri fileUri = new Uri(filePath);
        //    Uri baseUri = new Uri(baseDirectory);

        //    // Вычисляем относительный путь
        //    return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString());
        //}

        private void ChangePhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var relativePathIs1 = Regex.Split(openFileDialog.FileName, @"E:\\учёба\\3 курс\\Разработка прогр модулей\\ЛР\\Языковая школа\\Языковая школа\\Сессия 1\\");
                currentClient.PhotoPath = relativePathIs1[1];
                ClientImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        //// Устанавливаем начальную папку (папка "Клиенты" в текущей директории проекта)
        //string relativeFolderPath = "Клиенты"; // Относительный путь к папке
        //string absoluteFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFolderPath);

        //// Если папка существует, устанавливаем ее как начальную
        //if (System.IO.Directory.Exists(absoluteFolderPath))
        //{
        //    openFileDialog.InitialDirectory = absoluteFolderPath;
        //}

        //if (openFileDialog.ShowDialog() == true)
        //{
        //    // Получаем полный путь к выбранному файлу
        //    string fullFilePath = openFileDialog.FileName;

        //    // Вычисляем относительный путь
        //    string relativeFilePath = MakeRelativePath(fullFilePath, AppDomain.CurrentDomain.BaseDirectory);

        //    // Сохраняем относительный путь в базу данных
        //    currentClient.PhotoPath = relativeFilePath;

        //    ClientImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
        //}

        //currentClient.PhotoPath = openFileDialog.FileName;
        //ClientImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
        //}


        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            string FIOpattern = @"^[a-zA-Z\p{L} -]+$";

            if (LastNameTB.Text.Length < 1)
                errors.AppendLine("Введите фамилию.");
            else if (!Regex.IsMatch(LastNameTB.Text, FIOpattern))
                errors.AppendLine("Фамилия содержит недопустимые символы. Разрешены только буквы, пробелы и дефис.");
            
            if (FirstNameTB.Text.Length < 1)
                errors.AppendLine("Введите имя.");
            else if (!Regex.IsMatch(FirstNameTB.Text, FIOpattern))
                errors.AppendLine("Имя содержит недопустимые символы. Разрешены только буквы, пробелы и дефис.");

            if (PatronymicTB.Text.Length < 1)
                errors.AppendLine("Введите отчество.");
            else if (!Regex.IsMatch(PatronymicTB.Text, FIOpattern))
                errors.AppendLine("Отчество содержит недопустимые символы. Разрешены только буквы, пробелы и дефис.");

            if (LastNameTB.Text.Length > 50 || FirstNameTB.Text.Length > 50 || PatronymicTB.Text.Length > 50)
                errors.AppendLine("Значения фамилии, имени и отчества не могут быть длиннее 50 символов.");

            
            string emailPattern = @"^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (EmailTB.Text.Length < 1)
                errors.AppendLine("Введите Email.");
            else if (EmailTB.Text.Length - EmailTB.Text.Replace("@", "").Length != 1 ||
                !Regex.IsMatch(EmailTB.Text, emailPattern) ||
                EmailTB.Text.StartsWith(".") || EmailTB.Text.StartsWith("@") ||
                EmailTB.Text.EndsWith(".") || EmailTB.Text.EndsWith("@") ||
                EmailTB.Text.Contains(".@") || EmailTB.Text.Contains("@.") || EmailTB.Text.Contains("..") ||
                EmailTB.Text.Contains(" ")
                )
                errors.AppendLine("Некорректный Email.");


            if (PhoneTB.Text.Length < 1)
                errors.AppendLine("Введите телефон.");
            else
            {
                string phonePattern = @"^[+\-\s()0-9]+$";
                
                string ph = PhoneTB.Text.
                   Replace("(", "").Replace("-", "").Replace("+", "").
                   Replace(")", "").Replace(" ", "");
                if (ph.Any(char.IsLetter))
                    errors.AppendLine("Телефон не должен содержать буквы.");
                else if (!Regex.IsMatch(PhoneTB.Text, phonePattern))
                    errors.AppendLine("Телефон содержит недопустимые символы. Разрешены только цифры, +, -, (, ), пробел.");

                /*
                else
                {
                    if (ph.Length < 11 || ph.Length > 12)
                        errors.AppendLine("Длина телефона должна быть равна 11 или 12 символам.");
                    else
                    {
                        if ((ph.Length < 11 || ph.Length > 12) &&
                        ((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) ||
                        (ph[1] == '3' && ph.Length != 12))
                            errors.AppendLine("Укажите правильно телефон клиента.");
                    }
                }
                */
            }


            if (BDayDatePicker.Text.Length < 1)
                errors.AppendLine("Укажите дату рождения.");
            else
            {
                try
                {
                    if (Convert.ToDateTime(BDayDatePicker.Text) >= DateTime.Now)
                        errors.AppendLine("Укажите правильную дату рождения.");
                }
                catch
                {
                    errors.AppendLine("Некорректная дата рождения.");
                }
            }


            if (RadioBtnMale.IsChecked == false && RadioBtnFemale.IsChecked == false)
                errors.AppendLine("Укажите пол.");


            //////// можно не проверять а резать при сохранении
            //if (FirstNameTB.Text != FirstNameTB.Text.Trim())
            //    errors.AppendLine("Некорректный ввод имени: введены пробелы в начале и/или в конце");
            //if (LastNameTB.Text != LastNameTB.Text.Trim())
            //    errors.AppendLine("Некорректный ввод фамилии: введены пробелы в начале и/или в конце");
            //if (PatronymicTB.Text != PatronymicTB.Text.Trim())
            //    errors.AppendLine("Некорректный ввод отчества: введены пробелы в начале и/или в конце");
            //if (PhoneTB.Text != PhoneTB.Text.Trim())
            //    errors.AppendLine("Некорректный ввод телефона: введены пробелы в начале и/или в конце");
            //if (BDayDatePicker.Text != BDayDatePicker.Text.Trim())
            //    errors.AppendLine("Некорректный ввод даты рождения: введены пробелы в начале и/или в конце");
            /////////////////////////


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            else
            {
                currentClient.FirstName = FirstNameTB.Text.Trim();
                currentClient.LastName = LastNameTB.Text.Trim();
                currentClient.Email = EmailTB.Text.Trim();
                currentClient.Phone = PhoneTB.Text;
                currentClient.Birthday = Convert.ToDateTime(BDayDatePicker.Text).Date;

                if (currentClient.ID == 0)
                {
                    currentClient.RegistrationDate = DateTime.Now.Date;
                    IgnatevaLanguageEntities.GetContext().Client.Add(currentClient);                    
                }

                try
                {
                    IgnatevaLanguageEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    this.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void RadioBtnFemale_Checked(object sender, RoutedEventArgs e)
        {
            currentClient.GenderCode = "ж";
        }

        private void RadioBtnMale_Checked(object sender, RoutedEventArgs e)
        {
            currentClient.GenderCode = "м";
        }
    }
}
