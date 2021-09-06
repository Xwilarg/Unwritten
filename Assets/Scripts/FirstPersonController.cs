using UnityEngine;

namespace Unwritten
{
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _runSpeed;
        [SerializeField] private float _jumpSpeed;

        [Header("Audio")]
        [SerializeField] private AudioClip[] _footstepSounds;
        [SerializeField] private AudioClip _landSound;
        [SerializeField] private AudioClip _jumpSound;

        [Header("Footsteps")]
        [SerializeField] [Range(0f, 1f)] private float _runstepLenghten;
        [SerializeField] private float _stepInterval;

        [Header("Others")]
        [SerializeField] private MouseLook _mouseLook;
        [SerializeField] private float _gravityMutliplicator;
        [SerializeField] private FOVKick _fovKick;
        [SerializeField] private float _stickToGroundForce;

        private bool _jump;
        private bool _jumping;
        private CharacterController _characterController;
        private bool _previouslyGrounded;
        private Vector3 _moveDir = Vector3.zero;
        private AudioSource _audioSource;
        private float _stepCycle = 0f;
        private float _nextStep = 0f;
        private CollisionFlags _collisionFlags;
        private bool _isWalking;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _audioSource = GetComponent<AudioSource>();
            _mouseLook.Init(transform, Camera.main.transform);
        }

        private void Update()
        {
            _mouseLook.LookRotation(transform, Camera.main.transform);
            if (!_jump)
            {
                _jump = Input.GetButtonDown("Jump");
            }

            _isWalking = !Input.GetKey(KeyCode.LeftShift);

            // Jump landing
            if (!_previouslyGrounded && _characterController.isGrounded)
            {
                // Landing sound
                _audioSource.clip = _landSound;
                _audioSource.Play();
                _nextStep = _stepCycle + .5f;

                _moveDir.y = 0f;
                _jumping = false;
            }

            if (!_characterController.isGrounded && !_jumping && _previouslyGrounded)
            {
                _moveDir.y = 0f;
            }

            _previouslyGrounded = _characterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool wasWalking = _isWalking;

            var speed = _isWalking ? _walkSpeed : _runSpeed;
            var input = new Vector2(horizontal, vertical);

            if (input.sqrMagnitude > 1)
            {
                input.Normalize();
            }

            // FOV kick
            if (_isWalking != wasWalking && _characterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!_isWalking ? _fovKick.FOVKickUp() : _fovKick.FOVKickDown());
            }

            Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;

            // Get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out RaycastHit hitInfo,
                               _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            _moveDir.x = desiredMove.x * speed;
            _moveDir.z = desiredMove.z * speed;


            if (_characterController.isGrounded)
            {
                _moveDir.y = -_stickToGroundForce;

                if (_jump)
                {
                    _moveDir.y = _jumpSpeed;
                    _audioSource.clip = _jumpSound;
                    _audioSource.Play();
                    _jump = false;
                    _jumping = true;
                }
            }
            else
            {
                _moveDir += Physics.gravity * _gravityMutliplicator * Time.fixedDeltaTime;
            }
            _collisionFlags = _characterController.Move(_moveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed, input);

            _mouseLook.UpdateCursorLock();
        }

        private void ProgressStepCycle(float speed, Vector3 input)
        {
            if (_characterController.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
            {
                _stepCycle += (_characterController.velocity.magnitude + (speed * (_isWalking ? 1f : _runstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(_stepCycle > _nextStep))
            {
                return;
            }

            _nextStep = _stepCycle + _stepInterval;

            if (!_characterController.isGrounded)
            {
                return;
            }
            // Get a random footstep sound between 1 and length then move it to index 0 to not play it next time
            int n = Random.Range(1, _footstepSounds.Length);
            _audioSource.clip = _footstepSounds[n];
            _audioSource.PlayOneShot(_audioSource.clip);
            _footstepSounds[n] = _footstepSounds[0];
            _footstepSounds[0] = _audioSource.clip;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            // Don't move the rigidbody if the character is on top of it
            if (_collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(_characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
