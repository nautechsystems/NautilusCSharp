﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ISendScheduler.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// This interface defines a scheduler that is able to send messages on a set schedule.
    /// </summary>
    public interface ISendScheduler
    {
        /// <summary>
        /// Schedules a message to be sent once after a specified period of time.
        /// </summary>
        /// <param name="delay">The time period that has to pass before the message is sent.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        void ScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent once after a specified period of time.
        /// </summary>
        /// <param name="delay">The time period that has to pass before the message is sent.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        /// <param name="cancelable">A cancelable used to cancel sending the message. Once the message has been sent, it cannot be canceled.</param>
        void ScheduleTellOnce(TimeSpan delay, IEndpoint receiver, object message, IEndpoint sender, ICancelable cancelable);

        /// <summary>
        /// Schedules a message to be sent repeatedly after an initial delay.
        /// </summary>
        /// <param name="initialDelay">The time period that has to pass before the first message is sent.</param>
        /// <param name="interval">The time period that has to pass between sending of the message.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        void ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender);

        /// <summary>
        /// Schedules a message to be sent repeatedly after an initial delay.
        /// </summary>
        /// <param name="initialDelay">The time period that has to pass before the first message is sent.</param>
        /// <param name="interval">The time period that has to pass between sending of the message.</param>
        /// <param name="receiver">The actor that receives the message.</param>
        /// <param name="message">The message that is being sent.</param>
        /// <param name="sender">The actor that sent the message.</param>
        /// <param name="cancelable">An cancelable used to cancel sending the message. Once the message has been sent, it cannot be canceled.</param>
        void ScheduleTellRepeatedly(TimeSpan initialDelay, TimeSpan interval, IEndpoint receiver, object message, IEndpoint sender, ICancelable cancelable);
    }
}
