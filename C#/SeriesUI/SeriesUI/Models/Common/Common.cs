using System;
using Catel.IoC;
using Catel.Services;

namespace SeriesUI.Models.Common
{
    public enum CompletenessState
    {
        NotApplicable,
        Complete,
        NotSubbedNl,
        NotSubbed,
        NotDownloaded
    }

    public enum SubTitle
    {
        Nl,
        En
    }

    public static class Common
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public static class Dialog
    {
        public static void ShowError(string message)
        {
            GetService().ShowErrorAsync(message);
        }

        public static void ShowWarning(string message)
        {
            GetService().ShowWarningAsync(message);
        }

        public static void Show(string message)
        {
            GetService().ShowInformationAsync(message);
        }

        public static MessageResult ShowDialog(string message, string caption, MessageButton buttons, MessageImage image)
        {
            return GetService().ShowAsync(message, caption, buttons, image).Result;
        }

        /// <summary>
        ///     Returns the MessageService interface
        /// </summary>
        /// <returns>IMessageService object</returns>
        private static IMessageService GetService()
        {
            // Instantiate the singleton once
            var serviceResolver = DialogServiceResolver.Instance;
            var service = DialogServiceResolver.MessageService;

            return service;
        }
    }

    /// <summary>
    ///     A lazy, thread safe singleton to retrieve the message service
    /// </summary>
    /// <remarks>
    ///     GetDependencyResolver is an extension on Object and cannot be called without object.
    ///     If not in a static context, we could have used "this."
    ///     https://app.pluralsight.com/player?course=patterns-library&author=steve-smith&name=patterns-singleton&clip=7&mode
    ///     =live
    /// </remarks>
    public class DialogServiceResolver
    {
        private static readonly string dummy = "";

        private DialogServiceResolver()
        {
        }

        public static DialogServiceResolver Instance => Nested.Instance;

        private static IDependencyResolver DependencyResolver { get; set; }

        public static IMessageService MessageService { get; set; }

        private class Nested
        {
            internal static readonly DialogServiceResolver Instance = new DialogServiceResolver();

            static Nested()
            {
                DependencyResolver = dummy.GetDependencyResolver();
                MessageService = DependencyResolver.Resolve<IMessageService>();
            }
        }
    }
}