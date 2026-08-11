using Unity.Cinemachine;
using UnityEngine;

namespace ShoelaceStudios.Utilities.Extensions
{
    public enum CameraProjection
    {
        /// <summary>
        /// Uses the CinemachineCamera's own transform and lens.
        /// Cinemachine pipeline effects such as noise and damping are ignored.
        /// </summary>
        Raw,

        /// <summary>
        /// Uses the final state produced by the Cinemachine pipeline.
        /// Includes effects such as noise, damping, and other pipeline stages.
        /// Only meaningful once the pipeline has evaluated at least once
        /// (e.g. in Play mode, or with editor preview running).
        /// </summary>
        Final
    }

    /// <summary>
    /// Camera-style projection helpers for CinemachineCamera.
    ///
    /// A CinemachineCamera is not a Camera — it has no aspect ratio and no
    /// concept of what render target it will end up driving. All methods
    /// therefore take aspect explicitly.
    ///
    /// Projection:
    ///   Viewport space (0..1, 0..1, camera-space depth)
    ///       ↕
    ///   Camera space (local XYZ)
    ///       ↕
    ///   World space
    ///
    /// Limitations:
    ///   - Physical camera properties such as sensor size, lens shift and
    ///     gate fit are not reproduced.
    ///   - Perspective projection assumes LensSettings.FieldOfView is
    ///     vertical FOV.
    /// </summary>
    public static class CinemachineCameraExtensions
    {
        // A perspective frame has zero width at zero depth. This is the
        // smallest half-height we'll allow, to avoid dividing by zero on
        // the world-to-viewport pass.
        private const float MinFrameHalfHeight = 0.0001f;

        /// <summary>
        /// Converts a viewport-space point into world space.
        ///
        /// viewportPoint.x/y are normalized viewport coordinates.
        /// viewportPoint.z is distance from the camera in camera space.
        /// </summary>
        public static Vector3 ViewportToWorldPoint(
            this CinemachineCamera vcam,
            Vector3 viewportPoint,
            float aspect,
            CameraProjection projection = CameraProjection.Raw)
        {
            ProjectionState state = GetProjectionState(vcam, projection);

            Vector3 cameraSpacePoint = ViewportToCameraSpace(
                viewportPoint,
                state.Lens,
                aspect);

            return state.Position + state.Rotation * cameraSpacePoint;
        }

        /// <summary>
        /// Converts a world-space point into viewport space.
        ///
        /// The returned x/y values are normalized viewport coordinates.
        /// The returned z value is camera-space depth.
        /// </summary>
        public static Vector3 WorldToViewportPoint(
            this CinemachineCamera vcam,
            Vector3 worldPoint,
            float aspect,
            CameraProjection projection = CameraProjection.Raw)
        {
            ProjectionState state = GetProjectionState(vcam, projection);

            Vector3 cameraSpacePoint =
                Quaternion.Inverse(state.Rotation) *
                (worldPoint - state.Position);

            return CameraSpaceToViewport(
                cameraSpacePoint,
                state.Lens,
                aspect);
        }

        /// <summary>
        /// Builds a world-space ray from a viewport point.
        ///
        /// For perspective cameras, the ray originates at the camera position.
        /// For orthographic cameras, the ray originates on the camera plane and
        /// travels parallel to the camera's forward direction.
        /// </summary>
        public static Ray ViewportPointToRay(
            this CinemachineCamera vcam,
            Vector2 viewportPoint,
            float aspect,
            CameraProjection projection = CameraProjection.Raw)
        {
            ProjectionState state = GetProjectionState(vcam, projection);

            return state.Lens.Orthographic
                ? OrthographicRay(viewportPoint, state, aspect)
                : PerspectiveRay(viewportPoint, state, aspect);
        }

        private static Ray OrthographicRay(
            Vector2 viewportPoint,
            ProjectionState state,
            float aspect)
        {
            Vector3 cameraSpaceOrigin = ViewportToCameraSpace(
                new Vector3(viewportPoint.x, viewportPoint.y, 0f),
                state.Lens,
                aspect);

            Vector3 worldOrigin = state.Position + state.Rotation * cameraSpaceOrigin;
            Vector3 worldDirection = state.Rotation * Vector3.forward;

            return new Ray(worldOrigin, worldDirection);
        }

