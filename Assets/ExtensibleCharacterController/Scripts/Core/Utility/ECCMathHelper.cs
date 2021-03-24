using UnityEngine;

namespace ExtensibleCharacterController.Core.Utility
{
    /// <summary>
    /// Static class containing various different helper methods related to math operations.
    /// </summary>
    public static class ECCMathHelper
    {
        /// <summary>
        /// Returns a Matrix4x4 struct. Useful for transforming world/local points or directions while easily taking scale into account.
        /// </summary>
        /// <param name="position">Position to be used for matrix.</param>
        /// <param name="rotation">Rotation to be used for matrix.</param>
        /// <param name="scale">Scale to be used for matrix.</param>
        /// <returns>Matrix4x4 struct used for any transformations.</returns>
        public static Matrix4x4 GetMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }

        /// <summary>
        /// Transforms a local point to a world point while maintaining scale.
        /// </summary>
        /// <param name="position">World position.</param>
        /// <param name="rotation">World rotation.</param>
        /// <param name="scale">World scale.</param>
        /// <param name="localPosition">Local position.</param>
        /// <returns>Position that has been transformed to a world point from a local point.</returns>
        public static Vector3 TransformPoint(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 localPosition)
        {
            Matrix4x4 matrix = GetMatrix(position, rotation, scale);
            return matrix.MultiplyPoint3x4(localPosition);
        }

        /// <summary>
        /// Transform a local point to a world point while ignoring scale.
        /// </summary>
        /// <param name="position">World position.</param>
        /// <param name="rotation">World rotation.</param>
        /// <param name="localPosition">Local position.</param>
        /// <returns>Position that has been transformed to a world point from a local point.</returns>
        public static Vector3 TransformPoint(Vector3 position, Quaternion rotation, Vector3 localPosition)
        {
            return TransformPoint(position, rotation, Vector3.one, localPosition);
        }

        /// <summary>
        /// Transforms a world point to a local point while maintaining scale.
        /// </summary>
        /// <param name="position">Local position.</param>
        /// <param name="rotation">Local rotation.</param>
        /// <param name="scale">Local scale.</param>
        /// <param name="worldPosition">World position.</param>
        /// <returns>Position that has been transformed to a local point from a world point.</returns>
        public static Vector3 InverseTransformPoint(Vector3 position, Quaternion rotation, Vector3 scale, Vector3 worldPosition)
        {
            Matrix4x4 matrix = GetMatrix(position, rotation, scale);
            return matrix.inverse.MultiplyPoint3x4(worldPosition);
        }

        /// <summary>
        /// Transforms a world point to a local point while ignoring scale.
        /// </summary>
        /// <param name="position">Local position.</param>
        /// <param name="rotation">Local position.</param>
        /// <param name="worldPosition">World position.</param>
        /// <returns>Position that has been transformed to a local point from a world point.</returns>
        public static Vector3 InverseTransformPoint(Vector3 position, Quaternion rotation, Vector3 worldPosition)
        {
            return InverseTransformPoint(position, rotation, Vector3.one, worldPosition);
        }
    }
}
