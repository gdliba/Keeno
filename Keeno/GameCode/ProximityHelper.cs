using System;
using System.Collections.Generic;

namespace GameUtilities
{
    /// <summary>
    /// Class that provides utility methods for finding the closest object based on a distance metric.
    /// </summary>
    public static class ProximityHelper
    {
        /// <summary>
        /// Tries to find the closest valid object from a list of candidates based on a distance metric.
        /// </summary>
        /// <typeparam name="T">The type of the candidates.</typeparam>
        /// <param name="candidates">The list of candidate objects.</param>
        /// <param name="getDistanceSquared">A function that returns the squared distance of a candidate.</param>
        /// <param name="isValid">A predicate to determine if a candidate is valid.</param>
        /// <param name="closest">The closest valid candidate found, or null if none found.</param>
        /// <returns>True if a closest valid candidate was found; otherwise, false.</returns>
        public static bool TryFindClosest<T>(
            IReadOnlyList<T> candidates,
            Func<T, float> getDistanceSquared,
            Predicate<T> isValid,
            out T closest) where T : class
        {
            closest = null;
            float closestDistanceSquared = float.PositiveInfinity;

            for (int i = 0; i < candidates.Count; i++)
            {
                T candidate = candidates[i];

                if (candidate == null || !isValid(candidate))
                    continue;

                float distanceSquared = getDistanceSquared(candidate);

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closest = candidate;
                }
            }

            return closest != null;
        }
    }
}