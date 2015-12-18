using System.Collections.Generic;
using System.Net.Mail;

namespace Augment.Mailing
{
    /// <summary>
    /// Defines the fluent layout for the Mailer
    /// </summary>
    public interface IMailer
    {
        #region Recipients

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        IMailer From(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        IMailer To(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        IMailer To(IEnumerable<string> emails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        IMailer Cc(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        IMailer Cc(IEnumerable<string> emails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        IMailer Bcc(string email);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        IMailer Bcc(IEnumerable<string> emails);

        #endregion

        #region EMail Contents

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        IMailer Subject(string subject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        IMailer Body(string body);

        /// <summary>
        /// Uses the specified item and template to render the final
        /// body using DotLiquid's Template engine. Auto Injects the
        /// specified Type as Safe with all Public,Instance properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        IMailer RenderBodyWith<T>(T item, string template);

        #endregion

        #region Send

        /// <summary>
        /// 
        /// </summary>
        void Send();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MailMessage ToMessage();

        #endregion
    }
}
