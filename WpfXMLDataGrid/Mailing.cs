using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailingsWPF
{
    public sealed class Mailing
    {
        private DateTime _mailingDate;
        private int _zipCodeOfTheSender;
        private int _zipCodeOfTheRecipient;
        private int _packageWeightInGrams;

        /// <summary>
        /// Создает пустое почтовое отправление.
        /// </summary>
        public Mailing()
        {
            this._mailingDate = new DateTime(1990, 25, 03);
            this._zipCodeOfTheSender = 101000;
            this._zipCodeOfTheRecipient = 440000;
            this._packageWeightInGrams = 100;
        }

        /// <summary>
        /// Создает новое почтовое отправление с указанными параметрами.
        /// </summary>
        /// <param name="mailingDate">Дата отправления.</param>
        /// <param name="zipCodeOfTheSender">Индекс отправителя.</param>
        /// <param name="zipCodeOfTheRecipient">Индекс получателя.</param>
        /// <param name="packageWeightInGrams">Вес отправления в граммах.</param>
        public Mailing(DateTime mailingDate, int zipCodeOfTheSender, int zipCodeOfTheRecipient, int packageWeightInGrams)
        {
            this._mailingDate = mailingDate;
            this._zipCodeOfTheSender = zipCodeOfTheSender;
            this._zipCodeOfTheRecipient = zipCodeOfTheRecipient;
            this._packageWeightInGrams = packageWeightInGrams;
        }

        #region Public variables

        /// <summary>
        /// Получение или установка значения даты отправления.
        /// </summary>
        public DateTime MailingDate
        {
            get { return this._mailingDate; }
            set { this._mailingDate = value; }
        }

        /// <summary>
        /// Получение или установка значения индекса отправителя.
        /// </summary>
        public int ZipCodeOfSender
        {
            get { return this._zipCodeOfTheSender; }
            set { this._zipCodeOfTheSender = value; }
        }

        /// <summary>
        /// Получение или установка значения индекса получателя.
        /// </summary>
        public int ZipCodeOfRecipient
        {
            get { return this._zipCodeOfTheRecipient; }
            set { this._zipCodeOfTheRecipient = value; }
        }

        /// <summary>
        /// Получение или установка значения веса отправления в граммах.
        /// </summary>
        public int PackageWeightInGrams
        {
            get { return this._packageWeightInGrams; }
            set { this._packageWeightInGrams = value; }
        }

        #endregion // Public variables

        #region Public methods

        #endregion // Private methods

    }
}
