﻿using System;
using EnsureThat;
using Godot;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AlleyCat.Logging
{
    public class PrintLogger : ILogger
    {
        [NotNull]
        public string Name { get; }

        public PrintLogger([NotNull] string name)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            [CanBeNull] Exception exception,
            [NotNull] Func<TState, Exception, string> formatter)
        {
            Ensure.Any.IsNotNull(formatter, nameof(formatter));

            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message) && exception == null) return;

            var prefix = GetLevelPrefix(logLevel);

            GD.Print($"[{prefix}][{Name}] {message}");
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        protected virtual string GetLevelPrefix(LogLevel level)
        {
            var prefix = level.ToString();

            return prefix.Length < 6 ? prefix : prefix.Left(4);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
