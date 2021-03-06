﻿using UnityEngine;
using UnityEngine.Serialization;
using TrueSync.Physics3D;

namespace TrueSync {

    /**
     *  @brief Represents a physical 3D rigid body.
     **/
    [RequireComponent(typeof(TSCollider))]
    [AddComponentMenu("TrueSync/Physics/TSRigidBody", 11)]
    public class TSRigidBody : MonoBehaviour {

        public enum InterpolateMode { None, Interpolate, Extrapolate };

        [FormerlySerializedAs("mass")]
        [SerializeField]
        private FP _mass = 1;

        /**
         *  @brief Mass of the body. 
         **/
        public FP mass {
            get {
                if (tsCollider._rigidBody != null) {
                    return tsCollider._rigidBody.Mass;
                }

                return _mass;
            }

            set {
                _mass = value;

                if (tsCollider._rigidBody != null) {
                    tsCollider._rigidBody.Mass = value;
                }
            }
        }

        [SerializeField]
        private bool _useGravity = true;

        /**
         *  @brief If true it uses gravity force. 
         **/
        public bool useGravity {
            get {
                if (tsCollider.IsInitialized) {
                    return tsCollider.RigidBody.TSAffectedByGravity;
                }

                return _useGravity;
            }

            set {
                _useGravity = value;

                if (tsCollider.IsInitialized) {
                    tsCollider.RigidBody.TSAffectedByGravity = _useGravity;
                }
            }
        }

        [SerializeField]
        private bool _isKinematic;

        /**
         *  @brief If true it doesn't get influences from external forces. 
         **/
        public bool isKinematic {
            get {
                if (tsCollider.IsInitialized) {
                    return tsCollider.RigidBody.TSIsKinematic;
                }

                return _isKinematic;
            }

            set {
                _isKinematic = value;

                if (tsCollider.IsInitialized) {
                    tsCollider.RigidBody.TSIsKinematic = _isKinematic;
                }
            }
        }

        [SerializeField]
        private FP _linearDrag;

        /**
         *  @brief Linear drag coeficient.
         **/
        public FP drag {
            get {
                if (tsCollider.IsInitialized) {
                    return tsCollider.RigidBody.TSLinearDrag;
                }

                return _linearDrag;
            }

            set {
                _linearDrag = value;

                if (tsCollider.IsInitialized) {
                    tsCollider.RigidBody.TSLinearDrag = _linearDrag;
                }
            }
        }

        [SerializeField]
        private FP _angularDrag = 0.05f;

        /**
         *  @brief Angular drag coeficient.
         **/
        public FP angularDrag {
            get {
                if (tsCollider.IsInitialized) {
                    return tsCollider.RigidBody.TSAngularDrag;
                }

                return _angularDrag;
            }

            set {
                _angularDrag = value;

                if (tsCollider.IsInitialized) {
                    tsCollider.RigidBody.TSAngularDrag = _angularDrag;
                }
            }
        }

        /**
         *  @brief Interpolation mode that should be used. 
         **/
        public InterpolateMode interpolation;

        [SerializeField]
        [HideInInspector]
        private TSRigidBodyConstraints _constraints = TSRigidBodyConstraints.None;

        /**
         *  @brief Freeze constraints applied to this body. 
         **/
        public TSRigidBodyConstraints constraints {
            get {
                if (tsCollider.IsInitialized) {
                    return tsCollider._rigidBody.FreezeConstraints;
                }

                return _constraints;
            }

            set {
                _constraints = value;

                if (tsCollider.IsInitialized) {
                    tsCollider._rigidBody.FreezeConstraints = value;
                }
            }
        }

        private TSCollider _tsCollider;

        /**
         *  @brief Returns the {@link TSCollider} attached.
         */
        public TSCollider tsCollider {
            get {
                if (_tsCollider == null) {
                    _tsCollider = GetComponent<TSCollider>();
                }

                return _tsCollider;
            }
        }

        private TSTransform _tsTransform;

        /**
         *  @brief Returns the {@link TSTransform} attached.
         */
        public TSTransform tsTransform {
            get {
                if (_tsTransform == null) {
                    _tsTransform = GetComponent<TSTransform>();
                }

                return _tsTransform;
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector} representing the force to be applied.
         **/
        public void AddForce(TSVector force) {
            AddForce(force, ForceMode.Force);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector} representing the force to be applied.
         *  @param mode Indicates how the force should be applied.
         **/
        public void AddForce(TSVector force, ForceMode mode) {
            if (mode == ForceMode.Force) {
                tsCollider.RigidBody.TSApplyForce(force);
            } else if (mode == ForceMode.Impulse) {
                tsCollider.RigidBody.TSApplyImpulse(force);
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(TSVector force, TSVector position) {
            AddForceAtPosition(force, position, ForceMode.Force);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(TSVector force, TSVector position, ForceMode mode) {
            if (mode == ForceMode.Force) {
                tsCollider.RigidBody.TSApplyForce(force, position);
            } else if (mode == ForceMode.Impulse) {
                tsCollider.RigidBody.TSApplyImpulse(force, position);
            }
        }

        /**
         *  @brief Returns the velocity of the body at some position in world space. 
         **/
        public TSVector GetPointVelocity(TSVector worldPoint) {
            TSVector directionPoint = position - tsCollider.RigidBody.TSPosition;
            return TSVector.Cross(tsCollider.RigidBody.TSAngularVelocity, directionPoint) + tsCollider.RigidBody.TSLinearVelocity;
        }

        /**
         *  @brief Simulates the provided tourque in the body. 
         *  
         *  @param torque A {@link TSVector} representing the torque to be applied.
         **/
        public void AddTorque(TSVector torque) {
            tsCollider.RigidBody.TSApplyTorque(torque);
        }

        /**
         *  @brief Simulates the provided relative tourque in the body. 
         *  
         *  @param torque A {@link TSVector} representing the relative torque to be applied.
         **/
        public void AddRelativeTorque(TSVector torque)
        {
            tsCollider.RigidBody.TSApplyRelativeTorque(torque);
        }

        /**
         *  @brief Changes orientation to look at target position. 
         *  
         *  @param target A {@link TSVector} representing the position to look at.
         **/
        public void LookAt(TSVector target) {
            rotation = TSQuaternion.CreateFromMatrix(TSMatrix.CreateFromLookAt(position, target));
        }

        /**
         *  @brief Moves the body to a new position. 
         **/
        public void MovePosition(TSVector position) {
            this.position = position;
        }

        /**
         *  @brief Rotates the body to a provided rotation. 
         **/
        public void MoveRotation(TSQuaternion rot) {
            this.rotation = rot;
        }

        /**
        *  @brief Position of the body. 
        **/
        public TSVector position {
            get {
                return tsTransform.position;
            }

            set {
                tsTransform.position = value;
            }
        }

        /**
        *  @brief Orientation of the body. 
        **/
        public TSQuaternion rotation {
            get {
                return tsTransform.rotation;
            }

            set {
                tsTransform.rotation = value;
            }
        }

        /**
        *  @brief LinearVelocity of the body. 
        **/
        public TSVector velocity {
            get {
                return tsCollider.RigidBody.TSLinearVelocity;
            }

            set {
                tsCollider.RigidBody.TSLinearVelocity = value;
            }
        }

        /**
        *  @brief AngularVelocity of the body. 
        **/
        public TSVector angularVelocity {
            get {
                return tsCollider.RigidBody.TSAngularVelocity;
            }

            set {
                tsCollider.RigidBody.TSAngularVelocity = value;
            }
        }

    }

}