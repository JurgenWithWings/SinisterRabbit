using KinematicCharacterController;
using UnityEngine;

public enum CrouchInput {
    None,
    Toggle,
    Held,
    NotHeld,
}

public enum Stance {
    Stand,
    Crouch,
    Slide
}

public struct CharacterState {
    public bool Grounded;
    public Stance Stance;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput {
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool JumpSustain;
    public CrouchInput Crouch;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController {
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform root;
    [SerializeField] private Transform cameraTarget;
    public Transform CameraTarget => cameraTarget;
    
    // Settings
    [Space]
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float crouchSpeed = 5f;
    [SerializeField] private float walkResponse = 25f;
    [SerializeField] private float crouchResponse = 20f;
    [Space] 
    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 70f;
    [Space] 
    [SerializeField] private int jumpCount = 1;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField, Range(0f, 1f)] private float jumpSustainGravity = 0.4f;
    [SerializeField] private float gravity = -90f;
    [Space]
    [SerializeField] private float slideStartSpeed = 25f;
    [SerializeField] private float slideEndSpeed = 15f;
    [SerializeField] private float slideFriction = 0.8f;
    [SerializeField] private float slideSteerAcceleration = 5f;
    [SerializeField] private float slideGravity = -90f;
    [Space]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float crouchHeightResponse = 15f;
    [SerializeField, Range(0f, 1f)] private float standCameraTargetHeight = 0.9f;
    [SerializeField, Range(0f, 1f)] private float crouchCameraTargetHeight = 0.7f;
    
    // State
    private CharacterState state;
    private CharacterState lastState;
    private CharacterState tempState;
    
    public CharacterState State => state;
    public CharacterState LastState => lastState;
    
    //Input
    private Quaternion requestedRotation;
    private Vector3 requestedMovement;
    private bool requestedJump;
    private bool requestedJumpSustain;
    private bool requestedCrouch;
    private bool requestedCrouchInAir;
    
    // Trackers
    private int remainingJumps;
    private float timeSinceUngrounded;
    private float timeSinceJumpRequest;
    private bool ungroundedDueToJump;
    
    public void Initialize() {
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input) {
        requestedRotation = input.Rotation;
        
        requestedMovement = new Vector3(input.Move.x, 0, input.Move.y);
        requestedMovement = Vector3.ClampMagnitude(requestedMovement, 1f);
        requestedMovement = input.Rotation * requestedMovement;

        if (jumpCount > 0) {
            bool wasRequestingJump = requestedJump;
            requestedJump = requestedJump || input.Jump;
            if (requestedJump && !wasRequestingJump) {
                timeSinceJumpRequest = 0f;
            }
            requestedJumpSustain = input.JumpSustain;
        }

        bool wasRequestingCrouch = requestedCrouch;
        requestedCrouch = input.Crouch switch {
            CrouchInput.Toggle => !requestedCrouch,
            CrouchInput.None => requestedCrouch,
            CrouchInput.Held => true,
            CrouchInput.NotHeld => false,
            _ => requestedCrouch,
        };
        if (requestedCrouch && !wasRequestingCrouch) {
            requestedCrouchInAir = !state.Grounded;
        }
        else if (!requestedCrouch && wasRequestingCrouch) {
            requestedCrouchInAir = false;
        }
    }

    public void UpdateBody(float deltaTime) {
        // Camera Position
        float currentHeight = motor.Capsule.height;
        float cameraTargetHeightTarget = state.Stance switch {
            Stance.Stand => standCameraTargetHeight,
            Stance.Crouch or Stance.Slide => crouchCameraTargetHeight,
            _ => standingHeight,
        };
        float cameraTargetHeight = currentHeight * cameraTargetHeightTarget;

        cameraTarget.localPosition = Vector3.Lerp(
            a: cameraTarget.localPosition,
            b: new Vector3(0f, cameraTargetHeight, 0f),
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );
        
        // Mesh Root Scale
        float normalizedHeight = currentHeight / standingHeight;
        Vector3 rootTargetScale = new(1f, normalizedHeight, 1f);
        root.transform.localScale = Vector3.Lerp(
            a: root.localScale,
            b: rootTargetScale,
            t: 1f - Mathf.Exp(-crouchHeightResponse * deltaTime)
        );
    }
    
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime) {
        Vector3 forward = Vector3.ProjectOnPlane(cameraTarget.forward, motor.CharacterUp);
        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }
    
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) {
        state.Grounded = motor.GroundingStatus.IsStableOnGround;
        state.Acceleration = Vector3.zero;
        
        // Grounded Movement
        if (state.Grounded) {
            timeSinceUngrounded = 0f;
            ungroundedDueToJump = false;
            remainingJumps = jumpCount;
            
            Vector3 groundedMovement = motor.GetDirectionTangentToSurface(
                direction: requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * requestedMovement.magnitude;
            
            // Start Sliding
            bool moving = groundedMovement.sqrMagnitude > 0f;
            bool crouching = state.Stance is Stance.Crouch;
            bool wasStanding = lastState.Stance is Stance.Stand;
            bool wasInAir = !lastState.Grounded;
            if (moving && crouching && (wasStanding || wasInAir)) {
                state.Stance = Stance.Slide;

                // Reproject the last frames (falling) velocity onto the ground normal to keep momentum in the slide.
                if (wasInAir) {
                    currentVelocity = Vector3.ProjectOnPlane(
                        vector: lastState.Velocity,
                        planeNormal: motor.GroundingStatus.GroundNormal
                    );
                }

                float effectiveSlideStartSpeed = slideStartSpeed;
                if (!lastState.Grounded && !requestedCrouchInAir) {
                    effectiveSlideStartSpeed = 0f;
                    requestedCrouchInAir = false;
                }
                
                float slideSpeed = Mathf.Max(effectiveSlideStartSpeed, currentVelocity.magnitude);
                currentVelocity = motor.GetDirectionTangentToSurface(
                    direction: currentVelocity,
                    surfaceNormal: motor.GroundingStatus.GroundNormal
                ) * slideSpeed;
            }
            
            // Normal Movement
            if (state.Stance is Stance.Stand or Stance.Crouch) {
                // Calculate Speed and Responsiveness of movement based on the character's stance
                float speed = state.Stance switch {
                    Stance.Stand => walkSpeed,
                    Stance.Crouch or Stance.Slide => crouchSpeed,
                    _ => walkSpeed
                };
                float response = state.Stance switch {
                    Stance.Stand => walkResponse,
                    Stance.Crouch or Stance.Slide => crouchResponse,
                    _ => walkResponse
                };

                // And smoothly move along the ground in that direction.
                Vector3 targetVelocity = groundedMovement * speed;
                Vector3 moveVelocity = Vector3.Lerp(
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-response * deltaTime)
                );
                state.Acceleration = (moveVelocity - currentVelocity) / deltaTime;
                currentVelocity = moveVelocity;
            }
            // Slide Movement
            else {
                // Slide Friction
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);
                
                // Slide Slope
                Vector3 force = Vector3.ProjectOnPlane(
                    vector: -motor.CharacterUp,
                    planeNormal: motor.GroundingStatus.GroundNormal
                ) * slideGravity;
                currentVelocity -= force * deltaTime;
                
                // Slide Steer
                float currentSpeed = currentVelocity.magnitude;
                Vector3 targetVelocity = groundedMovement * currentVelocity.magnitude;
                Vector3 steerVelocity = currentVelocity;
                Vector3 steerForce = (targetVelocity - steerVelocity) * slideSteerAcceleration * deltaTime;
                // Add steer force, but clamp velocity so the slide doesn't increase due to direct movement input.
                steerVelocity += steerForce;
                steerVelocity = Vector3.ClampMagnitude(steerVelocity, currentSpeed);
                
                state.Acceleration = (steerVelocity - currentVelocity) / deltaTime;
                currentVelocity = steerVelocity;
                
                // Stop Sliding if too slow
                if (currentVelocity.magnitude < slideEndSpeed) {
                    state.Stance = Stance.Crouch;
                }
            }
        }
        // Air Movement
        else {
            timeSinceUngrounded += deltaTime;
            
            if (requestedMovement.sqrMagnitude > 0f) {
                Vector3 planarMovement = Vector3.ProjectOnPlane(
                    vector: requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * requestedMovement.magnitude;
                
                // Current movement velocity on plane.
                Vector3 currentPlanarVelocity = Vector3.ProjectOnPlane(
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );
                
                // Calculate movement force.
                // Will change depending on current velocity.
                Vector3 movementForce = planarMovement * airAcceleration * deltaTime;
                
                // If moving slower than the max air speed, treat movementForce as a simple steering force.
                if (currentPlanarVelocity.magnitude < airSpeed) {
                    // Add it to the current planar velocity for a target velocity.
                    Vector3 targetPlanarVelocity = currentPlanarVelocity + movementForce;
                    
                    // Limit target velocity to air speed.
                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);
                    
                    // Steer towards target velocity.
                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }
                // Otherwise, nerf the movement force when it is in the direction of the current planar velocity
                // to prevent accelerating further beyond the max air speed.
                else if (Vector3.Dot(currentVelocity, movementForce) > 0f) {
                    // Project movement force onto the plane whose normal is the current planar velocity.
                    Vector3 constrainedMovementForce = Vector3.ProjectOnPlane(
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );
                    
                    movementForce = constrainedMovementForce;
                }
                
                // Prevent air-climbing steep slopes.
                if (motor.GroundingStatus.FoundAnyGround) {
                    // If moving in the same direction as the resultant velocity...
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f) {
                        // Calculate obstruction normal
                        Vector3 obstructionNormal = Vector3.Cross(
                            motor.CharacterUp,
                            motor.GroundingStatus.GroundNormal
                        ).normalized;
                        
                        // Project movement force onto obstruction plane.
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }
                
                currentVelocity += movementForce;
            }
            
            // Gravity
            float effectiveGravity = gravity;
            float verticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
            if (requestedJumpSustain && verticalSpeed > 0f) {
                effectiveGravity *= jumpSustainGravity;
            }
            currentVelocity += motor.CharacterUp * effectiveGravity * deltaTime;
        }
        
        // Jumping
        if (requestedJump) {
            bool canCoyoteJump = timeSinceUngrounded < coyoteTime && !ungroundedDueToJump;
            bool isDoubleJump = remainingJumps > (ungroundedDueToJump ? 0 : 1);
            if (state.Grounded || canCoyoteJump || isDoubleJump) {
                requestedJump = false;
                requestedCrouch = false;
                requestedCrouchInAir = false;

                remainingJumps--;
                
                if (state.Grounded) {
                    ungroundedDueToJump = true;
                }

                // Unstick from ground
                motor.ForceUnground(time: 0.1f);

                float currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                float targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else {
                timeSinceJumpRequest += deltaTime;
                
                // Defer the jump request until coyote time has passed.
                bool canJumpLater = timeSinceJumpRequest < coyoteTime;
                requestedJump = canJumpLater;
            }
        }
    }
    
    public void BeforeCharacterUpdate(float deltaTime) {
        tempState = state;
        
        // Crouch
        if (requestedCrouch && state.Stance is Stance.Stand) {
            state.Stance = Stance.Crouch;
            motor.SetCapsuleDimensions(
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );
        }
    }
    
    private Collider[] unCrouchOverlapResults = new Collider[8];
    public void AfterCharacterUpdate(float deltaTime) {
        // Uncrouch
        if (!requestedCrouch && state.Stance is not Stance.Stand) {
            motor.SetCapsuleDimensions(
                radius: motor.Capsule.radius,
                height: standingHeight,
                yOffset: standingHeight * 0.5f
            );
            
            // Check if Uncrouch overlaps any colliders
            Vector3 pos = motor.TransientPosition;
            Quaternion rot = motor.TransientRotation;
            LayerMask mask = motor.CollidableLayers;
            if (motor.CharacterOverlap(pos, rot, unCrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0) {
                // Re-crouch
                requestedCrouch = true;
                motor.SetCapsuleDimensions(
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
            }
            else {
                state.Stance = Stance.Stand;
            }
        }
        
        // Stuff can happen after move acceleration (walk/slide movement) is applied to current velocity that
        // lowers the velocity, so after the character updates make sure move acceleration does not exceed
        // the total acceleration.
        Vector3 totalAcceleration = (state.Velocity - lastState.Velocity) / deltaTime;
        state.Acceleration = Vector3.ClampMagnitude(state.Acceleration, totalAcceleration.magnitude);

        state.Grounded = motor.GroundingStatus.IsStableOnGround;
        state.Velocity = motor.Velocity;
        lastState = tempState;
    }
    
    public void PostGroundingUpdate(float deltaTime) {
        if (!motor.GroundingStatus.IsStableOnGround && state.Stance is Stance.Slide) {
            state.Stance = Stance.Crouch;
        }
    }
    
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        state.Acceleration = Vector3.ProjectOnPlane(state.Acceleration, hitNormal);
    }
    
    
    
    //Debug
    public void SetPosition(Vector3 position, bool killVelocity = true) {
        motor.SetPosition(position);
        if (killVelocity) {
            motor.BaseVelocity = Vector3.zero;
        }
    }
    
    
    
    public bool IsColliderValidForCollisions(Collider coll) { return true; }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
}