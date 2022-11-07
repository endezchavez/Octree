using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Octant
{
    NWU,
    NEU,
    SEU,
    SWU,
    NWD,
    NED,
    SED,
    SWD
}

public class Octree<T>
{

    OctreeNode<T> rootNode;
    public OctreeNode<T>[] nodes { get; private set;}

    public Octree(uint rootNodeSize, Vector3 position, float[] detailLevels)
    {
        rootNode = new OctreeNode<T>(1, 0, new Bounds(position, Vector3.one * rootNodeSize));
        nodes = new OctreeNode<T>[(int)Mathf.Pow(detailLevels.Length, 8)];
        nodes[0] = rootNode;
    }

    public void Subdivide(Vector3 viewerPos, float[] detailLevels)
    {
        rootNode.Subdivide(viewerPos, detailLevels, nodes);
    }

    public OctreeNode<T>[] GetNodes()
    {
        return nodes;
    }

    public void DrawTree()
    {
        if(rootNode != null)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].DrawBounds();
                }
            }
        }
            //rootNode.DrawBounds();
    }

    public class OctreeNode<T>
    {
        uint id;
        uint lod;
        Bounds bounds;
        Bounds[] childBounds;
        OctreeNode<T>[] childNodes;

        public OctreeNode(uint id, uint lod, Bounds bounds)
        {
            this.id = id;
            this.lod = lod;
            this.bounds = bounds;
        }

        public void Subdivide(Vector3 viewerPos, float[] detailLevels, OctreeNode<T>[] nodes)
        {
            if(detailLevels.Length == 1)
            {
                childNodes = null;

                RemoveChildren(nodes);
            }

            
            if(lod >= detailLevels.Length - 1 || lod < 0)
            {
                return;
            }
            

            if(id * 8 >= nodes.Length - 1)
            {
                return;
            }

            //childNodes = null;

            /*
            nodes[id * 8] = null;
            nodes[id * 8 + 1] = null;
            nodes[id * 8 + 2] = null;
            nodes[id * 8 + 3] = null;
            nodes[id * 8 + 4] = null;
            nodes[id * 8 + 5] = null;
            nodes[id * 8 + 6] = null;
            nodes[id * 8 + 7] = null;
            */

            if(childBounds == null)
            {
                float quarter = bounds.size.x / 4;
                uint childLength = (uint)bounds.size.x / 2;
                Vector3 childSize = Vector3.one * childLength;


                childBounds = new Bounds[8];

                childBounds[0] = new Bounds(bounds.center + new Vector3(-quarter, quarter, quarter), childSize);
                childBounds[1] = new Bounds(bounds.center + new Vector3(quarter, quarter, quarter), childSize);
                childBounds[2] = new Bounds(bounds.center + new Vector3(quarter, quarter, -quarter), childSize);
                childBounds[3] = new Bounds(bounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
                childBounds[4] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
                childBounds[5] = new Bounds(bounds.center + new Vector3(quarter, -quarter, quarter), childSize);
                childBounds[6] = new Bounds(bounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
                childBounds[7] = new Bounds(bounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
            }

            bool subdived = false;

            uint childLod = lod + 1;

            for (int i = 0; i < 8; i++)
            {
                if (ViewerInRange(viewerPos, childLod, childBounds[i], detailLevels))
                {
                    subdived = true;
                    break;
                }
            }

            if (subdived)
            {
                uint index = id * 8;

                if(nodes[index] == null)
                {
                    childNodes = new OctreeNode<T>[8];

                    childNodes[0] = new OctreeNode<T>(id * 8, childLod, childBounds[0]);
                    childNodes[1] = new OctreeNode<T>(id * 8 + 1, childLod, childBounds[1]);
                    childNodes[2] = new OctreeNode<T>(id * 8 + 2, childLod, childBounds[2]);
                    childNodes[3] = new OctreeNode<T>(id * 8 + 3, childLod, childBounds[3]);
                    childNodes[4] = new OctreeNode<T>(id * 8 + 4, childLod, childBounds[4]);
                    childNodes[5] = new OctreeNode<T>(id * 8 + 5, childLod, childBounds[5]);
                    childNodes[6] = new OctreeNode<T>(id * 8 + 6, childLod, childBounds[6]);
                    childNodes[7] = new OctreeNode<T>(id * 8 + 7, childLod, childBounds[7]);

                    nodes[id * 8] = childNodes[0];
                    nodes[id * 8 + 1] = childNodes[1];
                    nodes[id * 8 + 2] = childNodes[2];
                    nodes[id * 8 + 3] = childNodes[3];
                    nodes[id * 8 + 4] = childNodes[4];
                    nodes[id * 8 + 5] = childNodes[5];
                    nodes[id * 8 + 6] = childNodes[6];
                    nodes[id * 8 + 7] = childNodes[7];
                }
                
            }
            else
            {
                RemoveChildren(nodes);
            }
            
            if (childNodes != null)
            {
                foreach (OctreeNode<T> node in childNodes)
                {
                    node.Subdivide(viewerPos, detailLevels, nodes);
                }
            }
            
        }

        void RemoveChildren(OctreeNode<T>[] nodes)
        {
            if(id * 8 >= nodes.Length)
            {
                return;
            }
            if (nodes[id * 8] == null)
            {
                return;
            }

            for (uint i = 0; i < 8; i++)
            {
                uint index = (id * 8) + i;
                if (nodes[index] != null)
                {
                    OctreeNode<T> childNode = nodes[index];

                    childNode.RemoveChildren(nodes);
                    nodes[index] = null;
                    childNode = null;
                }
            }

        }

        bool ViewerInRange(Vector3 viewerPos, uint lod, Bounds childBounds, float[] detailLevels)
        {
            float sqrDetailLevelDistance = detailLevels[lod] * detailLevels[lod];
            float sqrPlayerDistance = childBounds.SqrDistance(viewerPos);
            return sqrPlayerDistance <= sqrDetailLevelDistance;
        }

        bool IsLeaf()
        {
            return childNodes == null;
        }

        public void DrawBounds()
        {
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            if (IsLeaf())
            {
            }
            /*
            else
            {
                foreach(OctreeNode<T> node in childNodes)
                {
                    node.DrawBounds();
                }
            }
            */
        }

    }

    

}
