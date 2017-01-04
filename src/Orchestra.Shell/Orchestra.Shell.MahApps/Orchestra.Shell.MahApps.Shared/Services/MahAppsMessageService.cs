﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MahAppsMessageService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orchestra.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.IoC;
    using Catel.Services;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;

    public class MahAppsMessageService : Catel.Services.MessageService
    {
        private readonly IDispatcherService _dispatcherService;

        public MahAppsMessageService(IDispatcherService dispatcherService)
            : base(dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        protected override Task<MessageResult> ShowMessageBoxAsync(string message, string caption = "", MessageButton button = MessageButton.OK, MessageImage icon = MessageImage.None)
        {
            var style = MessageDialogStyle.Affirmative;
            var affirmativeResult = MessageResult.OK;
            var negativeResult = MessageResult.No;
            var auxiliaryResult = MessageResult.Cancel;

            var languageService = ServiceLocator.Default.ResolveType<ILanguageService>();
            MetroDialogSettings settings = new MetroDialogSettings();

            switch (button)
            {
                case MessageButton.OK:
                    style = MessageDialogStyle.Affirmative;
                    affirmativeResult = MessageResult.OK;
                    settings.AffirmativeButtonText = languageService.GetString("OK");
                    break;

                case MessageButton.OKCancel:
                    style = MessageDialogStyle.AffirmativeAndNegative;
                    affirmativeResult = MessageResult.OK;
                    negativeResult = MessageResult.Cancel;
                    settings.AffirmativeButtonText = languageService.GetString("OK");
                    settings.NegativeButtonText = languageService.GetString("Cancel");
                    break;

                case MessageButton.YesNo:
                    style = MessageDialogStyle.AffirmativeAndNegative;
                    affirmativeResult = MessageResult.Yes;
                    negativeResult = MessageResult.No;
                    settings.AffirmativeButtonText = languageService.GetString("Yes");
                    settings.NegativeButtonText = languageService.GetString("No");
                    break;

                case MessageButton.YesNoCancel:
                    style = MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary;
                    affirmativeResult = MessageResult.Yes;
                    negativeResult = MessageResult.No;
                    auxiliaryResult = MessageResult.Cancel;
                    settings.AffirmativeButtonText = languageService.GetString("Yes");
                    settings.NegativeButtonText = languageService.GetString("No");
                    settings.FirstAuxiliaryButtonText = languageService.GetString("Cancel");
                    break;

                default:
                    throw new ArgumentOutOfRangeException("button");
            }

            var tcs = new TaskCompletionSource<MessageResult>();

#pragma warning disable AvoidAsyncVoid
            _dispatcherService.BeginInvoke(async () =>
#pragma warning restore AvoidAsyncVoid
            {
                var window = Application.Current.MainWindow as MetroWindow;
                if (window == null)
                {
                    tcs.TrySetResult(MessageResult.Cancel);
                    return;
                }

                var result = await window.ShowMessageAsync(caption, message, style, settings);
                switch (result)
                {
                    case MessageDialogResult.Negative:
                        tcs.TrySetResult(negativeResult);
                        break;

                    case MessageDialogResult.Affirmative:
                        tcs.TrySetResult(affirmativeResult);
                        break;

                    case MessageDialogResult.FirstAuxiliary:
                        tcs.TrySetResult(auxiliaryResult);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            return tcs.Task;
        }
    }
}