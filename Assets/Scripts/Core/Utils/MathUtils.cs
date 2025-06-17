using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils {
    public static class MathUtils {

        public static float Remap(float inpMin, float inpMax, float outMin, float outMax, float inpValue){
            float invLerpResult = Mathf.InverseLerp(inpMin, inpMax, inpValue);
            float lerpResult = Mathf.Lerp(outMin, outMax, invLerpResult);
            return lerpResult;
        }

        public static bool Probability(int number) {
            return UnityEngine.Random.Range(0, 100) < number;
        }

        public static bool TossCoin() {
            return UnityEngine.Random.value > 0.5f;
        }

        public static int RandValue(int min, int max) {
            return UnityEngine.Random.Range(min, max);
        }
        
        public static float RandValue(float min, float max) {
            return UnityEngine.Random.Range(min, max);
        }
        
        public static float RandValueRound(float min, float max) {
            return Mathf.Round(UnityEngine.Random.Range(min, max));
        }

        public static Color GetRandomColor() {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDir() {
            return new Vector3(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).normalized;
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDirXZ() {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        public static Vector3 GetVectorFromAngle(int angle) {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI/180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        
        public static Vector3 GetVectorFromAngle(float angle) {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI/180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static float GetAngleFromVectorFloat(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static float GetAngleFromVectorFloatXZ(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static int GetAngleFromVector(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static int GetAngleFromVector180(Vector3 dir) {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation) {
            return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle) {
            return Quaternion.Euler(0, 0, angle) * vec;
        }

        public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle) {
            return Quaternion.Euler(0, angle, 0) * vec;
        }
    }
}
