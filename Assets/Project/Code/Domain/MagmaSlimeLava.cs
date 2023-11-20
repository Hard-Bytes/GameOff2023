using System;
using System.Collections.Generic;
using UnityEngine;
using Project.Code.Utils;

namespace Project.Code.Domain
{
    public class MagmaSlimeLava : MonoBehaviour
    {
        
        [SerializeField] private int damage = 3; 
        
        private TrailRenderer _trail;
        private EdgeCollider2D _collider;

        private void Awake()
        {
            _trail = this.GetComponent<TrailRenderer>();
            GameObject colliderGameObject = new GameObject("TrailCollider", typeof(EdgeCollider2D));
            _collider = colliderGameObject.GetComponent<EdgeCollider2D>();
            _collider.isTrigger = true;
            colliderGameObject.AddComponent<MagmaSlimeLavaColider>().SetDamage(damage) ;
        }

        private void OnDestroy()
        {
            if(_collider)
            {
                Destroy(_collider.gameObject);
            }
        }

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            ColliderPointsFromtTrail(_trail, _collider);
        }

        void ColliderPointsFromtTrail(TrailRenderer trail, EdgeCollider2D collider)
        {
            List<Vector2> points = new List<Vector2>();
            for(int pos = 0; pos < trail.positionCount; pos++)
            {
                points.Add(trail.GetPosition(pos));
            }
            collider.SetPoints(points);

        }

        public void SetDamage(int newDmg)
        {
            damage = newDmg;
            _collider.GetComponent<MagmaSlimeLavaColider>().SetDamage(newDmg);
        }

    }
}