        private static Ray PerspectiveRay(
            Vector2 viewportPoint,
            ProjectionState state,
            float aspect)
        {
            // Perspective rays all originate at the camera position.
            // Any positive depth works here — we only need the direction.
            Vector3 cameraSpacePoint = ViewportToCameraSpace(
                new Vector3(viewportPoint.x, viewportPoint.y, 1f),
                state.Lens,
                aspect);

            Vector3 worldPoint = state.Position + state.Rotation * cameraSpacePoint;
            Vector3 direction = (worldPoint - state.Position).normalized;

            return new Ray(state.Position, direction);
        }

        // ------------------------------------------------------------------
        // Projection math
        // ------------------------------------------------------------------

        private static Vector3 ViewportToCameraSpace(
            Vector3 viewportPoint,
            LensSettings lens,
            float aspect)
        {
            FrameExtents extents = FrameExtents.AtDepth(lens, aspect, viewportPoint.z);

            float normalizedX = (viewportPoint.x - 0.5f) * 2f;
            float normalizedY = (viewportPoint.y - 0.5f) * 2f;

            return new Vector3(
                normalizedX * extents.HalfWidth,
                normalizedY * extents.HalfHeight,
                viewportPoint.z);
        }

        private static Vector3 CameraSpaceToViewport(
            Vector3 cameraSpacePoint,
            LensSettings lens,
            float aspect)
        {
            float depth = cameraSpacePoint.z;
            FrameExtents extents = FrameExtents.AtDepth(lens, aspect, depth);

            float viewportX = (cameraSpacePoint.x / extents.HalfWidth) * 0.5f + 0.5f;
            float viewportY = (cameraSpacePoint.y / extents.HalfHeight) * 0.5f + 0.5f;

            return new Vector3(viewportX, viewportY, depth);
        }

        // ------------------------------------------------------------------
        // Camera frame
        // ------------------------------------------------------------------

        /// <summary>
        /// The half-width and half-height of the camera frame at a given
        /// camera-space depth.
        ///
        /// Orthographic: size is constant regardless of depth.
        /// Perspective: size grows linearly with depth.
        /// </summary>
        private readonly struct FrameExtents
        {
            public readonly float HalfWidth;
            public readonly float HalfHeight;

            private FrameExtents(float halfWidth, float halfHeight)
            {
                HalfWidth = halfWidth;
                HalfHeight = halfHeight;
            }

            public static FrameExtents AtDepth(
                LensSettings lens,
                float aspect,
                float depth)
            {
                float halfHeight = lens.Orthographic
                    ? lens.OrthographicSize
                    : depth * Mathf.Tan(lens.FieldOfView * 0.5f * Mathf.Deg2Rad);

                if (Mathf.Abs(halfHeight) < MinFrameHalfHeight)
                    halfHeight = Mathf.Sign(halfHeight) * MinFrameHalfHeight;

                return new FrameExtents(halfHeight * aspect, halfHeight);
            }
        }

        // ------------------------------------------------------------------
        // Projection state
        // ------------------------------------------------------------------

        /// <summary>
        /// Resolves the position, rotation and lens used by all projection
        /// operations, based on the requested CameraProjection mode.
        /// Centralized so Raw vs Final can't drift apart between methods.
        /// </summary>
        private static ProjectionState GetProjectionState(
            CinemachineCamera vcam,
            CameraProjection projection)
        {
            if (projection == CameraProjection.Raw)
            {
                return new ProjectionState(
                    vcam.transform.position,
                    vcam.transform.rotation,
                    vcam.Lens);
            }

            CameraState state = vcam.State;

            return new ProjectionState(
                state.GetFinalPosition(),
                state.GetFinalOrientation(),
                state.Lens);
        }

        private readonly struct ProjectionState
        {
            public readonly Vector3 Position;
            public readonly Quaternion Rotation;
            public readonly LensSettings Lens;

            public ProjectionState(Vector3 position, Quaternion rotation, LensSettings lens)
            {
                Position = position;
                Rotation = rotation;
                Lens = lens;
            }
        }
    }
}