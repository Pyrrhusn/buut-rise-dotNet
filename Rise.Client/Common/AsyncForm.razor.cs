
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Common
{
    public partial class AsyncForm<T, K> : AsyncData<T>
    {
        /// <summary>
        /// Reference to the form
        /// </summary>
        public required MudForm Form { get; set; }

        [Parameter]
        public object? Validation { get; set; }

        [Parameter]
        public int ValidationDelay { get; set; }

        /// <summary>
        /// The form data
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Raises the <see cref="FormDataChange"/> event upon change. When bound via <c>@bind-FromData</c>, this property is updated when the form data is changed.
        /// </remarks>
        [Parameter, EditorRequired]
        public K? FormData { get; set; }

        /// <summary>
        /// Occurs when the <see cref="FormData"/> value has changed.
        /// </summary>
        [Parameter]
        public EventCallback<K?> FormDataChanged { get; set; }

        [Parameter, EditorRequired]
        public required Func<T?, K?> DataToFormData { get; set; }

        [Parameter, EditorRequired]
        public required Func<K?, Task<T?>> OnSubmit
        {
            get; set;
        }

        protected override Task AssignData(T? data)
        {
            K? newFormData = DataToFormData(data);
            FormData = newFormData;
            return Task.WhenAll([base.AssignData(data), FormDataChanged.InvokeAsync(newFormData)]);
        }

        private async Task Submit()
        {
            await Form.Validate();

            if (Form.IsValid)
            {
                IsLoading = true;
                HasError = false;
                try
                {
                    // TODO make localisation
                    // TODO make use of the problemDetails
                    // TODO standardize loading and error throwing
                    T? newData = await OnSubmit(FormData);
                    await AssignData(newData);
                    ShowSuccesMessage();
                }
                catch (Exception e)
                {
                    HasError = true;
                    ErrorMessage = e.Message;
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private string TestIdSuccess()
        {
            return PrefixTestId("success");
        }

        private string TestIdSubmit()
        {
            return PrefixTestId("submit");
        }
    }
}