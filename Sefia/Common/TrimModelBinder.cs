using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sefia.Common;

public class TrimModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        var value = valueProviderResult.FirstValue;

        // 공백 제거 처리
        bindingContext.Result = ModelBindingResult.Success(value?.Trim());
        return Task.CompletedTask;
    }
}