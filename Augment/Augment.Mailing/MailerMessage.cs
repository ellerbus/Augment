using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Augment.Mailing
{
    /// <summary>
    /// 
    /// </summary>
    sealed class MailerMessage
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public MailerMessage()
        {
        }

        #endregion

        #region Methods

        public MailMessage ToMessage()
        {
            MailMessage mm = new MailMessage();

            if (!string.IsNullOrEmpty(From))
            {
                mm.From = ValidateEMail("From", From);
            }

            foreach (MailAddress address in ValidateEMail("To", To))
            {
                mm.To.Add(address);
            }

            foreach (MailAddress address in ValidateEMail("Cc", Cc))
            {
                mm.CC.Add(address);
            }

            foreach (MailAddress address in ValidateEMail("Bcc", Bcc))
            {
                mm.Bcc.Add(address);
            }

            mm.Subject = ValidateString("Subject", Subject);

            mm.IsBodyHtml = true;

            mm.Body = ValidateString("Body", Body);

            return mm;
        }

        private string ValidateString(string variable, string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                string msg = "'{0}' cannot be null or empty".FormatArgs(variable);

                throw new ArgumentException(msg);
            }

            return str;
        }

        private IEnumerable<MailAddress> ValidateEMail(string variable, IEnumerable<string> emails)
        {
            foreach (string email in emails)
            {
                yield return ValidateEMail(variable, email);
            }
        }

        private MailAddress ValidateEMail(string variable, string email)
        {
            try
            {
                MailAddress ma = new MailAddress(email);

                return ma;
            }
            catch (ArgumentNullException xp)
            {
                string msg = "'{0}' email address cannot be null".FormatArgs(variable);

                throw new ArgumentNullException(msg, xp);
            }
            catch (ArgumentException xp)
            {
                string msg = "'{0}' email address cannot be empty".FormatArgs(variable);

                throw new ArgumentException(msg, xp);
            }
            catch (FormatException xp)
            {
                string msg = "'{0}' '{1}' email address is an invalid format".FormatArgs(variable, email);

                throw new FormatException(msg, xp);
            }
        }

        #endregion

        #region Properties

        public string From { get; set; }

        public HashSet<string> To { get; private set; }

        public HashSet<string> Cc { get; private set; }

        public HashSet<string> Bcc { get; private set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        #endregion
    }
}
