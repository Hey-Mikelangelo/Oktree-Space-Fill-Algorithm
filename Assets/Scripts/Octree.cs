using System.Collections.Generic;
using UnityEngine;

public class Octree
{
    private int _maxDeph;
    private Node _rootNode;

    public Octree(Vector3 minimumCorner, float size, float minCellSize)
    {
        if(minCellSize <= 0)
        {
            minCellSize = 0.1f;
            Debug.LogWarning($"minCellSize cannot be <= 0. minCellSize is set to {minCellSize}");
        }
        int maxTreeDeph = Mathf.FloorToInt(Mathf.Log((size / minCellSize), 2));
        Init(minimumCorner, size, maxTreeDeph);

    }

    public void Draw()
    {
        _rootNode.Draw();
    }

    public void Insert(Bounds bounds)
    {
        _rootNode.Insert(bounds);
    }

    public bool OverlapsOccupied(Bounds bounds)
    {
        return _rootNode.OverlapsOccupied(bounds);
    }

    public bool OverlapsOccupied(Vector3 point)
    {
        return _rootNode.OverlapsOccupied(point);
    }

    private void Init(Vector3 minimumCorner, float size, int maxDeph)
    {
        _rootNode = new Node(minimumCorner, size, 0, this);
        _maxDeph = maxDeph;
    }

    [System.Serializable]
    public class Node
    {
        //'p' means '+', 'm' means '-'.
        private Node _mXmYmZ;
        private Node _pXmYmZ;
        private Node _mXpYmZ;
        private Node _pXpYmZ;
        private Node _mXmYpZ;
        private Node _pXmYpZ;
        private Node _mXpYpZ;
        private Node _pXpYpZ;

        private event System.Action _onOccupied;
        private List<Node> _childNodes;
        private Octree _myOctree;
        private Bounds _bounds;
        private int _deph;
        private int _occupiedChildCount = 0;
        private bool _isOccupied = false;
        private bool _isSubdivided = false;

        public Node(Vector3 minimumCorner, float size, int deph, Octree myOctree)
        {
            float halfSize = size / 2;

            Vector3 center = new Vector3(
                minimumCorner.x + halfSize, 
                minimumCorner.y + halfSize, 
                minimumCorner.z + halfSize);

            _myOctree = myOctree;
            _bounds = new Bounds(center, new Vector3(size, size, size));
            _deph = deph;
        }

        public void Draw()
        {
            if (_isOccupied)
            {
                Gizmos.DrawCube(_bounds.center, _bounds.size);
            }
            else
            {
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);
            }
            if (_isSubdivided)
            {
                foreach(Node childNode in _childNodes)
                {
                    childNode.Draw();
                }
            }
        }

        public bool Intersects(Bounds otherBounds)
        {
            return Intersects(this, otherBounds);
        }

        public bool Contains(Vector3 point)
        {
            return Contains(this, point);
        }

        public bool OverlapsOccupied(Vector3 point)
        {
            return OverlapsWithOccupiedNode(this, point, Contains);
        }

        public bool OverlapsOccupied(Bounds otherBounds)
        {
            return OverlapsWithOccupiedNode(this, otherBounds, Intersects);
        }

        public void Insert(Bounds otherBounds)
        {
            if (Intersects(otherBounds))
            {
                Subdivide();
                if (_isSubdivided)
                {
                    foreach (Node childNode in _childNodes)
                    {
                        childNode.Insert(otherBounds);
                    }
                }
                else
                {
                    Occupied();
                }
            }
        }

        public void Subdivide()
        {
            if (_deph == _myOctree._maxDeph || _isSubdivided)
            {
                return;
            }
            float halfSize = _bounds.size.x / 2;
            int childDeph = _deph + 1;
            Vector3 boundsMin = _bounds.min;
            _mXmYmZ = new Node(
                boundsMin, halfSize, childDeph, _myOctree);
            _pXmYmZ = new Node(
                new Vector3(boundsMin.x + halfSize, boundsMin.y, boundsMin.z), 
                halfSize, childDeph, _myOctree);
            _mXpYmZ = new Node(
                new Vector3(boundsMin.x, boundsMin.y + halfSize, boundsMin.z), 
                halfSize, childDeph, _myOctree);
            _pXpYmZ = new Node(
                new Vector3(boundsMin.x + halfSize, boundsMin.y + halfSize, boundsMin.z), 
                halfSize, childDeph, _myOctree);
            _mXmYpZ = new Node(
                new Vector3(boundsMin.x, boundsMin.y, boundsMin.z + halfSize), 
                halfSize, childDeph, _myOctree);
            _pXmYpZ = new Node(
                new Vector3(boundsMin.x + halfSize, boundsMin.y, boundsMin.z + halfSize), 
                halfSize, childDeph, _myOctree);
            _mXpYpZ = new Node(
                new Vector3(boundsMin.x, boundsMin.y + halfSize, boundsMin.z + halfSize), 
                halfSize, childDeph, _myOctree);
            _pXpYpZ = new Node(
                new Vector3(boundsMin.x + halfSize, boundsMin.y + halfSize, boundsMin.z + halfSize), 
                halfSize, childDeph, _myOctree);

            _childNodes = new List<Node>()
            {
                _mXmYmZ, _pXmYmZ, _mXpYmZ, _pXpYmZ, _mXmYpZ, _pXmYpZ, _mXpYpZ, _pXpYpZ
            };

            _isSubdivided = true;

            foreach (Node childNode in _childNodes)
            {
                childNode._onOccupied += OnChildOccupied;
            }
        }

        private static bool Intersects(Node node, Bounds otherBounds)
        {
            return node._bounds.Intersects(otherBounds);
        }

        private static bool Contains(Node node, Vector3 point)
        {
            return node._bounds.Contains(point);
        }

        private static bool OverlapsWithOccupiedNode<T>(Node node, T thingToCheck, System.Func<Node, T, bool> overlapCheckFunc)
        {
            if (overlapCheckFunc(node, thingToCheck))
            {
                if (node._isOccupied)
                {
                    return true;
                }
                else
                {
                    if (node._isSubdivided)
                    {
                        bool overlapsInChild = false;
                        foreach (Node childNode in node._childNodes)
                        {
                            if (OverlapsWithOccupiedNode(childNode, thingToCheck, overlapCheckFunc))
                            {
                                overlapsInChild = true;
                                break;
                            }
                        }
                        return overlapsInChild;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        private void Occupied()
        {
            _isOccupied = true;
            _onOccupied?.Invoke();
        }

        private void OnChildOccupied()
        {
            _occupiedChildCount++;
            if(_occupiedChildCount == 8)
            {
                Occupied();
            }
        }
    }

}
