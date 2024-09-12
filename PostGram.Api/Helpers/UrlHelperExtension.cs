using Microsoft.AspNetCore.Mvc;

using System.Reflection;

namespace PostGram.Api.Helpers;

public static class UrlHelperExtension
{
    public static string? ControllerAction<T>(this IUrlHelper urlHelper, string action, object? arg)
        where T : ControllerBase
    {
        Type controller = typeof(T);
        MethodInfo? metodAction = controller.GetMethod(action);
        if (metodAction == null)
            return null;
        string controllerShortName = controller.Name.Replace("Controller", string.Empty);
        return urlHelper.Action(action, controllerShortName, arg);
    }
}