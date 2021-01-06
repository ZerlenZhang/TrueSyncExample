using System;
using UnityEngine;
using UnityEngine.Serialization;
using TrueSync.Physics3D;

namespace TrueSync {
    /**
     *  @brief Abstract collider for 3D shapes. 
     **/
    [RequireComponent(typeof(TSTransform))]
    [Serializable]
    [ExecuteInEditMode]
    public abstract class TSCollider : MonoBehaviour, ICollider {

        #region Shape

        private Shape shape;
        public Shape Shape {
            get {
                if (shape == null)
                    shape = CreateShape();
                return shape;
            }
            protected set => shape = value;
        }        

        #endregion

        #region IsTrigger

        [FormerlySerializedAs("isTrigger")]
        [SerializeField]
        private bool _isTrigger;

        /**
         *  @brief If it is only a trigger and doesn't interfere on collisions. 
         **/
        public bool isTrigger {
            get {
                if (_rigidBody != null) {
                    return _rigidBody.IsColliderOnly;
                }

                return _isTrigger;
            }
            set {
                _isTrigger = value;

                if (_rigidBody != null) {
                    _rigidBody.IsColliderOnly = _isTrigger;
                }
            }
        }        

        #endregion

        #region Center

        [SerializeField]
        private TSVector center;        
        /**
         *  @brief Center of the collider shape.
         **/
        public TSVector Center {
            get => center;
            set => center = value;
        }
        #endregion

        #region tsRigidBody

        private TSRigidBody _tsRigidBody;

        public TSRigidBody tsRigidBody
        {
            get
            {
                if(!_isInternalInitialized)
                    InternalInit();
                return _tsRigidBody;
            }
        }
        

        #endregion

        #region tsTransform

        public TSTransform _tsTransform;

        public TSTransform tsTransform
        {
            get
            {
                if(!_isInternalInitialized)
                    InternalInit();
                return _tsTransform;
            }
        }

        #endregion

        #region tsPhysicsMaterial

        /**
         *  @brief Simulated material. 
         **/
        [SerializeField] private TSPhysicsMaterial _tsPhysicsMaterial;

        public TSPhysicsMaterial tsPhysicsMaterial
        {
            get => _tsPhysicsMaterial;
            private set => _tsPhysicsMaterial = value;
        }        

        #endregion
        
        /**
         *  @brief Returns true if the body was already initialized.
         **/
        public bool IsInitialized => _rigidBody != null;

        /**
         *  @brief Returns the body linked to this collider.
         */
        public IBody3D RigidBody => _rigidBody;

        /**
         *  @brief Returns a version of collider's center scaled by parent's transform.
         */
        public TSVector ScaledCenter => TSVector.Scale (Center, lossyScale);        
        
        /**
         *  @brief Holds an first value of the GameObject's lossy scale.
         **/
        [SerializeField]
        protected TSVector lossyScale = TSVector.one;
        
        #region InitSettings

        private bool _isInternalInitialized = false;

        internal RigidBody _rigidBody;
        private void InternalInit()
        {
            _isInternalInitialized = true;
            _tsRigidBody = GetComponent<TSRigidBody>();
            if (lossyScale == TSVector.one) {
                lossyScale = TSVector.Abs(transform.localScale.ToTSVector());
            }
        }
        
        private RigidBody CreateBody() {
            var newBody = new RigidBody(Shape);

            //如果没有在检视面板赋值，就自动获取一下
            if (tsPhysicsMaterial == null) {
                tsPhysicsMaterial = GetComponent<TSPhysicsMaterial>();
            }

            if (tsPhysicsMaterial != null) {
                newBody.TSFriction = tsPhysicsMaterial.friction;
                newBody.TSRestitution = tsPhysicsMaterial.restitution;
            }
            else
            {
                newBody.TSFriction = TrueSyncManager.Config.friction;
                newBody.TSRestitution = TrueSyncManager.Config.restitution;
            }

            newBody.IsColliderOnly = isTrigger;
            newBody.IsKinematic = tsRigidBody != null && tsRigidBody.isKinematic;

            bool isStatic = tsRigidBody == null || tsRigidBody.isKinematic;

            if (tsRigidBody != null) {
                newBody.AffectedByGravity = tsRigidBody.useGravity;

                if (tsRigidBody.mass <= 0) {
                    tsRigidBody.mass = 1;
                }

                newBody.Mass = tsRigidBody.mass;
                newBody.TSLinearDrag = tsRigidBody.drag;
                newBody.TSAngularDrag = tsRigidBody.angularDrag;
            } else {
                newBody.SetMassProperties();
            }

            if (isStatic) {
                newBody.AffectedByGravity = false;
                newBody.IsStatic = true;
            }
            
            newBody.FreezeConstraints = tsRigidBody==null ? TSRigidBodyConstraints.None : tsRigidBody.constraints;
            
            return newBody;
        }

        /**
         *  @brief Initializes Shape and RigidBody and sets initial values to position and orientation based
         * on Unity's transform.
         **/
        public void NecessaryOuterInitialize() {
            _rigidBody = CreateBody();
        }
        
        #endregion
        
        
        public void Update() {
            if (!Application.isPlaying) {
                lossyScale = TSVector.Abs(transform.lossyScale.ToTSVector());
            }
        }
        

        /**
         *  @brief Do a base matrix transformation to draw correctly all collider gizmos.
         **/
        public virtual void OnDrawGizmosSelected() {
            if (!this.enabled) {
                return;
            }

            Vector3 position = _rigidBody != null ? _rigidBody.Position.ToVector() : (transform.position + ScaledCenter.ToVector());
            Quaternion rotation = _rigidBody != null ? _rigidBody.Orientation.ToQuaternion() : transform.rotation;

            Gizmos.color = Color.yellow;

			Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, GetGizmosSize());
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            DrawGizmos();

            Gizmos.matrix = oldGizmosMatrix;
        }

        #region abstract methods

        /**
         *  @brief Returns the gizmos size.
         **/
        protected abstract Vector3 GetGizmosSize();

        /**
         *  @brief Draws the specific gizmos of concrete collider (for example "Gizmos.DrawWireCube" for a {@link TSBoxCollider}).
         **/
        protected abstract void DrawGizmos();   
        /**
         *  @brief Creates the shape related to a concrete implementation of TSCollider.
         **/
        public abstract Shape CreateShape();        

        #endregion
    }

}