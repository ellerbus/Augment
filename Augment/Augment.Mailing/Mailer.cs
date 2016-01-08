using System.Collections.Generic;
using System.Net.Mail;

namespace Augment.Mailing
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Mailer : IMailer
    {
        #region Members

        private MailerMessage _message;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of <see cref="Mailer"/>
        /// </summary>
        /// <returns></returns>
        public static IMailer Create()
        {
            return new Mailer();
        }

        /// <summary>
        /// 
        /// </summary>
        private Mailer()
        {
            _message = new MailerMessage();
        }

        #endregion

        #region Explicit Interface Implementation

        IMailer IMailer.From(string email)
        {
            _message.From = email;

            return this;
        }

        IMailer IMailer.To(IEnumerable<string> emails)
        {
            _message.To.AddRange(emails);

            return this;
        }

        IMailer IMailer.To(string email)
        {
            _message.To.Add(email);

            return this;
        }

        IMailer IMailer.Cc(IEnumerable<string> emails)
        {
            _message.Cc.AddRange(emails);

            return this;
        }

        IMailer IMailer.Cc(string email)
        {
            _message.Cc.Add(email);


            return this;
        }

        IMailer IMailer.Bcc(IEnumerable<string> emails)
        {
            _message.Bcc.AddRange(emails);

            return this;
        }

        IMailer IMailer.Bcc(string email)
        {
            _message.Bcc.Add(email);

            return this;
        }

        IMailer IMailer.Subject(string subject)
        {
            _message.Subject = subject;

            return this;
        }

        IMailer IMailer.Body(string body)
        {
            _message.Body = body;

            return this;
        }

        IMailer IMailer.RenderBodyWith<T>(T item, string template)
        {
            _message.Body = TemplateRegistry.RenderWith(item, template);

            return this;
        }

        void IMailer.Send()
        {
            MailMessage mm = _message.ToMessage();

            using (SmtpClient sc = new SmtpClient())
            {
                sc.Send(mm);
            }
        }

        MailMessage IMailer.ToMessage()
        {
            return _message.ToMessage();
        }

        #endregion
    }
}
