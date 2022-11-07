using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteInEditMode]
public class OctreeComponent : MonoBehaviour
{
    [SerializeField] uint rootNodeSize;
    [SerializeField] float[] detailLevels;
    [HideInInspector] public bool settingsFoldout;

    [SerializeField] Transform viewer;

    uint previousRootNodeSize;
    float[] previoisDetailLevels;

    Octree<int> octree;

    private void Start()
    {
        InitAndSubdivide();
    }

    private void Update()
    {
        if (viewer.hasChanged)
        {
            Subdivide();
            viewer.hasChanged = false;
        }
    }

    void InitAndSubdivide()
    {
        octree = new Octree<int>(rootNodeSize, transform.position, detailLevels);
            
        if (viewer != null && detailLevels != null)
        {
            octree.Subdivide(viewer.position, detailLevels);
        }
    }

    public void Subdivide()
    {
        if(octree == null)
            octree = new Octree<int>(rootNodeSize, transform.position, detailLevels);


        if (viewer != null && detailLevels != null)
        {
            octree.Subdivide(viewer.position, detailLevels);
        }
    }

    private void OnValidate()
    {
        if(rootNodeSize != previousRootNodeSize)
        {
            previousRootNodeSize = rootNodeSize;
            InitAndSubdivide();
        }

        if (DetailLevelChaned())
        {
            detailLevels.CopyTo(previoisDetailLevels, 0);

            InitAndSubdivide();

            Debug.Log(octree.nodes.Length);
        }

    }

    bool DetailLevelChaned()
    {
        if (previoisDetailLevels.Length != detailLevels.Length)
        {
            previoisDetailLevels = new float[detailLevels.Length];

            return true;
        }

        bool detailLevelChanged = false;

        for (int i = 0; i < detailLevels.Length; i++)
        {
            if(detailLevels[i] < 0)
                detailLevels[i] = 0;

            if (previoisDetailLevels[i] != detailLevels[i])
            {
                detailLevelChanged = true;
            }
        }

        return detailLevelChanged;
    }


    private void OnDrawGizmos()
    {
        if(octree != null)
        {
            Gizmos.color = Color.green;
            octree.DrawTree();
        }
    }
}
