using System;
using System.Reflection;

namespace SquidTestingMod.Helpers
{
    public static class Reflec
    {
        /// <summary>
        /// Safely invokes an instance method.
        /// </summary>
        /// <param name="method">The MethodInfo to invoke.</param>
        /// <param name="targetInstance">The instance of the object's type (must not be null for instance methods).</param>
        /// <param name="parameters">An array of parameters to pass in (use new object[] {} if none).</param>
        /// <returns>The return value of the method.</returns>
        public static object SafeInvoke(MethodInfo method, object targetInstance, object[] parameters)
        {
            if (parameters == null)
                parameters = [];
            try
            {
                return method.Invoke(targetInstance, parameters);
            }
            catch (TargetInvocationException tie)
            {
                throw new Exception($"Error invoking method '{method.Name}': {tie.InnerException?.Message}", tie.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception($"Reflection invoke failed for method '{method.Name}': {ex.Message}", ex);
            }
        }
    }
}