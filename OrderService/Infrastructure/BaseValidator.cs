namespace Infrastructure
{
    using Core;
    using FluentValidation;
    using Microsoft.Extensions.Logging;

    public abstract class BaseValidator<T, TValidator> : AbstractValidator<T>, IBaseValidator<T>
        where TValidator : IBaseValidator<T>
    {
        protected readonly ILogger _logger;

        protected BaseValidator(ILogger<TValidator> logger)
        {
            _logger = logger;
            SetupValidation();
        }

        protected abstract void SetupValidation();

        public Task<OperationResult<None>> ValidateItemAsync(T value, CancellationToken cts)
        {
            try
            {
                cts.ThrowIfCancellationRequested();
                return this.ValidateAsync(value, cts)
                    .ContinueWith(x =>
                    {
                        OperationResult<None> res = x.IsCompletedSuccessfully && x.Result.IsValid ?
                            new SuccessResult<None>(None.Instance()) :
                            new ValidationFailureResult<None>(string.Join(" - ", x.Result.Errors.Select(y => y.ErrorMessage)));
                        return res;
                    });

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Invalid null argument exception");
                return Task.FromResult(new InvalidRequestResult<None>("Invalid null argument") as OperationResult<None>);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Operation cancelled exception");
                return Task.FromResult(new CriticalFailureResult<None>("Operation cancelled") as OperationResult<None>);
            }
        }
    }
}
