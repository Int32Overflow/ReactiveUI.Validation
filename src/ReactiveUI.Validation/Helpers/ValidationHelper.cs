// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Helpers
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// Encapsulation of a validation with bindable properties.
    /// </summary>
    public class ValidationHelper : ReactiveObject, IDisposable
    {
        private readonly IValidationComponent _validation;

        [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed with the _disposables field.")]
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed with the _disposables field.")]
        private readonly ObservableAsPropertyHelper<ValidationText> _message;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationHelper"/> class.
        /// </summary>
        /// <param name="validation">Validation property.</param>
        public ValidationHelper(IValidationComponent validation)
        {
            _validation = validation ?? throw new ArgumentNullException(nameof(validation));

            _isValid = _validation.ValidationStatusChange
                .Select(v => v.IsValid)
                .ToProperty(this, nameof(IsValid))
                .DisposeWith(_disposables);

            _message = _validation.ValidationStatusChange
                .Select(v => v.Text)
                .ToProperty<ValidationHelper, ValidationText>(this, nameof(Message))
                .DisposeWith(_disposables);
        }

        /// <summary>
        /// Gets a value indicating whether the validation is currently valid or not.
        /// </summary>
        public bool IsValid => _isValid.Value;

        /// <summary>
        /// Gets the current (optional) validation message.
        /// </summary>
        public ValidationText? Message => _message.Value;

        /// <summary>
        /// Gets the observable for validation state changes.
        /// </summary>
        public IObservable<ValidationState> ValidationChanged => _validation.ValidationStatusChange;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
            }
        }
    }
}
