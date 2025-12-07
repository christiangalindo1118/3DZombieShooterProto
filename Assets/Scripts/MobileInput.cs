using UnityEngine;

public class MobileInput : MonoBehaviour
{

    public Joystick joystick;
    public CharacterController controller;

    public float speed = 3f;

    private Vector3 move;

    void Update()
    {
        // Leer joystick
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        // Crear dirección
        move = new Vector3(h, 0, v);

        // Si el joystick está moviéndose, rotar hacia la dirección
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        // Mover
        controller.Move(move * speed * Time.deltaTime);
    }
}

