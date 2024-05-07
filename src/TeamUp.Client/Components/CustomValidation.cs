using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace TeamUp.Client.Components;

public sealed class CustomValidation : ComponentBase
{
	private ValidationMessageStore? _messageStore;

	[CascadingParameter]
	private EditContext? CurrentEditContext { get; set; }

	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;

	protected override void OnInitialized()
	{
		if (CurrentEditContext is null)
		{
			throw new InvalidOperationException($"{nameof(CustomValidation)} requires a cascading parameter of type {nameof(EditContext)}.");
		}

		_messageStore = new(CurrentEditContext);

		CurrentEditContext.OnFieldChanged += (s, e) => _messageStore?.Clear(e.FieldIdentifier);
		CurrentEditContext.OnValidationRequested += (s, e) =>
		{
			_messageStore?.Clear();

			var validationContext = new ValidationContext(CurrentEditContext.Model, ServiceProvider, null);
			var validationResults = new List<ValidationResult>();

			if (Validator.TryValidateObject(CurrentEditContext.Model, validationContext, validationResults, validateAllProperties: true))
			{
				return;
			}

			foreach (var item in validationResults)
			{
				var flag = false;
				foreach (var memberName in item.MemberNames)
				{
					flag = true;
					var fieldIdentifier = CurrentEditContext.Field(memberName);
					_messageStore?.Add(in fieldIdentifier, item.ErrorMessage ?? "");
				}

				if (!flag)
				{
					var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, string.Empty);
					_messageStore?.Add(in fieldIdentifier, item.ErrorMessage ?? "");
				}
			}

			CurrentEditContext.NotifyValidationStateChanged();
		};
	}

	public void DisplayErrors(IDictionary<string, string[]> errors)
	{
		if (CurrentEditContext is null)
		{
			return;
		}

		_messageStore?.Clear();

		foreach (var error in errors)
		{
			_messageStore?.Add(CurrentEditContext.Field(error.Key), error.Value);
		}

		CurrentEditContext.NotifyValidationStateChanged();
	}

	public void ClearErrors()
	{
		_messageStore?.Clear();
		CurrentEditContext?.NotifyValidationStateChanged();
	}
}
