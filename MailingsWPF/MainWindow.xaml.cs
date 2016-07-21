using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Microsoft.Win32;

namespace MailingsWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Mailing> _observableMailings;
        private Regex _regex;
        private Thread _thread;
        private ProgressDialog _progressDialog;
        private bool _stopThread;

        public MainWindow()
        {
            InitializeComponent();

            this._regex = new Regex(@"^[0-9]$");
            needToGenerateTextBox.PreviewTextInput += NeedToGenerateTextBox_PreviewTextInput;
            
        }

        private void NeedToGenerateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) //ограничиваем текст бокс на ввод только натуральных чисел от 0 до 99 999
        {
            Match match = _regex.Match(e.Text);
            TextBox natualNumberTextBox = sender as TextBox;

            bool isNaturalNumber = (natualNumberTextBox.Text != "") && (natualNumberTextBox.Text.Length < 6) && (match.Success);

            if (!isNaturalNumber)
            {
                (sender as TextBox).Text = "1";
                e.Handled = true;
            }
        }

        private void generateMailingsButton_Click(object sender, RoutedEventArgs e)
        {
            int mailingsAmount = Convert.ToInt32(needToGenerateTextBox.Text);

            this._progressDialog = new ProgressDialog();
            this._progressDialog.loadingProgressBar.Minimum = 0;
            this._progressDialog.loadingProgressBar.Maximum = mailingsAmount;

            this._stopThread = false;
            this._thread = new Thread(new ParameterizedThreadStart(MailingsGeneratorThread));
            this._thread.Start(mailingsAmount);



            if (this._progressDialog.ShowDialog() == true)
            {
                this._stopThread = true;
            }

        }

        private void MailingsGeneratorThread(object amount)
        {
            int mailingsAmount = (int)amount;
            List<Mailing> generatedMailings = GenerateMailings(mailingsAmount);
            this._observableMailings = new ObservableCollection<Mailing>(generatedMailings);
            try
            {
                Dispatcher.Invoke(() => mailingsDataGrid.ItemsSource = this._observableMailings);

                Dispatcher.Invoke(() => this._progressDialog.Close());
                Dispatcher.Invoke(() => UpdateMailingsAmount());
            }
            catch(Exception ex)
            {

            }

        }

        /// <summary>
        /// Возвращает список из случайо сгенерированных почтовых отправлений.
        /// </summary>
        /// <param name="mailingsAmount">Количество почтовых отправлений, которое необходимо сгенерировать.</param>
        /// <returns>Список из случайо сгенерированных почтовых отправлений.</returns>
        private List<Mailing> GenerateMailings(int mailingsAmount)
        {
            Mailing mailing;
            List<Mailing> mailings = new List<Mailing>();
            Random random = new Random();

            DateTime randomDateTime;
            int randomDay;
            int randomMonth;
            int randomYear;

            int randomZipCodeSender;
            int randomZipCodeRecipient;
            int randomPackageWeight;

            int i = 0;
            ApartmentState threadApartmentState = Thread.CurrentThread.GetApartmentState();
            while(i < mailingsAmount && !_stopThread)
            {
                randomYear = random.Next(2001, 2016);
                randomMonth = random.Next(1, 13);
                randomDay = random.Next(1, 29);
                randomDateTime = new DateTime(randomYear, randomMonth, randomDay);

                randomZipCodeSender = random.Next(100000, 1000000);
                randomZipCodeRecipient = random.Next(100000, 1000000);

                randomPackageWeight = random.Next(10, 50000);

                mailing = new Mailing(randomDateTime, randomZipCodeSender, randomZipCodeRecipient, randomPackageWeight);

                mailings.Add(mailing);

                Thread.Sleep(100);
                try
                {
                    Dispatcher.Invoke(() => this._progressDialog.loadingProgressBar.Value = i);
                }
                catch(Exception ex)
                {

                }

                i++;
            }

            return mailings;
        }

        private void mailingsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = ((sender as DataGrid).SelectedIndex != -1);
            if (isItemSelected)
            {
                SelectedMailTextBox.Text = "Выбрано отправление: " + ((sender as DataGrid).SelectedIndex + 1);
            } 
        }

        private void UpdateMailingsAmount()
        {
            MailingsAmountTextBox.Text = "Всего почтовых отправлений: " + mailingsDataGrid.Items.Count.ToString();
        }

        private void saveMailingsButton_Click(object sender, RoutedEventArgs e)
        {
            WriteMailingsToXML();
        }

        private void WriteMailingsToXML()
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = "\t";
            xmlSettings.NewLineOnAttributes = true;
            XmlWriter xmlWriter = XmlWriter.Create(@"mailings.xml",xmlSettings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("mailings");

            foreach(Mailing mailing in this._observableMailings)
            {
                xmlWriter.WriteStartElement("mailing");
                xmlWriter.WriteAttributeString("MailingDate", mailing.MailingDate.ToLongDateString());
                xmlWriter.WriteAttributeString("ZipCodeOfSender", mailing.ZipCodeOfSender.ToString());
                xmlWriter.WriteAttributeString("ZipCodeOfRecipient", mailing.ZipCodeOfRecipient.ToString());
                xmlWriter.WriteAttributeString("PackageWeightInGrams", mailing.PackageWeightInGrams.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        private void loadMailingsButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.DefaultExt = ".xml";
            fileDialog.Filter = "XML files (*.xml)|*.xml";

            if (fileDialog.ShowDialog() == true)
            {
                fileName = fileDialog.FileName;
                LoadMailingsFromXML(fileName);
            }           
        }

        private void LoadMailingsFromXML(string fileName)
        {
            List<Mailing> mailingsList = new List<Mailing>();

            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.Load(fileName);

                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("mailings");
                xmlNodeList = xmlNodeList[0].ChildNodes;

                DateTime mailingDate;
                int zipCodeSender;
                int zipCodeRecipient;
                int packageWeight;

                foreach(XmlNode node in xmlNodeList)
                {
                    mailingDate = Convert.ToDateTime(node.Attributes.GetNamedItem("MailingDate").Value.ToString());
                    zipCodeSender = Convert.ToInt32(node.Attributes.GetNamedItem("ZipCodeOfSender").Value.ToString());
                    zipCodeRecipient = Convert.ToInt32(node.Attributes.GetNamedItem("ZipCodeOfRecipient").Value.ToString());
                    packageWeight = Convert.ToInt32(node.Attributes.GetNamedItem("PackageWeightInGrams").Value.ToString());

                    mailingsList.Add(new Mailing(mailingDate, zipCodeSender, zipCodeRecipient, packageWeight));
                }

                this._observableMailings = new ObservableCollection<Mailing>(mailingsList);
                mailingsDataGrid.ItemsSource = this._observableMailings;

                UpdateMailingsAmount();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
