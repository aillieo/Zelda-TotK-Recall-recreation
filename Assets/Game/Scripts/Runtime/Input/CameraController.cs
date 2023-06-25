namespace AillieoTech.Game.Input
{
    using UnityEngine;

    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private new Camera camera;

        [SerializeField]
        private float moveSpeed = 16f;

        [SerializeField]
        private float rotSpeed = 3f;

        private Vector2 moveDirection;
        private float rotationX = 0f;
        private float rotationY = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            this.rotationX = this.camera.transform.eulerAngles.y;
            this.rotationY = this.camera.transform.eulerAngles.x;
        }

        private void LateUpdate()
        {
            this.UpdatePosition();
            this.UpdateRotation();
        }

        private void UpdatePosition()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            this.moveDirection = new Vector2(horizontal, vertical);

            this.moveDirection = this.moveSpeed * Time.deltaTime * this.moveDirection.normalized;
            Vector3 motion = (this.camera.transform.forward * this.moveDirection.y) + (this.camera.transform.right * this.moveDirection.x);

            this.camera.transform.position += motion;
        }

        private void UpdateRotation()
        {
            this.rotationX += Input.GetAxis("Mouse X") * this.rotSpeed;
            this.rotationY += Input.GetAxis("Mouse Y") * this.rotSpeed;

            this.rotationY = Mathf.Clamp(this.rotationY, -90f, 90f);
            this.camera.transform.localRotation = Quaternion.Euler(-this.rotationY, this.rotationX, 0f);
        }
    }
}
