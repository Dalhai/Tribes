namespace TribesOfDust.UI.Navigation
{
    public interface INavigatable<T>
    {
        /// <summary>
        /// Navigates to the specified target.
        /// </summary>
        /// 
        /// <param name="target">The target to navigate to.</param>
        /// <param name="route">Information about the route taken.</param>
        /// <returns>True, if the target was loaded, false otherwise.</returns>
        bool NavigateTo(T target, RouteArgs route);
    }
}