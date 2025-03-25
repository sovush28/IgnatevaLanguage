using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IgnatevaLanguage
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;

        int RecordsQ, PagesQ;
        int CurrentPage = 0;
        int RecordsOnPage; //выбранное количество отображаемых записей
        //Boolean ShowAllRecords = false;

        public ClientPage()
        {
            InitializeComponent();
            var currentClients = IgnatevaLanguageEntities.GetContext().Client.ToList();
            

            ClientListView.ItemsSource = currentClients;

            ShownRecordsCombo.SelectedIndex = 0;

            UpdateClients();
        }

        private void UpdateClients()
        {
            var currentClients = IgnatevaLanguageEntities.GetContext().Client.ToList();

            switch (ShownRecordsCombo.SelectedIndex)
            {
                // 10
                case 0:
                    RecordsOnPage = 10;
                    break;

                // 50
                case 1:
                    RecordsOnPage = 50;
                    break;

                // 200
                case 2:
                    RecordsOnPage = 200;
                    break;

                // все
                case 3:
                    RecordsOnPage = RecordsQ;
                    //ShowAllRecords = true;
                    break;                        
            }

            ClientListView.ItemsSource = currentClients;
            TableList = currentClients;

            RecordsQ = TableList.Count;

            ShownRecordsQTB.Text = currentClients.Count.ToString();
            AllRecordsQTB.Text = RecordsQ.ToString();

            ChangePage(0, 0);

        }

        private void ChangePage(int direction, int? selectedPage)
        {
            // direction = 0 начало; 1 пред стр, 2 след стр
            // selectedPage = номер страницы; при нажатии на стрелки null

            CurrentPageList.Clear();

            RecordsQ = TableList.Count;

            if (RecordsQ % RecordsOnPage > 0)
            {
                PagesQ = RecordsQ / RecordsOnPage + 1;
            }
            else
            {
                PagesQ = RecordsQ / RecordsOnPage;
            }

            Boolean IfUpdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= PagesQ)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * RecordsOnPage + RecordsOnPage < RecordsQ ? CurrentPage * RecordsOnPage + RecordsOnPage : RecordsQ;
                    
                    for (int i = CurrentPage * RecordsOnPage; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction) {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * RecordsOnPage + RecordsOnPage < RecordsQ ? CurrentPage * RecordsOnPage + RecordsOnPage : RecordsQ;

                            for(int i = CurrentPage * RecordsOnPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            IfUpdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < PagesQ - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * RecordsOnPage + RecordsOnPage < RecordsQ ? CurrentPage * RecordsOnPage + RecordsOnPage : RecordsQ;

                            for (int i = CurrentPage * RecordsOnPage; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            IfUpdate = false;
                        }
                        break;
                }
            }

            if (IfUpdate)
            {
                PageListBox.Items.Clear();
                for(int i = 1; i <= PagesQ; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                //if (!ShowAllRecords)
                    ClientListView.ItemsSource = CurrentPageList;

                ClientListView.Items.Refresh();
            }

        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void PrevPageBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void NextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;

            if (currentClient.VisitCount != 0)
            {
                MessageBox.Show("Удаление запрещено: у клиента есть данные о посещениях.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult confirmDeletion = MessageBox.Show("Удалить клиента?", "Подтвердите удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmDeletion == MessageBoxResult.Yes)
            {
                try
                {
                    IgnatevaLanguageEntities.GetContext().Client.Remove(currentClient);
                    IgnatevaLanguageEntities.GetContext().SaveChanges();
                    UpdateClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }

            }
            else
                return;
        }

        private void ShownRecordsCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateClients();
        }
    }
}
