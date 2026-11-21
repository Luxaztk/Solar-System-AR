using System;
using UnityEngine;

namespace SolarSystemAR.Core
{
    /// <summary>
    /// Converts astronomical-unit distances to metres and calculates a body's
    /// position from an origin and a horizontal direction. This class does not
    /// access AR, scene objects, or transforms, so callers can use it after AR
    /// placement has supplied the origin and direction.
    /// </summary>
    public static class ScaleCalculator
    {
        /// <summary>Default model scale: one astronomical unit equals one metre.</summary>
        public const float DefaultMetresPerAU = 1f;

        /// <summary>Converts an AU distance to metres at the requested model scale.</summary>
        /// <param name="astronomicalUnits">Distance in AU; must be non-negative.</param>
        /// <param name="metresPerAU">Model scale in metres per AU; must be positive.</param>
        public static float AuToMeters(
            float astronomicalUnits,
            float metresPerAU = DefaultMetresPerAU)
        {
            ValidateFinite(astronomicalUnits, nameof(astronomicalUnits));
            ValidateFinite(metresPerAU, nameof(metresPerAU));

            if (astronomicalUnits < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(astronomicalUnits), "AU distance must be non-negative.");
            }

            if (metresPerAU <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(metresPerAU), "Scale must be greater than zero.");
            }

            return astronomicalUnits * metresPerAU;
        }

        /// <summary>
        /// Calculates a body's world position as origin plus normalized,
        /// horizontal direction multiplied by its AU distance in metres.
        /// Horizontal means the Unity world XZ plane.
        /// </summary>
        /// <param name="origin">World-space origin, such as the placed Sun.</param>
        /// <param name="horizontalDirection">Direction to the body; its Y component is ignored.</param>
        /// <param name="astronomicalUnits">Distance in AU; must be non-negative.</param>
        /// <param name="metresPerAU">Model scale in metres per AU; must be positive.</param>
        public static Vector3 CalculatePosition(
            Vector3 origin,
            Vector3 horizontalDirection,
            float astronomicalUnits,
            float metresPerAU = DefaultMetresPerAU)
        {
            var direction = new Vector3(horizontalDirection.x, 0f, horizontalDirection.z);
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                throw new ArgumentException(
                    "Direction must have a non-zero horizontal component.",
                    nameof(horizontalDirection));
            }

            direction.Normalize();
            return origin + direction * AuToMeters(astronomicalUnits, metresPerAU);
        }

        private static void ValidateFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName, "Value must be finite.");
            }
        }
    }
}
