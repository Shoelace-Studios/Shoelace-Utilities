using UnityEngine;

namespace ShoelaceStudios.Utilities.Helpers
{
    public static class MeshCreatorHelper
    {
        private static Mesh quadMesh;

        private static void CreateQuadMesh()
        {
            if (quadMesh == null)
            {
                quadMesh = new Mesh();

                quadMesh.SetVertices(
                    new Vector3[]
                    {
                        new(-.5f, 0f, 0f), new(-.5f, 1f, 0f),
                        new(.5f, 0f, 0f), new(.5f, 1f, 0f)
                    });

                quadMesh.SetTriangles(
                    new int[]
                    {
                        0, 1,
                        2, 2,
                        1, 3
                    },
                    0);

                quadMesh.SetUVs(
                    0, new Vector2[]
                    {
                        new(0f, 0f), new(0f, 1f),
                        new(1f, 0f), new(1f, 1f)
                    });

                quadMesh.RecalculateNormals();
                quadMesh.RecalculateBounds();
            }
        }

        public static Mesh GetQuadMesh()
        {
            if (quadMesh == null) CreateQuadMesh();

            return quadMesh;
        }
    }
}
