using System;
using UnityEngine;

namespace Unwritten
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool ClampVerticalRotation = true;
        public float MinimumX = -90f;
        public float MaximumX = 90f;
        private bool _isCursorLock = true;


        private Quaternion _characterTargetRot;
        private Quaternion _cameraTargetRot;

        public void Init(Transform character, Transform camera)
        {
            _characterTargetRot = character.localRotation;
            _cameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = Input.GetAxis("Mouse X") * XSensitivity;
            float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

            _characterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            _cameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if (_isCursorLock)
            {
                _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);
            }

            /*
             * character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);*/
            character.localRotation = _characterTargetRot;
            camera.localRotation = _cameraTargetRot;

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            _isCursorLock = value;
            if (!_isCursorLock)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _isCursorLock = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isCursorLock = true;
            }

            Cursor.lockState = _isCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_isCursorLock;
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
