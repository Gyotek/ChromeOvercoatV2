using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(menuName = "Character/Character Data")]
    public class CharacterDatas : ScriptableObject
    {
        [Header("Movement Settings")]
        [Tooltip("How fast the player moves while walking and strafing."), SerializeField]
        public float walkingSpeed = 5f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        public float runningSpeed = 9f;

        [Tooltip("Approximately the amount of time it will take for the player to reach maximum running or walking speed."), SerializeField]
        public float movementSmoothness = 0.125f;

        [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
        public float jumpForce = 35f;
        [Tooltip("Amount of force applied to the player when double-jumping."), SerializeField]
        public float doublejumpForce = 35f;
        [Tooltip("1 is full air control, 0 is none"), SerializeField]
        public float airControl = 0.8f;

        [Header("Look Settings")]
        [Tooltip("Rotation speed of the fps controller."), SerializeField]
        public float mouseSensitivity = 7f;

        [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
        public float rotationSmoothness = 0.05f;

        [Tooltip("Minimum rotation of the arms and camera on the x axis."), SerializeField]
        public float minVerticalAngle = -90f;

        [Tooltip("Maximum rotation of the arms and camera on the axis."), SerializeField]
        public float maxVerticalAngle = 90f;
    }
